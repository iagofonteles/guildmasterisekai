using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Converts a gesture's cell endpoints into a brush area.</summary>
    public interface IBuildBrush
    {
        RectInt GetArea(Vector2Int first, Vector2Int last);
    }
}
