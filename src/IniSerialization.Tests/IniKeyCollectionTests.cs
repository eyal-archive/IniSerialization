namespace Ace.Ini.Tests
{
	using FluentAssertions;

	using Xunit;

	public class IniKeyCollectionTests
	{
		public class Add
		{
			[Fact]
			public void Should_add_a_new_key()
			{
				// Arrange
				var keys = new IniKeyCollection();

				// Act
				keys.Add(new IniKey("Name", "Value"));

				// Assert
				keys.Should().HaveCount(1);
			}

			[Fact]
			public void Should_not_contain_duplications()
			{
				// Arrange
				var keys = new IniKeyCollection();

				// Act
				keys.Add(new IniKey("Name", "Value"));
				keys.Add(new IniKey("NAME", "Value"));
				keys.Add(new IniKey("name", "Value"));
				keys.Add(new IniKey("NaMe", "Value"));

				// Assert
				keys.Should().HaveCount(1);
			}
		}

		public class GetKeyByName
		{
			[Theory, 
			 InlineData("Name"), 
			 InlineData("NAME"), 
			 InlineData("name")]
			public void Should_return_an_instance_when_the_name_is_valid(string name)
			{
				// Arrange
				var keys = new IniKeyCollection
				{
					new IniKey("Name", "Value")
				};

				// Act
				IniKey key = keys.GetKeyByName(name);

				// Assert
				key.Should().NotBeNull();
			}

			[Theory, 
			 InlineData("N="), 
			 InlineData("N"), 
			 InlineData("Na="), 
			 InlineData("Na"), 
			 InlineData("Nam="), 
			 InlineData("Nam"), 
			 InlineData("Name=")]
			public void Should_return_null_when_the_name_is_invalid(string name)
			{
				// Arrange
				var keys = new IniKeyCollection
				{
					new IniKey("Name", "Value")
				};

				// Act
				IniKey key = keys.GetKeyByName(name);

				// Assert
				key.Should().BeNull();
			}
		}

		public class Remove
		{
			[Fact]
			public void Should_remove_the_key()
			{
				// Arrange
				var keys = new IniKeyCollection();

				IniKey key = new IniKey("Name", "Value");

				// Act
				keys.Remove(key);

				// Assert
				keys.Should().HaveCount(0);
			}
		}
	}
}