using System;
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
        [SerializeField] private PanelRenderer panel;
        [SerializeField, SO] private GridBuilderTools gridTools;

        private Action _unsubscribe;

        private IEnumerator Start()
        {
            panel.panelSettings.sortingOrder = 100;
            while (!builder.IsInitialized)
                yield return null;
            yield return null;
            panel.RegisterUIReloadCallback(Subscribe);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            panel.UnregisterUIReloadCallback(Subscribe);
            _unsubscribe?.Invoke();
        }

        private void Subscribe(PanelRenderer renderer, VisualElement root)
        {
            _unsubscribe?.Invoke();
            var b = builder ?? throw new NullReferenceException(nameof(builder));
            var undo = root.Q<Button>("undo");
            var redo = root.Q<Button>("redo");

            root.AddToClassList("build-document");
            root.pickingMode = PickingMode.Ignore;
            root.Q("toolbar").RegisterCallback<WheelEvent>(e => e.StopPropagation());

            foreach (var child in root.Children())
                child.pickingMode = PickingMode.Ignore;

            undo.clicked += b.Undo;
            redo.clicked += b.Redo;
            b.Changed += RefreshStatus;

            _unsubscribe += () => undo.clicked -= b.Undo;
            _unsubscribe += () => redo.clicked -= b.Redo;
            _unsubscribe += () => b.Changed -= RefreshStatus;

            _unsubscribe += DrawLayers(root, b.Build, gridTools.Layers);
            _unsubscribe += DrawBrushes(root, b.Build, gridTools.Brushes);
            _unsubscribe += DrawTools(root, b.Build, gridTools.Tools);
            _unsubscribe += DrawTiles(root, b.Build);

            void RefreshStatus()
            {
                var ready = builder && builder.IsInitialized;
                undo.SetEnabled(ready && builder.CanUndo);
                redo.SetEnabled(ready && builder.CanRedo);
            }
        }

        private static Action DrawLayers(VisualElement root, BuildArgs e, IReadOnlyList<LayerConfig> list)
        {
            var layers = root.Q("layers");
            var buttons = new List<Button>(list.Count);

            foreach (var layer in list)
            {
                var lName = layer.DisplayName;
                var button = new Button(() => e.layer.Value = layer.Index) { text = lName };
                //button.SetEnabled(true);
                button.EnableInClassList("selected", e.layer.Value == layer.Index);
                button.userData = layer.Index;
                layers.Add(button);
                buttons.Add(button);
            }

            Refresh(e.layer.Value);
            e.layer.OnChanged += Refresh;
            return () => e.layer.OnChanged -= Refresh;

            void Refresh(int i)
            {
                foreach (var button in buttons)
                    button.EnableInClassList("selected", (int)button.userData == i);
            }
        }

        private static Action DrawBrushes(VisualElement root, BuildArgs e, IReadOnlyList<IBuildBrush> list)
        {
            var brushes = root.Q("brushes");
            var buttons = new List<Button>(list.Count);

            foreach (var brush in list)
            {
                if (brush == null) continue;
                var button = new Button(() => e.brush.Value = brush) { tooltip = brush.DisplayName };
                button.AddToClassList("brush-choice");
                button.EnableInClassList("selected", e.brush.Value == brush);

                if (brush.Icon)
                {
                    var image = new Image { sprite = brush.Icon, scaleMode = ScaleMode.ScaleToFit };
                    image.AddToClassList("brush-icon");
                    image.pickingMode = PickingMode.Ignore;
                    button.Add(image);
                }
                else button.text = brush.DisplayName;

                button.userData = brush;
                brushes.Add(button);
                buttons.Add(button);
            }

            e.brush.OnChanged += Refresh;
            return () => e.brush.OnChanged -= Refresh;

            void Refresh(IBuildBrush b)
            {
                foreach (var button in buttons)
                    button.EnableInClassList("selected", ReferenceEquals(button.userData, b));
            }
        }

        private static Action DrawTools(VisualElement root, BuildArgs e, IReadOnlyList<IBuildTool> list)
        {
            var tools = root.Q("tools");
            var buttons = new List<Button>(list.Count);

            foreach (var tool in list)
            {
                var button = new Button(() => e.tool.Value = tool)
                {
                    text = tool.DisplayName,
                    userData = tool
                };
                tools.Add(button);
                buttons.Add(button);
            }

            e.layer.OnChanged += Refresh;
            e.tool.OnChanged += Refresh;
            Refresh(0);

            return () =>
            {
                e.layer.OnChanged -= Refresh;
                e.tool.OnChanged -= Refresh;
            };

            void Refresh<T>(T value)
            {
                foreach (var button in buttons)
                {
                    var tool = (IBuildTool)button.userData;

                    if (value is int layer)
                        button.visible = ((int)tool.LayerMask & (1 << layer)) > 0;

                    if (value is IBuildTool t)
                        button.EnableInClassList("selected", tool == t);
                }
            }
        }

        private Action DrawTiles(VisualElement root, BuildArgs e)
        {
            var tiles = root.Q<ScrollView>("tiles");
            Refresh(e.tool.Value);
            e.tool.OnChanged += Refresh;
            return () => e.tool.OnChanged -= Refresh;

            void Refresh(IBuildTool tool)
            {
                tiles.Clear();
                AddItems(tiles, e, e.tool.Value?.GetItems(e));

                if (tiles.contentContainer.childCount == 0)
                    tiles.Add(new Label("No available tiles on this layer."));
            }
        }

        private static void AddItems(ScrollView tiles, BuildArgs e, IEnumerable<ISlot> slots)
        {
            if (slots == null) return;

            foreach (var slot in slots)
            {
                if (slot == null) continue;
                if (slot.Item is not Color && slot.Item is not ITile) continue;
                var element = new SlotElement
                {
                    Slot = slot,
                    SelectedItem = slot.Item is Color ? e.color.Value : e.item.Value
                };

                if (element.Slot.Item is Color color)
                    element.clicked += () => e.color.Value = color;
                if (element.Slot.Item is ITile selected)
                    element.clicked += () => e.item.Value = selected;

                tiles.Add(element);
            }
        }
    }
}