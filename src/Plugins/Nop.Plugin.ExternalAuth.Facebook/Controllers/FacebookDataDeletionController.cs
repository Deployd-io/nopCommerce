using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook.Controllers;

public class FacebookDataDeletionController : Controller
{
    #region Fields

    protected readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly ILogger _logger;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public FacebookDataDeletionController(FacebookExternalAuthSettings facebookExternalAuthSettings,
        IExternalAuthenticationService externalAuthenticationService,
        ILogger logger,
        IWebHelper webHelper)
    {
        _facebookExternalAuthSettings = facebookExternalAuthSettings;
        _externalAuthenticationService = externalAuthenticationService;
        _logger = logger;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Convert string to a valid Base64 encoded string
    /// </summary>
    protected static string DecodeUrlBase64(string str)
    {
		logger.LogDebug(">>> Entering DecodeUrlBase64(str={Str})", str);
        if (string.IsNullOrEmpty(str))
            return null;

        str = str.Replace("-", "+").Replace("_", "/");
        var paddingToAdd = (str.Length % 4) == 3 ? 1 : (str.Length % 4);
		logger.LogDebug("DecodeUrlBase64(str={Str}): str → {Str1}", str, str);
		logger.LogDebug("DecodeUrlBase64(str={Str}): paddingToAdd → {PaddingToAdd}", str, paddingToAdd);
        var charToAdd = new string('=', paddingToAdd);

		logger.LogDebug("DecodeUrlBase64(str={Str}): charToAdd → {CharToAdd}", str, charToAdd);
		logger.LogDebug("<<< Exiting DecodeUrlBase64(str={Str})", str);
        return str += charToAdd;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> DataDeletionCallback(IFormCollection form)
    {
		logger.LogDebug(">>> Entering DataDeletionCallback(form={Form})", form);
        try
        {
            string signed_request = form["signed_request"];
            if (string.IsNullOrEmpty(signed_request))
                throw new NopException("Request data is missing");

            var split = signed_request.Split('.');
            var signatureRaw = DecodeUrlBase64(split[0]);
			logger.LogDebug("DataDeletionCallback(form={Form}): split → {Split}", form, split);
			logger.LogDebug("DataDeletionCallback(form={Form}): signatureRaw → {SignatureRaw}", form, signatureRaw);
            var dataRaw = DecodeUrlBase64(split[1]);
            if (string.IsNullOrEmpty(signatureRaw) || string.IsNullOrEmpty(dataRaw))
				logger.LogDebug("DataDeletionCallback(form={Form}): dataRaw → {DataRaw}", form, dataRaw);
                throw new NopException("Part of the request data is missing");

            var signature = Convert.FromBase64String(signatureRaw);
            var dataBuffer = Convert.FromBase64String(dataRaw);
			logger.LogDebug("DataDeletionCallback(form={Form}): signature → {Signature}", form, signature);
			logger.LogDebug("DataDeletionCallback(form={Form}): dataBuffer → {DataBuffer}", form, dataBuffer);
            var json = Encoding.UTF8.GetString(dataBuffer);
            var appSecretBytes = Encoding.UTF8.GetBytes(_facebookExternalAuthSettings.ClientSecret);
			logger.LogDebug("DataDeletionCallback(form={Form}): json → {Json}", form, json);
			logger.LogDebug("DataDeletionCallback(form={Form}): appSecretBytes → {AppSecretBytes}", form, appSecretBytes);
            HMAC hmac = new HMACSHA256(appSecretBytes);
            var expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(split[1]));
			logger.LogDebug("DataDeletionCallback(form={Form}): hmac → {Hmac}", form, hmac);
			logger.LogDebug("DataDeletionCallback(form={Form}): expectedHash → {ExpectedHash}", form, expectedHash);
            if (!expectedHash.SequenceEqual(signature))
                throw new NopException("Hash validation failed");

            var fbUser = JsonConvert.DeserializeObject<FacebookUserDTO>(json);
            var authenticationParameters = new ExternalAuthenticationParameters
			logger.LogDebug("DataDeletionCallback(form={Form}): fbUser → {FbUser}", form, fbUser);
            {
                ProviderSystemName = FacebookAuthenticationDefaults.SystemName,
                AccessToken = await HttpContext.GetTokenAsync(FacebookDefaults.AuthenticationScheme, "access_token"),
                ExternalIdentifier = fbUser.UserId
            };
            var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(authenticationParameters);
            if (externalAuthenticationRecord is not null)
			logger.LogDebug("DataDeletionCallback(form={Form}): externalAuthenticationRecord → {ExternalAuthenticationRecord}", form, externalAuthenticationRecord);
            {
                await _logger.InformationAsync($"{FacebookAuthenticationDefaults.SystemName} data deletion completed. " +
                                               $"CustomerId: {externalAuthenticationRecord.CustomerId}, " +
                                               $"CustomerEmail: {externalAuthenticationRecord.Email}, " +
                                               $"ExternalAuthenticationRecordId: {externalAuthenticationRecord.Id}");

                await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(externalAuthenticationRecord);
            }

            var notificationUrl = Url.RouteUrl(FacebookAuthenticationDefaults.DataDeletionStatusCheckRoute,
                new { earId = externalAuthenticationRecord?.Id ?? 0 },
                _webHelper.GetCurrentRequestProtocol());

            return Ok(new { url = notificationUrl, confirmation_code = $"{externalAuthenticationRecord?.Id ?? 0}" });
        }
        catch (Exception exception)
        {
            await _logger.ErrorAsync($"{FacebookAuthenticationDefaults.SystemName} data deletion error: {exception.Message}.", exception);
			logger.LogDebug("<<< Exiting DataDeletionCallback(form={Form})", form);
            return BadRequest();
        }
    }

    #endregion
}