using TileGrid.Tiles;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    public sealed class SlotElement : Button
    {
        private readonly Label _amount;
        private readonly Image _image;
        private readonly Label _placeholder;
        private ISlot _slot;
        private bool _subscribed;
        private object _selectedItem;

        public ISlot Slot
        {
            get => _slot;
            set {
                Unsubscribe();
                _slot = value;
                if (panel != null) Subscribe();
                RefreshItem(_slot?.Item);
                RefreshAmount(_slot?.Amount ?? 0);
            }
        }

        public object SelectedItem
        {
            get => _selectedItem;
            set {
                _selectedItem = value;
                RefreshItem(_slot?.Item);
            }
        }

        public SlotElement()
        {
            AddToClassList("tile-choice");
            _image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                pickingMode = PickingMode.Ignore
            };
            _image.AddToClassList("slot-icon");
            Add(_image);
            _placeholder = new Label("?") { pickingMode = PickingMode.Ignore };
            _placeholder.AddToClassList("slot-placeholder");
            Add(_placeholder);
            _amount = new Label { pickingMode = PickingMode.Ignore };
            _amount.AddToClassList("tile-amount");
            Add(_amount);
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                Subscribe();
                RefreshItem(_slot?.Item);
                RefreshAmount(_slot?.Amount ?? 0);
            });
            RegisterCallback<DetachFromPanelEvent>(_ => Unsubscribe());
        }

        private void Subscribe()
        {
            if (_slot == null || _subscribed) return;
            _slot.OnAmountChanged += RefreshAmount;
            _slot.OnItemChanged += RefreshItem;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            _slot.OnAmountChanged -= RefreshAmount;
            _slot.OnItemChanged -= RefreshItem;
            _subscribed = false;
        }

        private void RefreshAmount(int amount)
        {
            _amount.text = amount > 0 ? "x" + amount : string.Empty;
            _amount.EnableInClassList("hidden", amount < 1);
        }

        private void RefreshItem(object value)
        {
            if (value is Color color)
            {
                EnableInClassList("selected", color.Equals(_selectedItem));
                tooltip = "#" + ColorUtility.ToHtmlStringRGBA(color);
                _image.image = Texture2D.whiteTexture;
                _image.tintColor = new Color(color.r, color.g, color.b, 1);
                _image.EnableInClassList("hidden", false);
                _placeholder.EnableInClassList("hidden", true);
                return;
            }

            var item = value as ITile;
            var icon = item?.Icon;
            EnableInClassList("selected", item != null && Equals(item, _selectedItem));
            tooltip = item?.DisplayName ?? string.Empty;
            _image.sprite = icon;
            _image.tintColor = Color.white;
            _image.EnableInClassList("hidden", !icon);
            _placeholder.EnableInClassList("hidden", icon);
        }
    }
}
