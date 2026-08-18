using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency exchange rate provider model
/// </summary>
public partial record CurrencyExchangeRateProviderModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CurrencyExchangeRateProviderModel()
    {
		_logger.LogDebug(">>> Entering CurrencyExchangeRateProviderModel()");
        ExchangeRates = new List<CurrencyExchangeRateModel>();
        ExchangeRateProviders = new List<SelectListItem>();
		_logger.LogDebug("CurrencyExchangeRateProviderModel(): ExchangeRateProviders → {ExchangeRateProviders}", ExchangeRateProviders);
		_logger.LogDebug("CurrencyExchangeRateProviderModel(): ExchangeRates → {ExchangeRates}", ExchangeRates);
		_logger.LogDebug("<<< Exiting CurrencyExchangeRateProviderModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyRateAutoUpdateEnabled")]
    public bool AutoUpdateEnabled { get; set; }

    public IList<CurrencyExchangeRateModel> ExchangeRates { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.ExchangeRateProvider")]
    public string ExchangeRateProvider { get; set; }
    public IList<SelectListItem> ExchangeRateProviders { get; set; }

    #endregion
}