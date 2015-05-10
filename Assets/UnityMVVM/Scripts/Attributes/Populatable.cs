using UnityEngine;
using System;
using System.Collections;

public class Populatable : System.Attribute {

	public string ViewModelTableName 	{ get; private set; }
	public string HandlingKey 			{ get; private set; }

	public Populatable(string viewModelTableName,string handlingKey) {

		this.ViewModelTableName = viewModelTableName;
		this.HandlingKey 		= handlingKey;

	}

}
