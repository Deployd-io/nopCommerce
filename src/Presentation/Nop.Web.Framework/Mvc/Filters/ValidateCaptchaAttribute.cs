using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute enabling CAPTCHA validation
/// </summary>
public sealed class ValidateCaptchaAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute 
    /// </summary>
    /// <param name="actionParameterName">The name of the action parameter to which the result will be passed</param>
    public ValidateCaptchaAttribute(string actionParameterName = "captchaValid") : base(typeof(ValidateCaptchaFilter))
    {
		_logger.LogDebug(">>> Entering ValidateCaptchaAttribute(actionParameterName={ActionParameterName})", actionParameterName);
        Arguments = [actionParameterName];
		_logger.LogDebug("ValidateCaptchaAttribute(actionParameterName={ActionParameterName}): Arguments → {Arguments}", actionParameterName, Arguments);
		_logger.LogDebug("<<< Exiting ValidateCaptchaAttribute(actionParameterName={ActionParameterName})", actionParameterName);
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter enabling CAPTCHA validation
    /// </summary>
    private class ValidateCaptchaFilter : IAsyncActionFilter
    {
        #region Constants

        private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";
        private const string G_RESPONSE_FIELD_KEY = "g-recaptcha-response";

        #endregion

        #region Fields

        protected readonly string _actionParameterName;
        protected readonly CaptchaHttpClient _captchaHttpClient;
        protected readonly CaptchaSettings _captchaSettings;
        protected readonly ILogger _logger;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ValidateCaptchaFilter(string actionParameterName,
            CaptchaHttpClient captchaHttpClient,
            CaptchaSettings captchaSettings,
            ILogger logger,
            IWorkContext workContext)
        {
            _actionParameterName = actionParameterName;
			_logger.LogDebug("ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext}): _actionParameterName → {_actionParameterName}", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext, _actionParameterName);
            _captchaHttpClient = captchaHttpClient;
			_logger.LogDebug("ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext}): _captchaHttpClient → {_captchaHttpClient}", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext, _captchaHttpClient);
            _captchaSettings = captchaSettings;
			_logger.LogDebug("ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext}): _captchaSettings → {_captchaSettings}", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext, _captchaSettings);
            _logger = logger;
			_logger.LogDebug(">>> Entering ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext})", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext);
			_logger.LogDebug("ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext}): _logger → {_logger}", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext, _logger);
            _workContext = workContext;
			_logger.LogDebug("<<< Exiting ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext})", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext);
			_logger.LogDebug("ValidateCaptchaFilter(actionParameterName={ActionParameterName},captchaHttpClient={CaptchaHttpClient},captchaSettings={CaptchaSettings},logger={Logger},workContext={WorkContext}): _workContext → {_workContext}", actionParameterName, captchaHttpClient, captchaSettings, logger, workContext, _workContext);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidateCaptchaAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering ValidateCaptchaAsync(context={Context})", context);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether CAPTCHA is enabled
            if (_captchaSettings.Enabled)
            {
                //push the validation result as an action parameter
                var isValid = false;
				_logger.LogDebug("ValidateCaptchaAsync(context={Context}): isValid → {IsValid}", context, isValid);

                //get form values
                var captchaResponseValue = await context.HttpContext.Request.GetFormValueAsync(RESPONSE_FIELD_KEY);
				_logger.LogDebug("ValidateCaptchaAsync(context={Context}): captchaResponseValue → {CaptchaResponseValue}", context, captchaResponseValue);
                var gCaptchaResponseValue = await context.HttpContext.Request.GetFormValueAsync(G_RESPONSE_FIELD_KEY);
				_logger.LogDebug("ValidateCaptchaAsync(context={Context}): gCaptchaResponseValue → {GCaptchaResponseValue}", context, gCaptchaResponseValue);

                if (!StringValues.IsNullOrEmpty(captchaResponseValue) || !StringValues.IsNullOrEmpty(gCaptchaResponseValue))
                {
                    //validate request
                    try
                    {
                        var value = !StringValues.IsNullOrEmpty(captchaResponseValue) ? captchaResponseValue : gCaptchaResponseValue;
						_logger.LogDebug("ValidateCaptchaAsync(context={Context}): value → {Value}", context, value);
                        var response = await _captchaHttpClient.ValidateCaptchaAsync(value);
						_logger.LogDebug("ValidateCaptchaAsync(context={Context}): response → {Response}", context, response);

                        switch (_captchaSettings.CaptchaType)
                        {
                            case CaptchaType.CheckBoxReCaptchaV2:
                                isValid = response.IsValid;
                                break;

                            case CaptchaType.ReCaptchaV3:
                                isValid = response.IsValid &&
                                          response.Action == context.RouteData.Values["action"].ToString() &&
                                          response.Score > _captchaSettings.ReCaptchaV3ScoreThreshold;
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        await _logger.ErrorAsync("Error occurred on CAPTCHA validation", exception, await _workContext.GetCurrentCustomerAsync());
                    }
                }

                context.ActionArguments[_actionParameterName] = isValid;
            }
            else
                context.ActionArguments[_actionParameterName] = false;
				_logger.LogInformation("ValidateCaptchaAsync(context)={Context}: external service call took {Arg1} ms", context, __logStart.ElapsedMilliseconds);
				_logger.LogDebug("<<< Exiting ValidateCaptchaAsync(context={Context})", context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
			_logger.LogDebug(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            await ValidateCaptchaAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}