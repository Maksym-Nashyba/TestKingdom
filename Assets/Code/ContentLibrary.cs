using System.Linq;
using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "ContentLibrary", menuName = "ScriptableObjects/ContentLibrary")]
    public class ContentLibrary : ScriptableObject
    {
        [field: SerializeField] public BuildingLine[] Buildings;

        public BuildingLine GetBuilding(string buildingDataId)
        {
            return Buildings.First(building => building.Id == buildingDataId);
        }
    }
}