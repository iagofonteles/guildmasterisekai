using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class CostomerView : ViewElement<Costomer>
    {
        private readonly Image _portrait = new();
        private readonly Label _title = new();
        private readonly Label _description = new();

        public CostomerView()
        {
            AddToClassList("card");
            _portrait.AddToClassList("portrait");
            Add(_portrait);
            Add(_title);
            Add(_description);
        }

        protected override void Subscribe()
        {
            Data.OnChanged += Refresh;
            _portrait.sprite = Data.Icon;
        }

        protected override void Unsubscribe() => Data.OnChanged -= Refresh;

        public override void Refresh()
        {
            _title.text = Data == null ? "" : $"{Data.DisplayName} (#{Data.Id})";
            _description.text = Data == null ? "" : Data.Description;
            if (Data == null) _portrait.sprite = null;
        }

        public override void SetSelected(object selected) { }
    }
}
