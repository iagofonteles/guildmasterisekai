using System;
using System.Linq;
using UnityEngine;
namespace Drafts {

	public class SOSearchSettings : AssetSearchSettings {
		public SOSearchSettings(Type type, string folder = "Assets/")
			: base(typeof(ScriptableObject), folder) {
			Title = $"{type.Name} in {folder}";
			GetItens = () => _findAssets(typeof(ScriptableObject), folder).Where(o => type.IsAssignableFrom(o.GetType()));
		}
		public SOSearchSettings(Type type, Func<ScriptableObject, bool> validate, string folder = "Assets/")
			: this(type, folder) {
			GetItens = () => _findAssets(typeof(ScriptableObject), folder).Where(o => type.IsAssignableFrom(o.GetType()) && validate((ScriptableObject)o));
		}
	}

	public class SOSearchSettings<T> : AssetSearchSettings {
		public SOSearchSettings(string folder = "Assets/")
			: base(typeof(ScriptableObject), folder) {
			Title = $"{typeof(T).Name} in {folder}";
			GetItens = () => _findAssets(typeof(ScriptableObject), folder).Where(o => o is T);
		}
		public SOSearchSettings(Func<T, bool> validate, string folder = "Assets/")
			: this(folder) {
			GetItens = () => _findAssets(typeof(ScriptableObject), folder).Where(o => o is T t && validate(t));
		}
	}
}