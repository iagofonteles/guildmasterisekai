using System;
using UnityEngine;

namespace TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/ColorPaletteConfig")]
    public class ColorPaletteConfig : ScriptableObject
    {
        // Storage layout shared with IndexedPalette.shader, independent of the palette's ramp length.
        public const int EncodedShades = 8;
        public const int MaxPaletteColors = 128;
        public const int OpaqueFlag = 64;
        public const int RegionCount = 4;

        private static readonly int PaletteTex = Shader.PropertyToID("_PaletteTex");
        private static readonly int PaletteColorCountId = Shader.PropertyToID("_PaletteColorCount");
        private static readonly int PaletteShadesId = Shader.PropertyToID("_PaletteShades");

        [SerializeField] private Shader tilemapShader;
        [SerializeField] private Material tilemapMaterial;
        [SerializeField] private Texture2D paletteTexture;
        [SerializeField, Range(1, MaxPaletteColors)] private int paletteColorCount = 65;
        [Tooltip("Changing the shade count requires encoding tiles again.")]
        [SerializeField, Range(1, EncodedShades)] private int paletteColorShades = 5;

        public Shader TilemapShader => tilemapShader;
        public Material TilemapMaterial => tilemapMaterial;
        public Texture2D PaletteTexture => paletteTexture;
        public int PaletteColorCount => paletteColorCount;
        public int PaletteColorShades => paletteColorShades;
        public int RampCount => (paletteColorCount + paletteColorShades - 1) / paletteColorShades;

        public int GetPaletteIndex(int ramp, int shade)
        {
            if (ramp < 1 || ramp > RampCount)
                throw new ArgumentOutOfRangeException(nameof(ramp), "Ramp zero preserves the original pixel.");
            if (shade < 0 || shade >= paletteColorShades)
                throw new ArgumentOutOfRangeException(nameof(shade));
            return Math.Min((ramp - 1) * paletteColorShades + shade, paletteColorCount - 1);
        }

        public void ValidateConfiguration()
        {
            if (!tilemapShader || !tilemapMaterial || !paletteTexture)
                throw new InvalidOperationException($"{name}: assign the shader, materials and palette texture.");
            if (paletteColorCount < 1 || paletteColorCount > MaxPaletteColors ||
                paletteColorShades < 1 || paletteColorShades > EncodedShades)
                throw new InvalidOperationException($"{name}: palette exceeds the indexed sprite storage limits.");
            if (paletteTexture.width * paletteTexture.height < paletteColorCount)
                throw new InvalidOperationException($"{name}: palette texture has too few pixels.");
            if (!tilemapMaterial.HasProperty(PaletteTex))
                throw new InvalidOperationException($"{name}: palette material does not support _PaletteTex.");
            if (tilemapShader.FindPropertyIndex("_PaletteTex") < 0 ||
                tilemapShader.FindPropertyIndex("_PaletteColorCount") < 0 ||
                tilemapShader.FindPropertyIndex("_PaletteShades") < 0)
                throw new InvalidOperationException($"{name}: tilemap shader does not support the palette settings.");
        }

        [ContextMenu("Apply Material Settings")]
        public void ApplyMaterialSettings()
        {
            ValidateConfiguration();
            tilemapMaterial.shader = tilemapShader;
            ApplyPalette(tilemapMaterial);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(tilemapMaterial);
#endif
        }

        private void ApplyPalette(Material material)
        {
            if (material.HasProperty(PaletteTex)) material.SetTexture(PaletteTex, paletteTexture);
            if (material.HasProperty(PaletteColorCountId)) material.SetInt(PaletteColorCountId, paletteColorCount);
            if (material.HasProperty(PaletteShadesId)) material.SetInt(PaletteShadesId, paletteColorShades);
        }

        private void OnEnable()
        {
            // Newly created configurations are intentionally unassigned.
            if (tilemapShader && tilemapMaterial && paletteTexture)
                ApplyMaterialSettings();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            paletteColorCount = Mathf.Clamp(paletteColorCount, 1, MaxPaletteColors);
            paletteColorShades = Mathf.Clamp(paletteColorShades, 1, EncodedShades);
            UnityEditor.EditorApplication.delayCall -= ApplyInEditor;
            UnityEditor.EditorApplication.delayCall += ApplyInEditor;
        }

        private void ApplyInEditor()
        {
            if (!this || !tilemapShader || !tilemapMaterial || !paletteTexture) return;
            try
            {
                ApplyMaterialSettings();
            }
            catch (InvalidOperationException exception)
            {
                Debug.LogError(exception.Message, this);
            }
        }
#endif
    }
}
