using System;
using UnityEngine;

namespace Code
{
    internal sealed class SystemLocator : MonoBehaviour
    {
        public static SystemLocator I { get; private set; }

        public GameData GameData => Game.GameData; 
        
        [field:SerializeField] public Game Game { get; private set; }
        [field:SerializeField] public PlayerController PlayerController { get; private set; }
        [field:SerializeField] public Map Map { get; private set; }
        [field:SerializeField] public ContentLibrary ContentLibrary { get; private set; }

        private void Awake()
        {
            if (I == null) I = this;
            else if (I != this) throw new InvalidOperationException("Only one instance of SystemLocator can be active at a time.");
        }
    }
}