using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code
{
    internal sealed class SaveSystem : MonoBehaviour
    {
        public bool IsSetUp { get; private set; }
        public GameData GameData => IsSetUp ? _gameData : throw new InvalidOperationException("Game is not set up yet!");
        private GameData _gameData;

        private string _saveFilePath; 

        private void Awake()
        {
            _saveFilePath = Application.persistentDataPath + "/SaveData.json";
            
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
            SaveGameData(GameData);
            return true;
        }
        
        private bool TryLoadGameData(ref GameData gameData)
        {
            if (!File.Exists(_saveFilePath)) return false;
            
            string serializedData = File.ReadAllText(_saveFilePath);
            GameData loadedGameData = JsonConvert.DeserializeObject<GameData>(serializedData, new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            });
            bool invalid = loadedGameData is null
                || loadedGameData.Cells is null
                || loadedGameData.Cells.Count < 1
                || loadedGameData.Resources is null
                || loadedGameData.Resources.Count < 1;

            if (invalid)
            {
                Debug.LogError("Failed to load game data");
                return false;
            }

            gameData = loadedGameData;
            return true;
        }

        private void SaveGameData(GameData gameData)
        {
            gameData.SaveTime = DateTime.Now;
            string json = JsonConvert.SerializeObject(gameData, Formatting.Indented, new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            });
            File.WriteAllText(_saveFilePath, json);
        }
    }
}