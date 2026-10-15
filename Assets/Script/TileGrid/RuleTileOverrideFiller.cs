#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public static class RuleTileOverrideFiller
    {
        private const string MenuPath = "Game/Assign Rule Tile Sprite Overrides";

        [MenuItem(MenuPath)]
        private static void AssignOverrides()
        {
            var tiles = Selection.objects.OfType<RuleOverrideTile>().ToArray();
            var sprites = Selection.objects.OfType<Sprite>().ToArray();

            if (tiles.Length != 1 || sprites.Length == 0 || !tiles[0].m_Tile)
            {
                Debug.LogWarning("Select one Rule Override Tile with a base Rule Tile and the replacement sprites.");
                return;
            }

            var ruleTile = tiles[0];
            var overrides = new List<KeyValuePair<Sprite, Sprite>>();
            var validCount = 0;
            ruleTile.GetOverrides(overrides, ref validCount);

            if (sprites.Length != validCount)
            {
                Debug.LogError($"Sprites selected {sprites.Length}/{validCount}", ruleTile);
                return;
            }

            // Use natural name order (sprite_2 before sprite_10); selection order is unspecified.
            // Destination slots follow the Rule Override Tile inspector order.
            System.Array.Sort(sprites, (a, b) =>
            {
                var comparison = EditorUtility.NaturalCompare(a.name, b.name);
                return comparison != 0 ? comparison : string.CompareOrdinal(AssetDatabase.GetAssetPath(a), AssetDatabase.GetAssetPath(b));
            });

            const string undoName = "Assign Rule Tile Sprite Overrides";
            Undo.RegisterCompleteObjectUndo(ruleTile, undoName);
            var instanceTile = ruleTile.m_InstanceTile;
            if (instanceTile) Undo.RegisterCompleteObjectUndo(instanceTile, undoName);

            for (var index = 0; index < sprites.Length; index++)
                ruleTile[overrides[index].Key] = sprites[index];

            ruleTile.Override();
            if (!instanceTile && ruleTile.m_InstanceTile)
                Undo.RegisterCreatedObjectUndo(ruleTile.m_InstanceTile, undoName);

            EditorUtility.SetDirty(ruleTile);
            if (ruleTile.m_InstanceTile) EditorUtility.SetDirty(ruleTile.m_InstanceTile);
            AssetDatabase.SaveAssetIfDirty(ruleTile);
            SceneView.RepaintAll();
        }

        [MenuItem(MenuPath, true)]
        private static bool CanAssignOverrides()
        {
            var tiles = Selection.objects.OfType<RuleOverrideTile>().ToArray();
            return tiles.Length == 1 && tiles[0].m_Tile && Selection.objects.OfType<Sprite>().Any();
        }
    }
}
#endif