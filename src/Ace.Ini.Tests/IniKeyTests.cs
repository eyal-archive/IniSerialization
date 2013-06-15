namespace Ace.Ini.Tests
{
	using FluentAssertions;

	using Xunit;
	using Xunit.Extensions;

	public class IniKeyTests
	{
		public new class Equals
		{
			[Fact]
			public void Should_return_false_when_the_object_to_comapre_is_null()
			{
				// Arrange
				IniKey key = new IniKey("Name", "Value");

				// Act
				bool equals = key.Equals(null);

				// Assert
				equals.Should().BeFalse();
			}

			[Fact]
			public void Should_return_false_when_the_object_to_compare_is_not_kind_of_IniKey()
			{
				// Arrange
				IniKey key = new IniKey("Name", "Value");

				const string str = "Name=Value";

				// Act
				// ReSharper disable SuspiciousTypeConversion.Global
				bool equals = key.Equals(str);
				// ReSharper restore SuspiciousTypeConversion.Global

				// Assert
				equals.Should().BeFalse();
			}

			[Fact]
			public void Should_return_true_when_passing_itself()
			{
				// Arrange
				IniKey key = new IniKey("Name", "Value");

				// Act
				bool equals = key.Equals(key);

				// Assert
				equals.Should().BeTrue();
			}

			[Fact]
			public void Should_return_true_when_two_keys_holds_the_same_name_regardless_to_the_value()
			{
				// Arrange
				IniKey key1 = new IniKey("Name", "Value1");
				IniKey key2 = new IniKey("Name", "Value2");

				// Act
				bool equals = key1.Equals(key2);

				// Assert
				equals.Should().BeTrue();
			}
		}

		public new class GetHashCode
		{
			[Fact]
			public void Should_return_different_hash_code_when_two_objects_are_not_equal()
			{
				// Arrange
				IniKey key1 = new IniKey("Name1", "Value1");
				IniKey key2 = new IniKey("Name2", "Value2");

				// Act
				int hashCode = key1.GetHashCode();

				// Assert
				hashCode.Should().NotBe(key2.GetHashCode());
			}

			[Fact]
			public void Should_return_same_hash_code_when_two_objects_are_equal()
			{
				// Arrange
				IniKey key1 = new IniKey("Name", "Value");
				IniKey key2 = new IniKey("Name", "Value");

				// Act
				int hashCode = key1.GetHashCode();

				// Assert
				hashCode.Should().Be(key2.GetHashCode());
			}
		}

		public class Name
		{
			[Fact]
			public void Should_return_the_name_of_the_key()
			{
				// Arrange
				IniKey key = new IniKey("Name", "Value");

				// Act
				string name = key.Name;

				// Assert
				name.Should().Be("Name");
			}
		}

		public class Parse
		{
			[Theory, 
			 InlineData("Name=", "Name", ""), 
			 InlineData("Name=Value", "Name", "Value"), 
			 InlineData("Name1 = Value23", "Name1", "Value23"), 
			 InlineData("	Name5 = Value55", "Name5", "Value55"), 
			 InlineData("Name1 = Value77	   ", "Name1", "Value77"), 
			 InlineData("	Name56   =    Value67   ", "Name56", "Value67")]
			public void Should_return_an_instance_of_IniKey_when_the_key_is_valid(string key, string name, string value)
			{
				// Arrange

				// Act
				IniKey actualKey = IniKey.Parse(key);

				// Assert
				actualKey.Should().Match<IniKey>(k => k.Name == name && k.Value == value);
			}

			[Theory, 
			 InlineData("=Value"), 
			 InlineData(" = ")]
			public void Should_return_null_when_the_key_is_invalid(string key)
			{
				// Arrange

				// Act
				IniKey actualKey = IniKey.Parse(key);

				// Assert
				actualKey.Should().BeNull();
			}
		}

		public new class ToString
		{
			[Fact]
			public void Should_return_the_key_as_a_name_value_pair()
			{
				// Arrange
				IniKey key = new IniKey("Name", "Value");

				// Act
				string nameValuePair = key.ToString();

				// Assert
				nameValuePair.Should().Be("Name=Value");
			}
		}

		public class Value
		{
			[Fact]
			public void Should_return_the_value_of_the_key()
			{
				// Arrange
				IniKey key = new IniKey("Name", "Value");

				// Act
				string name = key.Value;

				// Assert
				name.Should().Be("Value");
			}
		}
	}
}