using System.Collections.Specialized;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework;

/// <summary>
/// Represents a RemotePost helper class
/// </summary>
public partial class RemotePost
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IWebHelper _webHelper;

    /// <summary>
    /// Gets or sets a remote URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets a method
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets a form name
    /// </summary>
    public string FormName { get; set; }

    /// <summary>
    /// Gets or sets a form character-sets the server can handle for form-data.
    /// </summary>
    public string AcceptCharset { get; set; }

    /// <summary>
    /// A value indicating whether we should create a new "input" HTML element for each value (in case if there are more than one) for the same "name" attributes.
    /// </summary>
    public bool NewInputForEachValue { get; set; }

    /// <summary>
    /// Parames
    /// </summary>
    public NameValueCollection Params { get; }

    /// <summary>
    /// Creates a new instance of the RemotePost class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP Context accessor</param>
    /// <param name="webHelper">Web helper</param>
    public RemotePost(IHttpContextAccessor httpContextAccessor, IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper})", httpContextAccessor, webHelper);
        Params = new NameValueCollection();
        Url = "http://www.someurl.com";
		_logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Url → {Url}", httpContextAccessor, webHelper, Url);
        Method = "post";
        FormName = "formName";

		_logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Params → {Params}", httpContextAccessor, webHelper, Params);
		_logger.LogInformation("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Url → {Url}", httpContextAccessor, webHelper, Url);
		_logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Method → {Method}", httpContextAccessor, webHelper, Method);
		_logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): FormName → {FormName}", httpContextAccessor, webHelper, FormName);
        _httpContextAccessor = httpContextAccessor;
        _webHelper = webHelper;
		_logger.LogDebug("<<< Exiting RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper})", httpContextAccessor, webHelper);
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary (to be posted).
    /// </summary>
    /// <param name="name">The key of the element to add</param>
    /// <param name="value">The value of the element to add.</param>
    public virtual void Add(string name, string value)
    {
		_logger.LogDebug(">>> Entering Add(name={Name},value={Value})", name, value);
        Params.Add(name, value);
		_logger.LogDebug("<<< Exiting Add(name={Name},value={Value})", name, value);
    }

    /// <summary>
    /// Post
    /// </summary>
    public virtual void Post()
    {
		_logger.LogDebug(">>> Entering Post()");
        //text
        var sb = new StringBuilder();
		_logger.LogDebug("Post(): sb → {Sb}", sb);
        sb.Append("<html><head>");
        sb.Append($"</head><body onload=\"document.{FormName}.submit()\">");
        if (!string.IsNullOrEmpty(AcceptCharset))
        {
            //AcceptCharset specified
            sb.Append(
                $"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" accept-charset=\"{AcceptCharset}\">");
        }
        else
        {
            //no AcceptCharset specified
            sb.Append($"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" >");
        }
        if (NewInputForEachValue)
        {
            foreach (string key in Params.Keys)
            {
                var values = Params.GetValues(key);
				_logger.LogDebug("Post(): values → {Values}", values);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        sb.Append(
                            $"<input name=\"{WebUtility.HtmlEncode(key)}\" type=\"hidden\" value=\"{WebUtility.HtmlEncode(value)}\">");
                    }
                }
            }
        }
        else
        {
            for (var i = 0; i < Params.Keys.Count; i++)
                sb.Append(
                    $"<input name=\"{WebUtility.HtmlEncode(Params.Keys[i])}\" type=\"hidden\" value=\"{WebUtility.HtmlEncode(Params[Params.Keys[i]])}\">");
        }
        sb.Append("</form>");
        sb.Append("</body></html>");

        var data = Encoding.UTF8.GetBytes(sb.ToString());
		_logger.LogDebug("Post(): data → {Data}", data);

        //modify the response
        var httpContext = _httpContextAccessor.HttpContext;
        var response = httpContext.Response;
		_logger.LogDebug("Post(): response → {Response}", response);

        //post
        response.Clear();
        response.ContentType = "text/html; charset=utf-8";
		_logger.LogDebug("Post(): response.ContentType → {Arg0}", response.ContentType);
		_logger.LogDebug("Post(): response.ContentLength → {Arg0}", response.ContentLength);
        response.ContentLength = data.Length;

        response.Body
            .WriteAsync(data, 0, data.Length)
            .Wait();

        //store a value indicating whether POST has been done
        _webHelper.IsPostBeingDone = true;
		_logger.LogDebug("Post(): _webHelper.IsPostBeingDone → {Arg0}", _webHelper.IsPostBeingDone);
		_logger.LogDebug("<<< Exiting Post()");
    }
}