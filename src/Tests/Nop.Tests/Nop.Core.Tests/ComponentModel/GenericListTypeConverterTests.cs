using System.ComponentModel;
using FluentAssertions;
using Nop.Core.ComponentModel;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.ComponentModel;

[TestFixture]
public class GenericListTypeConverter
{
    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        TypeDescriptor.AddAttributes(typeof(List<int>),
            new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
        TypeDescriptor.AddAttributes(typeof(List<string>),
            new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));
			logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public void CanGetIntListTypeConverter()
    {
		logger.LogDebug(">>> Entering CanGetIntListTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(List<int>));
        converter.Should().BeOfType<GenericListTypeConverter<int>>();
		logger.LogDebug("<<< Exiting CanGetIntListTypeConverter()");
		logger.LogDebug("CanGetIntListTypeConverter(): converter → {Converter}", converter);
    }

    [Test]
    public void CanGetStringListTypeConverter()
    {
		logger.LogDebug(">>> Entering CanGetStringListTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        converter.Should().BeOfType<GenericListTypeConverter<string>>();
		logger.LogDebug("<<< Exiting CanGetStringListTypeConverter()");
		logger.LogDebug("CanGetStringListTypeConverter(): converter → {Converter}", converter);
    }

    [Test]
    public void CanGetIntListFromString()
    {
		logger.LogDebug(">>> Entering CanGetIntListFromString()");
        var items = "10,20,30,40,50";
        var converter = TypeDescriptor.GetConverter(typeof(List<int>));
		logger.LogDebug("CanGetIntListFromString(): items → {Items}", items);
		logger.LogDebug("CanGetIntListFromString(): converter → {Converter}", converter);
        var result = converter.ConvertFrom(items) as IList<int>;
        result.Should().NotBeNull();
		logger.LogDebug("CanGetIntListFromString(): result → {Result}", result);
        result.Count.Should().Be(5);
		logger.LogDebug("<<< Exiting CanGetIntListFromString()");
    }

    [Test]
    public void CanGetStringListFromString()
    {
		logger.LogDebug(">>> Entering CanGetStringListFromString()");
        var items = "foo, bar, day";
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
		logger.LogDebug("CanGetStringListFromString(): items → {Items}", items);
		logger.LogDebug("CanGetStringListFromString(): converter → {Converter}", converter);
        var result = converter.ConvertFrom(items) as List<string>;
        result.Should().NotBeNull();
		logger.LogDebug("CanGetStringListFromString(): result → {Result}", result);
        result.Count.Should().Be(3);
		logger.LogDebug("<<< Exiting CanGetStringListFromString()");
    }

    [Test]
    public void CanConvertIntListToString()
    {
		logger.LogDebug(">>> Entering CanConvertIntListToString()");
        var items = new List<int> { 10, 20, 30, 40, 50 };
        var converter = TypeDescriptor.GetConverter(items.GetType());
        var result = converter.ConvertTo(items, typeof(string)) as string;
		logger.LogDebug("CanConvertIntListToString(): converter → {Converter}", converter);
		logger.LogDebug("CanConvertIntListToString(): result → {Result}", result);

        result.Should().NotBeNull();
        result.Should().Be("10,20,30,40,50");
		logger.LogDebug("<<< Exiting CanConvertIntListToString()");
    }

    [Test]
    public void CanConvertStringListToString()
    {
		logger.LogDebug(">>> Entering CanConvertStringListToString()");
        var items = new List<string> { "foo", "bar", "day" };
        var converter = TypeDescriptor.GetConverter(items.GetType());
		logger.LogDebug("CanConvertStringListToString(): items → {Items}", items);
		logger.LogDebug("CanConvertStringListToString(): converter → {Converter}", converter);
        var result = converter.ConvertTo(items, typeof(string)) as string;
        result.Should().NotBeNull();
		logger.LogDebug("CanConvertStringListToString(): result → {Result}", result);
        result.Should().Be("foo,bar,day");
		logger.LogDebug("<<< Exiting CanConvertStringListToString()");
    }
    
    [Test]
    public void CanConvertNullToString()
    {
		logger.LogDebug(">>> Entering CanConvertNullToString()");
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        var result = converter.ConvertTo(null, typeof(string)) as string;
		logger.LogDebug("CanConvertNullToString(): converter → {Converter}", converter);
		logger.LogDebug("CanConvertNullToString(): result → {Result}", result);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
		logger.LogDebug("<<< Exiting CanConvertNullToString()");
    }
}
