namespace TileGrid.Build
{
    public interface IBuildCommand
    {
        bool Do();
        bool Undo();
    }
}
