using System;
using System.Collections;
using TileGrid.Build;
using System.Collections.Generic;
using System.Linq;
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

            b.BuildArgs.brush.Value = gridTools.Brushes[0];

            undo.clicked += b.Undo;
            redo.clicked += b.Redo;
            b.Changed += RefreshStatus;

            _unsubscribe += () => undo.clicked -= b.Undo;
            _unsubscribe += () => redo.clicked -= b.Redo;
            _unsubscribe += () => b.Changed -= RefreshStatus;

            _unsubscribe += DrawLayers(root, b.BuildArgs, gridTools);
            _unsubscribe += DrawBrushes(root, b.BuildArgs, gridTools);
            _unsubscribe += DrawTools(root, b.BuildArgs, gridTools);
            _unsubscribe += DrawTiles(root, b.BuildArgs);

            foreach (var child in root.Children())
                child.pickingMode = PickingMode.Ignore;

            void RefreshStatus()
            {
                var ready = builder && builder.IsInitialized;
                undo.SetEnabled(ready && builder.CanUndo);
                redo.SetEnabled(ready && builder.CanRedo);
            }
        }

        private static Action DrawLayers(VisualElement root, BuildArgs e, GridBuilderTools toolsSo)
        {
            var list = toolsSo.Layers;
            var layers = root.Q("layers");
            layers.Clear();
            var buttons = new List<Button>(list.Count);
            e.layer.Value = list[0].Index;

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

                var tMask = e.tool.Value?.LayerMask ?? 0;
                var iMask = (BuildLayerMask)(1 << i);

                if ((iMask & tMask) == 0)
                    e.tool.Value = toolsSo.Tools.First(t => (t.LayerMask & iMask) > 0);
                else e.tool.Value = e.tool.Value;
            }
        }

        private static Action DrawBrushes(VisualElement root, BuildArgs e, GridBuilderTools toolsSo)
        {
            var list = toolsSo.Brushes;
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

        private static Action DrawTools(VisualElement root, BuildArgs e, GridBuilderTools toolsSo)
        {
            var list = toolsSo.Tools;
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

            e.layer.OnChanged += Show;
            e.tool.OnChanged += Refresh;
            Show(e.layer.Value);
            Refresh(e.tool.Value);

            return () =>
            {
                e.layer.OnChanged -= Show;
                e.tool.OnChanged -= Refresh;
            };

            void Show(int layer)
            {
                foreach (var button in buttons)
                {
                    var tool = (IBuildTool)button.userData;
                    button.visible = ((int)tool.LayerMask & (1 << layer)) > 0;
                }
            }

            void Refresh(IBuildTool curr)
            {
                foreach (var button in buttons)
                {
                    var tool = (IBuildTool)button.userData;
                    button.EnableInClassList("selected", tool == curr);
                }
            }
        }

        private static Action DrawTiles(VisualElement root, BuildArgs e)
        {
            var tiles = root.Q<ScrollView>("tiles");
            var elements = new List<SlotElement>();
            Refresh(e.tool.Value);

            e.tool.OnChanged += Refresh;
            e.item.OnChanged += RefreshSelection;
            e.color.OnChanged += RefreshSelection;

            return () =>
            {
                e.tool.OnChanged -= Refresh;
                e.item.OnChanged -= RefreshSelection;
                e.color.OnChanged -= RefreshSelection;
            };

            void RefreshSelection<T>(T _)
            {
                foreach (var element in elements)
                    element.SelectedItem = e.item.Value;
            }

            void Refresh(IBuildTool tool)
            {
                tiles.Clear();
                elements.Clear();
                AddItems(tiles, e, tool.GetItems(e), elements);

                if (tiles.contentContainer.childCount == 0)
                    tiles.Add(new Label("No available tiles on this layer."));
            }
        }

        private static void AddItems(ScrollView tiles, BuildArgs e, IEnumerable<ISlot> slots, List<SlotElement> elements)
        {
            if (slots == null) return;

            foreach (var slot in slots)
            {
                if (slot == null) continue;
                var element = new SlotElement
                {
                    Slot = slot,
                    SelectedItem = e.item.Value
                };

                if (slot.Item is Color color)
                    element.clicked += () => e.color.Value = color;
                element.clicked += () => e.item.Value = slot.Item;

                elements.Add(element);
                tiles.Add(element);
            }
        }
    }
}
