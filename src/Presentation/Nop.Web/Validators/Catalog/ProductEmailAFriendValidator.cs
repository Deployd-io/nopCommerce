using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Catalog;

public partial class ProductEmailAFriendValidator : BaseNopValidator<ProductEmailAFriendModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductEmailAFriendValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ProductEmailAFriendValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.FriendEmail).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Products.EmailAFriend.FriendEmail.Required"));
        RuleFor(x => x.FriendEmail)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));

        RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Products.EmailAFriend.YourEmailAddress.Required"));
        RuleFor(x => x.YourEmailAddress)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
			_logger.LogDebug("<<< Exiting ProductEmailAFriendValidator(localizationService={LocalizationService})", localizationService);
    }
}