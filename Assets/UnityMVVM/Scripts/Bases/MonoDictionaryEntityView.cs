using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using System.Reflection;
using UniRx;

[Author("Donghyun You","20150512")]
public abstract class MonoDictionaryEntityView : MonoView {}

[Author("Donghyun You","20150512")]
public abstract class MonoDictionaryEntityView<TKey,TValue> : MonoDictionaryEntityView 
	where TValue : ViewModel 
{

	TKey _key;
	public TKey Key { 
		get {
			return _key;
		}
		set {
			if(_key.GetHashCode() != value.GetHashCode()) 
			{
				_key = value;
				_onRequireCheck();
			}
		}
	}

	public ReactiveDictionary<TKey,TValue> MainViewModelDictionary { get; private set; }

	public ReadOnlyReactiveProperty<TValue> MainViewModel { get; private set; }
	ReactiveProperty<TValue> _mainViewModel;
	
	protected virtual void Awake() 
	{
		this.InjectModel ();
	}

	public MonoDictionaryEntityView() 
	{
		MainViewModel = new ReadOnlyReactiveProperty<TValue> (_mainViewModel = new ReactiveProperty<TValue> ());
	}
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		_subscribe ();
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
		_unsubscribe ();
	}

	private void _subscribe() 
	{
		_disposables.Add(this.MainViewModel.Subscribe (OnMainViewModelChanged));
		
		TValue val;
		if (MainViewModelDictionary.TryGetValue (this.Key, out val)) 
		{
			// require wait for replaced, reset, removed. 
			this._mainViewModel.Value = val;
			_disposables.Add(MainViewModelDictionary.ObserveReset().Subscribe(x=>this._onRequireCheck()));
			_disposables.Add(MainViewModelDictionary.ObserveRemove().Where(ev=>ev.Key.GetHashCode() == this.Key.GetHashCode()).Subscribe(x=>this._onRequireCheck()));
			_disposables.Add(MainViewModelDictionary.ObserveReplace().Where(ev=>ev.Key.GetHashCode() == this.Key.GetHashCode()).Subscribe(x=>this._onRequireCheck()));
		} 
		else 
		{
			// require wait for added
			this._mainViewModel.Value = null;
			_disposables.Add(MainViewModelDictionary.ObserveAdd().Where(ev=>ev.Key.GetHashCode() == this.Key.GetHashCode()).Subscribe(x=> _onRequireCheck()));
		}
	}

	private void _unsubscribe() {
		_disposables.Dispose ();
	}

	private void _onRequireCheck() {
		_unsubscribe ();
		_subscribe ();
	}

	
	protected virtual void OnMainViewModelChanged (TValue model) {}
	
}
