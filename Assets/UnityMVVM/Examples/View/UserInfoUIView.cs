using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

using UniRx;

[Author("Donghyun You","20150503")]
[InjectModel("Me")]
public class UserInfoUIView : MonoView<UserInfo> {
	
	// ===============================================================================================================
	// Private members
	// ===============================================================================================================

	protected CompositeDisposable _userInfoDisposable;

	// ===============================================================================================================
	// Screen materials
	// ===============================================================================================================
	
	public Text nameView;
	public Text idView;

	// ===============================================================================================================
	// Additional ViewModel Injections
	// ===============================================================================================================

	[InjectModel("Friends")]
	public ReactiveDictionary<string,UserInfo> Friends { get; internal set; }

	// ===============================================================================================================
	// Customize Bind/Unbind
	// ===============================================================================================================

	protected override void OnSubscribe ()
	{
		base.OnSubscribe ();
		if(this.MainViewModel.Value != null)
		this.MainViewModel.Value.OnUserInfoEchoCommandExecuted += this.OnEchoExecuted;
	}

	protected override void OnUnsubscribe ()
	{
		base.OnUnsubscribe ();
		if(this.MainViewModel.Value != null)
		this.MainViewModel.Value.OnUserInfoEchoCommandExecuted -= this.OnEchoExecuted;
	}

	// ===============================================================================================================
	// Specified events
	// ===============================================================================================================

	protected virtual void OnEchoExecuted(UserInfo vm, string message) {

	}

	protected override void OnMainViewModelChanged (UserInfo model)
	{
		base.OnMainViewModelChanged (model);

		if (this._userInfoDisposable != null) {
			this._userInfoDisposable.Dispose ();
			this._disposables.Remove(this._userInfoDisposable);
		}

		this._userInfoDisposable = new CompositeDisposable ();
		this._disposables.Add(this._userInfoDisposable);

		if (model != null) 
		{
			this._userInfoDisposable.Add (model.Name.Subscribe (OnNameChanged));
			this._userInfoDisposable.Add (model.UserID.Subscribe (OnIDChanged));
		}
		else 
		{
			this._clearView();
		}

	}

	void _clearView() 
	{
		nameView.text = "[name]";
		idView.text = "[id]";
	}

	public void OnNameChanged(string name) {
		nameView.text = name;
	}

	public void OnIDChanged(string id) {
		idView.text = id;
	}

}
