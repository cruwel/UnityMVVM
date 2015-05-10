using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UniLinq; // System.Linq is unsafe on AOT
using UniRx;
using System.Reflection;

public class ViewModelManager : Singleton<ViewModelManager> {

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
	public ViewModelManager() 
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
	// Models
	// ========================================================================================================================

	[ExportableModel]
	public ReactiveProperty<UserInfo> Me { get; private set; }

	[ExportableModel]
	public ReactiveDictionary<string,UserInfo> Friends { get; private set; }

	[ExportableModel]
	public ReactiveDictionary<int,ItemInfo> Items { get; private set; }
	
	[ExportableModel]
	public Dictionary<int,ItemDesc> ItemDesc { get; private set; }
	
	[ExportableModel]
	public ReactiveDictionary<int,int> IntDicTest { get; private set; }

	[ExportableModel]
	public ReactiveCollection<int> IntTest { get; private set; }

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

			#if UNITY_EDITOR
			if(!_exportables.TryGetValue(attr.ViewModelTableName,out table)) 
			{
				throw new System.InvalidProgramException("Invalid ViewModelTableName : "+attr.ViewModelTableName);
			}
			#endif

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
				
				if(list != null && list.Contains(key)) 
					prop.SetValue(target,list[key],null);

			}

		}

	}
}
