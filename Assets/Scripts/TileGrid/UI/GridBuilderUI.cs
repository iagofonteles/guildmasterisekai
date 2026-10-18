using System;
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
        private ScrollView _tiles;
        private Button _undo, _redo;
        private GridBuilder _subscribedBuilder;
        private UIDocument[] _documents = Array.Empty<UIDocument>();
        private bool _bound, _initialized;

        private void Awake()
        {
            if (!input) input = GetComponent<GridBuilderInput>();
            if (!document) document = GetComponent<UIDocument>();
            if (!document) document = gameObject.AddComponent<UIDocument>();
            if (panelSettings) document.panelSettings = panelSettings;

            if (!document.panelSettings)
            {
                _ownedPanel = ScriptableObject.CreateInstance<PanelSettings>();
                _ownedPanel.name = "TileGrid runtime panel";
                _ownedPanel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                _ownedPanel.referenceResolution = new Vector2Int(1280, 720);
                _ownedPanel.themeStyleSheet = Resources.Load<ThemeStyleSheet>("TileGrid/GridBuilderTheme");
                document.panelSettings = _ownedPanel;
            }

            if (!document.visualTreeAsset)
                document.visualTreeAsset = Resources.Load<VisualTreeAsset>("TileGrid/GridBuilder");
            document.sortingOrder = 100;
        }

        private void OnEnable()
        {
            if (!document) return;
            document.enabled = true;
            _documents = FindObjectsByType<UIDocument>(FindObjectsInactive.Include);
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
            _tiles = _root.Q<ScrollView>("tiles");
            if (_tiles == null) return;
            _undo = _root.Q<Button>("undo");
            _redo = _root.Q<Button>("redo");
            _root.Q<Button>("baseLayer").clicked += () => input.SetLayer(BuildLayer.Base);
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
        }

        private void OnDestroy()
        {
            if (_ownedPanel) Destroy(_ownedPanel);
        }

        public bool BlocksWorldInput(Vector2 screen, bool refreshDocuments = false)
        {
            if (refreshDocuments)
                _documents = FindObjectsByType<UIDocument>(FindObjectsInactive.Include);

            foreach (var candidate in _documents)
            {
                if (!candidate || !candidate.isActiveAndEnabled) continue;
                var root = candidate.rootVisualElement;
                var panel = root?.panel;
                if (panel == null) continue;
                // Documents may share a panel; only count hits inside this document's controls.
                var controls = candidate == document ? root.Q("toolbar") : root;
                if (controls == null) continue;
                var captured = panel.GetCapturingElement(PointerId.mousePointerId) as VisualElement;
                if (captured != null && (captured == controls || controls.Contains(captured))) return true;
                var position = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(screen.x, Screen.height - screen.y));
                var picked = panel.Pick(position);
                if (picked != null && (picked == controls || controls.Contains(picked)) &&
                    (candidate == document || picked != root)) return true;
            }

            return false;
        }

        public void Refresh()
        {
            if (!_bound) return;
            Selected("baseLayer", input.Layer == BuildLayer.Base);
            Selected("furnitureLayer", input.Layer == BuildLayer.Furniture);
            Selected("toppingLayer", input.Layer == BuildLayer.Topping);
            _root.Q<Button>("baseLayer").SetEnabled(input.HasLayer(BuildLayer.Base));
            _root.Q<Button>("furnitureLayer").SetEnabled(input.HasLayer(BuildLayer.Furniture));
            _root.Q<Button>("toppingLayer").SetEnabled(input.HasLayer(BuildLayer.Topping));
            RefreshTools();
            _tiles.Clear();
            if (input.Builder && input.Builder.IsInitialized && input.Builder.CurrentTool?.Main != null)
                AddItems(input.Builder.CurrentTool.Main.GetItems());
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
                button.SetEnabled(tool.Main != null || tool.Alt != null);
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
                if (slot == null || slot.Item is not DatabaseEntrySo item || !item) continue;
                var element = new SlotElement { Slot = slot, SelectedItem = input.SelectedTile };
                element.clicked += () =>
                {
                    if (element.Slot.Item is DatabaseEntrySo selected && selected) input.SelectTile(selected);
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