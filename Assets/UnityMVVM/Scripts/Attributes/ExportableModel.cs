using System;

[AttributeUsage(AttributeTargets.All,AllowMultiple = false)]
public class ExportableModel : Attribute {

	public ExportableModel() 
	{
	}

}
