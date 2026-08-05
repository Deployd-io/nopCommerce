using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Plugins;

public partial class PluginValidator : BaseNopValidator<PluginModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PluginValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering PluginValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.FriendlyName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
		_logger.LogDebug("<<< Exiting PluginValidator(localizationService={LocalizationService})", localizationService);
    }
}