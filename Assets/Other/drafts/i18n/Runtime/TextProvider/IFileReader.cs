using System.Collections.Generic;
namespace Drafts.Internationalization {
	public interface IFileReader { 
		IEnumerable<string> GetLines(string textFile);
		I18nEntry GetEntry(string line);
	}
}
