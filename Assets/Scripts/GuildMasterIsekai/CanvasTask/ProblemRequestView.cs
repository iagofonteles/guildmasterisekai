using System;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    public sealed class ProblemRequestView : ViewElement<ProblemRequest>
    {
        private readonly Label _client = new();
        private readonly Label _problem = new();
        public event Action<ProblemRequest> Selected;

        public ProblemRequestView()
        {
            AddToClassList("card");
            Add(_client);
            Add(_problem);
            Add(new Button(() => Selected?.Invoke(Data)) { text = "View problem" });
        }

        protected override void Subscribe()
        {
            if (Data.Costomer != null) Data.Costomer.OnChanged += Refresh;
            if (Data.Problem != null) Data.Problem.OnChanged += Refresh;
        }

        protected override void Unsubscribe()
        {
            if (Data.Costomer != null) Data.Costomer.OnChanged -= Refresh;
            if (Data.Problem != null) Data.Problem.OnChanged -= Refresh;
        }

        public override void Refresh()
        {
            _client.text = Data?.Costomer?.DisplayName ?? "";
            _problem.text = Data?.Problem?.DisplayName ?? "";
        }

        public override void SetSelected(object selected) { }
    }
}
