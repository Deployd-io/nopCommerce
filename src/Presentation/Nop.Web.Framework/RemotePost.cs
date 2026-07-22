using System.Collections.Specialized;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Core;

namespace Nop.Web.Framework;

/// <summary>
/// Represents a RemotePost helper class
/// </summary>
public partial class RemotePost
{
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
		logger.LogDebug(">>> Entering RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper})", httpContextAccessor, webHelper);
        Params = new NameValueCollection();
        Url = "http://www.someurl.com";
		logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Params → {Params}", httpContextAccessor, webHelper, Params);
		logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Url → {Url}", httpContextAccessor, webHelper, Url);
		logger.LogInformation("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Url → {Url}", httpContextAccessor, webHelper, Url);
        Method = "post";
        FormName = "formName";
		logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): Method → {Method}", httpContextAccessor, webHelper, Method);
		logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): FormName → {FormName}", httpContextAccessor, webHelper, FormName);

        _httpContextAccessor = httpContextAccessor;
        _webHelper = webHelper;
		logger.LogDebug("<<< Exiting RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper})", httpContextAccessor, webHelper);
		logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): _httpContextAccessor → {_httpContextAccessor}", httpContextAccessor, webHelper, _httpContextAccessor);
		logger.LogDebug("RemotePost(httpContextAccessor={HttpContextAccessor},webHelper={WebHelper}): _webHelper → {_webHelper}", httpContextAccessor, webHelper, _webHelper);
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary (to be posted).
    /// </summary>
    /// <param name="name">The key of the element to add</param>
    /// <param name="value">The value of the element to add.</param>
    public virtual void Add(string name, string value)
    {
		logger.LogDebug(">>> Entering Add(name={Name},value={Value})", name, value);
        Params.Add(name, value);
		logger.LogDebug("<<< Exiting Add(name={Name},value={Value})", name, value);
    }

    /// <summary>
    /// Post
    /// </summary>
    public virtual void Post()
    {
		logger.LogDebug(">>> Entering Post()");
        //text
        var sb = new StringBuilder();
        sb.Append("<html><head>");
		logger.LogDebug("Post(): sb → {Sb}", sb);
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
                if (values != null)
				logger.LogDebug("Post(): values → {Values}", values);
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

		logger.LogDebug("Post(): data → {Data}", data);
        //modify the response
        var httpContext = _httpContextAccessor.HttpContext;
        var response = httpContext.Response;

		logger.LogDebug("Post(): response → {Response}", response);
        //post
        response.Clear();
        response.ContentType = "text/html; charset=utf-8";
		logger.LogDebug("Post(): response.ContentType → {Arg0}", response.ContentType);
		logger.LogDebug("Post(): response.ContentLength → {Arg0}", response.ContentLength);
        response.ContentLength = data.Length;

        response.Body
            .WriteAsync(data, 0, data.Length)
            .Wait();

        //store a value indicating whether POST has been done
        _webHelper.IsPostBeingDone = true;
		logger.LogDebug("<<< Exiting Post()");
		logger.LogDebug("Post(): _webHelper.IsPostBeingDone → {Arg0}", _webHelper.IsPostBeingDone);
    }
}
