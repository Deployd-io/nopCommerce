using FluentValidation;
using Nop.Core.Domain.Forums;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Forums;

public partial class ForumGroupValidator : BaseNopValidator<ForumGroupModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ForumGroupValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ForumGroupValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Fields.Name.Required"));

        SetDatabaseValidationRules<ForumGroup>();
		_logger.LogWarning("<<< Exiting ForumGroupValidator(localizationService={LocalizationService})", localizationService);
    }
}