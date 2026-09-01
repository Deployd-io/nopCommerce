using FluentValidation;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Validators;

/// <summary>
/// Represents an <see cref="TokenModel"/> validator.
/// </summary>
public class TokenModelValidator : BaseNopValidator<TokenModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TokenModelValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering TokenModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.Token).NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"));
        RuleFor(model => model.Token).Matches(@"^[0-9]{6,6}$")
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"));
			_logger.LogDebug("<<< Exiting TokenModelValidator(localizationService={LocalizationService})", localizationService);
    }
}