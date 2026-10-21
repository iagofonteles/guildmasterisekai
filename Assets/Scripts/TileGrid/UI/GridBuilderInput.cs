using TileGrid.Build;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class GridBuilderInput : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private PanelRenderer document;
        private BuildArgs Args => builder.BuildArgs;

        private VisualElement _outline;
        private Image _cursorIcon;
        // track mouseDrag
        // se ui.selectedArea.value
        //call builder.

        private void Start()
        {
            Subscribe();
            document.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDestroy()
        {
            document.UnregisterUIReloadCallback(OnUIReload);
            Unsubscribe();
            _outline?.RemoveFromHierarchy();
            _cursorIcon?.RemoveFromHierarchy();
        }

        private void Subscribe()
        {
            Args.brush.OnChanged += CancelDrag;
            Args.layer.OnChanged += CancelDrag;
            Args.tool.OnChanged += CancelDrag;
        }
        
        private void Unsubscribe()
        {
            Args.brush.OnChanged -= CancelDrag;
            Args.layer.OnChanged -= CancelDrag;
            Args.tool.OnChanged -= CancelDrag;
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
        }

        private void CancelDrag<T>(T _) => CancelDrag();

        private void CancelDrag()
        {
            
        }

        private void SetSelection(MouseButton button, RectInt area)
        {
            Args.area = area;
            Args.button = button;
            _outline?.MarkDirtyRepaint();
        }

        private void DrawSelection(MeshGenerationContext context)
        {
            if (Args.area == default) return;
            var area = ToScreenPosition(Args.area);
            var painter = context.painter2D;
            painter.strokeColor = Color.yellow;
            painter.lineWidth = 2;
            painter.BeginPath();

            for (var i = 0; i < 4; i++)
            {
                var screen = new Vector2(i == 0 || i == 3 ? area.xMin : area.xMax,
                    i < 2 ? area.yMin : area.yMax);
                var panelPoint = RuntimePanelUtils.ScreenToPanel(_outline.panel, screen);
                var point = _outline.WorldToLocal(panelPoint);
                if (i == 0) painter.MoveTo(point);
                else painter.LineTo(point);
            }

            painter.ClosePath();
            painter.Stroke();
        }

        private Rect ToScreenPosition(RectInt area)
        {
            //TODO builder.Grid.Origin
            throw new System.NotImplementedException();
        }
    }
}
