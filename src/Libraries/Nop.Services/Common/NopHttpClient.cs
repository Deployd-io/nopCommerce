using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Rss;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Services.Common;

/// <summary>
/// Represents the HTTP client to request nopCommerce official site
/// </summary>
public partial class NopHttpClient
{
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
        client.BaseAddress = new Uri("https://www.nopcommerce.com/");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

        _adminAreaSettings = adminAreaSettings;
        _emailAccountSettings = emailAccountSettings;
        _httpClient = client;
        _emailAccountService = emailAccountService;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _webHelper = webHelper;
        _workContext = workContext;
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
		logger.LogDebug(">>> Entering PingAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _httpClient.GetStringAsync("/");
		logger.LogDebug("<<< Exiting PingAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("PingAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("PingAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Check the current store for any recommendations/warnings
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetRecommendationsAsync()
    {
		logger.LogDebug(">>> Entering GetRecommendationsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
		logger.LogDebug("GetRecommendationsAsync(): isLocal → {IsLocal}", isLocal);
		logger.LogDebug("GetRecommendationsAsync(): storeUrl → {StoreUrl}", storeUrl);
        if (isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopWarningPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language).ToLowerInvariant();
			logger.LogDebug("GetRecommendationsAsync(): language → {Language}", language);

		logger.LogInformation("GetRecommendationsAsync(): external service call {StoreUrl} took {Arg1} ms", storeUrl, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetRecommendationsAsync()");
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
		logger.LogDebug(">>> Entering CheckLicenseTermsAsync(accepted={Accepted})", accepted);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
		logger.LogDebug("CheckLicenseTermsAsync(accepted={Accepted}): isLocal → {IsLocal}", accepted, isLocal);
		logger.LogDebug("CheckLicenseTermsAsync(accepted={Accepted}): storeUrl → {StoreUrl}", accepted, storeUrl);
        if (isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopLicenseTermsPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language,
            accepted).ToLowerInvariant();
			logger.LogDebug("CheckLicenseTermsAsync(accepted={Accepted}): language → {Language}", accepted, language);

		logger.LogInformation("CheckLicenseTermsAsync(accepted)={Accepted}: external service call {StoreUrl} took {Arg2} ms", accepted, storeUrl, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting CheckLicenseTermsAsync(accepted={Accepted})", accepted);
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
		logger.LogDebug(">>> Entering GetLicenseCheckDetailsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
		logger.LogDebug("GetLicenseCheckDetailsAsync(): isLocal → {IsLocal}", isLocal);
		logger.LogDebug("GetLicenseCheckDetailsAsync(): storeUrl → {StoreUrl}", storeUrl);
        if (!_adminAreaSettings.CheckLicense || isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopLicenseCheckPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language).ToLowerInvariant();
			logger.LogDebug("GetLicenseCheckDetailsAsync(): language → {Language}", language);

		logger.LogInformation("GetLicenseCheckDetailsAsync(): external service call {StoreUrl} took {Arg1} ms", storeUrl, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetLicenseCheckDetailsAsync()");
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
		logger.LogDebug(">>> Entering GetNewsRssAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopNewsRssPath,
            NopVersion.FULL_VERSION,
            _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
            _adminAreaSettings.HideAdvertisementsOnAdminArea,
            _webHelper.GetStoreLocation(),
            language).ToLowerInvariant();
			logger.LogDebug("GetNewsRssAsync(): language → {Language}", language);

        //get news feed
        await using var stream = await _httpClient.GetStreamAsync(url);
		logger.LogInformation("GetNewsRssAsync(): external service call {Url} took {Arg1} ms", url, __logStart.ElapsedMilliseconds);
		logger.LogDebug("GetNewsRssAsync(): stream → {Stream}", stream);
		logger.LogDebug("<<< Exiting GetNewsRssAsync()");
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
		logger.LogDebug(">>> Entering InstallationCompletedAsync(email={Email},languageCode={LanguageCode},culture={Culture})", email, languageCode, culture);
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

		logger.LogError("Exception in InstallationCompletedAsync(email={Email},languageCode={LanguageCode},culture={Culture}): {Arg3}", email, languageCode, culture, ex.Message);
		logger.LogDebug("<<< Exiting InstallationCompletedAsync(email={Email},languageCode={LanguageCode},culture={Culture})", email, languageCode, culture);
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
		logger.LogDebug(">>> Entering SubscribeNewsLettersAsync(email={Email})", email);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var url = string.Format(NopCommonDefaults.NopSubscribeNewsLettersPath,
                WebUtility.UrlEncode(email))
            .ToLowerInvariant();

		logger.LogInformation("SubscribeNewsLettersAsync(email)={Email}: external service call {Url} took {Arg2} ms", email, url, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SubscribeNewsLettersAsync(email={Email})", email);
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
		logger.LogDebug(">>> Entering GetExtensionsCategoriesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopExtensionsCategoriesPath, language).ToLowerInvariant();
		logger.LogDebug("GetExtensionsCategoriesAsync(): language → {Language}", language);
		logger.LogDebug("GetExtensionsCategoriesAsync(): url → {Url}", url);

        //get XML response
		logger.LogInformation("GetExtensionsCategoriesAsync(): external service call {Url} took {Arg1} ms", url, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetExtensionsCategoriesAsync()");
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
		logger.LogDebug(">>> Entering GetExtensionsVersionsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopExtensionsVersionsPath, language).ToLowerInvariant();
		logger.LogDebug("GetExtensionsVersionsAsync(): language → {Language}", language);
		logger.LogDebug("GetExtensionsVersionsAsync(): url → {Url}", url);

        //get XML response
		logger.LogInformation("GetExtensionsVersionsAsync(): external service call {Url} took {Arg1} ms", url, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetExtensionsVersionsAsync()");
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
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopExtensionsPath,
            categoryId, versionId, price, WebUtility.UrlEncode(searchTerm), pageIndex, pageSize, language).ToLowerInvariant();

        //get XML response
        return await _httpClient.GetStringAsync(url);
    }

    #endregion
}