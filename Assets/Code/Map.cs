using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code;
using UnityEngine;

internal sealed class Map : MonoBehaviour
{
    [SerializeField] private Cell[] _cells;
    [SerializeField] private Grid _grid;

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
    }

    public Vector2Int GetPosition(Cell cell)
    {
        return (Vector2Int)_grid.WorldToCell(cell.transform.position);
    }

    public bool CanBuild(Cell cell)
    {
        return !HasBuilding(cell);
    }

    public bool CanDemolish(Cell cell)
    {
        return HasBuilding(cell);
    }

    public bool CanBeUpgraded(Cell cell)
    {
        return HasBuilding(cell);
    }

    public bool HasBuilding(Cell cell)
    {
        return SystemLocator.I.GameData.Cells[GetPosition(cell)].HasBuilding;
    }

    public bool GetBuildingData(Cell cell, out BuildingData buildingData)
    {
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        buildingData = cellData.Building;
        return cellData.HasBuilding;
    }

    public void BuildBuilding(Cell cell, string buildingTypeId)
    {
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        cellData.HasBuilding = true;
        cellData.Building = new BuildingData
        {
            Level = 1, 
            TypeId = buildingTypeId
        };
        SystemLocator.I.GameData.Cells[position] = cellData;
        
        cell.DisplayBuildingView(cellData.Building);
    }

    public void DemolishBuilding(Cell cell)
    {
        Vector2Int position = GetPosition(cell);
        CellData cellData = SystemLocator.I.GameData.Cells[position];
        cellData.HasBuilding = false;
        cellData.Building = default;
        SystemLocator.I.GameData.Cells[position] = cellData;
        
        cell.DemolishBuildingView();
    }

    public IEnumerable<Vector2Int> GetPlayableCellPositions()
    {
        return _cells.Select(GetPosition);
    }
}
