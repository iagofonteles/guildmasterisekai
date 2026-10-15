using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Game/Wall Tile")]
    public class WallRuleTile : RuleTile<WallRuleTile.Neighbor>
    {
        public override bool RuleMatch(int neighbor, TileBase tile) => neighbor switch
        {
            Neighbor.Wall => tile is WallRuleTile,
            Neighbor.NotWall => tile is not WallRuleTile,
            _ => base.RuleMatch(neighbor, tile)
        };

        public class Neighbor : TilingRuleOutput.Neighbor
        {
            public const int Wall = 3;
            public const int NotWall = 4;
        }
    }
}