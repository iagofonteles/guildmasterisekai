namespace GuildMasterIsekai.TileGrid
{
    // floor 0/1/2 are interchangeable
    public enum BuildLayer
    {
        Default, // water or hole
        Floor0, // ground
        Floor1, // floor
        Floor2, // carpet
        World, // wall/furniture
        Roof
    }
}