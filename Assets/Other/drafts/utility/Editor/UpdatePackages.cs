using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public static class UpdatePackages {
	
	[MenuItem("Drafts/Update All Git Packages")]
	static void UpdateAllGitPackages() {
		var path = Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')) + "/Packages/packages-lock.json";
		File.WriteAllText(path, "{}");
		Client.Resolve();
	}

	//[MenuItem("Drafts/Update")]
	//static void Start() {
	//	var req = Client.List(true, false);
	//	void Update() {
	//		if(!req.IsCompleted) return;
	//		EditorApplication.update -= Update;
	//		//UpdateNonUnity(req.Result.Where(p => p.name == "com.drafts.utility"));
	//		UpdateNonUnity(req.Result.Where(p => !p.packageId.StartsWith("com.unity")));
	//	}
	//	EditorApplication.update += Update;
	//}

	//static void UpdateNonUnity(IEnumerable<PackageInfo> collection) {
	//	//for(int i = 0; i < file.Length; i++) {
	//	//	if(!collection.Any(p => file[i].Contains(p.name))) continue;
	//	//	Debug.Log("found" + file[i]);
	//	//	for(int j = i; j < i+; j++) {

	//	//	}
	//	//	file[i] = "";
	//	//}

	//	//var add = Client.AddAndRemove(collection.Select(i => i.name).ToArray(), null);
	//	//EditorRequest(add, () => Debug.Log("Done!"));

	//	//var rem = Client.AddAndRemove(null, collection.Select(i => i.name).ToArray());
	//	//EditorRequest(rem, AddAll);

	//	//void AddAll() {
	//	//	var add = Client.AddAndRemove(collection.Select(i => i.name).ToArray(), null);
	//	//	EditorRequest(add, () => Debug.Log("Done!"));
	//	//}

	//	//foreach(var item in collection) {
	//	//	var url = item.packageId.Substring(item.name.Length + 1);
	//	//	if(url.StartsWith("git@")) {
	//	//		Debug.Log(url);
	//	//		Client.Add(url);
	//	//	}
	//	//}
	//	//Client.Resolve();
	//}

	//static void EditorRequest(Request req, Action callback) {
	//	void Update() {
	//		if(!req.IsCompleted) return;
	//		EditorApplication.update -= Update;
	//		Client.Resolve();
	//		callback();
	//	}
	//	EditorApplication.update += Update;
	//}
}
