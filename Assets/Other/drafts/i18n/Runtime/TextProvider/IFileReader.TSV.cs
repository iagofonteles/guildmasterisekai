using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Drafts.Internationalization {

	public class TSVFileReader : IFileReader {
		char LineSeparator = '\n';
		char KeySeparator = '\t';

		public I18nEntry GetEntry(string line) {
			if(string.IsNullOrWhiteSpace(line)) return null;
			if(line.StartsWith("//")) return null;

			try {
				var split = line.Split(KeySeparator);
				if(split.Length > 2) Debug.LogWarning("<b>Drafts I18N:</b> Multiple separators in: " + line);
				if(split.Length == 1) return new(split[0], "[empty]");
				return new(split[0], split[1]);
			} catch {
				if(!string.IsNullOrWhiteSpace(line))
					Debug.LogError("<b>Drafts I18N:</b> Error parsing: " + line);
				return null;
			}
		}

		public IEnumerable<string> GetLines(string textFile) 
			=> textFile?.Split(LineSeparator) ?? Enumerable.Empty<string>();
	}
}
