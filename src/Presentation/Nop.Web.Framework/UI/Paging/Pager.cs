//Contributor : MVCContrib

using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.UI.Paging;

/// <summary>
/// Renders a pager component from an IPageableModel datasource.
/// </summary>
public partial class Pager : IHtmlContent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// ViewContext
    /// </summary>
    public ViewContext ViewContext { get; set; }

    #endregion

    #region Fields

    /// <summary>
    /// Page query string prameter name
    /// </summary>
    protected string _queryParam = "page";

    /// <summary>
    /// A value indicating whether to show Total summary
    /// </summary>
    protected bool _showTotalSummary = false;

    /// <summary>
    /// A value indicating whether to show pager items
    /// </summary>
    protected bool _showPagerItems = true;

    /// <summary>
    /// A value indicating whether to show the first item
    /// </summary>
    protected bool _showFirst = true;

    /// <summary>
    /// A value indicating whether to the previous item
    /// </summary>
    protected bool _showPrevious = true;

    /// <summary>
    /// A value indicating whether to show the next item
    /// </summary>
    protected bool _showNext = true;

    /// <summary>
    /// A value indicating whether to show the last item
    /// </summary>
    protected bool _showLast = true;

    /// <summary>
    /// A value indicating whether to show individual page
    /// </summary>
    protected bool _showIndividualPages = true;

    /// <summary>
    /// A value indicating whether to render empty query string parameters (without values)
    /// </summary>
    protected bool _renderEmptyParameters = true;

    /// <summary>
    /// Number of individual page items to display
    /// </summary>
    protected int _individualPagesDisplayedCount = 5;

    /// <summary>
    /// Boolean parameter names
    /// </summary>
    protected IList<string> _booleanParameterNames = new List<string>();

    /// <summary>
    /// First page css class name
    /// </summary>
    protected string _firstPageCssClass = "first-page";

    /// <summary>
    /// Previous page css class name
    /// </summary>
    protected string _previousPageCssClass = "previous-page";

    /// <summary>
    /// Current page css class name
    /// </summary>
    protected string _currentPageCssClass = "current-page";

    /// <summary>
    /// Individual page css class name
    /// </summary>
    protected string _individualPageCssClass = "individual-page";

    /// <summary>
    /// Next page css class name
    /// </summary>
    protected string _nextPageCssClass = "next-page";

    /// <summary>
    /// Last page css class name
    /// </summary>
    protected string _lastPageCssClass = "last-page";

    /// <summary>
    /// Main ul css class name
    /// </summary>
    protected string _mainUlCssClass = string.Empty;

    protected readonly IPageableModel _model;

    #endregion

    #region Ctor

    public Pager(IPageableModel model, ViewContext context)
    {
		_logger.LogError(">>> Entering Pager(model={Model},context={Context})", model, context);
        _model = model;
        ViewContext = context;
		_logger.LogDebug("Pager(model={Model},context={Context}): _model → {_model}", model, context, _model);
    _logger.LogWarning("<<< Exiting Pager(model={Model},context={Context})", model, context);
    }

    #endregion

    #region Methods

    #region Fields setters

    /// <summary>
    /// Set 
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public Pager QueryParam(string value)
    {
		_logger.LogError(">>> Entering QueryParam(value={Value})", value);
        _queryParam = value;
		_logger.LogDebug("QueryParam(value={Value}): _queryParam → {_queryParam}", value, _queryParam);
		_logger.LogWarning("<<< Exiting QueryParam(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a value indicating whether to show Total summary
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowTotalSummary(bool value)
    {
		_logger.LogError(">>> Entering ShowTotalSummary(value={Value})", value);
        _showTotalSummary = value;
		_logger.LogDebug("ShowTotalSummary(value={Value}): _showTotalSummary → {_showTotalSummary}", value, _showTotalSummary);
		_logger.LogWarning("<<< Exiting ShowTotalSummary(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a value indicating whether to show pager items
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowPagerItems(bool value)
    {
		_logger.LogError(">>> Entering ShowPagerItems(value={Value})", value);
        _showPagerItems = value;
		_logger.LogDebug("ShowPagerItems(value={Value}): _showPagerItems → {_showPagerItems}", value, _showPagerItems);
		_logger.LogWarning("<<< Exiting ShowPagerItems(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a value indicating whether to show the first item
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowFirst(bool value)
    {
		_logger.LogError(">>> Entering ShowFirst(value={Value})", value);
        _showFirst = value;
		_logger.LogDebug("ShowFirst(value={Value}): _showFirst → {_showFirst}", value, _showFirst);
		_logger.LogWarning("<<< Exiting ShowFirst(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a value indicating whether to the previous item
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowPrevious(bool value)
    {
		_logger.LogError(">>> Entering ShowPrevious(value={Value})", value);
        _showPrevious = value;
		_logger.LogDebug("ShowPrevious(value={Value}): _showPrevious → {_showPrevious}", value, _showPrevious);
		_logger.LogWarning("<<< Exiting ShowPrevious(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a  value indicating whether to show the next item
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowNext(bool value)
    {
		_logger.LogError(">>> Entering ShowNext(value={Value})", value);
        _showNext = value;
		_logger.LogDebug("ShowNext(value={Value}): _showNext → {_showNext}", value, _showNext);
		_logger.LogWarning("<<< Exiting ShowNext(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a value indicating whether to show the last item
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowLast(bool value)
    {
		_logger.LogError(">>> Entering ShowLast(value={Value})", value);
        _showLast = value;
		_logger.LogDebug("ShowLast(value={Value}): _showLast → {_showLast}", value, _showLast);
		_logger.LogWarning("<<< Exiting ShowLast(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set number of individual page items to display
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager ShowIndividualPages(bool value)
    {
		_logger.LogError(">>> Entering ShowIndividualPages(value={Value})", value);
        _showIndividualPages = value;
		_logger.LogDebug("ShowIndividualPages(value={Value}): _showIndividualPages → {_showIndividualPages}", value, _showIndividualPages);
		_logger.LogWarning("<<< Exiting ShowIndividualPages(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set a value indicating whether to render empty query string parameters (without values)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager RenderEmptyParameters(bool value)
    {
		_logger.LogError(">>> Entering RenderEmptyParameters(value={Value})", value);
        _renderEmptyParameters = value;
		_logger.LogDebug("RenderEmptyParameters(value={Value}): _renderEmptyParameters → {_renderEmptyParameters}", value, _renderEmptyParameters);
		_logger.LogWarning("<<< Exiting RenderEmptyParameters(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set number of individual page items to display
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager IndividualPagesDisplayedCount(int value)
    {
		_logger.LogError(">>> Entering IndividualPagesDisplayedCount(value={Value})", value);
        _individualPagesDisplayedCount = value;
		_logger.LogDebug("IndividualPagesDisplayedCount(value={Value}): _individualPagesDisplayedCount → {_individualPagesDisplayedCount}", value, _individualPagesDisplayedCount);
		_logger.LogWarning("<<< Exiting IndividualPagesDisplayedCount(value={Value})", value);
        return this;
    }

    /// <summary>
    /// little hack here due to ugly MVC implementation
    /// find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
    /// </summary>
    /// <param name="paramName">Parameter name</param>
    /// <returns>Pager</returns>
    public virtual Pager BooleanParameterName(string paramName)
    {
		_logger.LogError(">>> Entering BooleanParameterName(paramName={ParamName})", paramName);
        _booleanParameterNames.Add(paramName);
		_logger.LogWarning("<<< Exiting BooleanParameterName(paramName={ParamName})", paramName);
        return this;
    }

    /// <summary>
    /// Set first page pager css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager FirstPageCssClass(string value)
    {
		_logger.LogError(">>> Entering FirstPageCssClass(value={Value})", value);
        _firstPageCssClass = value;
		_logger.LogDebug("FirstPageCssClass(value={Value}): _firstPageCssClass → {_firstPageCssClass}", value, _firstPageCssClass);
		_logger.LogWarning("<<< Exiting FirstPageCssClass(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set previous page pager css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager PreviousPageCssClass(string value)
    {
		_logger.LogError(">>> Entering PreviousPageCssClass(value={Value})", value);
        _previousPageCssClass = value;
		_logger.LogDebug("PreviousPageCssClass(value={Value}): _previousPageCssClass → {_previousPageCssClass}", value, _previousPageCssClass);
		_logger.LogWarning("<<< Exiting PreviousPageCssClass(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set current page pager css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager CurrentPageCssClass(string value)
    {
		_logger.LogError(">>> Entering CurrentPageCssClass(value={Value})", value);
        _currentPageCssClass = value;
		_logger.LogDebug("CurrentPageCssClass(value={Value}): _currentPageCssClass → {_currentPageCssClass}", value, _currentPageCssClass);
		_logger.LogWarning("<<< Exiting CurrentPageCssClass(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set individual page pager css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager IndividualPageCssClass(string value)
    {
		_logger.LogError(">>> Entering IndividualPageCssClass(value={Value})", value);
        _individualPageCssClass = value;
		_logger.LogDebug("IndividualPageCssClass(value={Value}): _individualPageCssClass → {_individualPageCssClass}", value, _individualPageCssClass);
		_logger.LogWarning("<<< Exiting IndividualPageCssClass(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set next page pager css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager NextPageCssClass(string value)
    {
		_logger.LogError(">>> Entering NextPageCssClass(value={Value})", value);
        _nextPageCssClass = value;
		_logger.LogDebug("NextPageCssClass(value={Value}): _nextPageCssClass → {_nextPageCssClass}", value, _nextPageCssClass);
		_logger.LogWarning("<<< Exiting NextPageCssClass(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set last page pager css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager LastPageCssClass(string value)
    {
		_logger.LogError(">>> Entering LastPageCssClass(value={Value})", value);
        _lastPageCssClass = value;
		_logger.LogDebug("LastPageCssClass(value={Value}): _lastPageCssClass → {_lastPageCssClass}", value, _lastPageCssClass);
		_logger.LogWarning("<<< Exiting LastPageCssClass(value={Value})", value);
        return this;
    }

    /// <summary>
    /// Set main ul css class name
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Pager</returns>
    public virtual Pager MainUlCssClass(string value)
    {
		_logger.LogError(">>> Entering MainUlCssClass(value={Value})", value);
        _mainUlCssClass = value;
		_logger.LogDebug("MainUlCssClass(value={Value}): _mainUlCssClass → {_mainUlCssClass}", value, _mainUlCssClass);
		_logger.LogWarning("<<< Exiting MainUlCssClass(value={Value})", value);
        return this;
    }

    #endregion

    /// <summary>
    /// Write control
    /// </summary>
    /// <param name="writer">Writer</param>
    /// <param name="encoder">Encoder</param>
    public virtual void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
		_logger.LogError(">>> Entering WriteTo(writer={Writer},encoder={Encoder})", writer, encoder);
        writer.Write(ToString());
		_logger.LogWarning("<<< Exiting WriteTo(writer={Writer},encoder={Encoder})", writer, encoder);
    }

    /// <summary>
    /// Generate HTML control
    /// </summary>
    /// <returns>HTML control</returns>
    public override string ToString()
    {
		_logger.LogError(">>> Entering ToString()");
        _logger.LogWarning("<<< Exiting ToString()");
        return GenerateHtmlStringAsync().Result;
    }

    /// <summary>
    /// Is pager empty (only one page)?
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsEmpty()
    {
		_logger.LogError(">>> Entering IsEmpty()");
        _logger.LogWarning("<<< Exiting IsEmpty()");
        return string.IsNullOrEmpty(await GenerateHtmlStringAsync());
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Generate HTML control
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the hTML control
    /// </returns>
    protected virtual async Task<string> GenerateHtmlStringAsync()
    {
		_logger.LogError(">>> Entering GenerateHtmlStringAsync()");
        if (_model.TotalItems == 0)
            return null;

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogError("GenerateHtmlStringAsync(): localizationService → {LocalizationService}", localizationService);

        var links = new StringBuilder();
		_logger.LogError("GenerateHtmlStringAsync(): links → {Links}", links);
        if (_showTotalSummary && (_model.TotalPages > 0))
        {
            links.Append("<li class=\"total-summary\">");

            links.Append(string.Format(await localizationService.GetResourceAsync("Pager.CurrentPage"),
                _model.PageIndex + 1, _model.TotalPages, _model.TotalItems));

            links.Append("</li>");
        }

        if (_showPagerItems && (_model.TotalPages > 1))
        {
            if (_showFirst)
            {
                //first page
                if ((_model.PageIndex >= 3) && (_model.TotalPages > _individualPagesDisplayedCount))
                    links.Append(await CreatePageLinkAsync(1, await localizationService.GetResourceAsync("Pager.First"), _firstPageCssClass));
            }

            if (_showPrevious)
            {
                //previous page
                if (_model.PageIndex > 0)
                    links.Append(await CreatePageLinkAsync(_model.PageIndex, await localizationService.GetResourceAsync("Pager.Previous"), _previousPageCssClass));
            }

            if (_showIndividualPages)
            {
                //individual pages
                var firstIndividualPageIndex = GetFirstIndividualPageIndex();
				_logger.LogError("GenerateHtmlStringAsync(): firstIndividualPageIndex → {FirstIndividualPageIndex}", firstIndividualPageIndex);
                var lastIndividualPageIndex = GetLastIndividualPageIndex();
				_logger.LogError("GenerateHtmlStringAsync(): lastIndividualPageIndex → {LastIndividualPageIndex}", lastIndividualPageIndex);
                for (var i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                {
                    if (_model.PageIndex == i)
                        links.AppendFormat("<li class=\"" + _currentPageCssClass + "\"><span>{0}</span></li>", (i + 1));
                    else
                        links.Append(await CreatePageLinkAsync(i + 1, (i + 1).ToString(), _individualPageCssClass));
                }
            }

            if (_showNext)
            {
                //next page
                if ((_model.PageIndex + 1) < _model.TotalPages)
                    links.Append(await CreatePageLinkAsync(_model.PageIndex + 2, await localizationService.GetResourceAsync("Pager.Next"), _nextPageCssClass));
            }

            if (_showLast)
            {
                //last page
                if (((_model.PageIndex + 3) < _model.TotalPages) && (_model.TotalPages > _individualPagesDisplayedCount))
                    links.Append(await CreatePageLinkAsync(_model.TotalPages, await localizationService.GetResourceAsync("Pager.Last"), _lastPageCssClass));
            }
        }

        var result = links.ToString();
		_logger.LogError("GenerateHtmlStringAsync(): result → {Result}", result);
        if (!string.IsNullOrEmpty(result))
            result = string.Format("<ul{0}>", string.IsNullOrEmpty(_mainUlCssClass) ? "" : " class=\"" + _mainUlCssClass + "\"") + result + "</ul>";

		_logger.LogWarning("<<< Exiting GenerateHtmlStringAsync()");
        return result;
    }

    /// <summary>
    /// Get first individual page index
    /// </summary>
    /// <returns>Page index</returns>
    protected virtual int GetFirstIndividualPageIndex()
    {
		_logger.LogError(">>> Entering GetFirstIndividualPageIndex()");
        if ((_model.TotalPages < _individualPagesDisplayedCount) || ((_model.PageIndex - (_individualPagesDisplayedCount / 2)) < 0))
            return 0;

		_logger.LogWarning("<<< Exiting GetFirstIndividualPageIndex()");
        if ((_model.PageIndex + (_individualPagesDisplayedCount / 2)) >= _model.TotalPages) {
            return _model.TotalPages - _individualPagesDisplayedCount;
        }

        return _model.PageIndex - (_individualPagesDisplayedCount / 2);
    }

    /// <summary>
    /// Get last individual page index
    /// </summary>
    /// <returns>Page index</returns>
    protected virtual int GetLastIndividualPageIndex()
    {
		_logger.LogError(">>> Entering GetLastIndividualPageIndex()");
        var num = _individualPagesDisplayedCount / 2;
        if ((_individualPagesDisplayedCount % 2) == 0)
            num--;

        if ((_model.TotalPages < _individualPagesDisplayedCount) || ((_model.PageIndex + num) >= _model.TotalPages))
            return _model.TotalPages - 1;

		_logger.LogWarning("<<< Exiting GetLastIndividualPageIndex()");
        if ((_model.PageIndex - (_individualPagesDisplayedCount / 2)) < 0) {
            return _individualPagesDisplayedCount - 1;
        }

        return _model.PageIndex + num;
    }

    /// <summary>
    /// Create page link
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="text">Text</param>
    /// <param name="cssClass">CSS class</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the link
    /// </returns>
    protected virtual async Task<string> CreatePageLinkAsync(int pageNumber, string text, string cssClass)
    {
		_logger.LogError(">>> Entering CreatePageLinkAsync(pageNumber={PageNumber},text={Text},cssClass={CssClass})", pageNumber, text, cssClass);
        var liBuilder = new TagBuilder("li");
			_logger.LogError("CreatePageLinkAsync(pageNumber={PageNumber},text={Text},cssClass={CssClass}): liBuilder → {LiBuilder}", pageNumber, text, cssClass, liBuilder);
        if (!string.IsNullOrWhiteSpace(cssClass)) {
            liBuilder.AddCssClass(cssClass);
        }

        var aBuilder = new TagBuilder("a");
		_logger.LogError("CreatePageLinkAsync(pageNumber={PageNumber},text={Text},cssClass={CssClass}): aBuilder → {ABuilder}", pageNumber, text, cssClass, aBuilder);
        aBuilder.InnerHtml.AppendHtml(text);
        aBuilder.MergeAttribute("data-page", pageNumber.ToString());
        aBuilder.MergeAttribute("href", CreateDefaultUrl(pageNumber));

        liBuilder.InnerHtml.AppendHtml(aBuilder);
		_logger.LogWarning("<<< Exiting CreatePageLinkAsync(pageNumber={PageNumber},text={Text},cssClass={CssClass})", pageNumber, text, cssClass);
        return await liBuilder.RenderHtmlContentAsync();
    }

    /// <summary>
    /// Create default URL
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <returns>URL</returns>
    protected virtual string CreateDefaultUrl(int pageNumber)
    {
		_logger.LogError(">>> Entering CreateDefaultUrl(pageNumber={PageNumber})", pageNumber);
        var routeValues = new RouteValueDictionary();

		_logger.LogError("CreateDefaultUrl(pageNumber={PageNumber}): routeValues → {RouteValues}", pageNumber, routeValues);
        var parametersWithEmptyValues = new List<string>();
		_logger.LogError("CreateDefaultUrl(pageNumber={PageNumber}): parametersWithEmptyValues → {ParametersWithEmptyValues}", pageNumber, parametersWithEmptyValues);
        foreach (var key in ViewContext.HttpContext.Request.Query.Keys.Where(key => key != null))
        {
            var value = ViewContext.HttpContext.Request.Query[key].ToString();
			_logger.LogError("CreateDefaultUrl(pageNumber={PageNumber}): value → {Value}", pageNumber, value);
            if (_renderEmptyParameters && string.IsNullOrEmpty(value))
            {
                //we store query string parameters with empty values separately
                //we need to do it because they are not properly processed in the UrlHelper.GenerateUrl method (dropped for some reasons)
                parametersWithEmptyValues.Add(key);
            }
            else
            {
                if (_booleanParameterNames.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                {
                    //little hack here due to ugly MVC implementation
                    //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                    if (!string.IsNullOrEmpty(value) && value.Equals("true,false", StringComparison.InvariantCultureIgnoreCase))
                        value = "true";
                }

                routeValues[key] = value;
            }
        }

        if (pageNumber > 1)
            routeValues[_queryParam] = pageNumber;
        else
        {
            //SEO. we do not render pageindex query string parameter for the first page
            if (routeValues.ContainsKey(_queryParam))
                routeValues.Remove(_queryParam);
        }

        var webHelper = EngineContext.Current.Resolve<IWebHelper>();
		_logger.LogError("CreateDefaultUrl(pageNumber={PageNumber}): webHelper → {WebHelper}", pageNumber, webHelper);
        var url = webHelper.GetThisPageUrl(false);
		_logger.LogError("CreateDefaultUrl(pageNumber={PageNumber}): url → {Url}", pageNumber, url);
        foreach (var routeValue in routeValues)
            url = webHelper.ModifyQueryString(url, routeValue.Key, routeValue.Value?.ToString());

        if (_renderEmptyParameters && parametersWithEmptyValues.Any())
            foreach (var key in parametersWithEmptyValues)
                url = webHelper.ModifyQueryString(url, key);

		_logger.LogWarning("<<< Exiting CreateDefaultUrl(pageNumber={PageNumber})", pageNumber);
        return url;
    }

    #endregion
}