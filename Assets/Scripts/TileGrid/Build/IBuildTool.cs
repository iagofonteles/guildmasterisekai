namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Prepares a command without changing the live grid or inventory.</summary>
    public interface IBuildTool
    {
        public GridBuilder Builder { get; set; }

        /// <returns>Null on error</returns>
        IBuildCommand CreateCommand(OperationArgs e, out string error);
    }
}