using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Themes;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Extensions;

public static class HtmlExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ger JQuery Datepicker date format from the .net current culture
    /// </summary>
    /// <param name="html">HtmlHelper object.</param>
    /// <returns>Format string that supported in JQuery Datepicker.</returns>
    public static string GetJQueryDateFormat(this IHtmlHelper html)
    {
		_logger.LogDebug(">>> Entering GetJQueryDateFormat(html={Html})", html);
        /*
            *  Date used in this comment : 5th - Nov - 2009 (Thursday)
            *
            *  .NET    JQueryUI        Output      Comment
            *  --------------------------------------------------------------
            *  d       d               5           day of month(No leading zero)
            *  dd      dd              05          day of month(two digit)
            *  ddd     D               Thu         day short name
            *  dddd    DD              Thursday    day long name
            *  M       m               11          month of year(No leading zero)
            *  MM      mm              11          month of year(two digit)
            *  MMM     M               Nov         month name short
            *  MMMM    MM              November    month name long.
            *  yy      y               09          Year(two digit)
            *  yyyy    yy              2009        Year(four digit)             *
            */

        var currentFormat = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ?
            CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern :
			_logger.LogDebug("GetJQueryDateFormat(html={Html}): currentFormat → {CurrentFormat}", html, currentFormat);
            CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        // Convert the date
        currentFormat = currentFormat.Replace("dddd", "DD");
        currentFormat = currentFormat.Replace("ddd", "D");

        // Convert month
        if (currentFormat.Contains("MMMM"))
        {
            currentFormat = currentFormat.Replace("MMMM", "MM");
        }
        else if (currentFormat.Contains("MMM"))
        {
            currentFormat = currentFormat.Replace("MMM", "M");
        }
        else if (currentFormat.Contains("MM"))
        {
            currentFormat = currentFormat.Replace("MM", "mm");
        }
        else
        {
            currentFormat = currentFormat.Replace("M", "m");
        }

        // Convert year
        currentFormat = currentFormat.Contains("yyyy") ?
            currentFormat.Replace("yyyy", "yy") : currentFormat.Replace("yy", "y");

		_logger.LogDebug("<<< Exiting GetJQueryDateFormat(html={Html})", html);
        return currentFormat;
    }

    /// <summary>
    /// Prepare a common pager
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <param name="html">HTML helper</param>
    /// <param name="model">Pager model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pager
    /// </returns>
    /// <remarks>We have two pagers: The first one can have custom routes. The second one just adds query string parameter</remarks>
    public static async Task<IHtmlContent> PagerAsync<TModel>(this IHtmlHelper<TModel> html, PagerModel model)
    {
		_logger.LogDebug(">>> Entering PagerAsync(html={Html},model={Model})", html, model);
        if (model.TotalRecords == 0)
            return new HtmlString(string.Empty);

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

		_logger.LogDebug("PagerAsync(html={Html},model={Model}): localizationService → {LocalizationService}", html, model, localizationService);
        var links = new StringBuilder();
        if (model.ShowTotalSummary && (model.TotalPages > 0))
		_logger.LogDebug("PagerAsync(html={Html},model={Model}): links → {Links}", html, model, links);
        {
            links.Append("<li class=\"total-summary\">");
            links.Append(string.Format(await model.GetCurrentPageTextAsync(), model.PageIndex + 1, model.TotalPages, model.TotalRecords));
            links.Append("</li>");
        }

        if (model.ShowPagerItems && (model.TotalPages > 1))
        {
            if (model.ShowFirst)
            {
                //first page
                if ((model.PageIndex >= 3) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                {
                    model.RouteValues.PageNumber = 1;

                    links.Append("<li class=\"first-page\">");
                    if (model.UseRouteLinks)
                    {
                        var link = html.RouteLink(await model.GetFirstButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.FirstPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    else
                    {
                        var link = html.ActionLink(await model.GetFirstButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.FirstPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    links.Append("</li>");
                }
            }

            if (model.ShowPrevious)
            {
                //previous page
                if (model.PageIndex > 0)
                {
                    model.RouteValues.PageNumber = model.PageIndex;

                    links.Append("<li class=\"previous-page\">");
                    if (model.UseRouteLinks)
                    {
                        var link = html.RouteLink(await model.GetPreviousButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.PreviousPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    else
                    {
                        var link = html.ActionLink(await model.GetPreviousButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.PreviousPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    links.Append("</li>");
                }
            }

            if (model.ShowIndividualPages)
            {
                //individual pages
                var firstIndividualPageIndex = model.GetFirstIndividualPageIndex();
                var lastIndividualPageIndex = model.GetLastIndividualPageIndex();
				_logger.LogDebug("PagerAsync(html={Html},model={Model}): firstIndividualPageIndex → {FirstIndividualPageIndex}", html, model, firstIndividualPageIndex);
				_logger.LogDebug("PagerAsync(html={Html},model={Model}): lastIndividualPageIndex → {LastIndividualPageIndex}", html, model, lastIndividualPageIndex);
                for (var i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                {
                    if (model.PageIndex == i)
                        links.AppendFormat("<li class=\"current-page\"><span>{0}</span></li>", i + 1);
                    else
                    {
                        model.RouteValues.PageNumber = i + 1;

                        links.Append("<li class=\"individual-page\">");
                        if (model.UseRouteLinks)
                        {
                            var link = html.RouteLink((i + 1).ToString(),
                                model.RouteActionName,
                                model.RouteValues,
                                new { title = string.Format(await localizationService.GetResourceAsync("Pager.PageLinkTitle"), i + 1) });
                            links.Append(await link.RenderHtmlContentAsync());
                        }
                        else
                        {
                            var link = html.ActionLink((i + 1).ToString(),
                                model.RouteActionName,
                                model.RouteValues,
                                new { title = string.Format(await localizationService.GetResourceAsync("Pager.PageLinkTitle"), i + 1) });
                            links.Append(await link.RenderHtmlContentAsync());
                        }
                        links.Append("</li>");
                    }
                }
            }

            if (model.ShowNext)
            {
                //next page
                if ((model.PageIndex + 1) < model.TotalPages)
                {
                    model.RouteValues.PageNumber = (model.PageIndex + 2);

                    links.Append("<li class=\"next-page\">");
                    if (model.UseRouteLinks)
                    {
                        var link = html.RouteLink(await model.GetNextButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.NextPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    else
                    {
                        var link = html.ActionLink(await model.GetNextButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.NextPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    links.Append("</li>");
                }
            }

            if (model.ShowLast)
            {
                //last page
                if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                {
                    model.RouteValues.PageNumber = model.TotalPages;

                    links.Append("<li class=\"last-page\">");
                    if (model.UseRouteLinks)
                    {
                        var link = html.RouteLink(await model.GetLastButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.LastPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    else
                    {
                        var link = html.ActionLink(await model.GetLastButtonTextAsync(),
                            model.RouteActionName,
                            model.RouteValues,
                            new { title = await localizationService.GetResourceAsync("Pager.LastPageTitle") });
                        links.Append(await link.RenderHtmlContentAsync());
                    }
                    links.Append("</li>");
                }
            }
        }

        var result = links.ToString();
        if (!string.IsNullOrEmpty(result))
			_logger.LogDebug("PagerAsync(html={Html},model={Model}): result → {Result}", html, model, result);
            result = "<ul>" + result + "</ul>";

        return new HtmlString(result);
    }

    /// <summary>
    /// Prepare a common pager
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="model">Pager model</param>
    /// <returns>Pager</returns>
    /// <remarks>We have two pagers: The first one can have custom routes. The second one just adds query string parameter</remarks>
    public static Pager Pager(this IHtmlHelper helper, IPageableModel model)
    {
		_logger.LogDebug(">>> Entering Pager(helper={Helper},model={Model})", helper, model);
		_logger.LogDebug("<<< Exiting Pager(helper={Helper},model={Model})", helper, model);
        return new Pager(model, helper.ViewContext);
    }

    /// <summary>
    /// Prepare a special small pager for forum topics
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <param name="html">HTML helper</param>
    /// <param name="model">Model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pager
    /// </returns>
    public static async Task<IHtmlContent> ForumTopicSmallPagerAsync<TModel>(this IHtmlHelper<TModel> html, ForumTopicRowModel model)
    {
		_logger.LogDebug(">>> Entering ForumTopicSmallPagerAsync(html={Html},model={Model})", html, model);
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

		_logger.LogDebug("ForumTopicSmallPagerAsync(html={Html},model={Model}): localizationService → {LocalizationService}", html, model, localizationService);
        var forumTopicId = model.Id;
        var forumTopicSlug = model.SeName;
        var totalPages = model.TotalPostPages;

        if (totalPages > 0)
        {
            var links = new StringBuilder();

			_logger.LogDebug("ForumTopicSmallPagerAsync(html={Html},model={Model}): links → {Links}", html, model, links);
            if (totalPages <= 4)
            {
                for (var x = 1; x <= totalPages; x++)
                {
                    var link = html.RouteLink(x.ToString(),
                        NopRouteNames.Standard.TOPIC_SLUG_PAGED,
                        new { id = forumTopicId, pageNumber = x, slug = forumTopicSlug },
                        new { title = string.Format(await localizationService.GetResourceAsync("Pager.PageLinkTitle"), x.ToString()) });
                    links.Append(await link.RenderHtmlContentAsync());
                    if (x < totalPages)
                        links.Append(", ");
                }
            }
            else
            {
                var link1 = html.RouteLink("1",
                    NopRouteNames.Standard.TOPIC_SLUG_PAGED,
                    new { id = forumTopicId, pageNumber = 1, slug = forumTopicSlug },
                    new { title = string.Format(await localizationService.GetResourceAsync("Pager.PageLinkTitle"), 1) });
                links.Append(await link1.RenderHtmlContentAsync());

                links.Append(" ... ");

                for (var x = totalPages - 2; x <= totalPages; x++)
                {
                    var link2 = html.RouteLink(x.ToString(),
                        NopRouteNames.Standard.TOPIC_SLUG_PAGED,
                        new { id = forumTopicId, pageNumber = x, slug = forumTopicSlug },
                        new { title = string.Format(await localizationService.GetResourceAsync("Pager.PageLinkTitle"), x.ToString()) });
                    links.Append(await link2.RenderHtmlContentAsync());

                    if (x < totalPages)
                        links.Append(", ");
                }
            }

            // Inserts the topic page links into the localized string ([Go to page: {0}])
            return new HtmlString(string.Format(await localizationService.GetResourceAsync("Forum.Topics.GotoPostPager"), links));
        }

        return new HtmlString(string.Empty);
    }

    /// <summary>
    /// Get a value of the text flow uses for the current UI culture
    /// </summary>
    /// <param name="html">HTML helper</param>
    /// <param name="ignoreRtl">A value indicating whether to we should ignore RTL language property for admin area. False by default</param>
    /// <returns>"rtl" if text flows from right to left; otherwise, "ltr".</returns>
    public static string GetUIDirection(this IHtmlHelper html, bool ignoreRtl = false)
    {
		_logger.LogDebug(">>> Entering GetUIDirection(html={Html},ignoreRtl={IgnoreRtl})", html, ignoreRtl);
        if (ignoreRtl)
		_logger.LogDebug("<<< Exiting GetUIDirection(html={Html},ignoreRtl={IgnoreRtl})", html, ignoreRtl);
            return "ltr";

        return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";
    }

    /// <summary>
    /// Return a value indicating whether the working language and theme support RTL (right-to-left)
    /// </summary>
    /// <param name="html">HTML helper</param>
    /// <param name="themeName">Theme name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    public static async Task<bool> ShouldUseRtlThemeAsync(this IHtmlHelper html, string themeName = null)
    {
		_logger.LogDebug(">>> Entering ShouldUseRtlThemeAsync(html={Html},themeName={ThemeName})", html, themeName);
        if (!CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            return false;

        //ensure that the active theme also supports it
        themeName ??= await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();
        var theme = await EngineContext.Current.Resolve<IThemeProvider>().GetThemeBySystemNameAsync(themeName);

		_logger.LogDebug("ShouldUseRtlThemeAsync(html={Html},themeName={ThemeName}): theme → {Theme}", html, themeName, theme);
		_logger.LogDebug("<<< Exiting ShouldUseRtlThemeAsync(html={Html},themeName={ThemeName})", html, themeName);
        return theme?.SupportRtl ?? false;
    }

    /// <summary>
    /// Return a value indicating whether to display the admin tour
    /// </summary>
    /// <param name="html">HTML helper</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    public static async Task<bool> IsTourActiveAsync(this IHtmlHelper html)
    {
		_logger.LogDebug(">>> Entering IsTourActiveAsync(html={Html})", html);
        var webHelper = EngineContext.Current.Resolve<IWebHelper>();

		_logger.LogDebug("IsTourActiveAsync(html={Html}): webHelper → {WebHelper}", html, webHelper);
        if (webHelper.QueryString<bool?>("ShowTour") == true)
		_logger.LogDebug("<<< Exiting IsTourActiveAsync(html={Html})", html);
            return await EngineContext.Current.Resolve<IWorkContext>().GetCurrentVendorAsync() is null;

        return false;
    }
}