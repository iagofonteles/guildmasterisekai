using System.Collections;
using TileGrid.Build;
using System.Collections.Generic;
using Drafts;
using TileGrid.Tiles;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    [DisallowMultipleComponent]
    public sealed class GridBuilderUI : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private UIDocument document;
        [SerializeField, SO] private GridBuilderTools gridTools;
        public GestureArgs Args => builder.Gesture;

        private VisualElement _root;
        private ScrollView _tiles;
        private Button _undo, _redo;

        private IEnumerator Start()
        {
            document.sortingOrder = 100;
            while (!builder.IsInitialized)
                yield return null;
            yield return null;
            Bind();
            Subscribe();
            Refresh();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            Unsubscribe();
        }

        private void Subscribe()
        {
            Args.layer.OnChanged += RefreshValue;
            Args.tool.OnChanged += RefreshValue;
            Args.brush.OnChanged += RefreshValue;
            Args.item.OnChanged += RefreshValue;
            Args.color.OnChanged += RefreshValue;
            Args.lastError.OnChanged += RefreshError;
            builder.Changed += Refresh;
        }

        private void Unsubscribe()
        {
            Args.layer.OnChanged -= RefreshValue;
            Args.tool.OnChanged -= RefreshValue;
            Args.brush.OnChanged -= RefreshValue;
            Args.item.OnChanged -= RefreshValue;
            Args.color.OnChanged -= RefreshValue;
            Args.lastError.OnChanged -= RefreshError;
            builder.Changed -= Refresh;
        }

        private void Bind()
        {
            _root = document.rootVisualElement;
            _root.Clear();
            document.visualTreeAsset.CloneTree(_root);

            _root.AddToClassList("build-document");
            _root.pickingMode = PickingMode.Ignore;

            foreach (var child in _root.Children())
                child.pickingMode = PickingMode.Ignore;

            _tiles = _root.Q<ScrollView>("tiles");

            _undo = _root.Q<Button>("undo");
            _redo = _root.Q<Button>("redo");
            _undo.clicked += builder.Undo;
            _redo.clicked += builder.Redo;

            _root.Q("toolbar").RegisterCallback<WheelEvent>(e => e.StopPropagation());

            DrawLayers(_root);
            DrawBrushes(_root.Q("brushes"));
            DrawTools(_root, Args.layer.Value);
        }

        private void DrawLayers(VisualElement root)
        {
            var layers = root.Q("layers");
            layers.Clear();

            for (var i = 0; i < gridTools.Layers.Count; i++)
            {
                var lName = gridTools.Layers[i].DisplayName;
                var layer = i;
                var button = new Button(() => Args.layer.Value = layer) { text = lName };
                //button.SetEnabled(true);
                button.EnableInClassList("selected", Args.layer.Value == layer);
                layers.Add(button);
            }
        }

        private void DrawBrushes(VisualElement brushes)
        {
            foreach (var brush in gridTools.Brushes)
            {
                if (brush == null) continue;
                var button = new Button(() => Args.brush.Value = brush) { tooltip = brush.DisplayName };
                button.AddToClassList("brush-choice");
                button.EnableInClassList("selected", ReferenceEquals(Args.brush.Value, brush));

                if (brush.Icon)
                {
                    var image = new Image { sprite = brush.Icon, scaleMode = ScaleMode.ScaleToFit };
                    image.AddToClassList("brush-icon");
                    image.pickingMode = PickingMode.Ignore;
                    button.Add(image);
                }
                else button.text = brush.DisplayName;

                brushes.Add(button);
            }
        }

        private void DrawTools(VisualElement root, int layer)
        {
            var tools = root.Q("tools");
            tools.Clear();

            foreach (var tool in gridTools.Tools)
            {
                if (((int)tool.Layers & (1 << layer)) == 0) continue;
                var button = new Button(() => Args.tool.Value = tool.Left) { text = tool.DisplayName };
                button.EnableInClassList("selected", Args.tool.Value == tool.Left);
                button.SetEnabled(tool.Left != null || tool.Right != null);
                tools.Add(button);
            }
        }

        private void RefreshValue<T>(T value) => Refresh();
        private void RefreshError(string value) => RefreshStatus();

        public void Refresh()
        {
            _tiles.Clear();
            if (builder && builder.IsInitialized)
                AddItems(Args.tool.Value?.GetItems(Args));
            if (_tiles.contentContainer.childCount == 0) _tiles.Add(new Label("No available tiles on this layer."));
            RefreshStatus();
        }

        private void AddItems(IEnumerable<ISlot> slots)
        {
            if (slots == null) return;

            foreach (var slot in slots)
            {
                if (slot == null) continue;
                if (slot.Item is not Color && slot.Item is not ITile) continue;
                var element = new SlotElement
                {
                    Slot = slot,
                    SelectedItem = slot.Item is Color ? Args.color.Value : Args.item.Value
                };

                if (element.Slot.Item is Color color)
                    element.clicked += () => Args.color.Value = color;
                if (element.Slot.Item is ITile selected)
                    element.clicked += () => Args.item.Value = selected;

                _tiles.Add(element);
            }
        }

        public void RefreshStatus()
        {
            var ready = builder && builder.IsInitialized;
            _undo.SetEnabled(ready && builder.CanUndo);
            _redo.SetEnabled(ready && builder.CanRedo);
        }
    }
}