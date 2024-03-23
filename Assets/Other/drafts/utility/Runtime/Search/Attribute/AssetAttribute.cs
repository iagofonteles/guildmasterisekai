using System;
using UnityEngine;
namespace Drafts {
	public class AssetAttribute : PropertyAttribute {
		public Type Type { get; }
		public string Folder { get; }
		public AssetAttribute(string folder = "Assets") => Folder = folder;
		public AssetAttribute(Type type, string folder = "Assets") : this(folder) => Type = type;
	}
}