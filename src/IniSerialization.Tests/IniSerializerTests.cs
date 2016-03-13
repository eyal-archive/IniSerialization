namespace IniSerialization.Ini.Tests
{
	using System;

	using IniSerialization.Ini.Tests.Dummy;

	using FluentAssertions;

	using Xunit;

	public class IniSerializerTests
	{
		public class Deserialize
		{
			[Fact]
			public void Should_handle_attributes_decoration()
			{
				// Arrange
				IniSerializer serializer = new IniSerializer();

				const string data = @"	Key=Value1
										[FirstSection]
										Key=Value2
										[SecondSection]		
										Key=Value3
										[Section]
										FirstKey=Value1
										SecondKey=Value2";

				// Act
				DecoratedDocument document = serializer.Deserilize<DecoratedDocument>(data);

				// Assert
				document.Should().Match<DecoratedDocument>(d => d.DecoratedKey == "Value1"
				                                                && d.FirstSection.Key == "Value2"
				                                                && d.SecondSection.Key == "Value3"
				                                                && d.DecoratedSection.FirstKey == "Value1"
				                                                && d.DecoratedSection.SecondKey == "Value2");
			}

			[Fact]
			public void Should_handle_default_values()
			{
				// Arrange
				IniSerializer serializer = new IniSerializer();

				const string data = @"Key=";

				// Act
				DecoratedDocument document = serializer.Deserilize<DecoratedDocument>(data);

				// Assert
				document.Should().Match<DecoratedDocument>(d => d.DecoratedKey == "Value1");
			}

			[Fact]
			public void Should_set_all_keys_with_the_values()
			{
				// Arrange
				IniSerializer serializer = new IniSerializer();

				const string data = @"	Key=Value1
										[FirstSection]
										Key=Value2
										[SecondSection]		
										Key=Value3
										[Section]
										FirstKey=Value1
										SecondKey=Value2";

				// Act
				Document document = serializer.Deserilize<Document>(data);

				// Assert
				document.Should().Match<Document>(d => d.Key == "Value1"
				                                       && d.FirstSection.Key == "Value2"
				                                       && d.SecondSection.Key == "Value3"
				                                       && d.Section.FirstKey == "Value1"
				                                       && d.Section.SecondKey == "Value2");
			}

			[Fact]
			public void Should_throw_InvalidOperationException_when_a_type_representing_a_section_contains_properties_that_their_return_type_is_not_a_string()
			{
				// Arrange
				IniSerializer serializer = new IniSerializer();

				const string data = @"	Key=Value1
										[FirstSection]
										Key=Value2
										[SecondSection]		
										Key=Value3";

				// Act
				// ReSharper disable ReturnValueOfPureMethodIsNotUsed
				Action act = () => serializer.Deserilize<ComplexDocument>(data);
				// ReSharper restore ReturnValueOfPureMethodIsNotUsed

				// Assert
				act.ShouldThrow<InvalidOperationException>();
			}
		}
	}
}