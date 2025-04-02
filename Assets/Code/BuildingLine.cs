using System;
using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Building")]
    public class BuildingLine : ScriptableObject
    {
        [field:SerializeField] public string Id { get; private set; }
        [field:SerializeField] public string DisplayName { get; private set; }
        [field:SerializeField] public string DisplayDescription { get; private set; }
        [field:SerializeField] public Sprite BuyMenuIcon { get; private set; }
        [field:SerializeField] public BuildingLevel[] Levels { get; private set; }
    }

    [Serializable]
    public struct BuildingLevel
    {
        [field:SerializeField] public GameObject ViewPrefab { get; private set; }
        [field:SerializeField] public ProductionOrder[] ProductionOrders { get; private set; }
    }

    [Serializable]
    public struct ProductionOrder
    {
        public string Id;
        public string DisplayName;
        public Sprite DisplayIcon;
        public float DurationSeconds;
        public ResourceCount[] Inputs;
        public ResourceCount[] Outputs;
    }
}