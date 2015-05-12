using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UniLinq; // System.Linq is unsafe on AOT
using UniRx;
using System.Reflection;

public class ViewModelManagerBase {
	
	// ========================================================================================================================
	// Private Members
	// ========================================================================================================================
	
	Dictionary<string,PropertyInfo> _exportables;
	
	// ========================================================================================================================
	// Constructor
	// ========================================================================================================================
	/// <summary>
	/// Initializes a new instance of the <see cref="ViewModelManager"/> class.
	/// </summary>
	public ViewModelManagerBase() 
	{
		// ====================================================================================================================
		// make empty default containers
		// ====================================================================================================================
		var props = this.GetType ().GetProperties ().Where (prop => prop.GetCustomAttributes (typeof(ExportableModel), true) != null);
		
		foreach (var prop in props) {
			prop.SetValue(this,System.Activator.CreateInstance(prop.PropertyType),null);
		}
		
		_exportables = props.ToDictionary (prop => prop.Name);
		// ====================================================================================================================
	}

	
	// ========================================================================================================================
	// Utilities
	// ========================================================================================================================
	
	/// <summary>
	/// Inject related values
	/// </summary>
	/// <param name="target">Target.</param>
	public void Populate(object target) {
		
		var props = target.GetType ().GetProperties ().Where(prop=> prop.GetCustomAttributes(typeof(Populatable),true).Any());
		
		PropertyInfo table = null;
		foreach (var prop in props) {
			
			var attr = prop.GetCustomAttributes(typeof(Populatable),true).First() as Populatable;
			
			if(!_exportables.TryGetValue(attr.ViewModelTableName,out table)) 
			{
				throw new System.InvalidProgramException("Invalid ViewModelTableName : "+attr.ViewModelTableName);
			}
			
			if(typeof(IDictionary).IsAssignableFrom(table.PropertyType)) 
			{
				var dic = table.GetValue(this,null) as IDictionary;
				var srcProp = target.GetType().GetProperty(attr.HandlingKey);
				
				#if UNITY_EDITOR
				if(srcProp == null) throw new System.InvalidProgramException("Invalid Handling Key");
				#endif
				
				var key = srcProp.GetValue(target,null);
				
				if(dic != null && dic.Contains(key)) 
					prop.SetValue(target,dic[key],null);
				else 
					prop.SetValue(target,null,null);
				
			}
			else if(typeof(IList).IsAssignableFrom(table.PropertyType)) 
			{
				var list = table.GetValue(this,null) as IList;
				var srcProp = target.GetType().GetProperty(attr.HandlingKey);
				
				#if UNITY_EDITOR
				if(srcProp == null) throw new System.InvalidProgramException("Invalid Handling Key");
				if(!srcProp.PropertyType.IsValueType) throw new System.InvalidProgramException("Handling Key is invalid. it must be value-type able to cast into int");
				#endif
				
				var key = (int)srcProp.GetValue(target,null);
				
				if(list != null && list.Count >= key) 
					prop.SetValue(target,list[key],null);
				else 
					prop.SetValue(target,null,null);
				
			}
			
		}
		
	}
}
