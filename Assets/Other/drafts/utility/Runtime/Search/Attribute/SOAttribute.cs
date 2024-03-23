using System;
using UnityEngine;
namespace Drafts {
	public class SOAttribute : PropertyAttribute {
		public string Folder { get; }
		public Type Type { get; }
		public SOAttribute(Type type, string folder = "Assets") {
			Folder = folder;
			Type = type;
		}
	}
}