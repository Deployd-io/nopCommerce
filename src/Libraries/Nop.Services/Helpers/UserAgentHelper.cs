using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Helpers;

/// <summary>
/// User agent helper
/// </summary>
public partial class UserAgentHelper : IUserAgentHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static readonly object _locker = new();
    protected static readonly Regex _firstMobileDeviceRegex;
    protected static readonly Regex _secondMobileDeviceRegex;

    protected readonly AppSettings _appSettings;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    static UserAgentHelper()
    {
		_logger.LogError(">>> Entering UserAgentHelper()");
        _firstMobileDeviceRegex = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        _secondMobileDeviceRegex = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		_logger.LogError("UserAgentHelper(): _secondMobileDeviceRegex → {_secondMobileDeviceRegex}", _secondMobileDeviceRegex);
		_logger.LogDebug("UserAgentHelper(): _firstMobileDeviceRegex → {_firstMobileDeviceRegex}", _firstMobileDeviceRegex);
		_logger.LogError("UserAgentHelper(): _firstMobileDeviceRegex → {_firstMobileDeviceRegex}", _firstMobileDeviceRegex);
    }

    public UserAgentHelper(AppSettings appSettings,
        IHttpContextAccessor httpContextAccessor,
        INopFileProvider fileProvider)
    {
		_logger.LogError(">>> Entering UserAgentHelper(appSettings={AppSettings},httpContextAccessor={HttpContextAccessor},fileProvider={FileProvider})", appSettings, httpContextAccessor, fileProvider);
        _appSettings = appSettings;
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("UserAgentHelper(appSettings={AppSettings},httpContextAccessor={HttpContextAccessor},fileProvider={FileProvider}): _httpContextAccessor → {_httpContextAccessor}", appSettings, httpContextAccessor, fileProvider, _httpContextAccessor);
    _logger.LogDebug("UserAgentHelper(appSettings={AppSettings},httpContextAccessor={HttpContextAccessor},fileProvider={FileProvider}): _fileProvider → {_fileProvider}", appSettings, httpContextAccessor, fileProvider, _fileProvider);
    _logger.LogWarning("<<< Exiting UserAgentHelper(appSettings={AppSettings},httpContextAccessor={HttpContextAccessor},fileProvider={FileProvider})", appSettings, httpContextAccessor, fileProvider);
        _fileProvider = fileProvider;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get BrowscapXmlHelper
    /// </summary>
    /// <returns>BrowscapXmlHelper</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual BrowscapXmlHelper GetBrowscapXmlHelper()
    {
		_logger.LogError(">>> Entering GetBrowscapXmlHelper()");
        if (Singleton<BrowscapXmlHelper>.Instance != null)
            return Singleton<BrowscapXmlHelper>.Instance;

        //no database created
        if (string.IsNullOrEmpty(_appSettings.Get<CommonConfig>().UserAgentStringsPath))
            return null;

        //prevent multi loading data
        lock (_locker)
        {
            //data can be loaded while we waited
            if (Singleton<BrowscapXmlHelper>.Instance != null)
                return Singleton<BrowscapXmlHelper>.Instance;

            var userAgentStringsPath = _fileProvider.MapPath(_appSettings.Get<CommonConfig>().UserAgentStringsPath);
			_logger.LogError("GetBrowscapXmlHelper(): userAgentStringsPath → {UserAgentStringsPath}", userAgentStringsPath);
            var crawlerOnlyUserAgentStringsPath = !string.IsNullOrEmpty(_appSettings.Get<CommonConfig>().CrawlerOnlyUserAgentStringsPath)
                ? _fileProvider.MapPath(_appSettings.Get<CommonConfig>().CrawlerOnlyUserAgentStringsPath)
                : string.Empty;
            var additionalCrawlersFilePath = _fileProvider.MapPath(_appSettings.Get<CommonConfig>().CrawlerOnlyAdditionalUserAgentStringsPath);
			_logger.LogError("GetBrowscapXmlHelper(): additionalCrawlersFilePath → {AdditionalCrawlersFilePath}", additionalCrawlersFilePath);

            var browscapXmlHelper = new BrowscapXmlHelper(userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, _fileProvider);
			_logger.LogError("GetBrowscapXmlHelper(): browscapXmlHelper → {BrowscapXmlHelper}", browscapXmlHelper);
            Singleton<BrowscapXmlHelper>.Instance = browscapXmlHelper;

			_logger.LogWarning("<<< Exiting GetBrowscapXmlHelper()");
            return Singleton<BrowscapXmlHelper>.Instance;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get a value indicating whether the request is made by search engine (web crawler)
    /// </summary>
    /// <returns>Result</returns>
    public virtual bool IsSearchEngine()
    {
		_logger.LogError(">>> Entering IsSearchEngine()");
        if (_httpContextAccessor?.HttpContext == null)
            return false;

        //we put required logic in try-catch block
        //more info: https://www.nopcommerce.com/boards/topic/17711/unhandled-exception-request-is-not-available-in-this-context
        try
        {
            var browscapXmlHelper = GetBrowscapXmlHelper();
			_logger.LogError("IsSearchEngine(): browscapXmlHelper → {BrowscapXmlHelper}", browscapXmlHelper);

            //we cannot load parser
            if (browscapXmlHelper == null)
                return false;

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent];
            return !string.IsNullOrWhiteSpace(userAgent) && browscapXmlHelper.IsCrawler(userAgent);
        }
        catch
        {
			_logger.LogError("Exception in IsSearchEngine(): {Arg0}", ex.Message);
            // ignored
        }

		_logger.LogWarning("<<< Exiting IsSearchEngine()");
        return false;
    }

    /// <summary>
    /// Get a value indicating whether the request is made by mobile device
    /// </summary>
    /// <returns></returns>
    public virtual bool IsMobileDevice()
    {
		_logger.LogError(">>> Entering IsMobileDevice()");
        if (_httpContextAccessor?.HttpContext == null)
            return false;

        //we put required logic in try-catch block
        //more info: https://www.nopcommerce.com/boards/topic/17711/unhandled-exception-request-is-not-available-in-this-context
        try
        {
            //we don't parse browscap library here
            //in 99% of cases it's enough to use the approach suggested by http://detectmobilebrowsers.com/

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
			_logger.LogError("IsMobileDevice(): userAgent → {UserAgent}", userAgent);
            var mobile = _firstMobileDeviceRegex.IsMatch(userAgent) || _secondMobileDeviceRegex.IsMatch(userAgent[0..4]);
			_logger.LogError("IsMobileDevice(): mobile → {Mobile}", mobile);
            return mobile;
        }
        catch
        {
			_logger.LogError("Exception in IsMobileDevice(): {Arg0}", ex.Message);
            // ignored
        }

		_logger.LogWarning("<<< Exiting IsMobileDevice()");
        return false;
    }

    #endregion
}