using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Events;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.UI;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Controllers;

/// <summary>
/// Base controller
/// </summary>
[HttpsRequirement]
[PublishModelEvents]
[SignOutFromExternalAuthentication]
[ValidatePassword]
[SaveIpAddress]
[SaveLastActivity]
[SaveLastVisitedPage]
[ForceMultiFactorAuthentication]
public abstract partial class BaseController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Rendering

    /// <summary>
    /// Render component to string
    /// </summary>
    /// <param name="componentType">Component type</param>
    /// <param name="arguments">Arguments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected virtual async Task<string> RenderViewComponentToStringAsync(Type componentType, object arguments = null)
    {
		_logger.LogError(">>> Entering RenderViewComponentToStringAsync(componentType={ComponentType},arguments={Arguments})", componentType, arguments);
        var helper = new DefaultViewComponentHelper(
            EngineContext.Current.Resolve<IViewComponentDescriptorCollectionProvider>(),
            HtmlEncoder.Default,
            EngineContext.Current.Resolve<IViewComponentSelector>(),
            EngineContext.Current.Resolve<IViewComponentInvokerFactory>(),
            EngineContext.Current.Resolve<IViewBufferScope>());

        using var writer = new StringWriter();
		_logger.LogError("RenderViewComponentToStringAsync(componentType={ComponentType},arguments={Arguments}): writer → {Writer}", componentType, arguments, writer);
        var context = new ViewContext(ControllerContext, NullView.Instance, ViewData, TempData, writer, new HtmlHelperOptions());
		_logger.LogError("RenderViewComponentToStringAsync(componentType={ComponentType},arguments={Arguments}): context → {Context}", componentType, arguments, context);
        helper.Contextualize(context);
        var result = await helper.InvokeAsync(componentType, arguments);
		_logger.LogError("RenderViewComponentToStringAsync(componentType={ComponentType},arguments={Arguments}): result → {Result}", componentType, arguments, result);
        result.WriteTo(writer, HtmlEncoder.Default);
        await writer.FlushAsync();
		_logger.LogWarning("<<< Exiting RenderViewComponentToStringAsync(componentType={ComponentType},arguments={Arguments})", componentType, arguments);
        return writer.ToString();
    }

    /// <summary>
    /// Render partial view to string
    /// </summary>
    /// <param name="viewName">View name</param>
    /// <param name="model">Model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected virtual async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
    {
		_logger.LogError(">>> Entering RenderPartialViewToStringAsync(viewName={ViewName},model={Model})", viewName, model);
        //get Razor view engine
        var razorViewEngine = EngineContext.Current.Resolve<IRazorViewEngine>();
		_logger.LogError("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): razorViewEngine → {RazorViewEngine}", viewName, model, razorViewEngine);

        //create action context
        var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);
		_logger.LogError("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): actionContext → {ActionContext}", viewName, model, actionContext);

        //set view name as action name in case if not passed
        if (string.IsNullOrEmpty(viewName))
            viewName = ControllerContext.ActionDescriptor.ActionName;

        //partial view are not part of the controller life cycle.
        //hence, we could no use action filters to intercept the Models being returned
        //as we do in the /Nop.Web.Framework/Mvc/Filters/PublishModelEventsAttribute.cs for controllers
        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
		_logger.LogError("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): eventPublisher → {EventPublisher}", viewName, model, eventPublisher);
        await eventPublisher.ModelPreparedAsync(model);
        
        //set model
        ViewData.Model = model;
		_logger.LogDebug("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): ViewData.Model → {Arg2}", viewName, model, ViewData.Model);

        //try to get a view by the name
        var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
		_logger.LogError("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): viewResult → {ViewResult}", viewName, model, viewResult);

        if (viewResult.View == null)
        {
            //or try to get a view by the path
            viewResult = razorViewEngine.GetView(null, viewName, false);
            if (viewResult.View == null)
				_logger.LogError("Throwing ArgumentNullException from RenderPartialViewToStringAsync(viewName={ViewName},model={Model})", viewName, model);
                throw new ArgumentNullException($"{viewName} view was not found");
        }

        await using var stringWriter = new StringWriter();
		_logger.LogError("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): stringWriter → {StringWriter}", viewName, model, stringWriter);
        var viewContext = new ViewContext(actionContext, viewResult.View, ViewData, TempData, stringWriter, new HtmlHelperOptions());
		_logger.LogError("RenderPartialViewToStringAsync(viewName={ViewName},model={Model}): viewContext → {ViewContext}", viewName, model, viewContext);
        await viewResult.View.RenderAsync(viewContext);

		_logger.LogWarning("<<< Exiting RenderPartialViewToStringAsync(viewName={ViewName},model={Model})", viewName, model);
        return stringWriter.GetStringBuilder().ToString();
    }

    #endregion

    #region Notifications

    /// <summary>
    /// Error's JSON data
    /// </summary>
    /// <param name="error">Error text</param>
    /// <returns>Error's JSON data</returns>
    protected JsonResult ErrorJson(string error)
    {
		_logger.LogError(">>> Entering ErrorJson(error={Error})", error);
		_logger.LogWarning("<<< Exiting ErrorJson(error={Error})", error);
        return Json(new
        {
            error
		_logger.LogDebug("ErrorJson(errors={Errors}): error → {Error}", errors, error);
            _logger.LogWarning("<<< Exiting ErrorJson(errors={Errors})", errors);
        });
    }

    /// <summary>
    /// Error's JSON data
    /// </summary>
    /// <param name="errors">Error messages</param>
    /// <returns>Error's JSON data</returns>
    protected JsonResult ErrorJson(object errors)
    {
        return Json(new
        {
            error = errors
        });
    }
    /// <summary>
    /// Display "Edit" (manage) link (in public store)
    /// </summary>
    /// <param name="editPageUrl">Edit page URL</param>
    protected virtual void DisplayEditLink(string editPageUrl)
    {
		_logger.LogError(">>> Entering DisplayEditLink(editPageUrl={EditPageUrl})", editPageUrl);
        var nopHtmlHelper = EngineContext.Current.Resolve<INopHtmlHelper>();

		_logger.LogError("DisplayEditLink(editPageUrl={EditPageUrl}): nopHtmlHelper → {NopHtmlHelper}", editPageUrl, nopHtmlHelper);
        nopHtmlHelper.AddEditPageUrl(editPageUrl);
		_logger.LogWarning("<<< Exiting DisplayEditLink(editPageUrl={EditPageUrl})", editPageUrl);
    }

    #endregion

    #region Localization

    /// <summary>
    /// Add locales for localizable entities
    /// </summary>
    /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
    /// <param name="languageService">Language service</param>
    /// <param name="locales">Locales</param>
    protected virtual async Task AddLocalesAsync<TLocalizedModelLocal>(ILanguageService languageService,
        IList<TLocalizedModelLocal> locales) where TLocalizedModelLocal : ILocalizedLocaleModel
    {
		_logger.LogError(">>> Entering AddLocalesAsync(languageService={LanguageService},locales={Locales})", languageService, locales);
        await AddLocalesAsync(languageService, locales, null);
		_logger.LogWarning("<<< Exiting AddLocalesAsync(languageService={LanguageService},locales={Locales})", languageService, locales);
    }

    /// <summary>
    /// Add locales for localizable entities
    /// </summary>
    /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
    /// <param name="languageService">Language service</param>
    /// <param name="locales">Locales</param>
    /// <param name="configure">Configure action</param>
    protected virtual async Task AddLocalesAsync<TLocalizedModelLocal>(ILanguageService languageService,
        IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, int> configure) where TLocalizedModelLocal : ILocalizedLocaleModel
    {
        foreach (var language in await languageService.GetAllLanguagesAsync(true))
        {
            var locale = Activator.CreateInstance<TLocalizedModelLocal>();
			_logger.LogError("AddLocalesAsync(languageService={LanguageService},locales={Locales},configure={Configure}): locale → {Locale}", languageService, locales, configure, locale);
			_logger.LogDebug("AddLocalesAsync(languageService={LanguageService},locales={Locales},configure={Configure}): locale.LanguageId → {Arg3}", languageService, locales, configure, locale.LanguageId);
            locale.LanguageId = language.Id;

            configure?.Invoke(locale, locale.LanguageId);

            locales.Add(locale);
        }
    }


    #endregion

    #region Security

    /// <summary>
    /// Access denied view
    /// </summary>
    /// <returns>Access denied view</returns>
    protected virtual IActionResult AccessDeniedView()
    {
		_logger.LogError(">>> Entering AccessDeniedView()");
        var webHelper = EngineContext.Current.Resolve<IWebHelper>();

		_logger.LogError("AccessDeniedView(): webHelper → {WebHelper}", webHelper);
        //return Challenge();
		_logger.LogWarning("<<< Exiting AccessDeniedView()");
        return RedirectToAction("AccessDenied", "Security", new { pageUrl = webHelper.GetRawUrl(Request) });
    }

    /// <summary>
    /// Access denied JSON data
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the access denied JSON data
    /// </returns>
    protected virtual async Task<JsonResult> AccessDeniedJsonAsync()
    {
		_logger.LogError(">>> Entering AccessDeniedJsonAsync()");
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

		_logger.LogError("AccessDeniedJsonAsync(): localizationService → {LocalizationService}", localizationService);
		_logger.LogWarning("<<< Exiting AccessDeniedJsonAsync()");
        return ErrorJson(await localizationService.GetResourceAsync("Admin.AccessDenied.Description"));
    }

    #endregion

    #region Cards and tabs

    /// <summary>
    /// Save selected card name
    /// </summary>
    /// <param name="cardName">Card name to save</param>
    /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
    public virtual void SaveSelectedCardName(string cardName, bool persistForTheNextRequest = true)
    {
		_logger.LogError(">>> Entering SaveSelectedCardName(cardName={CardName},persistForTheNextRequest={PersistForTheNextRequest})", cardName, persistForTheNextRequest);
        //keep this method synchronized with
        //"GetSelectedCardName" method of \Nop.Web.Framework\Extensions\HtmlExtensions.cs
        ArgumentException.ThrowIfNullOrEmpty(cardName);

        const string dataKey = "nop.selected-card-name";
		_logger.LogError("SaveSelectedCardName(cardName={CardName},persistForTheNextRequest={PersistForTheNextRequest}): dataKey → {DataKey}", cardName, persistForTheNextRequest, dataKey);
        if (persistForTheNextRequest)
        {
            TempData[dataKey] = cardName;
        }
        else
        {
            ViewData[dataKey] = cardName;
        }
		_logger.LogWarning("<<< Exiting SaveSelectedCardName(cardName={CardName},persistForTheNextRequest={PersistForTheNextRequest})", cardName, persistForTheNextRequest);
    }

    /// <summary>
    /// Save selected tab name
    /// </summary>
    /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
    /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
    public virtual async Task SaveSelectedTabNameAsync(string tabName = "", bool persistForTheNextRequest = true)
    {
		_logger.LogError(">>> Entering SaveSelectedTabNameAsync(tabName={TabName},persistForTheNextRequest={PersistForTheNextRequest})", tabName, persistForTheNextRequest);
        //default root tab
        SaveSelectedTabName(tabName, await Request.GetFormValueAsync("selected-tab-name"), null, persistForTheNextRequest);
        //child tabs (usually used for localization)
        //Form is available for POST only
        if (!Request.IsPostRequest() || !Request.HasFormContentType)
            return;

        var form = await Request.ReadFormAsync();
		_logger.LogError("SaveSelectedTabNameAsync(tabName={TabName},persistForTheNextRequest={PersistForTheNextRequest}): form → {Form}", tabName, persistForTheNextRequest, form);

        foreach (var item in form)
            if (item.Key.StartsWith("selected-tab-name-", StringComparison.InvariantCultureIgnoreCase))
                SaveSelectedTabName(null, item.Value, item.Key["selected-tab-name-".Length..], persistForTheNextRequest);
				_logger.LogWarning("<<< Exiting SaveSelectedTabNameAsync(tabName={TabName},persistForTheNextRequest={PersistForTheNextRequest})", tabName, persistForTheNextRequest);
    }

    /// <summary>
    /// Save selected tab name
    /// </summary>
    /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
    /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
    /// <param name="selectedTabName">Selected tab name</param>
    /// <param name="dataKeyPrefix">A prefix for child tab to process</param>
    protected virtual void SaveSelectedTabName(string tabName, StringValues selectedTabName, string dataKeyPrefix, bool persistForTheNextRequest)
    {
		_logger.LogError(">>> Entering SaveSelectedTabName(tabName={TabName},selectedTabName={SelectedTabName},dataKeyPrefix={DataKeyPrefix},persistForTheNextRequest={PersistForTheNextRequest})", tabName, selectedTabName, dataKeyPrefix, persistForTheNextRequest);
        //keep this method synchronized with
        //"GetSelectedTabName" method of \Nop.Web.Framework\Extensions\HtmlExtensions.cs
        if (string.IsNullOrEmpty(tabName))
            tabName = selectedTabName;

        if (string.IsNullOrEmpty(tabName))
            return;

        var dataKey = "nop.selected-tab-name";
		_logger.LogError("SaveSelectedTabName(tabName={TabName},selectedTabName={SelectedTabName},dataKeyPrefix={DataKeyPrefix},persistForTheNextRequest={PersistForTheNextRequest}): dataKey → {DataKey}", tabName, selectedTabName, dataKeyPrefix, persistForTheNextRequest, dataKey);
        if (!string.IsNullOrEmpty(dataKeyPrefix))
            dataKey += $"-{dataKeyPrefix}";

        if (persistForTheNextRequest)
            TempData[dataKey] = tabName;
        else
            ViewData[dataKey] = tabName;
    }

    #endregion
}