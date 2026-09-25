using FluentValidation;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Validators;

/// <summary>
/// Represents an <see cref="AuthModel"/> validator.
/// </summary>
public class AuthModelValidator : BaseNopValidator<AuthModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public AuthModelValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering AuthModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.Code).NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"));
        RuleFor(model => model.Code).Matches(@"^[0-9]{6,6}$")
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"));
			_logger.LogWarning("<<< Exiting AuthModelValidator(localizationService={LocalizationService})", localizationService);
    }
}