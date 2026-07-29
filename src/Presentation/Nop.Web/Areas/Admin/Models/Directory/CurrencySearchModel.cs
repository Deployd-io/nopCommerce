using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency search model
/// </summary>
public partial record CurrencySearchModel : BaseSearchModel
{
    #region Ctor

    public CurrencySearchModel()
    {
		logger.LogDebug(">>> Entering CurrencySearchModel()");
        ExchangeRateProviderModel = new CurrencyExchangeRateProviderModel();
		logger.LogDebug("<<< Exiting CurrencySearchModel()");
		logger.LogDebug("CurrencySearchModel(): ExchangeRateProviderModel → {ExchangeRateProviderModel}", ExchangeRateProviderModel);
    }

    #endregion

    #region Properties

    public CurrencyExchangeRateProviderModel ExchangeRateProviderModel { get; set; }

    #endregion
}