using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Wall Rule Tile")]
public class WallRuleTile : RuleTile<WallRuleTile.Neighbor>
{
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Wall = 3;
        public const int NotWall = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) => neighbor switch
    {
        Neighbor.Wall => tile is WallRuleTile,
        Neighbor.NotWall => tile is not WallRuleTile,
        _ => base.RuleMatch(neighbor, tile)
    };
}