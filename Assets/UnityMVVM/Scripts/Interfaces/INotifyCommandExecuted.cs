using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Summary:
//     Notify Binders to command executed
public interface INotifyCommandExecuted
{
	// Summary:
	//     Occurs when a property value changes.
	event CommandExecutedEventHandler CommandExecuted;
}
