using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GuildMasterIsekai.TileGrid
{
    public interface IBuildBrush
    {
        GridBuilder Builder { set; }
        Sprite Icon { get; }
        bool IsPixel { get; }

        void OnMouseDown(IBuildTool tool, Vector2Int pos, MouseButton button);
        void OnMouseDrag(IBuildTool tool, Vector2Int pos, MouseButton button);
        void OnMouseUp(IBuildTool tool, Vector2Int pos, MouseButton button);
        void OnKeyDown(Vector2Int pos, Key key);
        void OnKeyUp(Vector2Int pos, Key key);
    }
}
