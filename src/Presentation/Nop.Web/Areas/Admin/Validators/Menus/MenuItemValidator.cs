using FluentValidation;
using Nop.Core.Domain.Menus;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Menus;

public partial class MenuItemValidator : BaseNopValidator<MenuItemModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MenuItemValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering MenuItemValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.RouteName)
            .NotEmpty()
            .When(x => x.MenuItemTypeId == (int)MenuItemType.StandardPage)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.RouteName.Required"));

        RuleFor(x => x.ProductId)
            .NotNull()
            .When(x => x.MenuItemTypeId == (int)MenuItemType.Product)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Product.Required"));

        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .When(x => x.MenuItemTypeId == (int)MenuItemType.TopicPage)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Topic.Required"));

        RuleFor(x => x.Title)
            .NotEmpty()
            .When(x =>
                x.MenuItemTypeId == (int)MenuItemType.StandardPage ||
                x.MenuItemTypeId == (int)MenuItemType.CustomLink ||
                x.MenuItemTypeId == (int)MenuItemType.Text ||
                (x.MenuItemTypeId == (int)MenuItemType.Category && x.CategoryId == 0) ||
                (x.MenuItemTypeId == (int)MenuItemType.Manufacturer && x.ManufacturerId == 0) ||
                (x.MenuItemTypeId == (int)MenuItemType.Vendor && x.VendorId == 0))
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Title.Required"));

        RuleFor(x => x.Url)
            .NotEmpty()
            .When(x => x.MenuItemTypeId == (int)MenuItemType.CustomLink)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Url.Required"));

        SetDatabaseValidationRules<MenuItem>();
		_logger.LogWarning("<<< Exiting MenuItemValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}
