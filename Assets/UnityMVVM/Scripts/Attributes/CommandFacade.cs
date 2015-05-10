using System;

[AttributeUsage(AttributeTargets.All,AllowMultiple = false)]
public class CommandFacade : Attribute {

	public Type CommandType { get; private set; }

	public CommandFacade(Type commandType) 
	{
		this.CommandType = commandType;
    }
    
}
