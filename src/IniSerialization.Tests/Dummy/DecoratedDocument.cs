namespace IniSerialization.Ini.Tests.Dummy
{
	internal class DecoratedDocument
	{
		[IniKey("Key", "Value1")]
		public string DecoratedKey { get; set; }

		[IniSection("Section")]
		public AnotherSection DecoratedSection { get; set; }

		public Section FirstSection { get; set; }

		public string ReadOnlyProperty { get; private set; }

		public Section SecondSection { get; set; }

		public string WriteOnlyProperty { private get; set; }
	}
}