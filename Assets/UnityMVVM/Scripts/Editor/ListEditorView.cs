// The MIT License (MIT)
//
//	Copyright (c) <2015> <Donghyun-You>
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

/// <summary>
/// List editor view.
/// </summary>
public static class ListEditorView {
	
	public class ListDesc 
	{
		
		public IList	 		Body;
		public object 			EditingValue;
		public string[] 		ColumnList;
		public PropertyInfo[] 	PropertiesList;
		public FieldInfo[] 		FieldsList;
		public int	 			SortBy;
		public bool				SortByAsc=true;
		public Type				ValueType;
		
		public List<int> 		ViewKeys = new List<int> ();
		public int				ViewBegin = 0;
		public int				ViewRange = 20;
		
		public Vector2			ListScrollPos = Vector2.zero;
		
		public float			EntityWidth=100f;
		public float			EntityHeight=20f;
	}
	
	static Dictionary<IList,ListDesc> _localListDescs = new Dictionary<IList, ListDesc>();
	
	/// <summary>
	/// Draw Editor of the list
	/// </summary>
	/// <param name="target">Target.</param>
	public static void DrawList(IList list) {
		
		// add or obtain bases
		ListDesc info;
		if(!_localListDescs.TryGetValue(list,out info)) {

			var genericTypes = list.GetType().GetGenericArguments();
			var valType = genericTypes[0];

			info = _localListDescs [list] = new ListDesc() 
			{
				Body 			= list,
				ColumnList 		= _getColumnList(list),
				FieldsList		= valType
									.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
									.Where(field=>field.FieldType.IsValueType || field.FieldType == typeof(string))
									.ToArray(),
				PropertiesList 	= valType
									.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
									.Where(prop=>prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
									.ToArray(),
				EditingValue 	= _getDefaultValue(valType),
				ValueType		= valType,
			};
			
		}
		
		EditorTools.DrawSeparator ();
		
		EditorGUILayout.BeginHorizontal ();
		
		var sortBy = GUILayout.SelectionGrid (info.SortBy, info.ColumnList, info.ColumnList.Length,
		                                      GUILayout.Height((info.EntityHeight)),GUILayout.Width((info.EntityWidth+3f)*info.ColumnList.Length));
		
		if (sortBy != info.SortBy) 
		{
			info.SortBy = sortBy;
			_updateSortedList(info);
			GUI.FocusControl(null);
		}
		
		if (GUILayout.Button (info.SortByAsc?"Asc":"Desc",GUILayout.Width(50f))) 
		{
			info.SortByAsc = !info.SortByAsc;
			_updateSortedList(info);
			GUI.FocusControl(null);
		}
		
		EditorGUILayout.EndHorizontal ();
		
		var pos = EditorGUILayout.BeginScrollView (info.ListScrollPos,GUILayout.Height(200f));
		
		// must store for repaint use same position info of Layout. other control could be changed by life cycles
		if (Event.current.type != EventType.Repaint) 
		{
			info.ListScrollPos = pos;
		}
		
		int beginIndex 	= Mathf.Max(0,(int)(info.ListScrollPos.y / (info.EntityHeight+2f)));
		int showRange 	= (int)(250f / (info.EntityHeight+2f));
		int endIndex 	= Mathf.Min (info.ViewKeys.Count, beginIndex + showRange);
		
		GUILayout.Space((info.EntityHeight+2f)*beginIndex);
		
		for(int i=beginIndex,d=endIndex;i<d;i++) {
			
			// #######################################################################################
			// Render entities
			// #######################################################################################
			
			EditorGUILayout.BeginHorizontal();
			
			var key = info.ViewKeys[i];
			
			// key button
			GUILayout.Label(key.ToString(),GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight));
			
			// render values (direct value or class structure)
			if (_isDirectReplacable (info.Body)) 
			{
				info.Body[key] = EditorTools.DrawDynamicField(info.Body[key],info.ValueType,GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight));
			} 
			else 
			{
				foreach(var fieldInfo in info.FieldsList) {
					fieldInfo.SetValue(info.Body[key],EditorTools.DrawDynamicField(fieldInfo.GetValue(info.Body[key]),fieldInfo.FieldType,GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight)));
				}
				
				foreach(var propInfo in info.PropertiesList) {
					propInfo.SetValue(info.Body[key],EditorTools.DrawDynamicField(propInfo.GetValue(info.Body[key],null),propInfo.PropertyType,GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight)),null);
				}
			}
			
			// Remove
			if (GUILayout.Button ("Remove",GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight))) 
			{
				info.Body.RemoveAt(key);
				_updateSortedList(info);
				GUI.FocusControl(null);
				return;
			}
			
			EditorGUILayout.EndHorizontal();
			
			// #######################################################################################
		}
		
		if (info.ViewKeys.Count > endIndex) {
			GUILayout.Space ((info.EntityHeight+2f) * (info.ViewKeys.Count - endIndex));
		}
		
		EditorGUILayout.EndScrollView ();
		
		GUILayout.Space (5f);
		
		EditorTools.DrawSeparator ();
		
		// ===================================================================================================================================================
		// Add field
		// ===================================================================================================================================================
		
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField (info.Body.Count.ToString(),GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight));

		if (_isDirectReplacable (info.Body)) 
		{
			info.EditingValue = EditorTools.DrawDynamicField(info.EditingValue,info.ValueType,GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight));
		} 
		else 
		{	
			foreach(var fieldInfo in info.FieldsList) {
				fieldInfo.SetValue(info.EditingValue,EditorTools.DrawDynamicField(fieldInfo.GetValue(info.EditingValue),fieldInfo.FieldType,GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight)));
			}
			
			foreach(var propInfo in info.PropertiesList) {
				propInfo.SetValue(info.EditingValue,EditorTools.DrawDynamicField(propInfo.GetValue(info.EditingValue,null),propInfo.PropertyType,GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight)),null);
			}
		}
		
		if (GUILayout.Button ("Add",GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight)))
		{
			
			if(info.EditingValue is ViewModel) {
				(info.EditingValue as ViewModel).Populate();
			}
			
			info.Body.Add(info.EditingValue);
			info.EditingValue = _getDefaultValue(info.ValueType);
			_updateSortedList(info);
			GUI.FocusControl(null);
		}
		if (GUILayout.Button ("Add Mount",GUILayout.Width(info.EntityWidth),GUILayout.Height(info.EntityHeight)))
		{
			for(int i=0;i<100;i++) {
				info.Body.Add(info.EditingValue);
				info.EditingValue = _getDefaultValue(info.ValueType);
				GUI.FocusControl(null);
			}
			_updateSortedList(info);
		}
		EditorGUILayout.EndHorizontal();
		
		// ===================================================================================================================================================
		
	}
	
	static void _updateSortedList(ListDesc info) {
		
		info.ViewKeys.Clear ();

		if(info.Body.Count > 0)
			info.ViewKeys.AddRange (Enumerable.Range (0, info.Body.Count));

		if (info.ValueType.IsValueType || info.ValueType == typeof(string)) {
			_sortByValueTypeValue(info);
		} 
		else if (info.SortBy == 0) 
		{
			_sortByKey (info);
		} 
		else 
		{
			_sortByValue (info);
		}
		
		
	}
	
	static void _sortByKey(ListDesc info) {

		info.ViewKeys.Sort ((x,y) => {
			
			return info.SortByAsc ? (x as IComparable).CompareTo(y) : (y as IComparable).CompareTo(x);

		});
		

	}
	
	static void _sortByValueTypeValue(ListDesc info) {
		
		info.ViewKeys.Sort ((x,y) => {
			
			var valueCompare = info.SortByAsc ? (info.Body[x] as IComparable).CompareTo(info.Body[y]) : (info.Body[y] as IComparable).CompareTo(info.Body[x]);
			var keyCompare = info.SortByAsc ? (x as IComparable).CompareTo(y) : (y as IComparable).CompareTo(x);
			
			return valueCompare*0xffff+keyCompare;

		});
	}

	static void _sortByValue(ListDesc info) {
		
		MemberInfo member 		= null;
		Type handlingType 		= null;
		bool sortByFieldInfo 	= info.SortBy <= info.FieldsList.Length;
		
		if (sortByFieldInfo) {
			// sort by field
			var fieldInfo = info.FieldsList [info.SortBy - 1];
			handlingType = fieldInfo.FieldType;
			member = fieldInfo;
		} else {
			var propInfo = info.PropertiesList [info.SortBy - 1 - info.FieldsList.Length];
			handlingType = propInfo.PropertyType;
			member = propInfo;
		}

		// make sure handling type is castable into IComparer
		if(!typeof(IComparable).IsAssignableFrom(handlingType)) return;

		info.ViewKeys.Sort ((x,y) => {
			
			var l = info.Body[x];
			var r = info.Body[y];
			
			IComparable lcomp,rcomp;
			
			int keyCompare = info.SortByAsc ? (x as IComparable).CompareTo(y) : (y as IComparable).CompareTo(x);
			int valCompare = 0;
			if(info.SortBy > 0) {
				
				if(sortByFieldInfo) 
				{
					// sort by field
					var handle 	= member as FieldInfo;
					lcomp 		= (IComparable)(handle.GetValue(l) ?? _getDefaultValue(handle.FieldType));
					rcomp 		= (IComparable)(handle.GetValue(r) ?? _getDefaultValue(handle.FieldType));
					
				}
				else 
				{
					// sort by prop
					var handle 	= member as PropertyInfo;
					lcomp 		= (IComparable)(handle.GetValue(l,null) ?? _getDefaultValue(handle.PropertyType));
					rcomp 		= (IComparable)(handle.GetValue(r,null) ?? _getDefaultValue(handle.PropertyType));
					
				}

				valCompare = info.SortByAsc ? lcomp.CompareTo(rcomp) : rcomp.CompareTo(lcomp);

			}
			
			return valCompare*0xffff+keyCompare;
			
		});
	}
	
	static bool _isDirectReplacable(IList dic) {
		
		var genericTypes = dic.GetType().GetGenericArguments();
		var valType = genericTypes[0];
		return valType == typeof(string) || valType.IsValueType;
		
	}
	
	static string[] _getColumnList(IList list) {
		
		var genericTypes = list.GetType().GetGenericArguments();
		var valType = genericTypes[0];
		
		
		if (_isDirectReplacable(list)) {
			return new string[] { "Key","Value" };
		} 
		// consider it's custom class.
		else 
		{
			return 
			new string[] {"Key"}
			.Concat(valType
			        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
			        .Where(field=>field.FieldType.IsValueType || field.FieldType == typeof(string))
			        .Select(field=>field.Name))
			.Concat(valType
			        .GetProperties(BindingFlags.Instance | BindingFlags.Public| BindingFlags.FlattenHierarchy)
			        .Where(prop=>prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
			        .Select(prop=>prop.Name)
			        )
				.ToArray();
		}
		
		
	}
	
	
	/// <summary>
	/// _gets the default value.
	/// </summary>
	/// <returns>The default value.</returns>
	/// <param name="type">Type.</param>
	static object _getDefaultValue(Type type) {
		
		if (type == typeof(string)) {
			return "";
		} else {
			return System.Activator.CreateInstance(type) as object;
		}
		
	}
	
	static object _getAutoIncrementalValue(Type type,ref int incrementCache) {
		
		if (type == typeof(string)) {
			return incrementCache++.ToString ();
		} 
		else if (type == typeof(System.Int64) 	|| type == typeof(System.Int32) 	|| type == typeof(System.Int16) ||
		         type == typeof(System.UInt64) 	|| type == typeof(System.UInt32) 	|| type == typeof(System.UInt16) ||
		         type == typeof(System.Char) 	|| type == typeof(System.Byte) )
		{
			return incrementCache++;
		} 
		else 
		{
			return System.Activator.CreateInstance(type) as object;
		}
		
	}
}