namespace Ace.Ini.Tests.Dummy
{
	internal class ComplexSection
	{
		public string ReadOnlyProperty { get; private set; }

		public string ReadWriteProperty { get; set; }

		public Section Section { get; set; }

		public string WriteOnlyProperty { private get; set; }
	}
}