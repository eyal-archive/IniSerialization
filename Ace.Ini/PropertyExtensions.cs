namespace Ace.Ini
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reflection;

	internal static class PropertyExtensions
	{
		public static T GetAttribute<T>(this PropertyInfo property) where T : Attribute
		{
			Contract.Requires(property != null);

			var attributes = property.GetCustomAttributes(typeof(T), false);

			return attributes != null ? attributes.FirstOrDefault() as T : null;
		}
	}
}