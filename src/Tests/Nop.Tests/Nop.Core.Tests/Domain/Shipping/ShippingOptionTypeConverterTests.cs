using System.ComponentModel;
using FluentAssertions;
using Nop.Core.Domain.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Domain.Shipping;

[TestFixture]
public class ShippingOptionTypeConverterTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        TypeDescriptor.AddAttributes(typeof(ShippingOption),
            new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
			_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public void CanGetTypeConverter()
    {
		_logger.LogDebug(">>> Entering CanGetTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(ShippingOption));
        converter.GetType().Should().Be(typeof(ShippingOptionTypeConverter));
		_logger.LogDebug("<<< Exiting CanGetTypeConverter()");
		_logger.LogDebug("CanGetTypeConverter(): converter → {Converter}", converter);
    }

    [Test]
    public void CanConvertShippingOptionToStringAndBack()
    {
		_logger.LogDebug(">>> Entering CanConvertShippingOptionToStringAndBack()");
        var shippingOptionInput = new ShippingOption
        {
            Name = "1",
            Description = "2",
            Rate = 3.57M,
            ShippingRateComputationMethodSystemName = "4"
        };
        var converter = TypeDescriptor.GetConverter(shippingOptionInput.GetType());
		_logger.LogDebug("CanConvertShippingOptionToStringAndBack(): converter → {Converter}", converter);
        var result = converter.ConvertTo(shippingOptionInput, typeof(string)) as string;
		_logger.LogDebug("CanConvertShippingOptionToStringAndBack(): result → {Result}", result);

        var shippingOptionOutput = converter.ConvertFrom(result) as ShippingOption;
		_logger.LogDebug("CanConvertShippingOptionToStringAndBack(): shippingOptionOutput → {ShippingOptionOutput}", shippingOptionOutput);
        shippingOptionOutput.Should().NotBeNull();
        shippingOptionOutput.Name.Should().Be("1");
        shippingOptionOutput.Description.Should().Be("2");
        shippingOptionOutput.Rate.Should().Be(3.57M);
        shippingOptionOutput.ShippingRateComputationMethodSystemName.Should().Be("4");
		_logger.LogDebug("<<< Exiting CanConvertShippingOptionToStringAndBack()");
    }
}