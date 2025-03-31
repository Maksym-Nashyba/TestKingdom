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
        public long BuildingInstanceId;
    }
    
    [Serializable]
    struct BuildingData
    {
        public long InstanceId;
        public string TypeId;
        public byte Level;
    }
    
    internal sealed class PlayerData
    {
        private List<CellData> _cells = new List<CellData>();
        private List<BuildingData> _buildings = new List<BuildingData>();

        public bool GetBuildingData(Vector2Int position, out BuildingData buildingData)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i].Position == position)
                {
                    if (!_cells[i].HasBuilding) break;
                    buildingData = _buildings.First(building => building.InstanceId == _cells[i].BuildingInstanceId);
                    return true;
                }
            }

            buildingData = default;
            return false;
        }
    }
}