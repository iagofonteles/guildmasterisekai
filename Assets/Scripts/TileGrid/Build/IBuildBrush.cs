using UnityEngine;
using UnityEngine.InputSystem;

namespace TileGrid.Build
{
    public interface IBuildBrush
    {
        string DisplayName { get; }
        Sprite Icon { get; }
        bool HasDrag { get; }

        void OnMouseDown(BuildArgs e, Vector2Int pos);
        void OnMouseDrag(BuildArgs e, Vector2Int pos);
        void OnMouseUp(BuildArgs e, Vector2Int pos);
    }
}