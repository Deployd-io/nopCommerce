using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer;

public partial record CustomerNavigationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerNavigationModel()
    {
		_logger.LogError(">>> Entering CustomerNavigationModel()");
        CustomerNavigationItems = new List<CustomerNavigationItemModel>();
		_logger.LogDebug("CustomerNavigationModel(): CustomerNavigationItems → {CustomerNavigationItems}", CustomerNavigationItems);
    _logger.LogError("CustomerNavigationModel(): CustomerNavigationItems → {CustomerNavigationItems}", CustomerNavigationItems);
    _logger.LogWarning("<<< Exiting CustomerNavigationModel()");
    }

    public IList<CustomerNavigationItemModel> CustomerNavigationItems { get; set; }

    public int SelectedTab { get; set; }
}

public partial record CustomerNavigationItemModel : BaseNopModel
{
    public string RouteName { get; set; }
    public string Title { get; set; }
    public int Tab { get; set; }
    public string ItemClass { get; set; }
}

public enum CustomerNavigationEnum
{
    Info = 0,
    Addresses = 10,
    Orders = 20,
    BackInStockSubscriptions = 30,
    ReturnRequests = 40,
    DownloadableProducts = 50,
    RewardPoints = 60,
    ChangePassword = 70,
    Avatar = 80,
    ForumSubscriptions = 90,
    ProductReviews = 100,
    VendorInfo = 110,
    GdprTools = 120,
    CheckGiftCardBalance = 130,
    MultiFactorAuthentication = 140,
    RecurringPayments = 150,
}