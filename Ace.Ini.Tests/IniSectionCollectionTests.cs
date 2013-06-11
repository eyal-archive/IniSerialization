namespace Ace.Ini.Tests
{
	using System.Linq;

	using FluentAssertions;

	using Xunit;

	public class IniSectionCollectionTests
	{
		public class Add
		{
			[Fact]
			public void Should_add_a_named_section()
			{
				// Arrange
				var sections = new IniSectionCollection();

				// Act
				sections.Add(new IniSection("Section"));

				// Assert
				sections.Should().HaveCount(1);
			}

			[Fact]
			public void Should_add_unnamed_section()
			{
				// Arrange
				var sections = new IniSectionCollection();

				// Act
				sections.Add(new IniSection());

				// Assert
				sections.Should().HaveCount(1);
			}

			[Fact]
			public void Should_not_add_duplications()
			{
				// Arrange
				var sections = new IniSectionCollection();

				// Act
				sections.Add(new IniSection("Section"));
				sections.Add(new IniSection("Section"));
				sections.Add(new IniSection("SECTION"));
				sections.Add(new IniSection("section"));

				// Assert
				sections.Should().HaveCount(1);
			}
		}

		public class GetCurrentSection
		{
			[Fact]
			public void Should_return_null_when_the_collection_is_empty()
			{
				// Arrange
				var sections = new IniSectionCollection();

				// Act
				IniSection section = sections.GetCurrentSection();

				// Assert
				section.Should().BeNull();
			}

			[Fact]
			public void Should_return_the_most_recent_section()
			{
				// Arrange
				var sections = new IniSectionCollection
				{
					new IniSection("Section1"), 
					new IniSection("Section1"), 
					new IniSection("Section2")
				};

				// Act
				IniSection section = sections.GetCurrentSection();

				// Assert
				section.Should().Match<IniSection>(s => s.Name == "Section2");
			}

			[Fact]
			public void Should_return_the_most_recent_section_regardless_to_whether_it_was_added_to_the_collection_when_it_already_exists()
			{
				// Arrange
				var sections = new IniSectionCollection
				{
					new IniSection("Section1"), 
					new IniSection("Section2"), 
					new IniSection("Section1")
				};

				// Act
				IniSection section = sections.GetCurrentSection();

				// Assert
				section.Should().Match<IniSection>(s => s.Name == "Section1");
			}
		}

		public class GetSectionByName
		{
			[Fact]
			public void Should_return_null_when_the_section_was_not_found()
			{
				// Arrange
				var sections = new IniSectionCollection();

				// Act
				IniSection section = sections.GetSectionByName("Section");

				// Assert
				section.Should().BeNull();
			}

			[Fact]
			public void Should_return_the_section_by_name()
			{
				// Arrange
				var sections = new IniSectionCollection
				{
					new IniSection("Section")
				};

				// Act
				IniSection section = sections.GetSectionByName("Section");

				// Assert
				section.Should().NotBeNull();
			}

			[Fact]
			public void Should_return_unnamed_section()
			{
				// Arrange
				var sections = new IniSectionCollection
				{
					new IniSection()
				};

				// Act
				IniSection section = sections.GetSectionByName(string.Empty);

				// Assert
				section.Should().NotBeNull();
			}
		}

		public class Parse
		{
			[Fact]
			public void Should_accumulate_the_keys_under_the_same_section()
			{
				// Arrange
				const string data = @"	[SecondSection]
										Key1=Value1
										[FirstSection]
										[SecondSection]		
										Key2=Value2";

				// Act
				var sections = IniSectionCollection.Parse(data);

				// Assert
				sections.Should().Contain(s => s.Name == "SecondSection" && s.Keys.Count() == 2);
			}

			[Fact]
			public void Should_ignore_sections_that_do_not_exist()
			{
				// Arrange
				const string data = @"	[SecondSection]
										Key1=Value1
										[ThirdSection]
										Key=Value23
										[SecondSection]		
										Key2=Value2";

				// Act
				var sections = IniSectionCollection.Parse(data);

				// Assert
				sections.Should().Contain(s => s.Name == "SecondSection" && s.Keys.Count() == 2);
			}

			[Fact]
			public void Should_not_accumulate_duplications()
			{
				// Arrange
				const string data = @"	[SecondSection]
										Key=Value
										[SecondSection]		
										Key=Value";

				// Act
				var sections = IniSectionCollection.Parse(data);

				// Assert
				sections.Should().HaveCount(1).And.Contain(s => s.Keys.Count() == 1);
			}

			[Fact]
			public void Should_parse_a_single_line()
			{
				// Arrange
				const string data = @"Key1 = Value1";

				// Act
				var sections = IniSectionCollection.Parse(data).ToList();

				// Assert
				sections.Should()
				        .HaveCount(1)
				        .And.Contain(s => s.Keys.Any(k => k.Name == "Key1"));
			}

			[Fact]
			public void Should_parse_complex_data()
			{
				// Arrange
				const string data = @"Key1 = Value1
									; Some Very important comment here.
									Key2=Value2
									Key33=Value33
									Key4=Value4

									# Another important comment here. 
									[Section]
									Key1=Value1
									Key2=Value2
									Key3=Value3
									Key4=Value4";

				string[] expectedArray =
				{
					"Key1=Value1", 
					"Key2=Value2", 
					"Key33=Value33", 
					"Key4=Value4", 
					"[Section]", 
					"Key1=Value1", 
					"Key2=Value2", 
					"Key3=Value3", 
					"Key4=Value4"
				};

				// Act
				var sections = IniSectionCollection.Parse(data);

				string[] actualArray = sections.ToStringArray();

				// Assert
				actualArray.Should().ContainInOrder(expectedArray);
			}

			[Fact]
			public void Should_parse_multiple_lines()
			{
				// Arrange
				const string data = @"Key1 = Value1
									Key2=Value2
									Key3	=	Value3
									Key4  = Value4";

				string[] expectedArray =
				{
					"Key1=Value1", 
					"Key2=Value2", 
					"Key3=Value3", 
					"Key4=Value4", 
				};

				// Act
				var sections = IniSectionCollection.Parse(data);

				string[] actualArray = sections.ToStringArray();

				// Assert
				actualArray.Should().ContainInOrder(expectedArray);
			}

			[Fact]
			public void Should_parse_multiple_lines_with_section()
			{
				// Arrange
				const string data = @"[Section]
									Key1 = Value1
									Key2=Value2
									Key3	=	Value3
									Key4  = Value4";

				string[] expectedArray =
				{
					"[Section]", 
					"Key1=Value1", 
					"Key2=Value2", 
					"Key3=Value3", 
					"Key4=Value4", 
				};

				// Act
				var sections = IniSectionCollection.Parse(data);

				string[] actualArray = sections.ToStringArray();

				// Assert
				actualArray.Should().ContainInOrder(expectedArray);
			}

			[Fact]
			public void Should_parse_multiple_lines_with_section_and_keys_above()
			{
				// Arrange
				const string data = @"Key66=Value99
									[Section]
									Key1 = Value1
									Key2=Value2
									Key3	=	Value3
									Key4  = Value4";

				string[] expectedArray =
				{
					"Key66=Value99", 
					"[Section]", 
					"Key1=Value1", 
					"Key2=Value2", 
					"Key3=Value3", 
					"Key4=Value4", 
				};

				// Act
				var sections = IniSectionCollection.Parse(data);

				string[] actualArray = sections.ToStringArray();

				// Assert
				actualArray.Should().ContainInOrder(expectedArray);
			}

			[Fact]
			public void Should_update_the_key_when_the_value_is_different()
			{
				// Arrange
				const string data = @"	[SecondSection]
										Key=Value1
										[FirstSection]
										[SecondSection]		
										Key=Value2";

				// Act
				var sections = IniSectionCollection.Parse(data);

				// Assert
				sections.Should().Contain(s => s.Name == "SecondSection"
				                               && s.Keys.Any(k => k.Name == "Key" && k.Value == "Value2"));
			}
		}

		public class ToStringArray
		{
			[Fact]
			public void Should_return_an_array_of_strings()
			{
				// Arrange
				IniSectionCollection sections = new IniSectionCollection();

				IniSection unnamedSection = new IniSection();

				unnamedSection.Keys.Add(new IniKey("Key1", "Value1"));
				unnamedSection.Keys.Add(new IniKey("Key2", "Value2"));
				unnamedSection.Keys.Add(new IniKey("Key33", "Value33"));
				unnamedSection.Keys.Add(new IniKey("Key4", "Value4"));

				IniSection namedSection = new IniSection("Section");

				namedSection.Keys.Add(new IniKey("Key1", "Value1"));
				namedSection.Keys.Add(new IniKey("Key2", "Value2"));
				namedSection.Keys.Add(new IniKey("Key3", "Value3"));
				namedSection.Keys.Add(new IniKey("Key4", "Value4"));

				sections.Add(unnamedSection);
				sections.Add(namedSection);

				string[] expectedArray =
				{
					"Key1=Value1", 
					"Key2=Value2", 
					"Key33=Value33", 
					"Key4=Value4", 
					"[Section]", 
					"Key1=Value1", 
					"Key2=Value2", 
					"Key3=Value3", 
					"Key4=Value4"
				};

				// Act
				var actualArray = sections.ToStringArray();

				// Assert
				actualArray.Should().ContainInOrder(expectedArray);
			}
		}
	}
}