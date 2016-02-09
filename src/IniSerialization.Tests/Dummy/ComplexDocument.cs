namespace Ace.Ini.Tests.Dummy
{
	internal class ComplexDocument
	{
		public ComplexSection FirstSection { get; set; }

		public string Key { get; set; }

		public string ReadOnlyProperty { get; private set; }

		public CyclicSection SecondSection { get; set; }

		public string WriteOnlyProperty { private get; set; }
	}
}