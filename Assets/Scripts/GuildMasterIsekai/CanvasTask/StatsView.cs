using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class StatsView : ViewElement<Stats>
    {
        private readonly Label _summary = new();
        public StatsView() => Add(_summary);

        protected override void Subscribe()
        {
            Data.Power.OnChanged += Changed;
            Data.Knowledge.OnChanged += Changed;
            Data.Vigor.OnChanged += Changed;
        }

        protected override void Unsubscribe()
        {
            Data.Power.OnChanged -= Changed;
            Data.Knowledge.OnChanged -= Changed;
            Data.Vigor.OnChanged -= Changed;
        }

        private void Changed(int value) => Refresh();
        public static string Summary(IStats stats) =>
            $"Power: {stats.Power} | Knowledge: {stats.Knowledge} | Vigor: {stats.Vigor}";

        public override void Refresh() => _summary.text = Data == null ? "" : Summary(Data);
        public override void SetSelected(object selected) { }
    }
}
