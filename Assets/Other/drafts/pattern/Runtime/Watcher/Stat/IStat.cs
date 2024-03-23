namespace Drafts.Patterns {

	public interface ISimpleStat : IWatcher {
		object Owner { get; }
		object Current { get; }
		object Base { get; }
		float Percent { get; }
	}

	public interface IStat : ISimpleStat {
		object Add { get; }
		object Mult { get; }
		object Min { get; }
		object Max { get; }
		object Total { get; }
	}
}