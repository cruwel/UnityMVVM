using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.All,AllowMultiple = true)]
public class Author : System.Attribute
{
	public string 	Name { get; private set; }
	public DateTime Time { get; private set; }

	public Author(string name,string date) {
		
		this.Name = name;

		if(date != null && date.Length > 0 && date.Length <= 8)
		this.Time = new DateTime(Int32.Parse(date.Substring(0,4)),Int32.Parse(date.Substring(4,2)),Int32.Parse(date.Substring(6,2)));

	}
}
