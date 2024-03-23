using System.IO;
using UnityEditor;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace DraftsEditor {
	public static partial class EditorUtil {

		public static void WriteFile(string path, string content, bool overwrite = false) {
			if(!overwrite && File.Exists(path)) return;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, content);
		}

		/// <summary>Does not overwrite existent files.</summary>
		public static void WriteFile<T>(string path, T content, bool overwrite = false) where T : UObj {
			if(!overwrite && File.Exists(path)) return;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			AssetDatabase.CreateAsset(content, path);
		}

		public static void WriteFile(string path, Texture2D texture, bool overwrite = false) {
			if(!path.EndsWith(".png")) path += ".png";
			if(!overwrite && File.Exists(path)) return;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllBytes(path, texture.EncodeToPNG());
		}

		public static string GetRelativePath(this UObj relativeTo, string path)
			=> Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(relativeTo)), path);
	}
}
