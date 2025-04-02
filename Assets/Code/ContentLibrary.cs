using System;
using System.Linq;
using Code.UI;
using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "ContentLibrary", menuName = "ScriptableObjects/ContentLibrary")]
    internal sealed class ContentLibrary : ScriptableObject
    {
        [field: SerializeField] public BuildingLine[] Buildings { get; private set; }
        
        [field: SerializeField] public BuildOptionUI BuildOptionUIPrefab {get; private set;}
        [field: SerializeField] public ResourceCounterUI ResourceCounterUIPrefab {get; private set;}
        [field: SerializeField] public Sprite[] ResourceIcons { get; private set; } = new Sprite[Enum.GetValues(typeof(ResourceType)).Length];

        public BuildingLine GetBuilding(string buildingTypeId)
        {
            return Buildings.First(building => building.Id == buildingTypeId);
        }
        
        public ProductionOrder GetOrder(string buildingTypeId, string orderTypeId)
        {
            BuildingLine building = GetBuilding(buildingTypeId);
            return building.Levels
                .SelectMany(level => level.ProductionOrders)
                .First(order => order.Id == orderTypeId);
        }
    }
}