using System;
using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Building")]
    public class Building : ScriptableObject
    {
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public BuildingLevel[] Levels { get; private set; }
    }

    [Serializable]
    public struct BuildingLevel
    {
        [field:SerializeField] public GameObject ViewPrefab { get; private set; }
    }
}