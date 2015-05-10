using System;
using System.Collections.Generic;

[Author("Donghyun You","20150503")]
public class CommandExecutedEventArgs : EventArgs
{
	public CommandExecutedEventArgs(ViewModel viewModel, Command command) 
	{
		this.ViewModel  = viewModel;
		this.Command    = command;
	}
	public ViewModel   ViewModel   { get; private set; }
	public Command     Command     { get; private set; }
}
