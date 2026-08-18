using System.ComponentModel;
using Nop.Core.ComponentModel;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Core;

/// <summary>
/// Startup task for the registration custom type converters
/// </summary>
public partial class TypeConverterRegistrationStartUpTask : IStartupTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual void Execute()
    {
		_logger.LogDebug(">>> Entering Execute()");
        //lists
        TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
        TypeDescriptor.AddAttributes(typeof(List<decimal>), new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
        TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

        //dictionaries
        TypeDescriptor.AddAttributes(typeof(Dictionary<int, int>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, int>)));

        //shipping option
        TypeDescriptor.AddAttributes(typeof(ShippingOption), new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(List<ShippingOption>), new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(IList<ShippingOption>), new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));

        //pickup point
        TypeDescriptor.AddAttributes(typeof(PickupPoint), new TypeConverterAttribute(typeof(PickupPointTypeConverter)));
		_logger.LogDebug("<<< Exiting Execute()");
    }

    /// <summary>
    /// Gets order of this startup task implementation
    /// </summary>
    public int Order => 1;
}