namespace IniSerialization.Ini
{
	using System;
	using System.Diagnostics.Contracts;

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IniKeyAttribute : Attribute
	{
		public IniKeyAttribute(string name, string defaultValue = "")
		{
			Contract.Requires(name != null);
			Contract.Requires(defaultValue != null);

			name = name.Trim();

			if (IniKey.IsValidName(name))
			{
				Key = new IniKey(name, defaultValue);
			}
		}

		public IniKey Key { get; private set; }
	}
}