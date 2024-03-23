//using System;
//using UnityEngine;

//namespace Drafts.Databases {

//	[Serializable]
//	public class DBRef<T> : DBRef<T, T> where T : IDatabaseEntry {
//		public DBRef(T value) : base(value) { }
//		public DBRef(string name) : base(name) { }
//	}

//	[Serializable]
//	public class DBRef<T, E> : Drafts.IData<T> where T : E where E : IDatabaseEntry {
		
//		[SerializeField] string name;
//		[NonSerialized] T value;
//		public T Value => value ??= (T)Game.Database.Find<E>(name);

//		public DBRef(T value) {
//			name = value.Name;
//			this.value = value;
//		}

//		public DBRef(string name) {
//			this.name = name;
//			value = default;
//		}

//		object Drafts.IData.Data { get => Value; set => throw new InvalidOperationException("Cannot assign value to DBRef"); }
//		T Drafts.IData<T>.Data { get => Value; set => throw new InvalidOperationException("Cannot assign value to DBRef"); }

//		public override bool Equals(object obj)
//			=> obj is DBRef<T, E> r && Value.Equals(r.Value)
//			|| obj is T t && Value.Equals(t);
//		public override int GetHashCode() => Value.GetHashCode();
//		public static implicit operator T(DBRef<T, E> _ref) => _ref.Value;
//		public static implicit operator DBRef<T, E>(T value) => new DBRef<T, E>(value);
//		public static implicit operator DBRef<T, E>(string name) => new DBRef<T, E>(name);
//	}

//}