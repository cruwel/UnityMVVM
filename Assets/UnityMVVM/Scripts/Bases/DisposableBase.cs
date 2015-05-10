using System;

public abstract class DisposableBase : IDisposable
{
	// Flag: Has Dispose already been called?
	bool _disposed = false;

	// Public implementation of Dispose pattern callable by consumers.
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Protected implementation of Dispose pattern.
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		if (disposing)
		{
			// Free any other managed objects here.
		}

		// Free any unmanaged objects here.
		//
		_disposed = true;
	}
}
