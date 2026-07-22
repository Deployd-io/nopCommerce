using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

public partial class CountryController : BasePublicController
{
    #region Fields

    protected readonly ICountryModelFactory _countryModelFactory;
        
    #endregion

    #region Ctor

    public CountryController(ICountryModelFactory countryModelFactory)
    {
		logger.LogDebug(">>> Entering CountryController(countryModelFactory={CountryModelFactory})", countryModelFactory);
        _countryModelFactory = countryModelFactory;
		logger.LogDebug("<<< Exiting CountryController(countryModelFactory={CountryModelFactory})", countryModelFactory);
		logger.LogDebug("CountryController(countryModelFactory={CountryModelFactory}): _countryModelFactory → {_countryModelFactory}", countryModelFactory, _countryModelFactory);
    }

    #endregion

    #region States / provinces

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetStatesByCountryId(int countryId, bool addSelectStateItem)
    {
		logger.LogDebug(">>> Entering GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem})", countryId, addSelectStateItem);
        var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);

		logger.LogDebug("GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem}): model → {Model}", countryId, addSelectStateItem, model);
		logger.LogDebug("<<< Exiting GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem})", countryId, addSelectStateItem);
        return Json(model);
    }

    #endregion
}
