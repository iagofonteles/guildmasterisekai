namespace GuildMasterIsekai.TileGrid
{
    public sealed class BaseLayerPickTool : IBuildTool
    {
        public GridBuilder Builder { get; set; }

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            //TODO
            var item = e.grid[e.area.xMax, e.area.yMax].Floor;
            if (item) e.builder.SelectedItem = item;
            error = "";
            return null;
        }
    }
}