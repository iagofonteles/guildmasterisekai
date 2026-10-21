using UnityEngine;
using UnityEngine.InputSystem;

namespace TileGrid.Build
{
    public interface IBuildBrush
    {
        string DisplayName { get; }
        Sprite Icon { get; }
        bool HasDrag { get; }

        void OnMouseDown(GestureArgs e, Vector2Int pos);
        void OnMouseDrag(GestureArgs e, Vector2Int pos);
        void OnMouseUp(GestureArgs e, Vector2Int pos);
        void OnKeyDown(Vector2Int pos, Key key);
        void OnKeyUp(Vector2Int pos, Key key);
    }
}