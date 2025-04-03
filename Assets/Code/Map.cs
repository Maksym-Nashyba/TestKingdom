using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code;
using Code.Visualization;
using UnityEngine;

internal sealed class Map : MonoBehaviour
{
    public event Action<Cell> CellChanged;

    public IEnumerable<Vector2Int> PlayablePositions => _positionCache.Values;
    
    [SerializeField] private Cell[] _cells;
    [SerializeField] private Grid _grid;

    private Dictionary<Cell, Vector2Int> _positionCache;
    
    private void Start()
    {
#if UNITY_EDITOR
        Cell[] foundCells = FindObjectsByType<Cell>(FindObjectsSortMode.None);
        if (foundCells.Any(cell => !_cells.Contains(cell))) throw new InvalidDataException("Map's cells array does not contain all cells.");
#endif

        foreach (KeyValuePair<Vector2Int,CellData> cellData in SystemLocator.I.GameData.Cells)
        {
            if (!cellData.Value.HasBuilding) continue;
            
            Cell cell = _cells.First(c => GetPosition(c) == cellData.Key);
            cell.DisplayBuildingView(cellData.Value.Building);
        }

        if (_positionCache is null) GetPlayableCellPositions();
    }

    public Vector2Int GetPosition(Cell cell)
    {
        if (_positionCache is null || !_positionCache.TryGetValue(cell, out Vector2Int position))
        {
            Vector3Int rawPosition = _grid.WorldToCell(cell.transform.position);
            return new Vector2Int(rawPosition.x, rawPosition.y);
        }
        return position;
    }

    public bool CanBuild(Cell cell)
    {
        return !HasBuilding(cell);
    }

    public CanDemolishResult CanDemolish(Cell cell)
    {
        if (!GetBuildingData(cell, out BuildingData buildingData)) return CanDemolishResult.NoBuilding;
        if (buildingData.TypeId == "Castle") return CanDemolishResult.Castle;
        return CanDemolishResult.Ok;
    }

    public CanUpgradeResult CanUpgrade(Cell cell)
    {
        if (!HasBuilding(cell)) return CanUpgradeResult.NoBuilding;
        
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        
        BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(cellData.Building.TypeId);
        if (buildingLine.Levels.Length <= cellData.Building.Level+1) return CanUpgradeResult.MaxLevel;

        int requiredCastleLevel = buildingLine.Levels[cellData.Building.Level + 1].CastleLevelRequired;
        if (GetCastleData().Building.Level < requiredCastleLevel) return CanUpgradeResult.CastleTooLow;
        
        ResourceCount[] cost = buildingLine.Levels[cellData.Building.Level+1].Cost;
        if (!SystemLocator.I.Game.CanAfford(cost)) return CanUpgradeResult.InsufficientResources;

        return CanUpgradeResult.Ok;
    }

    public bool CanProduce(Cell cell)
    {
        if (!GetBuildingData(cell, out BuildingData buildingData)) return false;
        if (buildingData.TypeId == "Castle") return false;
        return true;
    }

    public bool HasBuilding(Cell cell)
    {
        return SystemLocator.I.GameData.Cells[GetPosition(cell)].HasBuilding;
    }

    public CellData GetCellData(Cell cell)
    {
        Vector2Int position = GetPosition(cell);
        return SystemLocator.I.GameData.Cells[position];
    }

    public bool GetBuildingData(Cell cell, out BuildingData buildingData)
    {
        CellData cellData = GetCellData(cell);
        buildingData = cellData.Building;
        return cellData.HasBuilding;
    }

    public void BuildBuilding(Cell cell, string buildingTypeId)
    {
        BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(buildingTypeId);
        ResourceCount[] cost = buildingLine.Levels[0].Cost;
        
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        cellData.HasBuilding = true;
        cellData.Building = new BuildingData
        {
            Level = 0, 
            TypeId = buildingTypeId
        };
        SystemLocator.I.GameData.Cells[position] = cellData;
        SystemLocator.I.Game.Spend(cost);
        
        cell.DisplayBuildingView(cellData.Building);
        CellChanged?.Invoke(cell);
    }

    public void UpgradeBuilding(Cell cell)
    {
        CanUpgradeResult canUpgrade = CanUpgrade(cell);
        if (canUpgrade != CanUpgradeResult.Ok)
        {
#if UNITY_EDITOR
            Debug.LogError($"Can't upgrade {canUpgrade}");
#endif
            return;
        }
        
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(cellData.Building.TypeId);
        ResourceCount[] cost = buildingLine.Levels[cellData.Building.Level+1].Cost;
        
        SystemLocator.I.Game.Spend(cost);
        cellData.Building.Level++;
        SystemLocator.I.GameData.Cells[position] = cellData;
        
        cell.DisplayUpgrade(cellData.Building);
        CellChanged?.Invoke(cell);
    }

    public void DemolishBuilding(Cell cell)
    {
        CanDemolishResult canDemolish = CanDemolish(cell);
        if (canDemolish != CanDemolishResult.Ok)
        {
#if UNITY_EDITOR
            Debug.LogError($"Can't demolish {canDemolish}");
#endif
            return;
        }
        
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        
        BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(cellData.Building.TypeId);
        SystemLocator.I.Game.Aquire(buildingLine.Levels[cellData.Building.Level].Cost);
        
        cellData.HasOrder = false;
        cellData.Order = default;
        cellData.HasBuilding = false;
        cellData.Building = default;
        SystemLocator.I.GameData.Cells[position] = cellData;
        
        cell.DemolishBuildingView();
        CellChanged?.Invoke(cell);
    }

    public IEnumerable<Vector2Int> GetPlayableCellPositions()
    {
        _positionCache = _cells.ToDictionary(x=> x, GetPosition);
        return PlayablePositions;
    }

    public void StartProductionOrder(Cell cell, ProductionOrder order)
    {
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        cellData.HasOrder = true;
        cellData.Order = new OrderData
        {
            StartTime = DateTime.Now,
            TypeId = order.Id,
        };
        SystemLocator.I.GameData.Cells[position] = cellData;
        CellChanged?.Invoke(cell);
    }

    public void SetProductionProgressView(Vector2Int position, float progress)
    {
        Cell cell = _positionCache.First(x => x.Value == position).Key;
        cell.SetProductionProgress(progress);
    }

    public void DisplayOrderDone(Vector2Int position, ProductionOrder order)
    {
        Cell cell = _positionCache.First(x => x.Value == position).Key;
        cell.DisplayOrderDone(order);
    }

    public CellData GetCastleData()
    {
        return SystemLocator.I.GameData.Cells[new Vector2Int(0, 0)];
    }
    
    public enum CanUpgradeResult
    {
        Ok,
        NoBuilding,
        InsufficientResources,
        MaxLevel,
        CastleTooLow
    }
    
    public enum CanDemolishResult
    {
        Ok,
        NoBuilding,
        Castle
    }
}
