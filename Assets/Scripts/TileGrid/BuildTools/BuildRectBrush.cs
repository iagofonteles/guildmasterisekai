using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class BuildRectBrush : IBuildBrush
    {
        /// <summary>Includes both endpoints, regardless of drag direction.</summary>
        public RectInt GetArea(Vector2Int first, Vector2Int last) =>
            new(Vector2Int.Min(first, last), Vector2Int.Max(first, last) - Vector2Int.Min(first, last) + Vector2Int.one);
    }
}
