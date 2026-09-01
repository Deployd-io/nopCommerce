using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

public partial class CountryController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICountryModelFactory _countryModelFactory;
        
    #endregion

    #region Ctor

    public CountryController(ICountryModelFactory countryModelFactory)
    {
		_logger.LogDebug(">>> Entering CountryController(countryModelFactory={CountryModelFactory})", countryModelFactory);
        _countryModelFactory = countryModelFactory;
		_logger.LogDebug("CountryController(countryModelFactory={CountryModelFactory}): _countryModelFactory → {_countryModelFactory}", countryModelFactory, _countryModelFactory);
    }

    #endregion

    #region States / provinces

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetStatesByCountryId(int countryId, bool addSelectStateItem)
    {
		_logger.LogDebug(">>> Entering GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem})", countryId, addSelectStateItem);
        var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);

		_logger.LogDebug("GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem}): model → {Model}", countryId, addSelectStateItem, model);
		_logger.LogDebug("<<< Exiting GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem})", countryId, addSelectStateItem);
        return Json(model);
    }

    #endregion
}