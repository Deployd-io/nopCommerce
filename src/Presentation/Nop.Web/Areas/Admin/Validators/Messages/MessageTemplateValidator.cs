using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class MessageTemplateValidator : BaseNopValidator<MessageTemplateModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public MessageTemplateValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering MessageTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
        RuleFor(x => x.Body).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));

        SetDatabaseValidationRules<MessageTemplate>();
		_logger.LogWarning("<<< Exiting MessageTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}