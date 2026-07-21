using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Security;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to functional
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class CheckPermissionAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="permissionSystemName">Permission to check</param>
    /// <param name="resultType">The result type for not confirmed access situation</param>
    public CheckPermissionAttribute(string permissionSystemName, CheckPermissionResultType resultType = CheckPermissionResultType.Default) : base(typeof(CheckPermissionFilter))
    {
		logger.LogDebug(">>> Entering CheckPermissionAttribute(permissionSystemName={PermissionSystemName},resultType={ResultType})", permissionSystemName, resultType);
        Arguments = new object[] { resultType, new List<string> { permissionSystemName } };
        PermissionSystemName.Add(permissionSystemName);
		logger.LogDebug("CheckPermissionAttribute(permissionSystemName={PermissionSystemName},resultType={ResultType}): Arguments → {Arguments}", permissionSystemName, resultType, Arguments);
        ResultType = resultType;
		logger.LogDebug("CheckPermissionAttribute(permissionSystemName={PermissionSystemName},resultType={ResultType}): ResultType → {ResultType1}", permissionSystemName, resultType, ResultType);
		logger.LogDebug(">>> Entering CheckPermissionAttribute(permissionSystemNames={PermissionSystemNames},resultType={ResultType})", permissionSystemNames, resultType);
		logger.LogDebug("CheckPermissionAttribute(permissionSystemNames={PermissionSystemNames},resultType={ResultType}): Arguments → {Arguments}", permissionSystemNames, resultType, Arguments);
		logger.LogDebug("<<< Exiting CheckPermissionAttribute(permissionSystemNames={PermissionSystemNames},resultType={ResultType})", permissionSystemNames, resultType);
    }

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="permissionSystemNames">Permissions to check</param>
    /// <param name="resultType">The result type for not confirmed access situation</param>
    public CheckPermissionAttribute(string[] permissionSystemNames, CheckPermissionResultType resultType = CheckPermissionResultType.Default) : base(typeof(CheckPermissionFilter))
    {
        Arguments = new object[] { resultType, permissionSystemNames.ToList() };
        PermissionSystemName.AddRange(permissionSystemNames);
        ResultType = resultType;
		logger.LogDebug("CheckPermissionAttribute(permissionSystemNames={PermissionSystemNames},resultType={ResultType}): ResultType → {ResultType1}", permissionSystemNames, resultType, ResultType);
		logger.LogDebug("<<< Exiting CheckPermissionAttribute(permissionSystemName={PermissionSystemName},resultType={ResultType})", permissionSystemName, resultType);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a permission system name to check
    /// </summary>
    public List<string> PermissionSystemName { get; } = new();

    /// <summary>
    /// Gets a check permission result type
    /// </summary>
    public CheckPermissionResultType ResultType { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that confirms access
    /// </summary>
    private class CheckPermissionFilter : IAsyncAuthorizationFilter
    {
        #region Fields

        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IWebHelper _webHelper;

        protected readonly CheckPermissionResultType _resultType;
        protected readonly List<string> _permissionSystemNames;

        #endregion

        #region Ctor

        public CheckPermissionFilter(CheckPermissionResultType resultType,
            List<string> permissionSystemNames,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWebHelper webHelper)
        {
            _resultType = resultType;
            _permissionSystemNames = permissionSystemNames;

            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task AuthorizeAsync(AuthorizationFilterContext context)
        {
			logger.LogDebug(">>> Entering AuthorizeAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            if (!context.Filters.Any(filter => filter is CheckPermissionFilter))
                return;

            //authorize permission
            foreach (var permissionSystemName in _permissionSystemNames)
                if (await _permissionService.AuthorizeAsync(permissionSystemName))
                    return;

            var resultType = _resultType;

			logger.LogDebug("AuthorizeAsync(context={Context}): resultType → {ResultType}", context, resultType);
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                return;

            if (resultType == CheckPermissionResultType.Default)
                resultType = request.Method switch
                {
                    WebRequestMethods.Http.Post => _webHelper.IsAjaxRequest(request) ? CheckPermissionResultType.Json : CheckPermissionResultType.Html,
                    WebRequestMethods.Http.Get => CheckPermissionResultType.Html,
                    _ => CheckPermissionResultType.Text,
                };

            IActionResult html()
            {
                var controller = request.RouteValues.ContainsKey("controller")
                    ? request.RouteValues["controller"]
                    : string.Empty;

                var action = request.RouteValues.ContainsKey("action")
                    ? request.RouteValues["action"]
                    : string.Empty;

                var pageSystemNameKey = $"{controller}.{action}";

				logger.LogDebug("AuthorizeAsync(context={Context}): pageSystemNameKey → {PageSystemNameKey}", context, pageSystemNameKey);
                return new RedirectToActionResult("AccessDenied", "Security",
                    new { pageUrl = _webHelper.GetRawUrl(request), pageSystemNameKey });
            }

            context.Result = resultType switch
			logger.LogDebug("AuthorizeAsync(context={Context}): context.Result → {Arg1}", context, context.Result);
            {
                CheckPermissionResultType.Json => new JsonResult(new
                {
                    error = await _localizationService.GetResourceAsync("Admin.AccessDenied.Description")
                }),
                CheckPermissionResultType.Html => html(),
                CheckPermissionResultType.Text => new ContentResult
					logger.LogDebug("AuthorizeAsync(context={Context}): error → {Error}", context, error);
                {
                    Content = await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"),
                    ContentType = "text/plain",
                },
                _ => context.Result
            };
			logger.LogDebug("<<< Exiting AuthorizeAsync(context={Context})", context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
			logger.LogDebug(">>> Entering OnAuthorizationAsync(context={Context})", context);
            await AuthorizeAsync(context);
			logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion

    #region Nested class

    public enum CheckPermissionResultType
    {
        /// <summary>
        /// For Get requests it will be <see cref="Html"/>. For POST request it will be <see cref="Json"/> if request is Ajax, <see cref="Html"/> in other case. For other one it will by <see cref="Text"/>
        /// </summary>
        Default = 0,

        /// <summary>
        /// Redirect to Access Denied page
        /// </summary>
        Html = 1,

        /// <summary>
        /// Return the plain text content
        /// </summary>
        Text = 2,

        /// <summary>
        /// Return the JSON content
        /// </summary>
        Json = 3
    }

    #endregion
}
