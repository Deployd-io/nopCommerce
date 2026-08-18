using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency search model
/// </summary>
public partial record CurrencySearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CurrencySearchModel()
    {
		_logger.LogDebug(">>> Entering CurrencySearchModel()");
        ExchangeRateProviderModel = new CurrencyExchangeRateProviderModel();
		_logger.LogDebug("CurrencySearchModel(): ExchangeRateProviderModel → {ExchangeRateProviderModel}", ExchangeRateProviderModel);
		_logger.LogDebug("<<< Exiting CurrencySearchModel()");
    }

    #endregion

    #region Properties

    public CurrencyExchangeRateProviderModel ExchangeRateProviderModel { get; set; }

    #endregion
}