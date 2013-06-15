namespace Ace.Ini.Tests
{
	using FluentAssertions;

	using Xunit.Extensions;

	public class IniSectionAttributeTests
	{
		public class Section
		{
			[Theory, 
			 InlineData("SectionName="), 
			 InlineData("SectionName-")]
			public void Should_return_null_when_the_section_name_is_invalid(string sectionName)
			{
				// Arrange
				IniSectionAttribute attribute = new IniSectionAttribute(sectionName);

				// Act
				string name = attribute.SectionName;

				// Assert
				name.Should().BeNull();
			}

			[Theory, 
			 InlineData("SectionName"), 
			 InlineData("SectionName1 "), 
			 InlineData(" _SectionName"), 
			 InlineData(" SectionName ")]
			public void Should_return_the_section_name_when_is_valid(string sectionName)
			{
				// Arrange
				IniSectionAttribute attribute = new IniSectionAttribute(sectionName);

				// Act
				string name = attribute.SectionName;

				// Assert
				name.Should().NotBeBlank();
			}
		}
	}
}