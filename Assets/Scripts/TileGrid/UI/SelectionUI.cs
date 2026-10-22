using TileGrid.Build;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    public sealed class SelectionUI : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private GridBuilder builder;
        [SerializeField] private PanelRenderer document;
        [SerializeField] private Camera camera;
        [SerializeField] private Color outlineColor1 = Color.yellow;
        [SerializeField] private Color outlineColor2 = Color.softRed;

        private Image _cursorIcon;
        private VisualElement _outline;

        private void Start()
        {
            enabled = false;
            document.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDestroy()
        {
            document.UnregisterUIReloadCallback(OnUIReload);
            _outline?.RemoveFromHierarchy();
            _cursorIcon?.RemoveFromHierarchy();
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            if (root == null) return;
            _outline?.RemoveFromHierarchy();
            _cursorIcon?.RemoveFromHierarchy();

            _outline = new VisualElement { pickingMode = PickingMode.Ignore };
            _outline.AddToClassList("grid-selection-outline");
            _outline.generateVisualContent += DrawSelection;

            _cursorIcon = new Image { pickingMode = PickingMode.Ignore, scaleMode = ScaleMode.ScaleToFit };
            _cursorIcon.AddToClassList("tool-cursor-icon");
            _cursorIcon.AddToClassList("hidden");

            root.Add(_cursorIcon);
            root.Insert(0, _outline);
            enabled = true;
        }

        public void SetCursor(Vector2 screenPos, Sprite sprite)
        {
            if (!isActiveAndEnabled) return;
            _cursorIcon.sprite = sprite;
            _cursorIcon.EnableInClassList("hidden", !sprite);

            var point = RuntimePanelUtils.ScreenToPanel(_cursorIcon.panel, screenPos);
            point = _cursorIcon.parent.WorldToLocal(point);
            _cursorIcon.style.translate = new Vector3(point.x + 16, point.y + 16, 0);
        }

        public void UpdateArea()
        {
            if (!isActiveAndEnabled) return;
            _outline.MarkDirtyRepaint();
        }

        private void DrawSelection(MeshGenerationContext context)
        {
            if (builder.BuildArgs.area == default) return;
            var area = ToScreenPosition(builder.BuildArgs.area);
            var painter = context.painter2D;
            var rb = builder.BuildArgs.button == MouseButton.RightMouse;
            painter.strokeColor = rb ? outlineColor2 : outlineColor1;
            painter.lineWidth = 2;
            painter.BeginPath();

            for (var i = 0; i < 4; i++)
            {
                var screen = new Vector2(
                    i is 0 or 3 ? area.xMin : area.xMax,
                    i < 2 ? area.yMin : area.yMax);
                var panelPoint = RuntimePanelUtils.ScreenToPanel(_outline.panel, screen);
                var point = _outline.WorldToLocal(panelPoint);
                if (i == 0) painter.MoveTo(point);
                else painter.LineTo(point);
            }

            painter.ClosePath();
            painter.Stroke();
        }

        private Rect ToScreenPosition(RectInt a)
        {
            var o = tilemap.origin;
            var s = tilemap.cellSize / 2;
            var minInt = new Vector3Int(a.x + o.x, a.y + o.y, 0);
            var maxInt = new Vector3Int(a.x + a.width + o.x, a.y + a.height + o.y, 0);
            var min = tilemap.GetCellCenterWorld(minInt);
            var max = tilemap.GetCellCenterWorld(maxInt);
            min = camera.WorldToScreenPoint(min - s);
            max = camera.WorldToScreenPoint(max - s);
            min.y = Screen.height - min.y;
            max.y = Screen.height - max.y;
            return new Rect(min, max - min);
        }
    }
}