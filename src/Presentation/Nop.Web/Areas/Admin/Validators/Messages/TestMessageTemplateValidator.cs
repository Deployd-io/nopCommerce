using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class TestMessageTemplateValidator : BaseNopValidator<TestMessageTemplateModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TestMessageTemplateValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering TestMessageTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.SendTo).NotEmpty();
        RuleFor(x => x.SendTo)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
			_logger.LogDebug("<<< Exiting TestMessageTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}