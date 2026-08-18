using System.ComponentModel;
using FluentAssertions;
using Nop.Core.Domain.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Domain.Shipping;

[TestFixture]
public class ShippingOptionListTypeConverterTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        TypeDescriptor.AddAttributes(typeof(List<ShippingOption>),
            new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));
			_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public void CanGetTypeConverter()
    {
		_logger.LogDebug(">>> Entering CanGetTypeConverter()");
        var converter = TypeDescriptor.GetConverter(typeof(List<ShippingOption>));
        converter.GetType().Should().Be(typeof(ShippingOptionListTypeConverter));
		_logger.LogDebug("CanGetTypeConverter(): converter → {Converter}", converter);
		_logger.LogDebug("<<< Exiting CanGetTypeConverter()");
    }

    [Test]
    public void CanConvertShippingOptionListToStringAndBack()
    {
		_logger.LogDebug(">>> Entering CanConvertShippingOptionListToStringAndBack()");
        var shippingOptionsInput = new List<ShippingOption>
		_logger.LogDebug("<<< Exiting CanConvertShippingOptionListToStringAndBack()");
        {
            new() {
                Name = "a1",
                Description = "a2",
                Rate = 3.57M,
                ShippingRateComputationMethodSystemName = "a4"
            },
            new() {
                Name = "b1",
                Description = "b2",
                Rate = 7.00M,
                ShippingRateComputationMethodSystemName = "b4"
            }
        };

        var converter = TypeDescriptor.GetConverter(shippingOptionsInput.GetType());
        var result = converter.ConvertTo(shippingOptionsInput, typeof(string)) as string;

        var shippingOptionsOutput = converter.ConvertFrom(result) as List<ShippingOption>;
        shippingOptionsOutput.Should().NotBeNull();
        shippingOptionsOutput.Count.Should().Be(2);
        shippingOptionsOutput[0].Name.Should().Be("a1");
        shippingOptionsOutput[0].Description.Should().Be("a2");
        shippingOptionsOutput[0].Rate.Should().Be(3.57M);
        shippingOptionsOutput[0].ShippingRateComputationMethodSystemName.Should().Be("a4");

        shippingOptionsOutput[1].Name.Should().Be("b1");
        shippingOptionsOutput[1].Description.Should().Be("b2");
        shippingOptionsOutput[1].Rate.Should().Be(7.00M);
        shippingOptionsOutput[1].ShippingRateComputationMethodSystemName.Should().Be("b4");
    }
}