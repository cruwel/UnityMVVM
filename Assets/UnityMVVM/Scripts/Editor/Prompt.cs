using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class Prompt : EditorWindow {
	
	public static void OpenWindow(string title,string value,System.Action<string> onSubmit) {
		var window = EditorWindow.GetWindow<Prompt>(title,true);
		window.Show ();
		window.OnSubmit = onSubmit;
		window.Value = value;
	}

	public string Value { get; private set; }
	public System.Action<string> OnSubmit { get; private set; }

	public void OnGUI() {

		this.Value = (string)EditorTools.DrawDynamicField (this.Value, typeof(string));
		if (GUILayout.Button ("Submit")) {
			OnSubmit(this.Value);
			this.Close();
		}

	}

}
