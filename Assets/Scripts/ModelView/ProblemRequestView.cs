using Drafts.DataView;
using GuildMasterIsekai.Model;
using UnityEngine;

namespace GuildMasterIsekai.ModelView
{
    public class ProblemRequestView : DataView<ProblemRequest>
    {
        [SerializeField] private CostumerView costumer;
        [SerializeField] private ProblemView problem;

        protected override void Subscribe()
        {
            problem.TrySetData(Data.Problem);
            costumer.TrySetData(Data.Costomer);
        }

        public void Accept(Transform index) => Data.Reply(index.GetSiblingIndex() - 1);
        public void Accept(int index) => Data.Reply(index);
        public void Refuse() => Data.Reply(-1);
    }
}