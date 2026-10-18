using System;
using System.Collections.Generic;
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
        private Label _selection, _help, _status;
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
            Bind();
        }

        private void Start() { if (!_bound) Bind(); }

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
            _tiles = _root.Q<ScrollView>("tiles");
            if (_tiles == null) return;
            _selection = _root.Q<Label>("selection");
            _help = _root.Q<Label>("help");
            _status = _root.Q<Label>("status");
            _undo = _root.Q<Button>("undo");
            _redo = _root.Q<Button>("redo");
            _root.Q<Button>("baseLayer").clicked += () => input.SetLayer(BuildLayer.Base);
            _root.Q<Button>("furnitureLayer").clicked += () => input.SetLayer(BuildLayer.Furniture);
            _root.Q<Button>("toppingLayer").clicked += () => input.SetLayer(BuildLayer.Topping);
            _root.Q<Button>("place").clicked += () => input.SetAction(GridBuildAction.Place);
            _root.Q<Button>("paint").clicked += () => input.SetAction(GridBuildAction.Paint);
            _root.Q<Button>("move").clicked += () => input.SetAction(GridBuildAction.Move);
            _root.Q<Button>("pixel").clicked += () => input.SetBrush(GridBuildBrush.Pixel);
            _root.Q<Button>("rectangle").clicked += () => input.SetBrush(GridBuildBrush.Rectangle);
            _undo.clicked += input.Undo;
            _redo.clicked += input.Redo;
            var toolbar = _root.Q("toolbar");
            toolbar.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());
            toolbar.RegisterCallback<PointerUpEvent>(evt => evt.StopPropagation());
            toolbar.RegisterCallback<WheelEvent>(evt => evt.StopPropagation());
            input.SelectionChanged += Refresh;
            input.StatusChanged += RefreshStatus;
            _bound = true;
            Refresh();
        }

        private void OnDisable()
        {
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

        private void OnDestroy() { if (_ownedPanel) Destroy(_ownedPanel); }

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
                if (panel.GetCapturingElement(PointerId.mousePointerId) != null) return true;
                var position = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(screen.x, Screen.height - screen.y));
                var picked = panel.Pick(position);
                if (picked != null && picked != panel.visualTree) return true;
            }
            return false;
        }

        public void Refresh()
        {
            if (!_bound) return;
            Selected("baseLayer", input.Layer == BuildLayer.Base);
            Selected("furnitureLayer", input.Layer == BuildLayer.Furniture);
            Selected("toppingLayer", input.Layer == BuildLayer.Topping);
            Selected("place", input.Action == GridBuildAction.Place);
            Selected("paint", input.Action == GridBuildAction.Paint);
            Selected("move", input.Action == GridBuildAction.Move);
            Selected("pixel", input.Brush == GridBuildBrush.Pixel);
            Selected("rectangle", input.Brush == GridBuildBrush.Rectangle);
            _root.Q<Button>("paint").SetEnabled(input.Layer == BuildLayer.Base);
            _root.Q<Button>("move").SetEnabled(input.Layer == BuildLayer.Furniture);
            _root.Q<Button>("rectangle").SetEnabled(input.Layer != BuildLayer.Topping || input.Action != GridBuildAction.Place);
            _tiles.Clear();
            if (input.Builder && input.Builder.IsInitialized)
            {
                if (input.Layer == BuildLayer.Base) AddTiles(input.Builder.GetAvailable<BaseTileSo>());
                else if (input.Layer == BuildLayer.Furniture) AddTiles(input.Builder.GetAvailable<FurnitureTileSo>());
                else AddTiles(input.Builder.GetAvailable<ToppingTileSo>());
            }
            if (_tiles.contentContainer.childCount == 0) _tiles.Add(new Label("No available tiles on this layer."));
            _selection.text = input.SelectedTile ? input.SelectedTile.name : "Select a tile below.";
            _help.text = input.Action == GridBuildAction.Paint
                ? "Left-click paints. Right-click picks the tile material."
                : input.Action == GridBuildAction.Move
                ? input.Brush == GridBuildBrush.Pixel
                    ? "Drag furniture to its destination. Right-click cancels."
                    : "Drag a source rectangle, then click its new bottom-left cell. Right-click cancels."
                : input.Layer == BuildLayer.Topping && input.Action == GridBuildAction.Place
                    ? "Click or drag over furniture to fill its first free topping slot."
                    : input.Brush == GridBuildBrush.Rectangle
                        ? "Left-drag places a rectangle. Right-drag removes a rectangle."
                        : "Left-click or drag places tiles. Right-click or drag removes tiles.";
            RefreshStatus();
        }

        private void AddTiles<T>(IReadOnlyList<T> tiles) where T : DatabaseEntrySo
        {
            foreach (var item in tiles)
            {
                if (!item) continue;
                var button = new Button(() => input.SelectTile(item));
                button.AddToClassList("tile-choice");
                button.EnableInClassList("selected", input.SelectedTile == item);
                if (item.Icon)
                {
                    var image = new Image { sprite = item.Icon, scaleMode = ScaleMode.ScaleToFit };
                    image.AddToClassList("tile-icon");
                    image.pickingMode = PickingMode.Ignore;
                    button.Add(image);
                }
                button.Add(new Label(item is BaseTileSo floor ? $"{item.name}" : item.name));
                _tiles.Add(button);
            }
        }

        private void Selected(string name, bool selected) => _root.Q<Button>(name).EnableInClassList("selected", selected);

        private void RefreshStatus()
        {
            if (!_bound) return;
            var ready = input.Builder && input.Builder.IsInitialized;
            _undo.SetEnabled(ready && input.Builder.CanUndo);
            _redo.SetEnabled(ready && input.Builder.CanRedo);
            _status.text = ready ? input.Status ?? "Ready" : "Waiting for the grid to initialize.";
        }
    }
}
