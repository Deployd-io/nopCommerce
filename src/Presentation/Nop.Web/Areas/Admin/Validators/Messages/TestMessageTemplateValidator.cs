using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class TestMessageTemplateValidator : BaseNopValidator<TestMessageTemplateModel>
{
    public TestMessageTemplateValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering TestMessageTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.SendTo).NotEmpty();
        RuleFor(x => x.SendTo)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
			logger.LogDebug("<<< Exiting TestMessageTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}