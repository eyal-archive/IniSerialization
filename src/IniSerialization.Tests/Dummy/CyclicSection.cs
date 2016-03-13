namespace IniSerialization.Ini.Tests.Dummy
{
	internal class CyclicSection
	{
		public ComplexSection FirstSection { get; set; }

		public string ReadOnlyProperty { get; private set; }

		public string ReadWriteProperty { get; set; }

		public CyclicSection SecondSection { get; set; }

		public string WriteOnlyProperty { private get; set; }
	}
}