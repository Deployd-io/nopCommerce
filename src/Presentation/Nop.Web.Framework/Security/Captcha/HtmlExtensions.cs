using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Security.Captcha;

/// <summary>
/// HTML extensions
/// </summary>
public static class HtmlExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    /// <summary>
    /// Get the reCAPTCHA language
    /// </summary>
    /// <param name="captchaSettings">Captcha settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language code
    /// </returns>
    private static async Task<string> GetReCaptchaLanguageAsync(CaptchaSettings captchaSettings)
    {
		_logger.LogError(">>> Entering GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings})", captchaSettings);
        var language = (captchaSettings.ReCaptchaDefaultLanguage ?? string.Empty).ToLowerInvariant();
		_logger.LogError("GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings}): language → {Language}", captchaSettings, language);
        if (captchaSettings.AutomaticallyChooseLanguage)
        {
            //this list got from this site: https://developers.google.com/recaptcha/docs/language
            //but we use languages only with two letters in the code
            var supportedLanguageCodes = new List<string> { "af", "am", "ar", "az", "bg", "bn", "ca", "cs", "da", "de", "el", "en", "es", "et", "eu", "fa", "fi", "fil", "fr", "gl", "gu", "hi", "hr", "hu", "hy", "id", "is", "it", "iw", "ja", "ka", "kn", "ko", "lo", "lt", "lv", "ml", "mn", "mr", "ms", "nl", "no", "pl", "pt", "ro", "ru", "si", "sk", "sl", "sr", "sv", "sw", "ta", "te", "th", "tr", "uk", "ur", "vi", "zu" };
			_logger.LogError("GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings}): supportedLanguageCodes → {SupportedLanguageCodes}", captchaSettings, supportedLanguageCodes);

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
			_logger.LogError("GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings}): languageService → {LanguageService}", captchaSettings, languageService);
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
			_logger.LogError("GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings}): workContext → {WorkContext}", captchaSettings, workContext);

            var currentLanguage = await workContext.GetWorkingLanguageAsync();
			_logger.LogError("GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings}): currentLanguage → {CurrentLanguage}", captchaSettings, currentLanguage);
            var twoLetterIsoCode = currentLanguage != null
                ? languageService.GetTwoLetterIsoLanguageName(currentLanguage).ToLowerInvariant()
                : string.Empty;

            language = supportedLanguageCodes.Contains(twoLetterIsoCode) ? twoLetterIsoCode : language;
        }

		_logger.LogWarning("<<< Exiting GetReCaptchaLanguageAsync(captchaSettings={CaptchaSettings})", captchaSettings);
        return language;
    }

    /// <summary>
    /// Generate API script tag
    /// </summary>
    /// <param name="captchaSettings">Captcha settings</param>
    /// <param name="captchaId">Captcha ID</param>
    /// <param name="render">Render</param>
    /// <param name="language">Language</param>
    /// <returns>Script tag</returns>
    private static TagBuilder GenerateLoadApiScriptTag(CaptchaSettings captchaSettings, string captchaId, string render, string language)
    {
		_logger.LogError(">>> Entering GenerateLoadApiScriptTag(captchaSettings={CaptchaSettings},captchaId={CaptchaId},render={Render},language={Language})", captchaSettings, captchaId, render, language);
        var hl = !string.IsNullOrEmpty(language)
            ? $"&hl={language}"
            : string.Empty;
        var url = string.Format($"{captchaSettings.ReCaptchaApiUrl}{NopSecurityDefaults.RecaptchaScriptPath}", captchaId, render, hl);
		_logger.LogError("GenerateLoadApiScriptTag(captchaSettings={CaptchaSettings},captchaId={CaptchaId},render={Render},language={Language}): url → {Url}", captchaSettings, captchaId, render, language, url);
        var scriptLoadApiTag = new TagBuilder("script") { TagRenderMode = TagRenderMode.Normal };
		_logger.LogError("GenerateLoadApiScriptTag(captchaSettings={CaptchaSettings},captchaId={CaptchaId},render={Render},language={Language}): scriptLoadApiTag → {ScriptLoadApiTag}", captchaSettings, captchaId, render, language, scriptLoadApiTag);
        scriptLoadApiTag.Attributes.Add("src", url);
        scriptLoadApiTag.Attributes.Add("async", null);
        scriptLoadApiTag.Attributes.Add("defer", null);

		_logger.LogWarning("<<< Exiting GenerateLoadApiScriptTag(captchaSettings={CaptchaSettings},captchaId={CaptchaId},render={Render},language={Language})", captchaSettings, captchaId, render, language);
        return scriptLoadApiTag;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate reCAPTCHA Control
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="captchaSettings">Captcha settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public static async Task<IHtmlContent> GenerateCheckBoxReCaptchaV2Async(this IHtmlHelper helper, CaptchaSettings captchaSettings)
    {
		_logger.LogError(">>> Entering GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings})", helper, captchaSettings);
        //prepare language
        var language = await GetReCaptchaLanguageAsync(captchaSettings);
		_logger.LogError("GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings}): language → {Language}", helper, captchaSettings, language);

        //prepare theme
        var theme = (captchaSettings.ReCaptchaTheme ?? string.Empty).ToLowerInvariant();
		_logger.LogError("GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings}): theme → {Theme}", helper, captchaSettings, theme);
        theme = theme switch
        {
            "blackglass" or "dark" => "dark",
            "clean" or "red" or "white" or "light" => "light",
            _ => "light",
        };

        //prepare identifier
        var id = $"captcha_{CommonHelper.GenerateRandomInteger()}";
		_logger.LogError("GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings}): id → {Id}", helper, captchaSettings, id);

        //prepare public key
        var publicKey = captchaSettings.ReCaptchaPublicKey ?? string.Empty;

        //generate reCAPTCHA Control
        var scriptCallbackTag = new TagBuilder("script") { TagRenderMode = TagRenderMode.Normal };
		_logger.LogError("GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings}): scriptCallbackTag → {ScriptCallbackTag}", helper, captchaSettings, scriptCallbackTag);
        scriptCallbackTag.InnerHtml
            .AppendHtml($"var onloadCallback{id} = function() {{grecaptcha.render('{id}', {{'sitekey' : '{publicKey}', 'theme' : '{theme}' }});}};");

        var captchaTag = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
		_logger.LogError("GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings}): captchaTag → {CaptchaTag}", helper, captchaSettings, captchaTag);
        captchaTag.Attributes.Add("id", id);

        var scriptLoadApiTag = GenerateLoadApiScriptTag(captchaSettings, id, "explicit", language);
		_logger.LogError("GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings}): scriptLoadApiTag → {ScriptLoadApiTag}", helper, captchaSettings, scriptLoadApiTag);

		_logger.LogWarning("<<< Exiting GenerateCheckBoxReCaptchaV2Async(helper={Helper},captchaSettings={CaptchaSettings})", helper, captchaSettings);
        return new HtmlString(await scriptCallbackTag.RenderHtmlContentAsync() + await captchaTag.RenderHtmlContentAsync() + await scriptLoadApiTag.RenderHtmlContentAsync());
    }

    /// <summary>
    /// Generate reCAPTCHA v3 Control
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="captchaSettings">Captcha settings</param>
    /// <param name="actionName">Action name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public static async Task<IHtmlContent> GenerateReCaptchaV3Async(this IHtmlHelper helper, CaptchaSettings captchaSettings, string actionName = null)
    {
		_logger.LogError(">>> Entering GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName})", helper, captchaSettings, actionName);
        //prepare language
        var language = await GetReCaptchaLanguageAsync(captchaSettings);
		_logger.LogError("GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName}): language → {Language}", helper, captchaSettings, actionName, language);

        //prepare identifier
        var id = $"captcha_{CommonHelper.GenerateRandomInteger()}";
		_logger.LogError("GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName}): id → {Id}", helper, captchaSettings, actionName, id);

        //prepare public key
        var publicKey = captchaSettings.ReCaptchaPublicKey ?? string.Empty;

        //prepare reCAPTCHA script
        if (string.IsNullOrEmpty(actionName))
            actionName = helper.ViewContext.RouteData.Values["action"].ToString();
			_logger.LogError("GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName}): actionName → {ActionName1}", helper, captchaSettings, actionName, actionName);

        var scriptCallback = $@"
                var onloadCallback{id} = function() {{
                    var form = $('input[id=""g-recaptcha-response_{id}""]').closest('form');
                    var btn = $(form.find(':submit')[0]);

                    var actionBtn = btn.data('action');
                    if (actionBtn == null) {{
                        actionBtn = '{actionName}';
                    }}

                    var loaded = false;
                    var isBusy = false;
                    btn.on('click', function (e) {{
                        if (!isBusy) {{
                            isBusy = true;
                            grecaptcha.execute('{publicKey}', {{ 'action': actionBtn }}).then(function(token) {{
                                $('#g-recaptcha-response_{id}', form).val(token);
                                loaded = true;
                                btn.trigger('click');
                            }});
                        }}
                        return loaded;
                    }});
                }}
            ";
        var scriptCallbackTag = new TagBuilder("script") { TagRenderMode = TagRenderMode.Normal };
        scriptCallbackTag.InnerHtml.AppendHtml(scriptCallback);

        //prepare reCAPTCHA token input
        var captchaTokenInput = new TagBuilder("input") { TagRenderMode = TagRenderMode.Normal };
        captchaTokenInput.Attributes.Add("type", "hidden");
        captchaTokenInput.Attributes.Add("id", $"g-recaptcha-response_{id}");
        captchaTokenInput.Attributes.Add("name", "g-recaptcha-response");
		_logger.LogError("GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName}): scriptCallbackTag → {ScriptCallbackTag}", helper, captchaSettings, actionName, scriptCallbackTag);
		_logger.LogError("GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName}): captchaTokenInput → {CaptchaTokenInput}", helper, captchaSettings, actionName, captchaTokenInput);

        var scriptLoadApiTag = GenerateLoadApiScriptTag(captchaSettings, id, publicKey, language);
		_logger.LogError("GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName}): scriptLoadApiTag → {ScriptLoadApiTag}", helper, captchaSettings, actionName, scriptLoadApiTag);

		_logger.LogWarning("<<< Exiting GenerateReCaptchaV3Async(helper={Helper},captchaSettings={CaptchaSettings},actionName={ActionName})", helper, captchaSettings, actionName);
        return new HtmlString(await captchaTokenInput.RenderHtmlContentAsync() + await scriptCallbackTag.RenderHtmlContentAsync() + await scriptLoadApiTag.RenderHtmlContentAsync());
    }

    #endregion
}