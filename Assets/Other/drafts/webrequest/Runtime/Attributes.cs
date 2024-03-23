using System;
public class LocalDropdownAttribute : Attribute {
	public LocalDropdownAttribute(string _) { }
}
public class ReadOnlyAttribute : Attribute { 
	public ReadOnlyAttribute(bool _) { }
}
