using System;
using UnityEngine;
using System.Collections;
using UniRx;

[Author("Donghyun You","20150503")]
public class UserInfo : ViewModel {

	// ========================================================================================================================
	// Models
	// ========================================================================================================================

	public ReactiveProperty<string> 	UserID 			{ get; set; }
	public ReactiveProperty<string> 	Name 			{ get; set; }
	public ReactiveProperty<DateTime>	LastLoggedIn 	{ get; set; }

	public UserInfo() {
		UserID 			= new ReactiveProperty<string> ();
		Name 			= new ReactiveProperty<string> ();
		LastLoggedIn 	= new ReactiveProperty<DateTime> ();
	}
	
	// ========================================================================================================================
	// Command delegates
	// ========================================================================================================================

	public delegate void UserInfoEchoCommand (UserInfo vm,string message);
	public UserInfoEchoCommand OnUserInfoEchoCommandExecuted;

}
