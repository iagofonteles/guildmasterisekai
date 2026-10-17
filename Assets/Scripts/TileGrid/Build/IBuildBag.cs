using System.Collections.Generic;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    public interface IBuildBag
    {
        int GetBuildingMaterials(); // used for base layer
        IEnumerable<BaseTileSo> GetUnlockedTiles(); // used for base layer
        IEnumerable<(T,int)> GetAvailableItems<T>(); // used for furniture and topings

        /// <summary>Apply every delta or none. On failure, leave balances unchanged.</summary>
        bool CanPay(OperationCost cost);
        bool CanRefund(OperationCost cost);
        void Pay(OperationCost cost);
        void Refund(OperationCost cost);
    }
}