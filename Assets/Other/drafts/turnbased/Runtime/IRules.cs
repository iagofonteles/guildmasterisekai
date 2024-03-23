using System;
using System.Collections.Generic;
namespace Drafts.TurnBased {
	public interface IRules : IDisposable {
		bool IsOver { get; }
		IEnumerable<Phase> Start { get; }
		IEnumerable<Phase> Loop { get; }
		IEnumerable<Phase> End { get; }
		Phase CurrentPhase { get; set; }
		event Action<Phase> OnPhaseChaged;
	}
}