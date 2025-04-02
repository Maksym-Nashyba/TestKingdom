using System;

namespace Code
{
    public enum ResourceType : byte
    {
        Food = 0,
        Wood = 1,
        Stone = 2,
        Metal = 3,
    }

    [Serializable]
    public struct ResourceCount
    {
        public ResourceType Type;
        public int Count;
    }
}