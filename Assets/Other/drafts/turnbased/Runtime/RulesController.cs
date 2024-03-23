using System.Collections;
namespace Drafts.TurnBased {
	public class RulesController {

		public RulesController(IRules rules, ActionChain chain) => PlayMatch(rules, chain).Start();

		IEnumerator PlayMatch(IRules rules, ActionChain chain) {
			using(rules) {
				foreach(var phase in rules.Start) {
					rules.CurrentPhase = phase;
					yield return phase.Execute();
					yield return chain.Evaluate();
					if(rules.IsOver) break;
				}

				while(!rules.IsOver)
					foreach(var phase in rules.Loop) {
						rules.CurrentPhase = phase;
						yield return phase.Execute();
						yield return chain.Evaluate();
						if(rules.IsOver) break;
					}

				foreach(var phase in rules.End) {
					rules.CurrentPhase = phase;
					yield return phase.Execute();
					yield return chain.Evaluate();
				}
			}
		}
	}
}
