using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    internal sealed class Game : MonoBehaviour
    {
        public bool IsSetUp { get; private set; }
        public GameData GameData => IsSetUp ? _gameData : throw new InvalidOperationException("Game is not set up yet!");
        private GameData _gameData;

        private void Awake()
        {
            if (!TryLoadPlayerData(ref _gameData))
            {
                _gameData = new GameData(SystemLocator.I.Map.GetPlayableCellPositions());
            }
            
            IsSetUp = true;
        }

        private void Update()
        {
            DateTime now = DateTime.Now;
            
            foreach (Vector2Int position in SystemLocator.I.Map.PlayablePositions)
            {
                CellData cellData = GameData.Cells[position];
                if (!cellData.HasOrder) continue;

                ProductionOrder order = SystemLocator.I.ContentLibrary.GetOrder(cellData.Building.TypeId, cellData.Order.TypeId); 
                float secondsPassed = now.Subtract(cellData.Order.StartTime).Seconds;
                if (secondsPassed < order.DurationSeconds) return;

                cellData.Order.StartTime = now.Subtract(TimeSpan.FromSeconds(secondsPassed%order.DurationSeconds));
                
                Spend(order.Inputs);
                Aquire(order.Outputs);
            }
        }

        public void Spend(params ResourceCount[] resourceCounts)
        {
            for (int i = 0; i < resourceCounts.Length; i++)
            {
                GameData.Resources[resourceCounts[i].Type] -= resourceCounts[i].Count;
            }
        }
        
        public void Aquire(params ResourceCount[] resourceCounts)
        {
            for (int i = 0; i < resourceCounts.Length; i++)
            {
                GameData.Resources[resourceCounts[i].Type] += resourceCounts[i].Count;
            }
        }
        
        private bool TryLoadPlayerData(ref GameData gameData)
        {
            return false;
        }
    }
}