using Drafts.Databases;
using System;
using UnityEngine;
namespace GuildMasterIsekai {

	public interface IGuid : IComparable<IGuid>, IComparable<string> {
		string Guid { get; }

		int IComparable<IGuid>.CompareTo(IGuid other)
			=> Guid.CompareTo(other.Guid);

		int IComparable<string>.CompareTo(string other)
			=> Guid.CompareTo(other);
	}

	public interface IDisplay {
		string DisplayName { get; }
		string Description => "";
		Sprite Icon { get; }
		event Action OnChanged;
	}

	public abstract class DatabaseEntrySO : ScriptableObject, IDisplay, IDatabaseEntry {
		[SerializeField] protected string displayName;
		[SerializeField] protected Sprite icon;
		[SerializeField] protected string description;
		public event Action OnChanged;

		public virtual string Name => name;
		public virtual string DisplayName => displayName;
		public virtual Sprite Icon => icon;
		public virtual string Description => description;

		public void Changed() => OnChanged?.Invoke();
	}

	//public abstract class EntryInstance<T> : IDatabaseEntry where T : IDatabaseEntry {
	//	[SerializeField] protected string info;
	//	[SerializeField] protected string displayName;
	//	[SerializeField] protected Sprite icon;
	//	[SerializeField] protected string description;
	//	public event Action OnChanged;

	//	T _info;

	//	protected EntryInstance(T info) {
	//		this.info = info.Name;
	//		_info = info;
	//		displayName = info.DisplayName;
	//		icon = info.Icon;
	//		description = info.Description;
	//	}

	//	public T Info => _info ??= Game.Database.Find<T>(info);
	//	public virtual string Name => Info.Name;
	//	public virtual string DisplayName => displayName;
	//	public virtual Sprite Icon => icon;
	//	public virtual string Description => description;

	//	public void Changed() => OnChanged?.Invoke();
	//}

}