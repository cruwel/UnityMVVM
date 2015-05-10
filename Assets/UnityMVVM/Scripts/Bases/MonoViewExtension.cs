using UnityEngine;
using System.Collections;

using System.Reflection;
using UniLinq;

[Author("Donghyun You","20150503")]
public static class MonoViewExtension {
	
	/// <summary>
	/// Injects the view model to mono view
	/// </summary>
	public static void InjectModel(this MonoView self) {
		
		// ====================================================================================================================
		// props
		// ====================================================================================================================

		var mainViewModelInjection 	= self.GetType ().GetCustomAttributes (typeof(InjectModel), false).First () as InjectModel;
		var members					= ViewModelManager.Instance.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).Where(field=>field.GetCustomAttributes(typeof(ExportableModel),false).Count()>0).ToDictionary(field=>field.Name);
		var fields 					= self.GetType ().GetFields 	(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where (field => field.GetCustomAttributes(typeof(InjectModel),false).Count()>0);
		var properties 				= self.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where (field => field.GetCustomAttributes(typeof(InjectModel),false).Count()>0);

		// ====================================================================================================================
		// inject into main view model
		// ====================================================================================================================

		var mainViewModelProp = self.GetType ().GetProperty ("MainViewModel");

		if (mainViewModelProp != null) 
		{
			PropertyInfo pi;
			if(members.TryGetValue(mainViewModelInjection.Selector,out pi)) 
			{
				mainViewModelProp.SetValue(self,pi.GetValue(ViewModelManager.Instance,null),null);
			}
			#if DEBUG
			else 
			{
				throw new System.InvalidProgramException(mainViewModelInjection.Selector+" is not exists on ViewModelManager");
			}
			#endif
		}

		// ====================================================================================================================
		// inject into field
		// ====================================================================================================================

		foreach (var field in fields) 
		{
			// works with o(n=(0-1))
			var 			attr = field.GetCustomAttributes(typeof(InjectModel),false).Cast<InjectModel>().First();
			PropertyInfo 	pi;
			
			if(members.TryGetValue(attr.Selector,out pi)) 
			{
				field.SetValue(self,pi.GetValue(ViewModelManager.Instance,null));
			}
			#if DEBUG
			else 
			{
				throw new System.InvalidProgramException(attr.Selector+" is not exists on ViewModelManager");
			}
			#endif
		}
		
		// ====================================================================================================================
		// inject into property
		// ====================================================================================================================

		foreach (var property in properties) 
		{
			// works with o(n=(0-1))
			var 			attr = property.GetCustomAttributes(typeof(InjectModel),false).Cast<InjectModel>().First();
			PropertyInfo 	pi;

			if(members.TryGetValue(attr.Selector,out pi)) 
			{
				property.SetValue(self,pi.GetValue(ViewModelManager.Instance,null),null);
			}
			#if DEBUG
			else 
			{
				throw new System.InvalidProgramException(attr.Selector+" is not exists on ViewModelManager");
			}
			#endif
		}

		// ====================================================================================================================
		
	}

}
