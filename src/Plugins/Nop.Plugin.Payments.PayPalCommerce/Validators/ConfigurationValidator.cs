using FluentValidation;
using Nop.Plugin.Payments.PayPalCommerce.Models.Admin;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Validators;

/// <summary>
/// Represents the configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ConfigurationValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.ClientId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.ClientId.Required"))
            .When(model => !model.UseSandbox && model.SetCredentialsManually);

        RuleFor(model => model.SecretKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.SecretKey.Required"))
            .When(model => !model.UseSandbox && model.SetCredentialsManually);

        RuleFor(model => model.MerchantId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.MerchantId.Required"))
            .When(model => !model.UseSandbox && model.SetCredentialsManually);
			_logger.LogDebug("<<< Exiting ConfigurationValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}