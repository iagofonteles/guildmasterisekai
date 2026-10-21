using TileGrid.Build;
using UnityEngine;

namespace TileGrid.BuildTools
{
    public sealed class TilePickTool
    {
        public IBuildCommand CreateCommand(BuildArgs e)
        {
            var pos = e.area.max - Vector2Int.one;
            var cell = e.grid[pos.x, pos.y];
            var curr = cell.Tiles[e.layer.Value];
            if (curr == null) return null;

            e.item.Value = curr;
            return null;
        }
    }
}