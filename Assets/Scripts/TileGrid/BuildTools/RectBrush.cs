using System;
using Drafts.Localization;
using TileGrid.Build;
using UnityEngine;
using UnityEngine.Localization;

namespace TileGrid.BuildTools
{
    [Serializable]
    public sealed class RectBrush : IBuildBrush
    {
        [SerializeField, Loc] private LocalizedString displayName;
        [SerializeField] private Sprite icon;

        private Vector2Int _first, _last;

        public string DisplayName => displayName.GetLocalizedString();
        public Sprite Icon => icon;
        public bool HasDrag => false;

        public void OnMouseDown(BuildArgs e, Vector2Int pos)
        {
            e.cancel = false;
            _first = _last = pos;
        }

        public void OnMouseDrag(BuildArgs e, Vector2Int pos) => _last = pos;

        public void OnMouseUp(BuildArgs e, Vector2Int pos)
        {
            if (e.cancel) return;
            _last = pos;

            var min = Vector2Int.Min(_first, _last);
            var max = Vector2Int.Max(_first, _last);
            e.area = new RectInt(min, max - min + Vector2Int.one);
            e.builder.TryUseTool(e);
        }
    }
}