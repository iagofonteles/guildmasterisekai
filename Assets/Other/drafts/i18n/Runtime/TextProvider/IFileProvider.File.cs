using System.IO;
namespace Drafts.Internationalization {

	/// <summary>
	/// Uses File.ReadAllText to load text files.
	/// Uses string.Format to build file path using I18n.Lang and I18n.GetTable
	/// </summary>
	public class TextFileTableProvider : IFileProvider {

		string BasePath { get; }

		/// <summary>{0} lang, {1} table. Ex: "i18n/{1}_{0}"</summary>
		public TextFileTableProvider(string basePath) => BasePath = basePath;

		public string GetTableText(string lang, string table) {
			try {
				var path = string.Format(BasePath, lang, table);
				return File.ReadAllText(path);
			} catch {
				UnityEngine.Debug.LogError($"Error Loading file: {lang} {table} {BasePath}");
				throw new TableNotFound(lang, table);
			}
		}
	}
}