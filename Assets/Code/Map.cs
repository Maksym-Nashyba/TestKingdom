using Code;
using UnityEngine;

[RequireComponent(typeof(Grid))]
internal sealed class Map : MonoBehaviour
{
    private Grid _grid;
    private Cell[] _cells;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
        _cells = FindObjectsByType<Cell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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
        return SystemLocator.I.PlayerData.GetBuildingData(GetPosition(cell), out _);
    }

    public void BuildBuilding(Cell cell, string buildingTypeId)
    {
        BuildingData buildingData = new BuildingData
        {
            Level = 1, 
            TypeId = buildingTypeId
        };
        SystemLocator.I.PlayerData.SetBuildingData(GetPosition(cell), buildingData);
        
        cell.DisplayBuildingView(buildingData);
    }

    public void DemolishBuilding(Cell cell)
    {
        SystemLocator.I.PlayerData.RemoveBuildingData(GetPosition(cell));
        
        cell.DemolishBuildingView();
    }
}
