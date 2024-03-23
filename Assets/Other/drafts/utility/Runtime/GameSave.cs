using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Drafts {

	public class SaveNotLoadedException : Exception {
		public SaveNotLoadedException() : base() { }
	}

	public class GameSave {
		Dictionary<Type, IGameSave> pairs = new();
		public string Root { get; private set; }
		public string Folder { get; private set; }
		public string FullPath => Path.Combine(Root, Folder);
		public event Action<GameSave> OnLoaded;
		public event Action<GameSave> OnSaving;

		public GameSave(string root, string saveName) : this(root) => Folder = saveName;
		public GameSave(string root) => Root = root;

		public T Get<T>() where T : IGameSave {
			if(Folder == null) throw new SaveNotLoadedException();
			var type = typeof(T);
			if(!pairs.TryGetValue(type, out var obj)) {
				obj = (IGameSave)Activator.CreateInstance(type);
				pairs[type] = obj;
				obj.Load(FullPath);
			}
			return (T)obj;
		}

		public void Set<T>(T obj) where T : IGameSave => pairs[typeof(T)] = obj;

		public void New(string saveName) {
			Folder = saveName;
			pairs.Clear();
		}

		public void Save() {
			if(Folder == null) throw new SaveNotLoadedException();
			OnSaving?.Invoke(this);
			foreach(var item in pairs)
				item.Value.Save(FullPath);
		}

		public void Load() {
			if(Folder == null) throw new SaveNotLoadedException();
			pairs.Clear();
			OnLoaded?.Invoke(this);
		}

		public void Load(string saveName) {
			New(saveName);
			OnLoaded?.Invoke(this);
		}

		public void Clear() {
			if(Folder == null) throw new SaveNotLoadedException();
			pairs.Clear();
			throw new NotImplementedException();
		}

		public IEnumerable<string> GetNames() => Directory.EnumerateDirectories(Root).Select(Path.GetFileName);

		public IEnumerable<(string saveName, T data)> GetFromAll<T>() where T : IGameSave {
			foreach(var name in GetNames()) {
				var obj = (T)Activator.CreateInstance(typeof(T));
				obj.Load(Path.Combine(Root, name));
				yield return (name, obj);
			}
		}

		public T GetFrom<T>(string saveName) where T : IGameSave {
			if(!Exists(saveName)) return default;
			var obj = (T)Activator.CreateInstance(typeof(T));
			obj.Load(Path.Combine(Root, saveName));
			return obj;
		}

		public bool Exists(string saveName) => Directory.Exists(Path.Combine(Root, saveName));
	}

	public interface IGameSave {
		void Save(string path);
		bool Load(string path);
	}

	public interface IJsonGameSave : IGameSave {

		protected string FileName => GetType().Name + ".sav";
		void IGameSave.Save(string path) => Save(this, Path.Combine(path, FileName));
		bool IGameSave.Load(string path) => Load(this, Path.Combine(path, FileName));

		public static void Save(object obj, string path) {
			var json = JsonUtility.ToJson(obj, true);
			var dir = Path.GetDirectoryName(path);
			if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			File.WriteAllText(path, json);
		}

		public static bool Load(object obj, string path) {
			if(!File.Exists(path)) return false;
			var txt = File.ReadAllText(path);
			JsonUtility.FromJsonOverwrite(txt, obj);
			return true;
		}
	}
}
