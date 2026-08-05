using Nop.Core.Domain.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Domain.Configuration;

/// <summary>
/// Represents a setting
/// </summary>
public partial class Setting : BaseEntity, ILocalizedEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public Setting()
    {
	_logger.LogDebug(">>> Entering Setting()");
    }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    public Setting(string name, string value, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering Setting(name={Name},value={Value},storeId={StoreId})", name, value, storeId);
        Name = name;
        Value = value;
		_logger.LogDebug("Setting(name={Name},value={Value},storeId={StoreId}): Name → {Name1}", name, value, storeId, Name);
		_logger.LogDebug("Setting(name={Name},value={Value},storeId={StoreId}): Value → {Value1}", name, value, storeId, Value);
        StoreId = storeId;
		_logger.LogDebug("<<< Exiting Setting(name={Name},value={Value},storeId={StoreId})", name, value, storeId);
		_logger.LogDebug("<<< Exiting Setting()");
    }

	_logger.LogDebug("Setting(name={Name},value={Value},storeId={StoreId}): StoreId → {StoreId1}", name, value, storeId, StoreId);
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the store for which this setting is valid. 0 is set when the setting is for all stores
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// To string
    /// </summary>
    /// <returns>Result</returns>
    public override string ToString()
    {
		_logger.LogDebug(">>> Entering ToString()");
		_logger.LogDebug("<<< Exiting ToString()");
        return Name;
    }
}