using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Rss;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common;

/// <summary>
/// Represents the HTTP client to request nopCommerce official site
/// </summary>
public partial class NopHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly HttpClient _httpClient;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public NopHttpClient(AdminAreaSettings adminAreaSettings,
        EmailAccountSettings emailAccountSettings,
        HttpClient client,
        IEmailAccountService emailAccountService,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        //configure client
		_logger.LogDebug(">>> Entering NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext})", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext);
        client.BaseAddress = new Uri("https://www.nopcommerce.com/");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

		_logger.LogDebug("<<< Exiting NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext})", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext);
        _adminAreaSettings = adminAreaSettings;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _adminAreaSettings → {_adminAreaSettings}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _adminAreaSettings);
        _emailAccountSettings = emailAccountSettings;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _emailAccountSettings → {_emailAccountSettings}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _emailAccountSettings);
        _httpClient = client;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _httpClient → {_httpClient}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _httpClient);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _emailAccountService → {_emailAccountService}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _emailAccountService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _httpContextAccessor → {_httpContextAccessor}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _httpContextAccessor);
        _languageService = languageService;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _languageService → {_languageService}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _languageService);
        _webHelper = webHelper;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("NopHttpClient(adminAreaSettings={AdminAreaSettings},emailAccountSettings={EmailAccountSettings},client={Client},emailAccountService={EmailAccountService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},webHelper={WebHelper},workContext={WorkContext}): _workContext → {_workContext}", adminAreaSettings, emailAccountSettings, client, emailAccountService, httpContextAccessor, languageService, webHelper, workContext, _workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check whether the site is available
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result determines that request is completed
    /// </returns>
    public virtual async Task PingAsync()
    {
		_logger.LogDebug(">>> Entering PingAsync()");
        await _httpClient.GetStringAsync("/");
    }

    /// <summary>
    /// Check the current store for any recommendations/warnings
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetRecommendationsAsync()
    {
		_logger.LogDebug(">>> Entering GetRecommendationsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
		_logger.LogDebug("GetRecommendationsAsync(): isLocal → {IsLocal}", isLocal);
		_logger.LogDebug("GetRecommendationsAsync(): storeUrl → {StoreUrl}", storeUrl);
        if (isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("GetRecommendationsAsync(): language → {Language}", language);
        var url = string.Format(NopCommonDefaults.NopWarningPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language).ToLowerInvariant();

		_logger.LogInformation("GetRecommendationsAsync(): external service call {StoreUrl} took {Arg1} ms", storeUrl, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetRecommendationsAsync()");
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Check the current store for license terms
    /// </summary>
    /// <param name="accepted">Whether the license terms accepted</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the license terms check result
    /// </returns>
    public virtual async Task<string> CheckLicenseTermsAsync(bool accepted = false)
    {
		_logger.LogDebug(">>> Entering CheckLicenseTermsAsync(accepted={Accepted})", accepted);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
		_logger.LogDebug("CheckLicenseTermsAsync(accepted={Accepted}): isLocal → {IsLocal}", accepted, isLocal);
		_logger.LogDebug("CheckLicenseTermsAsync(accepted={Accepted}): storeUrl → {StoreUrl}", accepted, storeUrl);
        if (isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("CheckLicenseTermsAsync(accepted={Accepted}): language → {Language}", accepted, language);
        var url = string.Format(NopCommonDefaults.NopLicenseTermsPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language,
            accepted).ToLowerInvariant();

		_logger.LogInformation("CheckLicenseTermsAsync(accepted)={Accepted}: external service call {StoreUrl} took {Arg2} ms", accepted, storeUrl, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CheckLicenseTermsAsync(accepted={Accepted})", accepted);
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Check the current store for license compliance
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the license check details
    /// </returns>
    public virtual async Task<string> GetLicenseCheckDetailsAsync()
    {
		_logger.LogDebug(">>> Entering GetLicenseCheckDetailsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
		_logger.LogDebug("GetLicenseCheckDetailsAsync(): isLocal → {IsLocal}", isLocal);
		_logger.LogDebug("GetLicenseCheckDetailsAsync(): storeUrl → {StoreUrl}", storeUrl);
        if (!_adminAreaSettings.CheckLicense || isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("GetLicenseCheckDetailsAsync(): language → {Language}", language);
        var url = string.Format(NopCommonDefaults.NopLicenseCheckPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language).ToLowerInvariant();

		_logger.LogInformation("GetLicenseCheckDetailsAsync(): external service call {StoreUrl} took {Arg1} ms", storeUrl, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetLicenseCheckDetailsAsync()");
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Get official news RSS
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains news RSS feed
    /// </returns>
    public virtual async Task<RssFeed> GetNewsRssAsync()
    {
		_logger.LogDebug(">>> Entering GetNewsRssAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("GetNewsRssAsync(): language → {Language}", language);
        var url = string.Format(NopCommonDefaults.NopNewsRssPath,
            NopVersion.FULL_VERSION,
            _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
            _adminAreaSettings.HideAdvertisementsOnAdminArea,
            _webHelper.GetStoreLocation(),
            language).ToLowerInvariant();

        //get news feed
        await using var stream = await _httpClient.GetStreamAsync(url);
		_logger.LogDebug("GetNewsRssAsync(): stream → {Stream}", stream);
		_logger.LogInformation("GetNewsRssAsync(): external service call {Url} took {Arg1} ms", url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetNewsRssAsync()");
        return await RssFeed.LoadAsync(stream);
    }

    /// <summary>
    /// Notification about the successful installation
    /// </summary>
    /// <param name="email">Admin email</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="culture">Culture name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> InstallationCompletedAsync(string email, string languageCode, string culture)
    {
		_logger.LogDebug(">>> Entering InstallationCompletedAsync(email={Email},languageCode={LanguageCode},culture={Culture})", email, languageCode, culture);
        //prepare URL to request
        var url = string.Format(NopCommonDefaults.NopInstallationCompletedPath,
            NopVersion.FULL_VERSION,
            _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
            WebUtility.UrlEncode(email),
            _webHelper.GetStoreLocation(),
            languageCode,
            culture).ToLowerInvariant();

        //this request takes some more time
        try
        {
            if (_httpClient.Timeout != TimeSpan.FromSeconds(30))
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        catch { }
		_logger.LogError("Exception in InstallationCompletedAsync(email={Email},languageCode={LanguageCode},culture={Culture}): {Arg3}", email, languageCode, culture, ex.Message);

		_logger.LogDebug("<<< Exiting InstallationCompletedAsync(email={Email},languageCode={LanguageCode},culture={Culture})", email, languageCode, culture);
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Subscribe to nopCommerce newsletters during installation
    /// </summary>
    /// <param name="email">Admin email</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<HttpResponseMessage> SubscribeNewsLettersAsync(string email)
    {
		_logger.LogDebug(">>> Entering SubscribeNewsLettersAsync(email={Email})", email);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var url = string.Format(NopCommonDefaults.NopSubscribeNewsLettersPath,
                WebUtility.UrlEncode(email))
            .ToLowerInvariant();

		_logger.LogInformation("SubscribeNewsLettersAsync(email)={Email}: external service call {Url} took {Arg2} ms", email, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting SubscribeNewsLettersAsync(email={Email})", email);
        return await _httpClient.GetAsync(url);
    }

    /// <summary>
    /// Get a response regarding available categories of marketplace extensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetExtensionsCategoriesAsync()
    {
		_logger.LogDebug(">>> Entering GetExtensionsCategoriesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("GetExtensionsCategoriesAsync(): language → {Language}", language);
        var url = string.Format(NopCommonDefaults.NopExtensionsCategoriesPath, language).ToLowerInvariant();
		_logger.LogDebug("GetExtensionsCategoriesAsync(): url → {Url}", url);

        //get XML response
		_logger.LogInformation("GetExtensionsCategoriesAsync(): external service call {Url} took {Arg1} ms", url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetExtensionsCategoriesAsync()");
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Get a response regarding available versions of marketplace extensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetExtensionsVersionsAsync()
    {
		_logger.LogDebug(">>> Entering GetExtensionsVersionsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("GetExtensionsVersionsAsync(): language → {Language}", language);
        var url = string.Format(NopCommonDefaults.NopExtensionsVersionsPath, language).ToLowerInvariant();
		_logger.LogDebug("GetExtensionsVersionsAsync(): url → {Url}", url);

        //get XML response
		_logger.LogInformation("GetExtensionsVersionsAsync(): external service call {Url} took {Arg1} ms", url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetExtensionsVersionsAsync()");
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Get a response regarding marketplace extensions
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="versionId">Version identifier</param>
    /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetExtensionsAsync(int categoryId = 0,
        int versionId = 0, int price = 0, string searchTerm = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetExtensionsAsync(categoryId={CategoryId},versionId={VersionId},price={Price},searchTerm={SearchTerm},pageIndex={PageIndex},pageSize={PageSize})", categoryId, versionId, price, searchTerm, pageIndex, pageSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
		_logger.LogDebug("GetExtensionsAsync(categoryId={CategoryId},versionId={VersionId},price={Price},searchTerm={SearchTerm},pageIndex={PageIndex},pageSize={PageSize}): language → {Language}", categoryId, versionId, price, searchTerm, pageIndex, pageSize, language);
        var url = string.Format(NopCommonDefaults.NopExtensionsPath,
            categoryId, versionId, price, WebUtility.UrlEncode(searchTerm), pageIndex, pageSize, language).ToLowerInvariant();

        //get XML response
		_logger.LogDebug("<<< Exiting GetExtensionsAsync(categoryId={CategoryId},versionId={VersionId},price={Price},searchTerm={SearchTerm},pageIndex={PageIndex},pageSize={PageSize})", categoryId, versionId, price, searchTerm, pageIndex, pageSize);
		_logger.LogInformation("GetExtensionsAsync(categoryId,versionId,price,searchTerm,pageIndex,pageSize)={CategoryId},{VersionId},{Price},{SearchTerm},{PageIndex},{PageSize}: external service call {Url} took {Arg7} ms", categoryId, versionId, price, searchTerm, pageIndex, pageSize, url, __logStart.ElapsedMilliseconds);
        return await _httpClient.GetStringAsync(url);
    }

    #endregion
}