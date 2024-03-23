using System;
using System.Collections.Generic;
using System.Linq;
namespace Drafts {
	public class AssetSearchSettings : SearchSettings {
		public static Func<Type, string, IEnumerable<UnityEngine.Object>> _findAssets;

		public AssetSearchSettings(Type type, string folder = "Assets/") {
			Title = $"{type.Name} in {folder}";
			GetName = o => (o as UnityEngine.Object)?.name;
			GetItens = () => _findAssets(type, folder);
		}
		public AssetSearchSettings(Type type, Func<UnityEngine.Object, bool> validate, string folder = "Assets/") {
			Title = $"{type.Name} in {folder}";
			GetName = o => (o as UnityEngine.Object)?.name;
			GetItens = () => _findAssets(type, folder).Where(o => validate(o));
		}
	}
}