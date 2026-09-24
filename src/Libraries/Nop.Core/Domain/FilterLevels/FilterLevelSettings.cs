using Microsoft.Extensions.Logging;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.FilterLevels;

/// <summary>
/// Filter level settings
/// </summary>
public partial class FilterLevelSettings : ISettings
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public FilterLevelSettings()
    {
		_logger.LogError(">>> Entering FilterLevelSettings()");
        FilterLevelEnumDisabled = new List<int>();
		_logger.LogDebug("FilterLevelSettings(): FilterLevelEnumDisabled → {FilterLevelEnumDisabled}", FilterLevelEnumDisabled);
    _logger.LogError("FilterLevelSettings(): FilterLevelEnumDisabled → {FilterLevelEnumDisabled}", FilterLevelEnumDisabled);
    _logger.LogWarning("<<< Exiting FilterLevelSettings()");
    }

    /// <summary>
    /// Gets or sets a value indicating whether the filter levels is enabled
    /// </summary>
    public bool FilterLevelEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the filter levels should be displayed on the home page
    /// </summary>
    public bool DisplayOnHomePage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the filter levels should be displayed on the product details page
    /// </summary>
    public bool DisplayOnProductDetailsPage { get; set; }

    /// <summary>
    /// Gets or sets a list of disabled values of FilterLevelEnum
    /// </summary>
    public List<int> FilterLevelEnumDisabled { get; set; }
}
