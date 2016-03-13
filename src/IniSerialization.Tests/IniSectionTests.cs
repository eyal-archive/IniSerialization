namespace IniSerialization.Ini.Tests
{
	using FluentAssertions;

	using Xunit;

	public class IniSectionTests
	{
		public class EnclosedName
		{
			[Fact]
			public void Should_return_empty_for_unnamed_section()
			{
				// Arrange
				IniSection section = new IniSection();

				// Act
				string sectionName = section.EnclosedName;

				// Assert
				sectionName.Should().BeEmpty();
			}

			[Fact]
			public void Should_return_the_enclosed_section_name()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				// Act
				string sectionName = section.EnclosedName;

				// Assert
				sectionName.Should().Be("[Section]");
			}
		}

		public new class Equals
		{
			[Fact]
			public void Should_return_false_when_the_object_to_comapre_is_null()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				// Act
				bool equals = section.Equals(null);

				// Assert
				equals.Should().BeFalse();
			}

			[Fact]
			public void Should_return_false_when_the_object_to_compare_is_not_kind_of_IniSection()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				const string str = "Name=Value";

				// Act
				// ReSharper disable SuspiciousTypeConversion.Global
				bool equals = section.Equals(str);
				// ReSharper restore SuspiciousTypeConversion.Global

				// Assert
				equals.Should().BeFalse();
			}

			[Fact]
			public void Should_return_true_when_passing_itself()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				// Act
				bool equals = section.Equals(section);

				// Assert
				equals.Should().BeTrue();
			}

			[Fact]
			public void Should_return_true_when_two_sections_holds_the_same_section_name()
			{
				// Arrange
				IniSection section1 = new IniSection("Section");
				IniSection section2 = new IniSection("Section");

				// Act
				bool equals = section1.Equals(section2);

				// Assert
				equals.Should().BeTrue();
			}
		}

		public new class GetHashCode
		{
			[Fact]
			public void Should_return_different_hash_code_when_two_sections_are_not_equal()
			{
				// Arrange
				IniSection section1 = new IniSection("Section1");
				IniSection section2 = new IniSection("Section2");

				// Act
				int hashCode = section1.GetHashCode();

				// Assert
				hashCode.Should().NotBe(section2.GetHashCode());
			}

			[Fact]
			public void Should_return_same_hash_code_when_two_sections_are_equal()
			{
				// Arrange
				IniSection section1 = new IniSection("Section");
				IniSection section2 = new IniSection("Section");

				// Act
				int hashCode = section1.GetHashCode();

				// Assert
				hashCode.Should().Be(section2.GetHashCode());
			}
		}

		public class Keys
		{
			[Fact]
			public void Should_return_an_instance_of_IniKeyCollection()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				// Act
				var keys = section.Keys;

				// Assert
				keys.Should().NotBeNull();
			}
		}

		public class Name
		{
			[Fact]
			public void Should_return_empty_for_unnamed_section()
			{
				// Arrange
				IniSection section = new IniSection();

				// Act
				string sectionName = section.Name;

				// Assert
				sectionName.Should().BeEmpty();
			}

			[Fact]
			public void Should_return_the_section_name()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				// Act
				string sectionName = section.Name;

				// Assert
				sectionName.Should().Be("Section");
			}
		}

		public class Parse
		{
			[Theory, 
			 InlineData("[Section1]"), 
			 InlineData("[Section_Name] "), 
			 InlineData(" [_Section]"), 
			 InlineData(" [Section] ")]
			public void Should_return_an_instance_of_IniSection_when_the_format_is_valid(string formattedSectionName)
			{
				// Arrange

				// Act
				IniSection section = IniSection.Parse(formattedSectionName);

				// Assert
				section.Should().NotBeNull();
			}

			[Theory, 
			 InlineData("Section]"), 
			 InlineData("Section] "), 
			 InlineData(" Section ")]
			public void Should_return_null_when_the_format_is_invalid(string formattedSectionName)
			{
				// Arrange

				// Act
				IniSection section = IniSection.Parse(formattedSectionName);

				// Assert
				section.Should().BeNull();
			}
		}

		public new class ToString
		{
			[Fact]
			public void Should_return_the_section_name()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				// Act
				string sectionName = section.ToString();

				// Assert
				sectionName.Should().Be("Section");
			}
		}

		public class ToStringArray
		{
			[Fact]
			public void Should_not_add_empty_to_the_array_when_the_section_is_unnamed()
			{
				// Arrange
				IniSection section = new IniSection();

				var keys = section.Keys;

				keys.Add(new IniKey("Name1", "Value1"));
				keys.Add(new IniKey("Name2", "Value2"));

				string[] expectedData = {
					"Name1=Value1", 
					"Name2=Value2"
				};

				// Act
				string[] actualData = section.ToStringArray();

				// Assert
				actualData.Should().BeEquivalentTo(expectedData);
			}

			[Fact]
			public void Should_return_the_section_as_array_of_strings()
			{
				// Arrange
				IniSection section = new IniSection("Section");

				var keys = section.Keys;

				keys.Add(new IniKey("Name1", "Value1"));
				keys.Add(new IniKey("Name2", "Value2"));

				string[] expectedData = {
					"[Section]", 
					"Name1=Value1", 
					"Name2=Value2"
				};

				// Act
				string[] actualData = section.ToStringArray();

				// Assert
				actualData.Should().BeEquivalentTo(expectedData);
			}
		}
	}
}