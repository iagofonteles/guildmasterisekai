//TODO
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Drafts.Databases {

//	public abstract class DatabaseEntrySO<T> : ScriptableObject, IDatabaseEntryAsset where T : DatabaseEntrySO<T> {
//		[SerializeField] protected string mod;
//		//[NonSerialized] protected MagiString displayName;
//		//[NonSerialized] protected MagiString description;
//		[SerializeField] protected string[] tags;
//		[NonSerialized] protected Async<Sprite> icon;
//		[NonSerialized] bool discovered;

//		public virtual string Mod { get => mod ??= GetType().Assembly.GetName().Name; set => mod = value; }
//		public virtual string Name => name;
//		public virtual string DisplayName => displayName ??= new($"{Mod}_{typeof(T).Name}", Name);
//		public virtual string Description => description ??= new($"{Mod}_{typeof(T).Name}", Name + " desc");
//		public virtual IReadOnlyList<string> Tags => tags;
//		public virtual Async<Sprite> Icon => icon ??= new($"{Mod}/{typeof(T).Name}/{Name}/{Name}.png", DefaultIcon, this);
//		protected virtual string DefaultIcon => "Core/DefaultIcon.png";

//		public static event Action<T> OnDiscovered;
//		public bool Discovered {
//			get => discovered;
//			set {
//				var raise = value && !discovered;
//				discovered = value;
//				if(raise) OnDiscovered?.Invoke(this as T);
//			}
//		}

//		public static IReadOnlyList<T> All => Game.Database.All<T>();
//		public static IEnumerable<S> Filter<S>() => All.Where(s => s is S).Cast<S>();
//		public static IEnumerable<S> Filter<S>(Predicate<S> predicate) => All.Where(s => s is S ss && predicate(ss)).Cast<S>();
//		public static IEnumerable<T> Filter(Predicate<T> predicate) => All.Where(s => predicate(s));
//		public static T Find(string name) => Game.Database.Find<T>(name);
//	}

//	//public abstract class DatabaseEntry<T> : IDatabaseEntry where T : DatabaseEntry<T> {
//	//	protected string mod;
//	//	protected string name;
//	//	protected MagiString displayName;
//	//	protected MagiString description;
//	//	protected Async<Sprite> icon;
//	//	bool discovered;

//	//	public virtual string Mod => mod ??= GetType().Assembly.GetName().Name;
//	//	public virtual string Name => name ??= GetType().Name;
//	//	public virtual string DisplayName => displayName ??= new($"{Mod}_{typeof(T).Name}", Name);
//	//	public virtual string Description => description ??= new($"{Mod}_{typeof(T).Name}", Name + " desc");
//	//	public virtual Async<Sprite> Icon => icon ??= $"{Mod}/{typeof(T).Name}/{Name}";

//	//	public static event Action<T> OnDiscovered;
//	//	public bool Discovered {
//	//		get => discovered;
//	//		set {
//	//			var raise = value && !discovered;
//	//			discovered = value;
//	//			if(raise) OnDiscovered?.Invoke((T)this);
//	//		}
//	//	}

//	//	static IReadOnlyList<T> all;
//	//	public static IReadOnlyList<T> All => all ??= Game.Database.All<T>().Cast<T>().ToList();
//	//	public static T Find(string name) => Game.Database.Find<T>(name);
//	//	public string Root { get => mod; set => mod = value; }
//	//}

//}
