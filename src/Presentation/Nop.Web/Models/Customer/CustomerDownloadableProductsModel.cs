using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer;

public partial record CustomerDownloadableProductsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerDownloadableProductsModel()
    {
		_logger.LogDebug(">>> Entering CustomerDownloadableProductsModel()");
        Items = new List<DownloadableProductsModel>();
		_logger.LogDebug("CustomerDownloadableProductsModel(): Items → {Items}", Items);
    }

    public IList<DownloadableProductsModel> Items { get; set; }

    #region Nested classes

    public partial record DownloadableProductsModel : BaseNopModel
    {
        public Guid OrderItemGuid { get; set; }

        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public string ProductAttributes { get; set; }

        public int DownloadId { get; set; }
        public int LicenseId { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    #endregion
}

public partial record UserAgreementModel : BaseNopModel
{
    public Guid OrderItemGuid { get; set; }
    public string UserAgreementText { get; set; }
}