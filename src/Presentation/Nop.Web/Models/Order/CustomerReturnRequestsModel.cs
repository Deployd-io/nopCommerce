using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order;

public partial record CustomerReturnRequestsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerReturnRequestsModel()
    {
		_logger.LogDebug(">>> Entering CustomerReturnRequestsModel()");
        Items = new List<ReturnRequestModel>();
		_logger.LogDebug("CustomerReturnRequestsModel(): Items → {Items}", Items);
    }

    public IList<ReturnRequestModel> Items { get; set; }

    #region Nested classes

    public partial record ReturnRequestModel : BaseNopEntityModel
    {
        public string CustomNumber { get; set; }
        public string ReturnRequestStatus { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public int Quantity { get; set; }

        public string ReturnReason { get; set; }
        public string ReturnAction { get; set; }
        public string Comments { get; set; }
        public Guid UploadedFileGuid { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    #endregion
}