using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    internal sealed class SaveSystem : MonoBehaviour
    {
        public bool IsSetUp { get; private set; }
        public GameData GameData => IsSetUp ? _gameData : throw new InvalidOperationException("Game is not set up yet!");
        private GameData _gameData;

        private void Awake()
        {
            if (!TryLoadGameData(ref _gameData))
            {
                _gameData = new GameData(SystemLocator.I.Game.GetPlayableCellPositions());
            }
            
            Application.wantsToQuit += OnQuitRequested;
            IsSetUp = true;
        }

        private void OnDestroy()
        {
            Application.wantsToQuit -= OnQuitRequested;
        }

        private bool OnQuitRequested()
        {

            
            
            return true;
        }
        
        private bool TryLoadGameData(ref GameData gameData)
        {
            return false;
        }

        private void SaveGameData(GameData gameData)
        {
            
        }
    }
}