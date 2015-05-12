using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using System.Reflection;
using UniRx;

[Author("Donghyun You","20150503")]
public abstract class MonoView : MonoBehaviour {
	
	public UnityEvent OnEnabled;
	public UnityEvent OnDisabled;

	protected CompositeDisposable _disposables = null;

	protected virtual void OnEnable() 
	{
		if (OnEnabled != null) OnEnabled.Invoke();
		this.OnSubscribe ();
	}
	
	protected virtual void OnDisable() 
	{
		if (OnDisabled != null) OnDisabled.Invoke();
		this.OnUnsubscribe ();
	}

	protected virtual void OnSubscribe() {
		_disposables = new CompositeDisposable ();
	}

	protected virtual void OnUnsubscribe() {
		_disposables.Dispose ();
	}

}

[Author("Donghyun You","20150503")]
public abstract class MonoView<T> : MonoView where T : ViewModel {

	public ReactiveProperty<T> MainViewModel { get; private set; }
	
	protected virtual void Awake() 
	{
		this.InjectModel ();
	}

	protected override void OnSubscribe ()
	{
		base.OnSubscribe ();
		_disposables.Add(this.MainViewModel.Subscribe (OnMainViewModelChanged));
	}

	protected virtual void OnMainViewModelChanged (T model) {}

}