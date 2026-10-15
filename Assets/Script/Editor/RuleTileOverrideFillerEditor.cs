#if UNITY_EDITOR
using GuildMasterIsekai.TileGrid;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RuleTileOverrideFiller))]
    public class RuleTileOverrideFillerEditor : RuleTileEditor
    {
        [SerializeField] private Texture2D wallIcon;
        [SerializeField] private Texture2D notWallIcon;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            var icon = neighbor switch
            {
                WallRuleTile.Neighbor.Wall => wallIcon,
                WallRuleTile.Neighbor.NotWall => notWallIcon,
                _ => null
            };

            if (icon) GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
            else base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
#endif