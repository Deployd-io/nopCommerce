using Nop.Core;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Factories;

public class AuthenticationModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly GoogleAuthenticatorService _googleAuthenticatorService;
    protected readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AuthenticationModelFactory(IWorkContext workContext,
        GoogleAuthenticatorService googleAuthenticatorService,
        GoogleAuthenticatorSettings googleAuthenticatorSettings)
    {
        _workContext = workContext;
        _googleAuthenticatorService = googleAuthenticatorService;
        _googleAuthenticatorSettings = googleAuthenticatorSettings;
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<AuthModel> PrepareAuthModel(AuthModel model)
    {
		_logger.LogDebug(">>> Entering PrepareAuthModel(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var secretkey = Guid.NewGuid().ToString().Replace("-", "")[0..10];
        var setupInfo = await _googleAuthenticatorService.GenerateSetupCode(secretkey);
		_logger.LogDebug("PrepareAuthModel(model={Model}): secretkey → {Secretkey}", model, secretkey);
		_logger.LogDebug("PrepareAuthModel(model={Model}): setupInfo → {SetupInfo}", model, setupInfo);
		_logger.LogDebug("PrepareAuthModel(model={Model}): model.SecretKey → {Arg1}", model, model.SecretKey);
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("PrepareAuthModel(model={Model}): customer → {Customer}", model, customer);
        model.SecretKey = secretkey;
        model.Account = $"{_googleAuthenticatorSettings.BusinessPrefix} ({customer.Email})";
        model.ManualEntryQrCode = setupInfo.ManualEntryKey;
		_logger.LogDebug("PrepareAuthModel(model={Model}): model.ManualEntryQrCode → {Arg1}", model, model.ManualEntryQrCode);
		_logger.LogDebug("PrepareAuthModel(model={Model}): model.QrCodeImageUrl → {Arg1}", model, model.QrCodeImageUrl);
        model.QrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;

		_logger.LogDebug("<<< Exiting PrepareAuthModel(model={Model})", model);
        return model;
    }

    #endregion
}