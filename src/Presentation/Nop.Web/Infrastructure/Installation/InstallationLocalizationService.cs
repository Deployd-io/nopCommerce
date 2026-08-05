using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Installation;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Infrastructure.Installation;

/// <summary>
/// Localization service for installation process
/// </summary>
public partial class InstallationLocalizationService : IInstallationLocalizationService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IWebHelper _webHelper;

    protected IList<InstallationLanguage> _availableLanguages;

    #endregion

    #region Ctor

    public InstallationLocalizationService(IHttpContextAccessor httpContextAccessor,
        INopFileProvider fileProvider,
        IWebHelper webHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _fileProvider = fileProvider;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get locale resource value
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <returns>Resource value</returns>
    public string GetResource(string resourceName)
    {
        var language = GetCurrentLanguage();
        if (language == null)
            return resourceName;

        var resourceValue = language.Resources
            .Where(r => r.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase))
            .Select(r => r.Value).FirstOrDefault();

        if (string.IsNullOrEmpty(resourceValue))
            return resourceName;

        return resourceValue;
    }

    /// <summary>
    /// Get current browser culture
    /// </summary>
    /// <returns>Current culture</returns>
    public string GetBrowserCulture()
    {
		_logger.LogDebug(">>> Entering GetBrowserCulture()");
        _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var userLanguages);
		_logger.LogDebug("<<< Exiting GetBrowserCulture()");
        return userLanguages.FirstOrDefault()?.Split(',').FirstOrDefault() ?? NopCommonDefaults.DefaultLanguageCulture;
    }

    /// <summary>
    /// Get current language for the installation page
    /// </summary>
    /// <returns>Current language</returns>
    public virtual InstallationLanguage GetCurrentLanguage()
    {
		_logger.LogDebug(">>> Entering GetCurrentLanguage()");
        var httpContext = _httpContextAccessor.HttpContext;

        //try to get cookie
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.InstallationLanguageCookie}";
        httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieLanguageCode);
		_logger.LogDebug("GetCurrentLanguage(): cookieName → {CookieName}", cookieName);

        //ensure it's available (it could be delete since the previous installation)
        var availableLanguages = GetAvailableLanguages();

		_logger.LogDebug("GetCurrentLanguage(): availableLanguages → {AvailableLanguages}", availableLanguages);
        var language = availableLanguages
            .FirstOrDefault(l => l.Code.Equals(cookieLanguageCode, StringComparison.InvariantCultureIgnoreCase));
			_logger.LogDebug("GetCurrentLanguage(): language → {Language}", language);
        if (language != null)
            return language;

        //let's find by current browser culture
        if (httpContext.Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var userLanguages))
        {
            var userLanguage = userLanguages.FirstOrDefault()?.Split(',').FirstOrDefault() ?? string.Empty;
            if (!string.IsNullOrEmpty(userLanguage))
			_logger.LogDebug("GetCurrentLanguage(): userLanguage → {UserLanguage}", userLanguage);
            {
                //right. we do "StartsWith" (not "Equals") because we have shorten codes (not full culture names)
                language = availableLanguages.FirstOrDefault(l => userLanguage.StartsWith(l.Code, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        if (language != null)
            return language;

        //let's return the default one
        language = availableLanguages.FirstOrDefault(l => l.IsDefault);
        if (language != null)
            return language;

        //return any available language
        language = availableLanguages.FirstOrDefault();

		_logger.LogDebug("<<< Exiting GetCurrentLanguage()");
        return language;
    }

    /// <summary>
    /// Save a language for the installation page
    /// </summary>
    /// <param name="languageCode">Language code</param>
    public virtual void SaveCurrentLanguage(string languageCode)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddHours(24),
            HttpOnly = true,
            Secure = _webHelper.IsCurrentConnectionSecured()
        };
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.InstallationLanguageCookie}";
        httpContext.Response.Cookies.Delete(cookieName);
        httpContext.Response.Cookies.Append(cookieName, languageCode, cookieOptions);
    }

    /// <summary>
    /// Get a list of available languages
    /// </summary>
    /// <returns>Available installation languages</returns>
    public virtual IList<InstallationLanguage> GetAvailableLanguages()
    {
		_logger.LogDebug(">>> Entering GetAvailableLanguages()");
        if (_availableLanguages != null)
            return _availableLanguages;

        _availableLanguages = new List<InstallationLanguage>();
		_logger.LogDebug("GetAvailableLanguages(): _availableLanguages → {_availableLanguages}", _availableLanguages);
        foreach (var filePath in _fileProvider.EnumerateFiles(_fileProvider.MapPath($"{NopInstallationDefaults.LocalizationResourcesPath}/Installation/"), "*.xml"))
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
			_logger.LogDebug("GetAvailableLanguages(): xmlDocument → {XmlDocument}", xmlDocument);

            //get language code
            var languageCode = "";
            //we file name format: installation.{languagecode}.xml
			_logger.LogDebug("GetAvailableLanguages(): languageCode → {LanguageCode}", languageCode);
            var r = new Regex(Regex.Escape("installation.") + "(.*?)" + Regex.Escape(".xml"));
            var matches = r.Matches(_fileProvider.GetFileName(filePath));
			_logger.LogDebug("GetAvailableLanguages(): r → {R}", r);
			_logger.LogDebug("GetAvailableLanguages(): matches → {Matches}", matches);
            foreach (Match match in matches)
                languageCode = match.Groups[1].Value;

            //at now we use language codes only (not full culture names)
            languageCode = languageCode[..2];

            var languageNode = xmlDocument.SelectSingleNode(@"//Language");

			_logger.LogDebug("GetAvailableLanguages(): languageNode → {LanguageNode}", languageNode);
            if (languageNode == null || languageNode.Attributes == null)
                continue;

            //get language friendly name
            var languageName = languageNode.Attributes["Name"].InnerText.Trim();

			_logger.LogDebug("GetAvailableLanguages(): languageName → {LanguageName}", languageName);
            //is default
            var isDefaultAttribute = languageNode.Attributes["IsDefault"];
            var isDefault = isDefaultAttribute != null && Convert.ToBoolean(isDefaultAttribute.InnerText.Trim());
			_logger.LogDebug("GetAvailableLanguages(): isDefaultAttribute → {IsDefaultAttribute}", isDefaultAttribute);
			_logger.LogDebug("GetAvailableLanguages(): isDefault → {IsDefault}", isDefault);

            //is default
            var isRightToLeftAttribute = languageNode.Attributes["IsRightToLeft"];
            var isRightToLeft = isRightToLeftAttribute != null && Convert.ToBoolean(isRightToLeftAttribute.InnerText.Trim());
			_logger.LogDebug("GetAvailableLanguages(): isRightToLeftAttribute → {IsRightToLeftAttribute}", isRightToLeftAttribute);
			_logger.LogDebug("GetAvailableLanguages(): isRightToLeft → {IsRightToLeft}", isRightToLeft);

            //create language
            var language = new InstallationLanguage
            {
                Code = languageCode,
                Name = languageName,
                IsDefault = isDefault,
                IsRightToLeft = isRightToLeft,
            };

            //load resources
            var resources = xmlDocument.SelectNodes(@"//Language/LocaleResource");
            if (resources == null)
				_logger.LogDebug("GetAvailableLanguages(): resources → {Resources}", resources);
                continue;
            foreach (XmlNode resNode in resources)
            {
                if (resNode.Attributes == null)
                    continue;

                var resNameAttribute = resNode.Attributes["Name"];
                var resValueNode = resNode.SelectSingleNode("Value");
				_logger.LogDebug("GetAvailableLanguages(): resNameAttribute → {ResNameAttribute}", resNameAttribute);
				_logger.LogDebug("GetAvailableLanguages(): resValueNode → {ResValueNode}", resValueNode);

                if (resNameAttribute == null)
                    throw new NopException("All installation resources must have an attribute Name=\"Value\".");
                var resourceName = resNameAttribute.Value.Trim();
				_logger.LogDebug("GetAvailableLanguages(): Name → {Name}", Name);
				_logger.LogDebug("GetAvailableLanguages(): resourceName → {ResourceName}", resourceName);
                if (string.IsNullOrEmpty(resourceName))
                    throw new NopException("All installation resource attributes 'Name' must have a value.'");

                if (resValueNode == null)
                    throw new NopException("All installation resources must have an element \"Value\".");
                var resourceValue = resValueNode.InnerText.Trim();

				_logger.LogDebug("GetAvailableLanguages(): resourceValue → {ResourceValue}", resourceValue);
                language.Resources.Add(new InstallationLocaleResource
                {
                    Name = resourceName,
                    Value = resourceValue
                });
            }

            _availableLanguages.Add(language);
            _availableLanguages = _availableLanguages.OrderBy(l => l.Name).ToList();

        }
		_logger.LogDebug("<<< Exiting GetAvailableLanguages()");
        return _availableLanguages;
    }

    /// <summary>
    /// Get a dictionary of available data provider types
    /// </summary>
    /// <param name="valuesToExclude">Values to exclude</param>
    /// <param name="useLocalization">Localize</param>
    /// <returns>Key-value pairs of available data providers types</returns>
    public Dictionary<int, string> GetAvailableProviderTypes(int[] valuesToExclude = null, bool useLocalization = true)
    {
        return Enum.GetValues(typeof(DataProviderType))
            .Cast<DataProviderType>()
            .Where(enumValue => enumValue != DataProviderType.Unknown && (valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))))
            .ToDictionary(
                enumValue => Convert.ToInt32(enumValue),
                enumValue => useLocalization ? GetResource(enumValue.ToString()) : CommonHelper.SplitCamelCaseWord(enumValue.ToString()));
    }

    #endregion
}