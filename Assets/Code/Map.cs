using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code;
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

    public bool CanDemolish(Cell cell)
    {
        if (!GetBuildingData(cell, out BuildingData buildingData)) return false;
        if (buildingData.TypeId == "Castle") return false;
        return true;
    }

    public bool CanBeUpgraded(Cell cell)
    {
        return HasBuilding(cell);
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
        if (!SystemLocator.I.Game.CanAfford(cost))
        {
#if UNITY_EDITOR
            Debug.LogError("Tried to build with insufficient resources!");
#endif
            return;
        }
        
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        cellData.HasBuilding = true;
        cellData.Building = new BuildingData
        {
            Level = 1, 
            TypeId = buildingTypeId
        };
        SystemLocator.I.GameData.Cells[position] = cellData;
        SystemLocator.I.Game.Spend(cost);
        
        cell.DisplayBuildingView(cellData.Building);
        CellChanged?.Invoke(cell);
    }

    public void DemolishBuilding(Cell cell)
    {
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
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

    public void HaltProductionOrder(Cell cell)
    {
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        cellData.HasOrder = false;
        cellData.Order = default;
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
}
