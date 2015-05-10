using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using System.Reflection;
using UniRx;

[Author("Donghyun You","20150503")]
public class MonoView : MonoBehaviour {
	
	public UnityEvent OnEnabled;
	public UnityEvent OnDisabled;

	protected CompositeDisposable _disposables = null;

	protected virtual void Awake() 
	{
		this.InjectModel ();
	}
	
	protected virtual void OnEnable() 
	{
		if (OnEnabled != null) OnEnabled.Invoke();
		_disposables = new CompositeDisposable ();
	}
	
	protected virtual void OnDisable() 
	{
		if (OnDisabled != null) OnDisabled.Invoke();
		_disposables.Dispose ();
	}

}

[Author("Donghyun You","20150503")]
public class MonoView<T> : MonoView where T : ViewModel {

	public ReactiveProperty<T> MainViewModel { get; private set; }

	protected override void OnEnable ()
	{
		base.OnEnable ();
		_disposables.Add(this.MainViewModel.Subscribe (OnMainViewModelChanged));
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
	}

	protected virtual void OnMainViewModelChanged(T model) 
	{
	}

}
