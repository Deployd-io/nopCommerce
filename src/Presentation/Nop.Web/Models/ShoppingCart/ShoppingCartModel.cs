using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart;

public partial record ShoppingCartModel : BaseNopModel
{
    public ShoppingCartModel()
    {
		logger.LogDebug(">>> Entering ShoppingCartModel()");
        Items = new List<ShoppingCartItemModel>();
        Warnings = new List<string>();
		logger.LogDebug("ShoppingCartModel(): Items → {Items}", Items);
		logger.LogDebug("ShoppingCartModel(): Warnings → {Warnings}", Warnings);
        DiscountBox = new DiscountBoxModel();
        GiftCardBox = new GiftCardBoxModel();
		logger.LogDebug("ShoppingCartModel(): DiscountBox → {DiscountBox}", DiscountBox);
		logger.LogDebug("ShoppingCartModel(): GiftCardBox → {GiftCardBox}", GiftCardBox);
        CheckoutAttributes = new List<CheckoutAttributeModel>();
        OrderReviewData = new OrderReviewDataModel();
		logger.LogDebug("ShoppingCartModel(): CheckoutAttributes → {CheckoutAttributes}", CheckoutAttributes);
		logger.LogDebug("ShoppingCartModel(): OrderReviewData → {OrderReviewData}", OrderReviewData);

        ButtonPaymentMethodViewComponents = new List<Type>();
		logger.LogDebug("<<< Exiting ShoppingCartModel()");
		logger.LogDebug("ShoppingCartModel(): ButtonPaymentMethodViewComponents → {ButtonPaymentMethodViewComponents}", ButtonPaymentMethodViewComponents);
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
			logger.LogDebug(">>> Entering ShoppingCartItemModel()");
            Picture = new PictureModel();
            AllowedQuantities = new List<SelectListItem>();
			logger.LogDebug("ShoppingCartItemModel(): Picture → {Picture}", Picture);
			logger.LogDebug("ShoppingCartItemModel(): AllowedQuantities → {AllowedQuantities}", AllowedQuantities);
            Warnings = new List<string>();
			logger.LogDebug("<<< Exiting ShoppingCartItemModel()");
			logger.LogDebug("ShoppingCartItemModel(): Warnings → {Warnings}", Warnings);
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
			logger.LogDebug(">>> Entering CheckoutAttributeModel()");
            AllowedFileExtensions = new List<string>();
            Values = new List<CheckoutAttributeValueModel>();
			logger.LogDebug("<<< Exiting CheckoutAttributeModel()");
			logger.LogDebug("CheckoutAttributeModel(): AllowedFileExtensions → {AllowedFileExtensions}", AllowedFileExtensions);
			logger.LogDebug("CheckoutAttributeModel(): Values → {Values}", Values);
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
			logger.LogDebug(">>> Entering DiscountBoxModel()");
            AppliedDiscountsWithCodes = new List<DiscountInfoModel>();
            Messages = new List<string>();
			logger.LogDebug("<<< Exiting DiscountBoxModel()");
			logger.LogDebug("DiscountBoxModel(): AppliedDiscountsWithCodes → {AppliedDiscountsWithCodes}", AppliedDiscountsWithCodes);
			logger.LogDebug("DiscountBoxModel(): Messages → {Messages}", Messages);
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
			logger.LogDebug(">>> Entering OrderReviewDataModel()");
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
			logger.LogDebug("OrderReviewDataModel(): BillingAddress → {BillingAddress}", BillingAddress);
			logger.LogDebug("OrderReviewDataModel(): ShippingAddress → {ShippingAddress}", ShippingAddress);
            PickupAddress = new AddressModel();
            CustomValues = new CustomValues();
			logger.LogDebug("<<< Exiting OrderReviewDataModel()");
			logger.LogDebug("OrderReviewDataModel(): PickupAddress → {PickupAddress}", PickupAddress);
			logger.LogDebug("OrderReviewDataModel(): CustomValues → {CustomValues}", CustomValues);
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