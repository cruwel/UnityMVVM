[Author("Donghyun You","20150503")]
public abstract class ViewModel : DisposableBase
{
	public ViewModel() {
		this.Populate ();
	}

	public void Populate() {
		ViewModelManager.Instance.Populate (this);
	}

	public object Controller { get; set; }
}
