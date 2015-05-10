using System;
using UnityEngine;
using System.Collections;
using UniRx;

[Author("Donghyun You","20150503")]
public class UserInfoController : Controller<UserInfo,UserInfoController> {

	public override void Initialize (UserInfo vm)
	{
		base.Initialize (vm);
		vm.UserID 		= new ReactiveProperty<string> ();
		vm.Name 		= new ReactiveProperty<string> ();
		vm.LastLoggedIn = new ReactiveProperty<DateTime> ();
	}
	
	// ============================================================================================================================
	// Command delegate imeplementations
	// ============================================================================================================================

	public void ExecuteEcho(UserInfo vm, string message) 
	{
		// do something here works to viewModel
		Debug.Log ("Echo! as " + message);

		// invoke to other subscribers
		vm.OnUserInfoEchoCommandExecuted.Invoke (vm,message);
	}

	// ============================================================================================================================

}
