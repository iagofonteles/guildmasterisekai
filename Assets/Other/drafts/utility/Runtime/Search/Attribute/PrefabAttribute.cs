using System;
using UnityEngine;
namespace Drafts {
	public class PrefabAttribute : PropertyAttribute {
		public string Folder { get; }
		public Type Type { get; }
		public PrefabAttribute(string folder = "Assets") => Folder = folder;
		public PrefabAttribute(Type type, string folder = "Assets") {
			Folder = folder;
			Type = type;
		}
	}
}