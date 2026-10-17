namespace GuildMasterIsekai.TileGrid
{
    public interface IBuildCommand
    {
        bool Do();
        bool Undo();
    }
}
