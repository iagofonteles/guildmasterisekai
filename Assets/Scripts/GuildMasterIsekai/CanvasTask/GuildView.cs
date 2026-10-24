using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class GuildView : ViewElement<Guild>
    {
        private readonly Label _rank = new() { name = "rank" };
        private readonly Label _gold = new() { name = "gold" };

        public GuildView()
        {
            AddToClassList("header");
            var title = new Label("Guild Master") { name = "guildTitle" };
            title.AddToClassList("title");
            Add(title);
            Add(_rank);
            Add(_gold);
        }

        protected override void Subscribe() => Data.Gold.OnValueChanged += GoldChanged;
        protected override void Unsubscribe() => Data.Gold.OnValueChanged -= GoldChanged;
        private void GoldChanged(int value) => Refresh();

        public override void Refresh()
        {
            _rank.text = Data == null ? "" : $"Rank: {Data.Rank}";
            _gold.text = Data == null ? "" : $"Gold: {Data.Gold.Value:N0}";
        }

        public override void SetSelected(object selected) { }
    }
}
