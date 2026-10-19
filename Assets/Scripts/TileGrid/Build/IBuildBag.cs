using System.Collections.Generic;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    public interface IBuildBag
    {
        int GetBuildingMaterials();
        IEnumerable<ISlot> GetItems<T>();

        bool CanPay(OperationCost cost);
        bool CanRefund(OperationCost cost);
        void Pay(OperationCost cost);
        void Refund(OperationCost cost);
    }
}