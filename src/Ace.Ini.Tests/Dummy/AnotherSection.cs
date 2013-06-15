namespace Ace.Ini.Tests.Dummy
{
	internal class AnotherSection
	{
		public string FirstKey { get; set; }

		public string ReadOnlyProperty { get; private set; }

		public string SecondKey { get; set; }

		public string WriteOnlyProperty { private get; set; }
	}
}