using System;
using Drafts.Localization;
using TileGrid.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace TileGrid.BuildTools
{
    [Serializable]
    public sealed class RectBrush : IBuildBrush
    {
        [SerializeField, Loc] private LocalizedString displayName;
        [SerializeField] private Sprite icon;

        private Vector2Int _first, _last;
        private bool _cancel;

        public string DisplayName => displayName.GetLocalizedString();
        public Sprite Icon => icon;
        public bool HasDrag => false;

        public void CancelGesture() => _cancel = true;

        public void OnMouseDown(BuildArgs e, Vector2Int pos)
        {
            _cancel = false;
            _first = _last = pos;
        }

        public void OnMouseDrag(BuildArgs e, Vector2Int pos) => _last = pos;

        public void OnMouseUp(BuildArgs e, Vector2Int pos)
        {
            if (_cancel) return;
            _last = pos;

            var min = Vector2Int.Min(_first, _last);
            var max = Vector2Int.Max(_first, _last);
            e.area = new RectInt(min, max - min + Vector2Int.one);
            e.builder.TryUseTool(e);
        }

        public void OnKeyDown(Vector2Int pos, Key key)
        {
            if (key == Key.Escape) _cancel = true;
        }

        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}