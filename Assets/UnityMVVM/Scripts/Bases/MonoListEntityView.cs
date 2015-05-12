using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using System.Reflection;
using UniRx;

[Author("Donghyun You","20150512")]
public abstract class MonoListEntityView : MonoView {}

[Author("Donghyun You","20150512")]
public abstract class MonoListEntityView<TValue> : MonoListEntityView 
	where TValue : ViewModel 
{
	
	int _index=-1;
	public int Index { 
		get {
			return _index;
		}
		set {
			if(_index != value) 
			{
				_index = value;
				_onRequireCheck();
			}
		}
	}
	
	public ReactiveCollection<TValue> MainViewModelList { get; private set; }
	
	public ReadOnlyReactiveProperty<TValue> MainViewModel { get; private set; }
	ReactiveProperty<TValue> _mainViewModel;
	
	protected virtual void Awake() 
	{
		this.InjectModel ();
	}
	
	public MonoListEntityView() 
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
		if(this.Index > 0)
			if (MainViewModelList.Count > this.Index) 
		{
			// require wait for replaced, reset, removed. 
			this._mainViewModel.Value = MainViewModelList[this.Index];
			_disposables.Add(MainViewModelList.ObserveReset().Subscribe(x=>this._onRequireCheck()));
			_disposables.Add(MainViewModelList.ObserveMove().Where(ev=>ev.OldIndex == this.Index || ev.NewIndex == this.Index).Subscribe(x=>this._onRequireCheck()));
			_disposables.Add(MainViewModelList.ObserveRemove().Where(ev=>ev.Index == this.Index).Subscribe(x=>this._onRequireCheck()));
			_disposables.Add(MainViewModelList.ObserveReplace().Where(ev=>ev.Index == this.Index).Subscribe(x=>this._onRequireCheck()));
		} 
		else 
		{
			// require wait for added
			this._mainViewModel.Value = null;
			_disposables.Add(MainViewModelList.ObserveAdd().Where(ev=>ev.Index == this.Index).Subscribe(x=> _onRequireCheck()));
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
