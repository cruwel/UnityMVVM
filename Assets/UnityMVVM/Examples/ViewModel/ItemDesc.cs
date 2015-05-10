using System;
using UnityEngine;
using System.Collections;
using UniRx;

[Author("Donghyun You","20150503")]
public class ItemDesc : ViewModel {
	
	// ========================================================================================================================
	// Models
	// ========================================================================================================================
	
	public int 		ID 				{ get; protected set; }
	public string 	Name 			{ get; protected set; }
	public ItemType	Type 			{ get; protected set; }
	
	// ========================================================================================================================
	// Factory on separated
	// Note : by separating static class factory. number of create method will not copied on generating each viewModel
	// ========================================================================================================================
	
	public static class Factory {
		
		public static ItemDesc Create() {
			
			return new ItemDesc () 
			{
				ID = 0,
				Name = "",
				Type = ItemType.UNDEFINED,
			};

		}
		
		public static ItemDesc Create(string json) {
			
			return JsonFx.Json.JsonReader.Deserialize<ItemDesc> (json);

		}

	}
	
	// ========================================================================================================================
}
