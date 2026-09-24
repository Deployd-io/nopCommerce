using FluentValidation;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Polls;

public partial class PollAnswerValidator : BaseNopValidator<PollAnswerModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PollAnswerValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering PollAnswerValidator(localizationService={LocalizationService})", localizationService);
        //if validation without this set rule is applied, in this case nothing will be validated
        //it's used to prevent auto-validation of child models
        RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
        {
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Answers.Fields.Name.Required"));

            SetDatabaseValidationRules<PollAnswer>();
        });
		_logger.LogWarning("<<< Exiting PollAnswerValidator(localizationService={LocalizationService})", localizationService);
    }
}