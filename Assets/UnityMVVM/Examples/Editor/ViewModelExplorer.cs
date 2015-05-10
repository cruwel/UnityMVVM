// The MIT License (MIT)
//
//	Copyright (c) 2015 Donghyun-You(github.com#cruwel)
//	
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//	copies of the Software, and to permit persons to whom the Software is
//	furnished to do so, subject to the following conditions:
//	
//	The above copyright notice and this permission notice shall be included in
//	all copies or substantial portions of the Software.
//	
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//	THE SOFTWARE.

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // it's safe on editor

using System.Reflection;

public class ViewModelExplorer : EditorWindow {
	
	HashSet<string> 			foldSet		 	= new HashSet<string>();
	Vector2 					scrollPos 		= Vector2.zero;

    [MenuItem("MVVM/View Model Explorer")]
	public static void OpenWindow() {
		var window = EditorWindow.GetWindow<ViewModelExplorer>("VMExplorer",true);
		window.Show ();
	}

	public void OnEnable() {
		EditorApplication.update += this.Repaint;
	}
	
	public void OnDisable() {
		EditorApplication.update -= this.Repaint;
    }
    
	public void OnGUI() {

		PropertyInfo[] mainProps = ViewModelManager.Instance.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.Public);
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

		foreach (var prop in mainProps) {
			string typeName = EditorTools.GetTypeName (prop.PropertyType);

			if (EditorTools.FoldOut (foldSet, prop.Name, prop.Name + " (" + typeName + ")")) {
				_drawProperties (prop.Name, prop.GetValue (ViewModelManager.Instance, null),prop.PropertyType);
			}
		}

		EditorGUILayout.EndScrollView ();

	}

	object _drawProperties(string label,object target,Type type) {

		if (typeof(IDictionary).IsAssignableFrom(type)) {

			EditorTools.BeginContents();

			DictionaryEditorView.DrawDictionary(target as IDictionary);

			EditorTools.EndContents();

			return target;
		} 
		else if (typeof(IList).IsAssignableFrom(type))
		{
			EditorTools.BeginContents();
			ListEditorView.DrawList(target as IList);
			EditorTools.EndContents();
			return target;
		}
		else if (target != null && (target.GetType ().IsValueType || target is string)) 
		{
			EditorTools.BeginContents();
			var ret= this._drawSingleProperty(label,target,target.GetType());
			EditorTools.EndContents();
			return ret;
		}
		else if (target != null)
		{
			PropertyInfo[] props = target.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);

			EditorTools.BeginContents();

			foreach (var prop in props) 
			{
				var val = prop.GetValue(target,null);
				
				if (typeof(ViewModel).IsAssignableFrom (prop.PropertyType)) 
				{
					var buttonName = target == null ? "null" : target.ToString ();
					if (GUILayout.Button ("("+EditorTools.GetTypeName(prop.PropertyType)+")"+buttonName)) 
					{
						ViewModelEditorView.OpenWindow(prop,target as object);
					}
				}
				else 
				{
					_drawSingleProperty(prop.Name,val,prop.PropertyType);
				}
			}

			EditorTools.EndContents();

			return target;
		}

		return target;
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
