using System;
using System.Collections.Generic;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class InventoryBuildBag : MonoBehaviour, IBuildBag
    {
        public DatabaseEntrySo buildingMaterial;
        public ListInventory<DatabaseEntrySo> inventory;
        private bool _applying;

        private void Start() => inventory.GetStackSize = _ => int.MaxValue;

        public InventoryBuildBag(ListInventory<DatabaseEntrySo> inventory, DatabaseEntrySo buildingMaterial)
        {
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            this.buildingMaterial = buildingMaterial
                ? buildingMaterial
                : throw new ArgumentNullException(nameof(buildingMaterial));
        }

        public int GetBuildingMaterials() => inventory.Count(buildingMaterial);

        public IEnumerable<BaseTileSo> GetUnlockedTiles()
        {
            foreach (var slot in inventory)
                if (slot.Item is BaseTileSo so)
                    yield return so;
        }

        public IEnumerable<(T, int)> GetAvailableItems<T>()
        {
            foreach (var slot in inventory)
                if (slot.Item is T item)
                    yield return (item, slot.Amount);
        }

        public bool CanPay(OperationCost cost) => CanApply(cost, false);
        public bool CanRefund(OperationCost cost) => CanApply(cost, true);
        public void Pay(OperationCost cost) => Apply(cost, false);
        public void Refund(OperationCost cost) => Apply(cost, true);

        private bool CanApply(OperationCost cost, bool refund)
        {
            if (_applying || cost == null) return false;

            foreach (var pair in cost.Items)
            {
                if (pair.Key is not DatabaseEntrySo item || !item) return false;
                var delta = refund ? pair.Value : -(long)pair.Value;
                var count = inventory.Count(item) + delta;
                if (count < 0 || count > int.MaxValue || delta > int.MaxValue || delta < -int.MaxValue ||
                    (delta > 0 && inventory.Fits(item) < delta)) return false;
            }

            return true;
        }

        private void Apply(OperationCost cost, bool refund)
        {
            if (!CanApply(cost, refund))
                throw new InvalidOperationException("Insufficient items or inventory capacity.");
            _applying = true;

            try
            {
                foreach (var pair in cost.Items)
                {
                    var item = (DatabaseEntrySo)pair.Key;
                    var delta = refund ? pair.Value : checked(-pair.Value);
                    if (delta > 0) inventory.Add(item, delta);
                    else if (delta < 0) inventory.Remove(item, -delta);
                }
            }
            finally
            {
                _applying = false;
            }
        }
    }
}