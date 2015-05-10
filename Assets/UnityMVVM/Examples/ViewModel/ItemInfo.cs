using System;
using UnityEngine;
using System.Collections;
using UniRx;

[Author("Donghyun You","20150503")]

public class ItemInfo : ViewModel {

	// ========================================================================================================================
	// Models
	// ========================================================================================================================

	public int 		ID 				{ get; protected set; }
	public string 	GUID 			{ get; protected set; }
	public DateTime RegisteredDate 	{ get; protected set; }

	[Populatable("ItemDesc","ID")]
	public ItemDesc Desc			{ get; set; }

	// ========================================================================================================================
	// Factory on separated
	// Note : by separating static class factory. number of create method will not copied on generating each viewModel
	// ========================================================================================================================

	public static class Factory {

		public static ItemInfo Create() {

			return new ItemInfo () 
			{
				ID = 0,
				GUID = "",
				RegisteredDate = DateTime.Now
			};

		}

		public static ItemInfo Create(string json) {

			return JsonFx.Json.JsonReader.Deserialize<ItemInfo> (json);

		}

	}

	// ========================================================================================================================
}
