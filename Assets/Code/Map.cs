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

    public void BuildBuilding(Cell currentCell, string selectedOptionId)
    {
        BuildingData buildingData = new BuildingData
        {
            Level = 1, 
            TypeId = selectedOptionId
        };
        SystemLocator.I.PlayerData.SetBuildingData(GetPosition(currentCell), buildingData);
    }
}
