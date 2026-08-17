using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record ProductDetailsModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductDetailsModel()
    {
		_logger.LogDebug(">>> Entering ProductDetailsModel()");
        DefaultPictureModel = new PictureModel();
        PictureModels = new List<PictureModel>();
		_logger.LogDebug("ProductDetailsModel(): PictureModels → {PictureModels}", PictureModels);
		_logger.LogDebug("ProductDetailsModel(): DefaultPictureModel → {DefaultPictureModel}", DefaultPictureModel);
        VideoModels = new List<VideoModel>();
		_logger.LogDebug("ProductDetailsModel(): VideoModels → {VideoModels}", VideoModels);
        GiftCard = new GiftCardModel();
		_logger.LogDebug("ProductDetailsModel(): GiftCard → {GiftCard}", GiftCard);
        ProductPrice = new ProductPriceModel();
		_logger.LogDebug("ProductDetailsModel(): ProductPrice → {ProductPrice}", ProductPrice);
        AddToCart = new AddToCartModel();
		_logger.LogDebug("ProductDetailsModel(): AddToCart → {AddToCart}", AddToCart);
        ProductAttributes = new List<ProductAttributeModel>();
		_logger.LogDebug("ProductDetailsModel(): ProductAttributes → {ProductAttributes}", ProductAttributes);
        AssociatedProducts = new List<ProductDetailsModel>();
		_logger.LogDebug("ProductDetailsModel(): AssociatedProducts → {AssociatedProducts}", AssociatedProducts);
        VendorModel = new VendorBriefInfoModel();
		_logger.LogDebug("ProductDetailsModel(): VendorModel → {VendorModel}", VendorModel);
        Breadcrumb = new ProductBreadcrumbModel();
		_logger.LogDebug("ProductDetailsModel(): Breadcrumb → {Breadcrumb}", Breadcrumb);
        ProductTags = new List<ProductTagModel>();
		_logger.LogDebug("ProductDetailsModel(): ProductTags → {ProductTags}", ProductTags);
        ProductSpecificationModel = new ProductSpecificationModel();
		_logger.LogDebug("ProductDetailsModel(): ProductSpecificationModel → {ProductSpecificationModel}", ProductSpecificationModel);
        ProductManufacturers = new List<ManufacturerBriefInfoModel>();
		_logger.LogDebug("ProductDetailsModel(): ProductManufacturers → {ProductManufacturers}", ProductManufacturers);
        ProductReviewOverview = new ProductReviewOverviewModel();
		_logger.LogDebug("ProductDetailsModel(): ProductReviewOverview → {ProductReviewOverview}", ProductReviewOverview);
        ProductReviews = new ProductReviewsModel();
		_logger.LogDebug("ProductDetailsModel(): ProductReviews → {ProductReviews}", ProductReviews);
        TierPrices = new List<TierPriceModel>();
		_logger.LogDebug("ProductDetailsModel(): TierPrices → {TierPrices}", TierPrices);
        ProductEstimateShipping = new ProductEstimateShippingModel();
		_logger.LogDebug("<<< Exiting ProductDetailsModel()");
		_logger.LogDebug("ProductDetailsModel(): ProductEstimateShipping → {ProductEstimateShipping}", ProductEstimateShipping);
    }

    //picture(s)
    public bool DefaultPictureZoomEnabled { get; set; }
    public PictureModel DefaultPictureModel { get; set; }
    public IList<PictureModel> PictureModels { get; set; }

    //videos
    public IList<VideoModel> VideoModels { get; set; }

    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
    public string JsonLd { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }
    public bool VisibleIndividually { get; set; }

    public ProductType ProductType { get; set; }

    public bool ShowSku { get; set; }
    public string Sku { get; set; }

    public bool ShowManufacturerPartNumber { get; set; }
    public string ManufacturerPartNumber { get; set; }

    public bool ShowGtin { get; set; }
    public string Gtin { get; set; }

    public bool ShowVendor { get; set; }
    public VendorBriefInfoModel VendorModel { get; set; }

    public bool HasSampleDownload { get; set; }

    public GiftCardModel GiftCard { get; set; }

    public bool IsShipEnabled { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool FreeShippingNotificationEnabled { get; set; }
    public string DeliveryDate { get; set; }

    public bool IsRental { get; set; }
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }

    public DateTime? AvailableEndDate { get; set; }

    public ManageInventoryMethod ManageInventoryMethod { get; set; }

    public string StockAvailability { get; set; }

    public bool DisplayBackInStockSubscription { get; set; }

    public bool DisplayAttributeCombinationImagesOnly { get; set; }

    public bool EmailAFriendEnabled { get; set; }
    public bool CompareProductsEnabled { get; set; }

    public string PageShareCode { get; set; }

    public ProductPriceModel ProductPrice { get; set; }

    public AddToCartModel AddToCart { get; set; }

    public ProductBreadcrumbModel Breadcrumb { get; set; }

    public IList<ProductTagModel> ProductTags { get; set; }

    public IList<ProductAttributeModel> ProductAttributes { get; set; }

    public ProductSpecificationModel ProductSpecificationModel { get; set; }

    public IList<ManufacturerBriefInfoModel> ProductManufacturers { get; set; }

    public ProductReviewOverviewModel ProductReviewOverview { get; set; }

    public ProductReviewsModel ProductReviews { get; set; }

    public ProductEstimateShippingModel ProductEstimateShipping { get; set; }

    public IList<TierPriceModel> TierPrices { get; set; }

    //a list of associated products. For example, "Grouped" products could have several child "simple" products
    public IList<ProductDetailsModel> AssociatedProducts { get; set; }

    public bool DisplayDiscontinuedMessage { get; set; }

    public string CurrentStoreName { get; set; }

    public bool InStock { get; set; }

    public bool AllowAddingOnlyExistingAttributeCombinations { get; set; }

    #region Nested Classes

    public partial record ProductBreadcrumbModel : BaseNopModel
    {
        public ProductBreadcrumbModel()
        {
			_logger.LogDebug(">>> Entering ProductBreadcrumbModel()");
            CategoryBreadcrumb = new List<CategorySimpleModel>();
			_logger.LogDebug("<<< Exiting ProductBreadcrumbModel()");
			_logger.LogDebug("ProductBreadcrumbModel(): CategoryBreadcrumb → {CategoryBreadcrumb}", CategoryBreadcrumb);
        }

        public bool Enabled { get; set; }
        public string JsonLd { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
    }

    public partial record AddToCartModel : BaseNopModel
    {
        public AddToCartModel()
        {
			_logger.LogDebug(">>> Entering AddToCartModel()");
            AllowedQuantities = new List<SelectListItem>();
            ProductToWishlist = new ProductToWishlistModel();
			_logger.LogDebug("<<< Exiting AddToCartModel()");
			_logger.LogDebug("AddToCartModel(): ProductToWishlist → {ProductToWishlist}", ProductToWishlist);
			_logger.LogDebug("AddToCartModel(): AllowedQuantities → {AllowedQuantities}", AllowedQuantities);
        }
        public int ProductId { get; set; }

        //qty
        [NopResourceDisplayName("Products.Qty")]
        public int EnteredQuantity { get; set; }
        public string MinimumQuantityNotification { get; set; }
        public List<SelectListItem> AllowedQuantities { get; set; }

        //price entered by customers
        [NopResourceDisplayName("Products.EnterProductPrice")]
        public bool CustomerEntersPrice { get; set; }
        [NopResourceDisplayName("Products.EnterProductPrice")]
        public decimal CustomerEnteredPrice { get; set; }
        public string CustomerEnteredPriceRange { get; set; }

        public bool DisableBuyButton { get; set; }
        public bool DisableWishlistButton { get; set; }

        //rental
        public bool IsRental { get; set; }

        //pre-order
        public bool AvailableForPreOrder { get; set; }
        public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }
        public string PreOrderAvailabilityStartDateTimeUserTime { get; set; }

        //updating existing shopping cart or wishlist item?
        public int UpdatedShoppingCartItemId { get; set; }
        public ShoppingCartType? UpdateShoppingCartItemType { get; set; }
        public ProductToWishlistModel ProductToWishlist { get; set; }
    }
    
    public partial record GiftCardModel : BaseNopModel
    {
        public bool IsGiftCard { get; set; }

        [NopResourceDisplayName("Products.GiftCard.RecipientName")]
        public string RecipientName { get; set; }

        [NopResourceDisplayName("Products.GiftCard.RecipientEmail")]
        [DataType(DataType.EmailAddress)]
        public string RecipientEmail { get; set; }

        [NopResourceDisplayName("Products.GiftCard.SenderName")]
        public string SenderName { get; set; }

        [NopResourceDisplayName("Products.GiftCard.SenderEmail")]
        [DataType(DataType.EmailAddress)]
        public string SenderEmail { get; set; }

        [NopResourceDisplayName("Products.GiftCard.Message")]
        public string Message { get; set; }

        public GiftCardType GiftCardType { get; set; }
    }

    public partial record TierPriceModel : BaseNopModel
    {
        public string Price { get; set; }
        public decimal PriceValue { get; set; }

        public int Quantity { get; set; }
    }

    public partial record ProductAttributeModel : BaseNopEntityModel
    {
        public ProductAttributeModel()
        {
			_logger.LogDebug(">>> Entering ProductAttributeModel()");
            AllowedFileExtensions = new List<string>();
            Values = new List<ProductAttributeValueModel>();
			_logger.LogDebug("<<< Exiting ProductAttributeModel()");
			_logger.LogDebug("ProductAttributeModel(): Values → {Values}", Values);
			_logger.LogDebug("ProductAttributeModel(): AllowedFileExtensions → {AllowedFileExtensions}", AllowedFileExtensions);
        }

        public int ProductId { get; set; }

        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string TextPrompt { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }
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
        /// A value indicating whether this attribute depends on some other attribute
        /// </summary>
        public bool HasCondition { get; set; }

        /// <summary>
        /// Allowed file extensions for customer uploaded files
        /// </summary>
        public IList<string> AllowedFileExtensions { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<ProductAttributeValueModel> Values { get; set; }
    }

    public partial record ProductAttributeValueModel : BaseNopEntityModel
    {
        public ProductAttributeValueModel()
        {
			_logger.LogDebug(">>> Entering ProductAttributeValueModel()");
            ImageSquaresPictureModel = new PictureModel();
			_logger.LogDebug("<<< Exiting ProductAttributeValueModel()");
			_logger.LogDebug("ProductAttributeValueModel(): ImageSquaresPictureModel → {ImageSquaresPictureModel}", ImageSquaresPictureModel);
        }

        public string Name { get; set; }

        public string ColorSquaresRgb { get; set; }

        //picture model is used with "image square" attribute type
        public PictureModel ImageSquaresPictureModel { get; set; }

        public string PriceAdjustment { get; set; }

        public bool PriceAdjustmentUsePercentage { get; set; }

        public decimal PriceAdjustmentValue { get; set; }

        public bool IsPreSelected { get; set; }

        //product picture ID (associated to this value)
        public int PictureId { get; set; }

        public bool CustomerEntersQty { get; set; }

        public int Quantity { get; set; }
    }

    public partial record ProductEstimateShippingModel : EstimateShippingModel
    {
        public int ProductId { get; set; }
    }

    #endregion
}