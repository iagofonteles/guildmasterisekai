using System;
using UnityEngine;
namespace Drafts {
	public abstract class DropdownAttribute : PropertyAttribute {
		public ISearchSettings Settings { get; }
		public Func<object, object, bool> Compare { get; }

		public DropdownAttribute(ISearchSettings settings) : this(settings, (a, b) => a == b) { }
		public DropdownAttribute(ISearchSettings settings, Func<object, object, bool> compare) {
			Settings = settings;
			Compare = compare;
		}
	}
}