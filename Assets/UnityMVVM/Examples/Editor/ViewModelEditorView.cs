using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

public class ViewModelEditorView : EditorWindow {

	public class Accesser {
		public PropertyInfo Info;
		public object Carrier;
	}

	public Accesser 	Target { get; private set; }

	Stack<Accesser>		_oldTargets = new Stack<Accesser> ();
	Vector2 			_scrollPos 	= Vector2.zero;

	public static void OpenWindow(PropertyInfo prop,object carrier) 
	{
		var window = EditorWindow.GetWindow<ViewModelEditorView>("ViewModel",true);

		if (window.Target != null) 
		{
			window._oldTargets.Push(window.Target);
		}

		window.Target = new Accesser { Info = prop, Carrier = carrier };
		window.Show ();
	}
	
	public void OnGUI() {

		if (_oldTargets.Count > 0) 
		{
			if(GUILayout.Button("<<",GUILayout.Width(100f))) {
				this.Target = _oldTargets.Pop();
				GUI.FocusControl(null);
			}
		}

		if (Target == null) return;

		var target = Target.Info.GetValue (Target.Carrier, null);

		if (target == null) 
		{
			if(GUILayout.Button("Create New Instance of "+Target.Info.PropertyType)) {
				Target.Info.SetValue(Target.Carrier,EditorTools.GetDefaultValue(Target.Info.PropertyType),null);
			}
			return;
		}

		PropertyInfo[] mainProps = target.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.Public);
		_scrollPos = EditorGUILayout.BeginScrollView (_scrollPos);
		
		foreach (var prop in mainProps) 
		{

			if(!prop.PropertyType.IsValueType && prop.PropertyType != typeof(string))
			{
				var buttonName = target == null ? "null" : prop.Name;
				if (GUILayout.Button ("("+EditorTools.GetTypeName(prop.PropertyType)+")"+buttonName)) 
				{
					ViewModelEditorView.OpenWindow(prop,target);
				}
			}
			else
			{
				prop.SetValue(target,this._drawSingleProperty (prop.Name, prop.GetValue (target, null),prop.PropertyType),null);
			}
		}
		
		EditorGUILayout.EndScrollView ();
		
	}

	
	/// <summary>
	/// draws the single property.
	/// </summary>
	/// <returns>The single property.</returns>
	/// <param name="label">Label.</param>
	/// <param name="target">Target.</param>
	object _drawSingleProperty(string label,object target,Type type) 
	{
		if (type == typeof(string)) {
			return EditorGUILayout.TextField (label,target as string) as object;
		}
		
		if (type == typeof(int)) {
			return EditorGUILayout.IntField(label,(int)target) as object;
		}
		
		if (type == typeof(float)) {
			return EditorGUILayout.FloatField(label,(float)target) as object;
		}
		
		if (type == typeof(Color)) {
			return EditorGUILayout.ColorField(label,(Color)target) as object;
		}
		
		if (type == typeof(Vector2)) {
			return EditorGUILayout.Vector2Field(label,(Vector2)target) as object;
		}
		
		if (type == typeof(Vector3)) {
			return EditorGUILayout.Vector3Field(label,(Vector3)target) as object;
		}
		
		if (type == typeof(Vector4)) {
			return EditorGUILayout.Vector4Field(label,(Vector4)target) as object;
		}

		return target;
	}
}
