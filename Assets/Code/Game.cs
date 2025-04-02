using System;
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

        private bool TryLoadPlayerData(ref GameData gameData)
        {
            return false;
        }
    }
}