using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using System.Reflection;

public static class EditorTools {
	
	/// <summary>
	/// make floding easy
	/// </summary>
	/// <returns>new folding state</returns>
	/// <param name="table">Table.</param>
	/// <param name="key">Key.</param>
	/// <param name="name">Name.</param>
	public static bool FoldOut(HashSet<string> table,string key,string name) {
		
		var visible = EditorGUILayout.Foldout(table.Contains(key),name);
		
		if(!table.Contains(key) && visible) 
		{
			table.Add(key);
		}
		else if(table.Contains(key) && !visible)
		{
			table.Remove(key);
		}
		return visible;

	}

	public static void BeginContents() {
		
		EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(2f));
		GUILayout.Space(10f);
		
		GUILayout.BeginVertical();
		GUILayout.Space(2f);

	}

	public static void EndContents() {
		
		GUILayout.Space(3f);

		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(3f);
	}

	public static object GetDefaultValue(Type type) {
		
		if (type == typeof(string)) {
			return "";
		} else {
			return System.Activator.CreateInstance(type) as object;
		}
		
	}

	public static void DrawSeparator ()
	{
		GUILayout.Space(3f);
		
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = EditorGUIUtility.whiteTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 0f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 0f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 1f, Screen.width, 1f), tex);
			GUI.color = Color.white;

		}

		GUILayout.Space(3f);
	}
	
	public static string GetTypeName(Type type) {
		
		if(type.IsGenericType) 
		{
			int	n=0;
			int typeNameLen = type.Name.Length;
			return type.GetGenericArguments().Aggregate(type.Name.Substring(0,typeNameLen-2),(s,t)=>s+((n++==0)?"<":",")+t)+">";
		}
		else 
		{
			return type.Name;
		}
		
	}
//
//	public static void DrawGridEntity(System.Action<int,object> onChanged,params object[] values) {
//		
//		EditorGUILayout.BeginHorizontal ();
//
//		int oldHash = 0;
//
//		for(int i=0,d=values.Length;i<d;i++) {
//
//			oldHash = values[i].GetHashCode();
//			values[i] = DrawDynamicField(values[i]);
//
//			if(oldHash != values[i].GetHashCode()) {
//				onChanged(i,values[i]);
//			}
//		}
//		
//		EditorGUILayout.EndHorizontal ();
//	}

	public static object DrawDynamicField(FieldInfo field,object target,params GUILayoutOption[] guiparams) {
		
		if(typeof(ViewModel).IsAssignableFrom(field.FieldType)) 
		{
			var value = field.GetValue(target);
			if(GUILayout.Button(value != null ? value.ToString() : "null",guiparams)) 
			{
				ViewModelEditorView.OpenWindow(field,target as object);
			}
			return value;
		}
		else
		{
			return DrawDynamicField(field.GetValue(target),field.FieldType,guiparams);
		}
		
	}

	public static object DrawDynamicField(PropertyInfo prop,object target,params GUILayoutOption[] guiparams) {
		
		if(typeof(ViewModel).IsAssignableFrom(prop.PropertyType)) 
		{
			var value = prop.GetValue(target,null);
			if(GUILayout.Button(value != null ? value.ToString() : "null",guiparams)) 
			{
				ViewModelEditorView.OpenWindow(prop,target as object);
			}
			return value;
		}
		else
		{
			return DrawDynamicField(prop.GetValue(target,null),prop.PropertyType,guiparams);
		}

	}

	public static object DrawDynamicField(object target,Type type, params GUILayoutOption[] guiparams) {

		if (type.IsEnum) {
			return EditorGUILayout.EnumPopup ((Enum)target, guiparams);
		}

		if (type == typeof(string)) {
			return EditorGUILayout.TextField (target as string, guiparams) as object;
		}
	
		if (type == typeof(int)) {
			return EditorGUILayout.IntField ((int)target, guiparams) as object;
		}
	
		if (type == typeof(float)) {
			return EditorGUILayout.FloatField ((float)target, guiparams) as object;
		}

		if (type == typeof(DateTime)) {
			return new DateTime (EditorGUILayout.LongField (((DateTime)target).Ticks, guiparams));
		}

		return target;
	}
	


}
