using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.ShoppingCart;

public partial record ShoppingCartModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ShoppingCartModel()
    {
		_logger.LogDebug(">>> Entering ShoppingCartModel()");
        Items = new List<ShoppingCartItemModel>();
        Warnings = new List<string>();
        DiscountBox = new DiscountBoxModel();
		_logger.LogDebug("ShoppingCartModel(): DiscountBox → {DiscountBox}", DiscountBox);
		_logger.LogDebug("ShoppingCartModel(): Items → {Items}", Items);
		_logger.LogDebug("ShoppingCartModel(): Warnings → {Warnings}", Warnings);
        GiftCardBox = new GiftCardBoxModel();
		_logger.LogDebug("ShoppingCartModel(): GiftCardBox → {GiftCardBox}", GiftCardBox);
        CheckoutAttributes = new List<CheckoutAttributeModel>();
		_logger.LogDebug("ShoppingCartModel(): CheckoutAttributes → {CheckoutAttributes}", CheckoutAttributes);
        OrderReviewData = new OrderReviewDataModel();
		_logger.LogDebug("ShoppingCartModel(): OrderReviewData → {OrderReviewData}", OrderReviewData);

        ButtonPaymentMethodViewComponents = new List<Type>();
		_logger.LogDebug("ShoppingCartModel(): ButtonPaymentMethodViewComponents → {ButtonPaymentMethodViewComponents}", ButtonPaymentMethodViewComponents);
		_logger.LogDebug("<<< Exiting ShoppingCartModel()");
    }

    public bool OnePageCheckoutEnabled { get; set; }

    public bool ShowSku { get; set; }
    public bool ShowProductImages { get; set; }
    public bool IsEditable { get; set; }
    public bool IsReadyToCheckout { get; set; }
    public IList<ShoppingCartItemModel> Items { get; set; }

    public IList<CheckoutAttributeModel> CheckoutAttributes { get; set; }

    public IList<string> Warnings { get; set; }
    public string MinOrderSubtotalWarning { get; set; }
    public bool DisplayTaxShippingInfo { get; set; }
    public bool TermsOfServiceOnShoppingCartPage { get; set; }
    public bool TermsOfServiceOnOrderConfirmPage { get; set; }
    public bool TermsOfServicePopup { get; set; }
    public DiscountBoxModel DiscountBox { get; set; }
    public GiftCardBoxModel GiftCardBox { get; set; }
    public OrderReviewDataModel OrderReviewData { get; set; }

    public IList<Type> ButtonPaymentMethodViewComponents { get; set; }

    public bool HideCheckoutButton { get; set; }
    public bool ShowVendorName { get; set; }

    #region Nested Classes

    public partial record ShoppingCartItemModel : BaseNopEntityModel
    {
        public ShoppingCartItemModel()
        {
			_logger.LogDebug(">>> Entering ShoppingCartItemModel()");
            Picture = new PictureModel();
            AllowedQuantities = new List<SelectListItem>();
			_logger.LogDebug("ShoppingCartItemModel(): AllowedQuantities → {AllowedQuantities}", AllowedQuantities);
			_logger.LogDebug("ShoppingCartItemModel(): Picture → {Picture}", Picture);
            Warnings = new List<string>();
			_logger.LogDebug("ShoppingCartItemModel(): Warnings → {Warnings}", Warnings);
			_logger.LogDebug("<<< Exiting ShoppingCartItemModel()");
        }

        public string Sku { get; set; }

        public string VendorName { get; set; }

        public PictureModel Picture { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public string UnitPrice { get; set; }
        public decimal UnitPriceValue { get; set; }

        public string SubTotal { get; set; }
        public decimal SubTotalValue { get; set; }

        public string Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public int? MaximumDiscountedQty { get; set; }

        public int Quantity { get; set; }
        public List<SelectListItem> AllowedQuantities { get; set; }

        public string AttributeInfo { get; set; }

        public string RecurringInfo { get; set; }

        public string RentalInfo { get; set; }

        public bool AllowItemEditing { get; set; }

        public bool DisableRemoval { get; set; }

        public IList<string> Warnings { get; set; }
    }

    public partial record CheckoutAttributeModel : BaseNopEntityModel
    {
        public CheckoutAttributeModel()
        {
			_logger.LogDebug(">>> Entering CheckoutAttributeModel()");
            AllowedFileExtensions = new List<string>();
            Values = new List<CheckoutAttributeValueModel>();
			_logger.LogDebug("CheckoutAttributeModel(): Values → {Values}", Values);
			_logger.LogDebug("CheckoutAttributeModel(): AllowedFileExtensions → {AllowedFileExtensions}", AllowedFileExtensions);
			_logger.LogDebug("<<< Exiting CheckoutAttributeModel()");
        }

        public string Name { get; set; }

        public string DefaultValue { get; set; }

        public string TextPrompt { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Selected day value for datepicker
        /// </summary>
        public int? SelectedDay { get; set; }
        /// <summary>
        /// Selected month value for datepicker
        /// </summary>
        public int? SelectedMonth { get; set; }
        /// <summary>
        /// Selected year value for datepicker
        /// </summary>
        public int? SelectedYear { get; set; }

        /// <summary>
        /// Allowed file extensions for customer uploaded files
        /// </summary>
        public IList<string> AllowedFileExtensions { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<CheckoutAttributeValueModel> Values { get; set; }
    }

    public partial record CheckoutAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string ColorSquaresRgb { get; set; }

        public string PriceAdjustment { get; set; }

        public bool IsPreSelected { get; set; }
    }

    public partial record DiscountBoxModel : BaseNopModel
    {
        public DiscountBoxModel()
        {
			_logger.LogDebug(">>> Entering DiscountBoxModel()");
            AppliedDiscountsWithCodes = new List<DiscountInfoModel>();
            Messages = new List<string>();
			_logger.LogDebug("DiscountBoxModel(): Messages → {Messages}", Messages);
			_logger.LogDebug("DiscountBoxModel(): AppliedDiscountsWithCodes → {AppliedDiscountsWithCodes}", AppliedDiscountsWithCodes);
			_logger.LogDebug("<<< Exiting DiscountBoxModel()");
        }

        public List<DiscountInfoModel> AppliedDiscountsWithCodes { get; set; }
        public bool Display { get; set; }
        public List<string> Messages { get; set; }
        public bool IsApplied { get; set; }

        public partial record DiscountInfoModel : BaseNopEntityModel
        {
            public string CouponCode { get; set; }
        }
    }

    public partial record GiftCardBoxModel : BaseNopModel
    {
        public bool Display { get; set; }
        public string Message { get; set; }
        public bool IsApplied { get; set; }
    }

    public partial record OrderReviewDataModel : BaseNopModel
    {
        public OrderReviewDataModel()
        {
			_logger.LogDebug(">>> Entering OrderReviewDataModel()");
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
			_logger.LogDebug("OrderReviewDataModel(): ShippingAddress → {ShippingAddress}", ShippingAddress);
			_logger.LogDebug("OrderReviewDataModel(): BillingAddress → {BillingAddress}", BillingAddress);
            PickupAddress = new AddressModel();
			_logger.LogDebug("OrderReviewDataModel(): PickupAddress → {PickupAddress}", PickupAddress);
            CustomValues = new CustomValues();
			_logger.LogDebug("OrderReviewDataModel(): CustomValues → {CustomValues}", CustomValues);
			_logger.LogDebug("<<< Exiting OrderReviewDataModel()");
        }
        public bool Display { get; set; }

        public AddressModel BillingAddress { get; set; }

        public bool IsShippable { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public bool SelectedPickupInStore { get; set; }
        public AddressModel PickupAddress { get; set; }
        public string ShippingMethod { get; set; }

        public string PaymentMethod { get; set; }

        public CustomValues CustomValues { get; set; }
    }

    #endregion
}