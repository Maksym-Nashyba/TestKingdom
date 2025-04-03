﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Code.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
    [Serializable]
    struct CellData
    {
        public bool HasBuilding;
        public BuildingData Building;
        public bool HasOrder;
        public OrderData Order;
    }

    [Serializable]
    struct BuildingData
    {
        public string TypeId;
        public byte Level;
    }
    
    [Serializable]
    struct OrderData
    {
        public string TypeId;
        public DateTime StartTime;
    }
    
    [Serializable]
    internal sealed class GameData : IDisposable
    {
        [field:NonSerialized] public bool IsDirty { get; private set; }
        
        public readonly ObservableDictionary<Vector2Int, CellData> Cells;  //All data mush be of value types to ensure 
        public readonly ObservableDictionary<ResourceType, int> Resources; //that CollectionChanged events are emitted correctly
        
        public GameData(IEnumerable<Vector2Int> cellPositions)
        {
            Cells = new ObservableDictionary<Vector2Int, CellData>(
                cellPositions.ToDictionary(x => x, _ => new CellData()));
            Resources = new ObservableDictionary<ResourceType, int>(
                Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>().ToDictionary(x => x, _ => 0));

            Cells.PropertyChanged += (_, _) => MarkDirty();
            Cells.CollectionChanged += (_, _) => MarkDirty();
            Resources.PropertyChanged += (_, _) => MarkDirty();
            Resources.CollectionChanged += (_, _) => MarkDirty();
            
            Cells[new Vector2Int(0, 0)] = new CellData
            {
                HasBuilding = true,
                Building = new BuildingData
                {
                    TypeId = "Castle",
                    Level = 0
                }
            };

            for (int i = 0; i < 3; i++)
            {
                Vector2Int position = Vector2Int.zero;
                while (!Cells.ContainsKey(position) || Cells[position].HasBuilding)
                {
                    position = new Vector2Int(Random.Range(-5, 5), Random.Range(-5, 5));
                }
                Cells[position] = new CellData
                {
                    HasBuilding = true,
                    Building = new BuildingData
                    {
                        TypeId = "Ruins",
                        Level = 0
                    }
                };
            }
            
            for (int i = 0; i < 2; i++)
            {
                Vector2Int position = Vector2Int.zero;
                while (!Cells.ContainsKey(position) || Cells[position].HasBuilding)
                {
                    position = new Vector2Int(Random.Range(-5, 5), Random.Range(-5, 5));
                }
                Cells[position] = new CellData
                {
                    HasBuilding = true,
                    Building = new BuildingData
                    {
                        TypeId = "Ruins",
                        Level = 1
                    }
                };
            }
            
            MarkDirty();
        }

        public void Dispose()
        {
            Cells.Dispose();
            Resources.Dispose();
        }

        private void MarkDirty() => IsDirty = true;
    }
}