using System;
using System.Collections.Generic;
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
        [field: SerializeField] public ProductionOptionUI ProductionOptionUIPrefab {get; private set;}
        [field: SerializeField] public ResourceCounterUI ResourceCounterUIPrefab {get; private set;}
        [field: SerializeField] public Sprite[] ResourceIcons { get; private set; } = new Sprite[Enum.GetValues(typeof(ResourceType)).Length];

        public BuildingLine GetBuilding(string buildingTypeId)
        {
            return Buildings.First(building => building.Id == buildingTypeId);
        }
        
        public ProductionOrder GetOrder(string buildingTypeId, string orderTypeId)
        {
            return EnumerateOrders(buildingTypeId).First(order => order.Id == orderTypeId);
        }

        public IEnumerable<ProductionOrder> EnumerateOrders(string buildingTypeId)
        {
            return GetBuilding(buildingTypeId).Levels.SelectMany(level => level.ProductionOrders);
        }
    }
}