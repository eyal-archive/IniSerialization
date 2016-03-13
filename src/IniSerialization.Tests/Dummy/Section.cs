namespace IniSerialization.Ini.Tests.Dummy
{
	internal class Section
	{
		public string Key { get; set; }

		public string ReadOnlyProperty { get; private set; }

		public string WriteOnlyProperty { private get; set; }
	}
}