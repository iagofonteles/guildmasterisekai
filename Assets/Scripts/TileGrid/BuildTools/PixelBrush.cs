using System;
using Drafts.Localization;
using TileGrid.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace TileGrid.BuildTools
{
    [Serializable]
    public sealed class PixelBrush : IBuildBrush
    {
        [SerializeField,Loc] private LocalizedString displayName;
        [SerializeField] private Sprite icon;

        public string DisplayName => displayName.GetLocalizedString();
        public Sprite Icon => icon;
        public bool HasDrag => true;

        public void CancelGesture() { }

        public void OnMouseDown(BuildArgs e, Vector2Int pos)
        {
            e.area = new(pos, Vector2Int.one);
            e.builder.TryUseTool(e);
        }

        public void OnMouseDrag(BuildArgs e, Vector2Int pos)
        {
            e.area = new(pos, Vector2Int.one);
            e.builder.TryUseTool(e);
        }

        public void OnMouseUp(BuildArgs e, Vector2Int pos) { }
        public void OnKeyDown(Vector2Int pos, Key key) { }
        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}