using System;
using System.Linq;
using UnityEngine;
namespace Drafts {
	public class PrefabSearchSettings : AssetSearchSettings {

		public PrefabSearchSettings(Type type, string folder = "Assets/") : base(type, folder)
			=> GetItens = () => _findAssets(typeof(GameObject), folder)
				.Select(o => (o as GameObject).GetComponent(type)).Where(c => c);

		public PrefabSearchSettings(Type type, Func<Component, bool> validate, string folder = "Assets/") : base(type, folder)
			=> GetItens = () => _findAssets(typeof(GameObject), folder)
				.Select(o => (o as GameObject).GetComponent(type)).Where(c => c && validate(c));
	}
}