namespace IniSerialization.Ini.Tests
{
	using FluentAssertions;

	using Xunit;

	public class IniKeyAttributeTests
	{
		public class Key
		{
			[Theory, 
			 InlineData("Name="), 
			 InlineData("Name-")]
			public void Should_return_null_when_is_invalid(string name)
			{
				// Arrange
				IniKeyAttribute attribute = new IniKeyAttribute(name, "DefaultValue");

				// Act
				IniKey key = attribute.Key;

				// Assert
				key.Should().BeNull();
			}

			[Theory, 
			 InlineData("Name"), 
			 InlineData("Name1 "), 
			 InlineData(" _Name"), 
			 InlineData(" Name ")]
			public void Should_return_the_name_when_is_valid(string name)
			{
				// Arrange
				IniKeyAttribute attribute = new IniKeyAttribute(name, "DefaultValue");

				// Act
				IniKey key = attribute.Key;

				// Assert
				key.Should().NotBeNull();
			}
		}
	}
}