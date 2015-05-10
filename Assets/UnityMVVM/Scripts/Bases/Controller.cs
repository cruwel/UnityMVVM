using System;
using System.Collections.Generic;

using System.Reflection;
using UniRx;

public abstract class Controller<TViewModel,TSingletonInstance>
	: Singleton<TSingletonInstance>
	where TSingletonInstance : class, new()
	where TViewModel : ViewModel
{

	public Controller() {
		this.InitializeController ();
	}

	protected void InitializeController() {
	
	}

	public virtual void Initialize (TViewModel viewModel) 
	{
		viewModel.Controller = this;
	}

}
