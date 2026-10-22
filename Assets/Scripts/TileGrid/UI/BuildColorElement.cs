using TileGrid.Build;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    public sealed class BuildColorElement : Button, IViewElement
    {
        private static readonly int PaletteTex = Shader.PropertyToID("_PaletteTex");
        private static readonly int RampTex = Shader.PropertyToID("_RampTex");

        private readonly VisualElement _preset;
        private readonly Texture2D _palette;
        private readonly Texture2D _ramps;
        private BuildColor _color;
        private bool _subscribed;

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
            if (paletteMaterial && paletteMaterial.HasProperty(PaletteTex) &&
                paletteMaterial.HasProperty(RampTex))
            {
                var palette = paletteMaterial.GetTexture(PaletteTex) as Texture2D;
                var ramps = paletteMaterial.GetTexture(RampTex) as Texture2D;

                if (palette && ramps && palette.isReadable && ramps.isReadable)
                {
                    _palette = palette;
                    _ramps = ramps;
                }
            }

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
            var name = new Label(string.IsNullOrEmpty(color.Name) ? "Preset" : color.Name)
                { pickingMode = PickingMode.Ignore };
            name.AddToClassList("preset-name");
            _preset.Add(name);
            var regions = new VisualElement { pickingMode = PickingMode.Ignore };
            regions.AddToClassList("preset-regions");
            _preset.Add(regions);
            var ids = new[] { color.Region1Ramp, color.Region2Ramp, color.Region3Ramp, color.Region4Ramp };

            foreach (var id in ids)
            {
                var ramp = new VisualElement { pickingMode = PickingMode.Ignore };
                ramp.AddToClassList("preset-region");
                regions.Add(ramp);

                if (id == 0 || !_palette || !_ramps)
                {
                    var label = new Label(id == 0 ? "Original" : $"Ramp {id}")
                        { pickingMode = PickingMode.Ignore };
                    label.AddToClassList("preset-ramp-label");
                    ramp.Add(label);
                    continue;
                }

                for (var shade = 0; shade < _ramps.width; shade++)
                {
                    var index = Mathf.RoundToInt(_ramps.GetPixel(shade, id).r * 255);
                    var swatch = new VisualElement { pickingMode = PickingMode.Ignore };
                    swatch.AddToClassList("preset-swatch");
                    swatch.style.backgroundColor = _palette.GetPixel(index, 0);
                    ramp.Add(swatch);
                }
            }
        }
    }
}