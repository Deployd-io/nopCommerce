using System.ComponentModel;
using FluentAssertions;
using Nop.Core.ComponentModel;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.ComponentModel;

[TestFixture]
public class GenericListTypeConverter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        TypeDescriptor.AddAttributes(typeof(List<int>),
            new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
        TypeDescriptor.AddAttributes(typeof(List<string>),
            new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));
			_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public void CanGetIntListTypeConverter()
    {
		_logger.LogDebug(">>> Entering CanGetIntListTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(List<int>));
        converter.Should().BeOfType<GenericListTypeConverter<int>>();
		_logger.LogDebug("<<< Exiting CanGetIntListTypeConverter()");
		_logger.LogDebug("CanGetIntListTypeConverter(): converter → {Converter}", converter);
    }

    [Test]
    public void CanGetStringListTypeConverter()
    {
		_logger.LogDebug(">>> Entering CanGetStringListTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        converter.Should().BeOfType<GenericListTypeConverter<string>>();
		_logger.LogDebug("<<< Exiting CanGetStringListTypeConverter()");
		_logger.LogDebug("CanGetStringListTypeConverter(): converter → {Converter}", converter);
    }

    [Test]
    public void CanGetIntListFromString()
    {
		_logger.LogDebug(">>> Entering CanGetIntListFromString()");
        var items = "10,20,30,40,50";
        var converter = TypeDescriptor.GetConverter(typeof(List<int>));
		_logger.LogDebug("CanGetIntListFromString(): converter → {Converter}", converter);
        var result = converter.ConvertFrom(items) as IList<int>;
		_logger.LogDebug("CanGetIntListFromString(): result → {Result}", result);
		_logger.LogDebug("CanGetIntListFromString(): items → {Items}", items);
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
		_logger.LogDebug("<<< Exiting CanGetIntListFromString()");
    }

    [Test]
    public void CanGetStringListFromString()
    {
		_logger.LogDebug(">>> Entering CanGetStringListFromString()");
        var items = "foo, bar, day";
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
		_logger.LogDebug("CanGetStringListFromString(): converter → {Converter}", converter);
        var result = converter.ConvertFrom(items) as List<string>;
		_logger.LogDebug("CanGetStringListFromString(): result → {Result}", result);
		_logger.LogDebug("CanGetStringListFromString(): items → {Items}", items);
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanGetStringListFromString()");
    }

    [Test]
    public void CanConvertIntListToString()
    {
		_logger.LogDebug(">>> Entering CanConvertIntListToString()");
        var items = new List<int> { 10, 20, 30, 40, 50 };
        var converter = TypeDescriptor.GetConverter(items.GetType());
		_logger.LogDebug("CanConvertIntListToString(): converter → {Converter}", converter);
        var result = converter.ConvertTo(items, typeof(string)) as string;
		_logger.LogDebug("CanConvertIntListToString(): result → {Result}", result);

        result.Should().NotBeNull();
        result.Should().Be("10,20,30,40,50");
		_logger.LogDebug("<<< Exiting CanConvertIntListToString()");
    }

    [Test]
    public void CanConvertStringListToString()
    {
		_logger.LogDebug(">>> Entering CanConvertStringListToString()");
        var items = new List<string> { "foo", "bar", "day" };
        var converter = TypeDescriptor.GetConverter(items.GetType());
		_logger.LogDebug("CanConvertStringListToString(): converter → {Converter}", converter);
        var result = converter.ConvertTo(items, typeof(string)) as string;
		_logger.LogDebug("CanConvertStringListToString(): result → {Result}", result);
		_logger.LogDebug("CanConvertStringListToString(): items → {Items}", items);
        result.Should().NotBeNull();
        result.Should().Be("foo,bar,day");
		_logger.LogDebug("<<< Exiting CanConvertStringListToString()");
    }
    
    [Test]
    public void CanConvertNullToString()
    {
		_logger.LogDebug(">>> Entering CanConvertNullToString()");
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        var result = converter.ConvertTo(null, typeof(string)) as string;
		_logger.LogDebug("CanConvertNullToString(): result → {Result}", result);
		_logger.LogDebug("CanConvertNullToString(): converter → {Converter}", converter);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
		_logger.LogDebug("<<< Exiting CanConvertNullToString()");
    }
}