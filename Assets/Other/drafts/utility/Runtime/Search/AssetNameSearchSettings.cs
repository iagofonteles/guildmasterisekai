using System;
using System.Linq;
namespace Drafts {
	public class AssetNameSearchSettings : SearchSettings<string> {

		public AssetNameSearchSettings(Type type, string folder = "Assets/")
			: base(() => AssetSearchSettings._findAssets(type, folder).Select(s => s.name), s => s) { }

		public AssetNameSearchSettings(Type type, Func<UnityEngine.Object, bool> validate, string folder = "Assets/")
			: base(() => AssetSearchSettings._findAssets(type, folder).Where(o => validate(o)).Select(s => s.name), s => s) { }
	}
}