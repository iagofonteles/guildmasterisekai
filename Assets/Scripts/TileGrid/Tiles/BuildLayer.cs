using System;

namespace TileGrid.Tiles
{
    public static class BuildLayer
    {
        public const int Default = 0;
        public const int Ground = 1;
        public const int Floor = 2;
        public const int Carpet = 3;
        public const int World = 4;
        public const int Roof = 5;
        public const int Count = 6;
    }

    [Flags]
    public enum BuildLayerMask
    {
        Default = 1, // water/hole
        Ground = 2, // dirt/sand
        Floor = 4, // floor/grass
        Carpet = 8, // carpet
        World = 16, // wall/furniture
        Roof = 32, // roof
        All = 63,
    }
}