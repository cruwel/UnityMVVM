using System;

[AttributeUsage(AttributeTargets.All,AllowMultiple = false)]
public class InjectModel : Attribute {

	public string Selector { get; private set; }

	public InjectModel(string selector) 
	{
		this.Selector = selector;
	}

}
