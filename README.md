Ace.Ini
=======

##### Ace libraries are few libraries that are developed as part of another project that I decided to release to the public.

This library is responsible to read and manipulate INI files.

Currently, it contains a parser and a pretty basic deserializer.

### Build Requirements

* .NET Framework 4.x

### Examples

Deserializing an INI string into a data structure. 

	private static void Main(string[] args)
	{
		IniSerializer serializer = new IniSerializer();
	
		const string data = @"	Name = Just an example
						
								[Application]
								Path = X:\Application\app.exe
								Options = /x /y";
	
		Settings settings = serializer.Deserilize<Settings>(data);
	
		Console.WriteLine("\r\n {0}\r\n\r\n {1} {2}", settings.Name, settings.Application.Path, settings.Application.Options);
	
		Console.ReadKey(true);
	}
  
Parsing the data into array from a collection.

	private static void Main(string[] args)
	{
		const string data = @"	Name = Just an example
						
								[Application]
								Path = X:\Application\app.exe
								Options = /x /y";
	
		IniSectionCollection sections = IniSectionCollection.Parse(data);
	
		string[] array = sections.ToStringArray();
	
		foreach (var item in array)
		{
			Console.WriteLine(item);
		}
	
		Console.ReadKey(true);
	}