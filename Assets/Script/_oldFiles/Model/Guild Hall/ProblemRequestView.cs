using Drafts;
using UnityEngine;

namespace GuildMasterIsekai {

	public class ProblemRequestView : DataView<ProblemRequest> {
		//[SerializeField] CostumerView costumer;
		[SerializeField] ProblemView problem;
		[SerializeField] CostumerView costumer;

		protected override void Subscribe() {
			problem.TrySetData(Data.Problem);
			costumer.TrySetData(Data.Costumer);
		}

		public void Accept(DataHolder outcomeHolder) => Data.Reply(
			Data.Problem.Outcomes.IndexOf(outcomeHolder.Data));

		public void Accept(int index) => Data.Reply(index);
		public void Refuse() => Data.Reply(-1);
	}
}