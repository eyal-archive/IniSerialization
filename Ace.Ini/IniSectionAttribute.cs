namespace Ace.Ini
{
	using System;
	using System.Diagnostics.Contracts;

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IniSectionAttribute : Attribute
	{
		public IniSectionAttribute(string sectionName)
		{
			Contract.Requires(sectionName != null);

			sectionName = sectionName.Trim();

			if (IniSection.IsValidSectionName(sectionName))
			{
				SectionName = sectionName;
			}
		}

		public string SectionName { get; set; }
	}
}