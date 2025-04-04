using System;

namespace Code
{
    public enum ResourceType : byte
    {
        Food = 0,
        Wood = 1,
        Stone = 2,
        Metal = 3,
        Water = 4, 
        Sugar = 5,
        Mana = 6,
        Crystal = 7,
    }

    [Serializable]
    public struct ResourceCount
    {
        public ResourceType Type;
        public int Count;
    }
}