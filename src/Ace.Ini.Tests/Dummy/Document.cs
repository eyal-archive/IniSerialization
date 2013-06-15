namespace Ace.Ini.Tests.Dummy
{
	internal class Document
	{
		public Section FirstSection { get; set; }

		public string Key { get; set; }

		public string ReadOnlyProperty { get; private set; }

		public Section SecondSection { get; set; }

		public AnotherSection Section { get; set; }

		public string WriteOnlyProperty { private get; set; }
	}
}