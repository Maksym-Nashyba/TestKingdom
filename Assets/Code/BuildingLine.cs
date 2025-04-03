using System;
using Code.Visualization;
using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Building")]
    internal sealed class BuildingLine : ScriptableObject
    {
        [field:SerializeField] public string Id { get; private set; }
        [field:SerializeField] public string DisplayName { get; private set; }
        [field:SerializeField] public string DisplayDescription { get; private set; }
        [field:SerializeField] public Sprite BuyMenuIcon { get; private set; }
        [field:SerializeField] public BuildingLevel[] Levels { get; private set; }
    }

    [Serializable]
    internal struct BuildingLevel
    {
        [field:SerializeField] public BuildingView ViewPrefab { get; private set; }
        [field:SerializeField] public ProductionOrder[] ProductionOrders { get; private set; }
        [field:SerializeField] public ResourceCount[] Cost { get; private set; }
    }

    [Serializable]
    internal struct ProductionOrder
    {
        public string Id;
        public string DisplayName;
        public Sprite DisplayIcon;
        public float DurationSeconds;
        public ResourceCount[] Inputs;
        public ResourceCount[] Outputs;
    }
}