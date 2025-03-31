using System;
using UnityEngine;

namespace Code
{
    internal sealed class SystemLocator : MonoBehaviour
    {
        public static SystemLocator I { get; private set; }

        public PlayerData PlayerData => _playerData;
        private PlayerData _playerData;
        
        [field:SerializeField] public PlayerController PlayerController { get; private set; }
        [field:SerializeField] public Map Map { get; private set; }
        [field:SerializeField] public ContentLibrary ContentLibrary { get; private set; }

        private void Awake()
        {
            if (I == null) I = this;
            else if (I != this) throw new InvalidOperationException("Only one instance of SystemLocator can be active at a time.");

            if (!TryLoadPlayerData(ref _playerData))
            {
                _playerData = new PlayerData();
            }
        }

        private bool TryLoadPlayerData(ref PlayerData playerData)
        {
            return false;
        }
    }
}