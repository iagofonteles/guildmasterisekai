using System;
using System.Data;
using UnityEngine;
public static partial class DraftsUtil {

	static Func<string, float> resolveMath;

	static Func<string, float> ResolveMathGetFunc() {
		var t = new DataTable();
		var c = t.Columns.Add("exp", typeof(float), "");
		var r = t.NewRow();
		t.Rows.Add(r);
		return s => {
			c.Expression = s;
			return (float)r["exp"];
		};
	}

	public static float ResolveMath(string equation) {
		if(string.IsNullOrWhiteSpace(equation)) return 0;
		resolveMath ??= ResolveMathGetFunc();
		return resolveMath(equation);
	}
}
