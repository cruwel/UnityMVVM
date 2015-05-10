using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

[Author("Donghyun You","20150503")]
public abstract class Singleton<T>
	 where T : class, new()
{
	static T _instance = new T();
	public static T Instance
	{
		get
		{
			return _instance;
		}
	}
}

[Author("Donghyun You","20150503")]
public static class Singleton {

	public static object GetInstance<T>() where T : class {
		return GetInstance(typeof(T));
	}

	public static object GetInstance(Type type) {
		var pi = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return pi.GetValue(null, null);
	}

}
