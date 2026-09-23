using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record CurrencySelectorModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CurrencySelectorModel()
    {
		_logger.LogError(">>> Entering CurrencySelectorModel()");
        AvailableCurrencies = new List<CurrencyModel>();
		_logger.LogDebug("CurrencySelectorModel(): AvailableCurrencies → {AvailableCurrencies}", AvailableCurrencies);
    _logger.LogError("CurrencySelectorModel(): AvailableCurrencies → {AvailableCurrencies}", AvailableCurrencies);
    _logger.LogWarning("<<< Exiting CurrencySelectorModel()");
    }

    public IList<CurrencyModel> AvailableCurrencies { get; set; }

    public int CurrentCurrencyId { get; set; }

    public bool DisplayCurrencySymbol { get; set; }
}