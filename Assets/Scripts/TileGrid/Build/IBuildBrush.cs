using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GuildMasterIsekai.TileGrid
{
    public interface IBuildBrush
    {
        GridBuilder Builder { set; }
        Sprite Icon { get; }

        void OnMouseDown(Vector2Int pos, MouseButton button);
        void OnMouseDrag(Vector2Int pos, MouseButton button);
        void OnMouseUp(Vector2Int pos, MouseButton button);
        void OnKeyDown(Vector2Int pos, Key key);
        void OnKeyUp(Vector2Int pos, Key key);
    }
}
