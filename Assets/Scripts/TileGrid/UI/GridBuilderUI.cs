using System.Collections.Generic;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.TileGrid
{
    [DisallowMultipleComponent]
    public sealed class GridBuilderUI : MonoBehaviour
    {
        [SerializeField] private GridBuilderInput input;
        [SerializeField] private UIDocument document;
        [SerializeField] private PanelSettings panelSettings;

        private PanelSettings _ownedPanel;
        private VisualElement _root;
        private VisualElement _outline;
        private Image _cursorIcon;
        private readonly Vector2[] _outlineCorners = new Vector2[4];
        private ScrollView _tiles;
        private Button _undo, _redo;
        private GridBuilder _subscribedBuilder;
        private bool _bound, _initialized;

        private void Awake() => document.sortingOrder = 100;

        private void OnEnable()
        {
            if (!document) return;
            document.enabled = true;
        }

        private void Start()
        {
            if (!_bound) Bind();
        }

        private void Update()
        {
            if (!_bound && document && document.rootVisualElement != null) Bind();
            if (!_bound || !input.Builder) return;

            if (_subscribedBuilder != input.Builder)
            {
                if (_subscribedBuilder) _subscribedBuilder.Changed -= Refresh;
                _subscribedBuilder = input.Builder;
                _subscribedBuilder.Changed += Refresh;
            }

            if (_initialized != input.Builder.IsInitialized)
            {
                _initialized = input.Builder.IsInitialized;
                Refresh();
            }
        }

        private void LateUpdate()
        {
            if (!_bound) return;
            _outline?.MarkDirtyRepaint();
            var builder = input.Builder;
            var icon = builder
                ? input.TemporaryTool != null ? input.TemporaryTool.Icon : builder.CurrentTool?.Icon
                : null;
            var visible = input.TryGetToolCursorPosition(out var screen) && icon;
            _cursorIcon.sprite = icon;
            _cursorIcon.EnableInClassList("hidden", !visible);
            if (visible && _cursorIcon.panel != null)
            {
                var panelPoint = RuntimePanelUtils.ScreenToPanel(_cursorIcon.panel, screen);
                var point = _root.WorldToLocal(panelPoint);
                _cursorIcon.style.translate = new Translate(point.x + 12, point.y - 44);
            }
        }

        private void DrawSelection(MeshGenerationContext context)
        {
            if (!input || _outline.panel == null || !input.TryGetSelectionScreenCorners(_outlineCorners)) return;
            var painter = context.painter2D;
            painter.strokeColor = Color.yellow;
            painter.lineWidth = 2;
            painter.BeginPath();
            for (var i = 0; i < _outlineCorners.Length; i++)
            {
                var panelPoint = RuntimePanelUtils.ScreenToPanel(_outline.panel, _outlineCorners[i]);
                var point = _outline.WorldToLocal(panelPoint);
                if (i == 0) painter.MoveTo(point);
                else painter.LineTo(point);
            }
            painter.ClosePath();
            painter.Stroke();
        }

        private void Bind()
        {
            if (_bound || !document.visualTreeAsset || !input) return;
            _root = document.rootVisualElement;
            if (_root == null) return;
            _root.Clear();
            document.visualTreeAsset.CloneTree(_root);
            _root.AddToClassList("build-document");
            _root.pickingMode = PickingMode.Ignore;
            foreach (var child in _root.Children()) child.pickingMode = PickingMode.Ignore;
            _outline = new VisualElement { pickingMode = PickingMode.Ignore };
            _outline.AddToClassList("grid-selection-outline");
            _outline.generateVisualContent += DrawSelection;
            _root.Insert(0, _outline);
            _cursorIcon = new Image { pickingMode = PickingMode.Ignore, scaleMode = ScaleMode.ScaleToFit };
            _cursorIcon.AddToClassList("tool-cursor-icon");
            _cursorIcon.AddToClassList("hidden");
            _root.Add(_cursorIcon);
            _tiles = _root.Q<ScrollView>("tiles");
            if (_tiles == null) return;
            _undo = _root.Q<Button>("undo");
            _redo = _root.Q<Button>("redo");
            _root.Q<Button>("baseLayer").clicked += () => input.SetLayer(BuildLayer.Floor);
            _root.Q<Button>("furnitureLayer").clicked += () => input.SetLayer(BuildLayer.Furniture);
            _root.Q<Button>("toppingLayer").clicked += () => input.SetLayer(BuildLayer.Topping);
            _undo.clicked += input.Undo;
            _redo.clicked += input.Redo;
            var toolbar = _root.Q("toolbar");
            toolbar.RegisterCallback<WheelEvent>(evt => evt.StopPropagation());
            input.SelectionChanged += Refresh;
            input.StatusChanged += RefreshStatus;
            _bound = true;
            Refresh();
        }

        private void OnDisable()
        {
            _tiles?.Clear();
            if (input)
            {
                input.CancelGesture();
                input.SelectionChanged -= Refresh;
                input.StatusChanged -= RefreshStatus;
            }

            if (_subscribedBuilder) _subscribedBuilder.Changed -= Refresh;
            _subscribedBuilder = null;
            if (document) document.enabled = false;
            _bound = false;
            _root = null;
            _outline = null;
            _cursorIcon = null;
        }

        private void OnDestroy()
        {
            if (_ownedPanel) Destroy(_ownedPanel);
        }

        public void Refresh()
        {
            if (!_bound) return;
            Selected("baseLayer", input.Layer == BuildLayer.Floor);
            Selected("furnitureLayer", input.Layer == BuildLayer.Furniture);
            Selected("toppingLayer", input.Layer == BuildLayer.Topping);
            _root.Q<Button>("baseLayer").SetEnabled(input.HasLayer(BuildLayer.Floor));
            _root.Q<Button>("furnitureLayer").SetEnabled(input.HasLayer(BuildLayer.Furniture));
            _root.Q<Button>("toppingLayer").SetEnabled(input.HasLayer(BuildLayer.Topping));
            RefreshTools();
            _tiles.Clear();
            if (input.Builder && input.Builder.IsInitialized && input.Builder.CurrentTool?.Left != null)
                AddItems(input.Builder.CurrentTool.Left.GetItems(input.Builder.Bag));
            if (_tiles.contentContainer.childCount == 0) _tiles.Add(new Label("No available tiles on this layer."));
            RefreshStatus();
        }

        private void RefreshTools()
        {
            var tools = _root.Q("tools");
            var brushes = _root.Q("brushes");
            tools.Clear();
            brushes.Clear();
            if (!input.Builder) return;

            foreach (var tool in input.AvailableTools)
            {
                if (tool == null) continue;
                var button = new Button(() => input.SelectTool(tool)) { text = tool.DisplayName };
                button.EnableInClassList("selected", ReferenceEquals(input.Builder.CurrentTool, tool));
                button.SetEnabled(tool.Left != null || tool.Right != null);
                tools.Add(button);
            }

            foreach (var brush in input.Builder.Brushes)
            {
                if (brush == null) continue;
                var name = brush.GetType().Name;
                var button = new Button(() => input.SelectBrush(brush)) { tooltip = name };
                button.AddToClassList("brush-choice");
                button.EnableInClassList("selected", ReferenceEquals(input.Builder.CurrentBrush, brush));

                if (brush.Icon)
                {
                    var image = new Image { sprite = brush.Icon, scaleMode = ScaleMode.ScaleToFit };
                    image.AddToClassList("brush-icon");
                    image.pickingMode = PickingMode.Ignore;
                    button.Add(image);
                }
                else button.text = name;

                brushes.Add(button);
            }
        }

        private void AddItems(IEnumerable<ISlot> slots)
        {
            if (slots == null) return;
            foreach (var slot in slots)
            {
                if (slot == null) continue;
                if (slot.Item is not Color && (slot.Item is not DatabaseEntrySo item || !item)) continue;
                var element = new SlotElement { Slot = slot, SelectedItem = input.Builder.SelectedItem };
                element.clicked += () =>
                {
                    if (element.Slot.Item is Color color) input.SelectColor(color);
                    else if (element.Slot.Item is DatabaseEntrySo selected && selected) input.SelectTile(selected);
                };
                _tiles.Add(element);
            }
        }

        private void Selected(string tileName, bool selected) =>
            _root.Q<Button>(tileName).EnableInClassList("selected", selected);

        private void RefreshStatus()
        {
            if (!_bound) return;
            var ready = input.Builder && input.Builder.IsInitialized;
            _undo.SetEnabled(ready && input.Builder.CanUndo);
            _redo.SetEnabled(ready && input.Builder.CanRedo);
        }
    }
}
