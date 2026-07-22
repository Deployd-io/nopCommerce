using System.ComponentModel;
using FluentAssertions;
using Nop.Core.Domain.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Domain.Shipping;

[TestFixture]
public class ShippingOptionTypeConverterTests
{
    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        TypeDescriptor.AddAttributes(typeof(ShippingOption),
            new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
			logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public void CanGetTypeConverter()
    {
		logger.LogDebug(">>> Entering CanGetTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(ShippingOption));
        converter.GetType().Should().Be(typeof(ShippingOptionTypeConverter));
		logger.LogDebug("<<< Exiting CanGetTypeConverter()");
		logger.LogDebug("CanGetTypeConverter(): converter → {Converter}", converter);
    }

    [Test]
    public void CanConvertShippingOptionToStringAndBack()
    {
		logger.LogDebug(">>> Entering CanConvertShippingOptionToStringAndBack()");
        var shippingOptionInput = new ShippingOption
        {
            Name = "1",
            Description = "2",
            Rate = 3.57M,
            ShippingRateComputationMethodSystemName = "4"
        };
        var converter = TypeDescriptor.GetConverter(shippingOptionInput.GetType());
        var result = converter.ConvertTo(shippingOptionInput, typeof(string)) as string;
		logger.LogDebug("CanConvertShippingOptionToStringAndBack(): converter → {Converter}", converter);
		logger.LogDebug("CanConvertShippingOptionToStringAndBack(): result → {Result}", result);

        var shippingOptionOutput = converter.ConvertFrom(result) as ShippingOption;
        shippingOptionOutput.Should().NotBeNull();
		logger.LogDebug("CanConvertShippingOptionToStringAndBack(): shippingOptionOutput → {ShippingOptionOutput}", shippingOptionOutput);
        shippingOptionOutput.Name.Should().Be("1");
        shippingOptionOutput.Description.Should().Be("2");
        shippingOptionOutput.Rate.Should().Be(3.57M);
        shippingOptionOutput.ShippingRateComputationMethodSystemName.Should().Be("4");
		logger.LogDebug("<<< Exiting CanConvertShippingOptionToStringAndBack()");
    }
}
