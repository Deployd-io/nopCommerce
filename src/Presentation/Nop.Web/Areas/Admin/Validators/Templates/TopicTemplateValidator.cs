using FluentValidation;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Templates;

public partial class TopicTemplateValidator : BaseNopValidator<TopicTemplateModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TopicTemplateValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering TopicTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Topic.Name.Required"));
        RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Topic.ViewPath.Required"));

        SetDatabaseValidationRules<TopicTemplate>();
		_logger.LogDebug("<<< Exiting TopicTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}