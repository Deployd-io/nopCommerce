using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Order;

public partial record SubmitReturnRequestModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SubmitReturnRequestModel()
    {
		_logger.LogDebug(">>> Entering SubmitReturnRequestModel()");
        Items = new List<OrderItemModel>();
		_logger.LogDebug("SubmitReturnRequestModel(): Items → {Items}", Items);
        AvailableReturnReasons = new List<ReturnRequestReasonModel>();
		_logger.LogDebug("SubmitReturnRequestModel(): AvailableReturnReasons → {AvailableReturnReasons}", AvailableReturnReasons);
        AvailableReturnActions = new List<ReturnRequestActionModel>();
    }

    public int OrderId { get; set; }
    public string CustomOrderNumber { get; set; }

    public IList<OrderItemModel> Items { get; set; }

    [NopResourceDisplayName("ReturnRequests.ReturnReason")]
    public int ReturnRequestReasonId { get; set; }
    public IList<ReturnRequestReasonModel> AvailableReturnReasons { get; set; }

    [NopResourceDisplayName("ReturnRequests.ReturnAction")]
    public int ReturnRequestActionId { get; set; }
    public IList<ReturnRequestActionModel> AvailableReturnActions { get; set; }

    [NopResourceDisplayName("ReturnRequests.Comments")]
    public string Comments { get; set; }

    public bool AllowFiles { get; set; }
    [NopResourceDisplayName("ReturnRequests.UploadedFile")]
    public Guid UploadedFileGuid { get; set; }

    public string Result { get; set; }

    #region Nested classes

    public partial record OrderItemModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public string AttributeInfo { get; set; }

        public string UnitPrice { get; set; }

        public int Quantity { get; set; }
    }

    public partial record ReturnRequestReasonModel : BaseNopEntityModel
    {
        public string Name { get; set; }
    }

    public partial record ReturnRequestActionModel : BaseNopEntityModel
    {
        public string Name { get; set; }
    }

    #endregion
}