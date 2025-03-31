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

        public BuildingLine GetBuilding(string buildingDataId)
        {
            return Buildings.First(building => building.Id == buildingDataId);
        }
    }
}