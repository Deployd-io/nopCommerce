using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.ShoppingCart;

public partial class WishlistEmailAFriendValidator : BaseNopValidator<WishlistEmailAFriendModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public WishlistEmailAFriendValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering WishlistEmailAFriendValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.FriendEmail).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Wishlist.EmailAFriend.FriendEmail.Required"));
        RuleFor(x => x.FriendEmail)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));

        RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Wishlist.EmailAFriend.YourEmailAddress.Required"));
        RuleFor(x => x.YourEmailAddress)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
			_logger.LogDebug("<<< Exiting WishlistEmailAFriendValidator(localizationService={LocalizationService})", localizationService);
    }
}