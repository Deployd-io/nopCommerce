using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerRoleValidator : BaseNopValidator<CustomerRoleModel>
{
    public CustomerRoleValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering CustomerRoleValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.Name.Required"));

        SetDatabaseValidationRules<CustomerRole>();
		logger.LogDebug("<<< Exiting CustomerRoleValidator(localizationService={LocalizationService})", localizationService);
    }
}