using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code
{
    [Serializable]
    struct CellData
    {
        public Vector2Int Position;
        public bool HasBuilding;
        public BuildingData Building;
    }

    [Serializable]
    struct BuildingData
    {
        public string TypeId;
        public byte Level;
    }
    
    [Serializable]
    internal sealed class PlayerData
    {
        [field:NonSerialized] public bool IsDirty { get; private set; } 
        
        private List<CellData> _cells = new List<CellData>();
        
        public void SetBuildingData(Vector2Int position, BuildingData buildingData)
        {
            IsDirty = true;
            
            if (_cells.All(cell => cell.Position != position))
            {
                _cells.Add(new CellData
                {
                    Position = position, 
                    HasBuilding = true, 
                    Building = buildingData
                });
                return;
            }
            
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i].Position == position) continue;

                _cells[i] = new CellData
                {
                    Position = position,
                    HasBuilding = true,
                    Building = buildingData
                };
            }
        }
        
        public bool GetBuildingData(Vector2Int position, out BuildingData buildingData)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i].Position == position)
                {
                    if (!_cells[i].HasBuilding) break;
                    buildingData = _cells[i].Building;
                    return true;
                }
            }

            buildingData = default;
            return false;
        }
    }
}