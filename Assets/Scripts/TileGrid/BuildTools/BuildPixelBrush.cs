using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class BuildPixelBrush : IBuildBrush
    {
        public RectInt GetArea(Vector2Int first, Vector2Int last) => new(last, Vector2Int.one);
    }
}
