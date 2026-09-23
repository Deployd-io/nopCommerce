using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerRoleValidator : BaseNopValidator<CustomerRoleModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerRoleValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering CustomerRoleValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.Name.Required"));

        SetDatabaseValidationRules<CustomerRole>();
		_logger.LogWarning("<<< Exiting CustomerRoleValidator(localizationService={LocalizationService})", localizationService);
    }
}