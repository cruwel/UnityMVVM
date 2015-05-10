using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

using UniRx;

[Author("Donghyun You","20150503")]
[InjectModel("Me")]
public class UserInfoUIView : MonoView<UserInfo> {

	// ===============================================================================================================
	// Screen materials
	// ===============================================================================================================

	public Text idView;

	// ===============================================================================================================
	// Additional ViewModel Injections
	// ===============================================================================================================

	[InjectModel("Friends")]
	public ReactiveDictionary<string,UserInfo> Friends { get; internal set; }

	// ===============================================================================================================
	// Customize Bind/Unbind
	// ===============================================================================================================

	protected override void OnEnable ()
	{
		base.OnEnable ();
		this.MainViewModel.Value.OnUserInfoEchoCommandExecuted += this.OnEchoExecuted;
	}

	protected override void OnDisable() {
		base.OnDisable ();
		this.MainViewModel.Value.OnUserInfoEchoCommandExecuted -= this.OnEchoExecuted;
	}

	// ===============================================================================================================
	// Specified events
	// ===============================================================================================================

	protected virtual void OnIDChanged(string id) {
		idView.text = id;
	}

	protected virtual void OnEchoExecuted(UserInfo vm, string message) {

	}

}
