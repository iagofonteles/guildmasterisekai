using System;
using TileGrid.Build;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    public sealed class BuildColorElement : Button, IViewElement
    {
        private static readonly int PaletteTex = Shader.PropertyToID("_PaletteTex");
        private static readonly int PaletteColorCount = Shader.PropertyToID("_PaletteColorCount");
        private static readonly int PaletteShades = Shader.PropertyToID("_PaletteShades");

        private readonly VisualElement _preset;
        private readonly Texture2D _palette;
        private readonly int _colorCount;
        private readonly int _shades;
        private BuildColor _color;
        public VisualElement Element => this;

        public object Data
        {
            get => _color;
            set {
                _color = value as BuildColor;
                Refresh();
            }
        }

        public BuildColorElement(Material paletteMaterial, BuildColor data)
        {
            if (!paletteMaterial || !paletteMaterial.HasProperty(PaletteTex) ||
                !paletteMaterial.HasProperty(PaletteColorCount) || !paletteMaterial.HasProperty(PaletteShades))
                throw new ArgumentException("A configured indexed palette material is required.", nameof(paletteMaterial));
            _palette = paletteMaterial.GetTexture(PaletteTex) as Texture2D;
            if (!_palette || !_palette.isReadable)
                throw new ArgumentException("The palette texture must be readable.", nameof(paletteMaterial));
            _colorCount = Mathf.Clamp(paletteMaterial.GetInt(PaletteColorCount), 1, ColorPaletteConfig.MaxPaletteColors);
            _shades = Mathf.Clamp(paletteMaterial.GetInt(PaletteShades), 1, ColorPaletteConfig.EncodedShades);
            if (_palette.width * _palette.height < _colorCount)
                throw new ArgumentException("The palette texture has too few colors.", nameof(paletteMaterial));

            AddToClassList("tile-choice");

            _preset = new VisualElement { pickingMode = PickingMode.Ignore };
            _preset.AddToClassList("slot-preset");
            Add(_preset);
            RegisterCallback<AttachToPanelEvent>(_ => Refresh());

            _color = data;
            Refresh();
        }

        public void Refresh()
        {
            tooltip = $"{_color.Name}\nRegions: {_color.Region1Ramp}, {_color.Region2Ramp}, " +
                      $"{_color.Region3Ramp}, {_color.Region4Ramp}";
            DrawPreset(_color);
        }

        public void SetSelected(object selectedItem)
        {
            var selected = _color != null && selectedItem is BuildColor c && _color.Id == c.Id;
            EnableInClassList("selected", selected);
        }

        private void DrawPreset(BuildColor color)
        {
            _preset.Clear();
            var regions = new VisualElement { pickingMode = PickingMode.Ignore };
            regions.AddToClassList("preset-regions");
            _preset.Add(regions);
            var ids = new[] { color.Region1Ramp, color.Region2Ramp, color.Region3Ramp, color.Region4Ramp };

            foreach (var id in ids)
            {
                var ramp = new VisualElement { pickingMode = PickingMode.Ignore };
                ramp.AddToClassList("preset-region");
                regions.Add(ramp);

                if (id == 0)
                {
                    var label = new Label("x")
                        { pickingMode = PickingMode.Ignore };
                    label.AddToClassList("preset-ramp-label");
                    ramp.Add(label);
                    continue;
                }

                var rampCount = (_colorCount + _shades - 1) / _shades;
                var index = Mathf.Min((Mathf.Min((int)id, rampCount) - 1) * _shades + _shades / 2,
                    _colorCount - 1);
                ramp.style.backgroundColor = _palette.GetPixel(index % _palette.width, index / _palette.width);
            }
        }
    }
}
