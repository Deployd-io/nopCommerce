using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Copy Product service
/// </summary>
public partial class CopyProductService : ICopyProductService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICategoryService _categoryService;
    protected readonly IDownloadService _downloadService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVideoService _videoService;

    #endregion

    #region Ctor

    public CopyProductService(IAclService aclService,
        ICategoryService categoryService,
        IDownloadService downloadService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IManufacturerService manufacturerService,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IProductTagService productTagService,
        ISpecificationAttributeService specificationAttributeService,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IVideoService videoService)
    {
		_logger.LogDebug(">>> Entering CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService})", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService);
        _aclService = aclService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _aclService → {_aclService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _aclService);
        _categoryService = categoryService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _categoryService → {_categoryService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _categoryService);
        _downloadService = downloadService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _downloadService → {_downloadService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _downloadService);
        _languageService = languageService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _languageService → {_languageService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _localizationService → {_localizationService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _localizedEntityService → {_localizedEntityService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _localizedEntityService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _manufacturerService → {_manufacturerService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _manufacturerService);
        _pictureService = pictureService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _pictureService → {_pictureService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _pictureService);
        _productAttributeParser = productAttributeParser;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _productAttributeParser → {_productAttributeParser}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _productAttributeParser);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _productAttributeService → {_productAttributeService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _productAttributeService);
        _productService = productService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _productService → {_productService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _productService);
        _productTagService = productTagService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _productTagService → {_productTagService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _productTagService);
        _specificationAttributeService = specificationAttributeService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _specificationAttributeService → {_specificationAttributeService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _specificationAttributeService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _storeMappingService → {_storeMappingService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _storeMappingService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _urlRecordService → {_urlRecordService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _urlRecordService);
        _videoService = videoService;
		_logger.LogDebug("<<< Exiting CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService})", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService);
		_logger.LogDebug("CopyProductService(aclService={AclService},categoryService={CategoryService},downloadService={DownloadService},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerService={ManufacturerService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},productTagService={ProductTagService},specificationAttributeService={SpecificationAttributeService},storeMappingService=<redacted>,urlRecordService={UrlRecordService},videoService={VideoService}): _videoService → {_videoService}", aclService, categoryService, downloadService, languageService, localizationService, localizedEntityService, manufacturerService, pictureService, productAttributeParser, productAttributeService, productService, productTagService, specificationAttributeService, urlRecordService, videoService, _videoService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Copy discount mappings
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyDiscountsMappingAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyDiscountsMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var discountMapping in await _productService.GetAllDiscountsAppliedToProductAsync(product.Id))
        {
            await _productService.InsertDiscountProductMappingAsync(new DiscountProductMapping { EntityId = productCopy.Id, DiscountId = discountMapping.DiscountId });
        }
		_logger.LogDebug("<<< Exiting CopyDiscountsMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy associated products
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="isPublished">A value indicating whether they should be published</param>
    /// <param name="copyMultimedia">A value indicating whether to copy images and videos</param>
    /// <param name="copyAssociatedProducts">A value indicating whether to copy associated products</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyAssociatedProductsAsync(Product product, bool isPublished, bool copyMultimedia, bool copyAssociatedProducts, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyAssociatedProductsAsync(product={Product},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts},productCopy={ProductCopy})", product, isPublished, copyMultimedia, copyAssociatedProducts, productCopy);
        if (!copyAssociatedProducts)
            return;

        var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id, showHidden: true);
		_logger.LogDebug("CopyAssociatedProductsAsync(product={Product},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts},productCopy={ProductCopy}): associatedProducts → {AssociatedProducts}", product, isPublished, copyMultimedia, copyAssociatedProducts, productCopy, associatedProducts);
        foreach (var associatedProduct in associatedProducts)
        {
            var associatedProductCopy = await CopyProductAsync(associatedProduct,
                string.Format(NopCatalogDefaults.ProductCopyNameTemplate, associatedProduct.Name),
                isPublished, copyMultimedia, false);
            associatedProductCopy.ParentGroupedProductId = productCopy.Id;
			_logger.LogDebug("CopyAssociatedProductsAsync(product={Product},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts},productCopy={ProductCopy}): associatedProductCopy.ParentGroupedProductId → {Arg5}", product, isPublished, copyMultimedia, copyAssociatedProducts, productCopy, associatedProductCopy.ParentGroupedProductId);
            await _productService.UpdateProductAsync(associatedProductCopy);
        }
		_logger.LogDebug("<<< Exiting CopyAssociatedProductsAsync(product={Product},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts},productCopy={ProductCopy})", product, isPublished, copyMultimedia, copyAssociatedProducts, productCopy);
    }

    /// <summary>
    /// Copy tier prices
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyTierPricesAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyTierPricesAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var tierPrice in await _productService.GetTierPricesByProductAsync(product.Id))
            await _productService.InsertTierPriceAsync(new TierPrice
            {
                ProductId = productCopy.Id,
                StoreId = tierPrice.StoreId,
                CustomerRoleId = tierPrice.CustomerRoleId,
                Quantity = tierPrice.Quantity,
                Price = tierPrice.Price,
                StartDateTimeUtc = tierPrice.StartDateTimeUtc,
                EndDateTimeUtc = tierPrice.EndDateTimeUtc
            });
			_logger.LogDebug("<<< Exiting CopyTierPricesAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
			_logger.LogDebug(">>> Entering CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts})", product, newName, isPublished, copyMultimedia, copyAssociatedProducts);
			_logger.LogWarning("Throwing ArgumentException from CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts})", product, newName, isPublished, copyMultimedia, copyAssociatedProducts);
			_logger.LogDebug("<<< Exiting CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts})", product, newName, isPublished, copyMultimedia, copyAssociatedProducts);
    }

    /// <summary>
    /// Copy attributes mapping
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <param name="originalNewPictureIdentifiers">Identifiers of pictures</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyAttributesMappingAsync(Product product, Product productCopy, Dictionary<int, int> originalNewPictureIdentifiers)
    {
		_logger.LogDebug(">>> Entering CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers})", product, productCopy, originalNewPictureIdentifiers);
        var associatedAttributes = new Dictionary<int, int>();
        var associatedAttributeValues = new Dictionary<int, int>();
		_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): associatedAttributeValues → {AssociatedAttributeValues}", product, productCopy, originalNewPictureIdentifiers, associatedAttributeValues);
		_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): associatedAttributes → {AssociatedAttributes}", product, productCopy, originalNewPictureIdentifiers, associatedAttributes);

        //attribute mapping with condition attributes
        var oldCopyWithConditionAttributes = new List<ProductAttributeMapping>();
		_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): oldCopyWithConditionAttributes → {OldCopyWithConditionAttributes}", product, productCopy, originalNewPictureIdentifiers, oldCopyWithConditionAttributes);

        //all product attribute mapping copies
        var productAttributeMappingCopies = new Dictionary<int, ProductAttributeMapping>();
		_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): productAttributeMappingCopies → {ProductAttributeMappingCopies}", product, productCopy, originalNewPictureIdentifiers, productAttributeMappingCopies);

        var languages = await _languageService.GetAllLanguagesAsync(true);
		_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): languages → {Languages}", product, productCopy, originalNewPictureIdentifiers, languages);

        foreach (var productAttributeMapping in await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
        {
            var productAttributeMappingCopy = new ProductAttributeMapping
            {
                ProductId = productCopy.Id,
                ProductAttributeId = productAttributeMapping.ProductAttributeId,
                TextPrompt = productAttributeMapping.TextPrompt,
                IsRequired = productAttributeMapping.IsRequired,
                AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                DisplayOrder = productAttributeMapping.DisplayOrder,
                ValidationMinLength = productAttributeMapping.ValidationMinLength,
                ValidationMaxLength = productAttributeMapping.ValidationMaxLength,
                ValidationFileAllowedExtensions = productAttributeMapping.ValidationFileAllowedExtensions,
                ValidationFileMaximumSize = productAttributeMapping.ValidationFileMaximumSize,
                DefaultValue = productAttributeMapping.DefaultValue
            };
            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMappingCopy);
            //localization
            foreach (var lang in languages)
            {
                var textPrompt = await _localizationService.GetLocalizedAsync(productAttributeMapping, x => x.TextPrompt, lang.Id, false, false);
				_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): textPrompt → {TextPrompt}", product, productCopy, originalNewPictureIdentifiers, textPrompt);
                if (!string.IsNullOrEmpty(textPrompt))
                    await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMappingCopy, x => x.TextPrompt, textPrompt,
                        lang.Id);
            }

            productAttributeMappingCopies.Add(productAttributeMappingCopy.Id, productAttributeMappingCopy);

            if (!string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml))
            {
                oldCopyWithConditionAttributes.Add(productAttributeMapping);
            }

            //save associated value (used for combinations copying)
            associatedAttributes.Add(productAttributeMapping.Id, productAttributeMappingCopy.Id);

            // product attribute values
            var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id);
			_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): productAttributeValues → {ProductAttributeValues}", product, productCopy, originalNewPictureIdentifiers, productAttributeValues);
            foreach (var productAttributeValue in productAttributeValues)
            {
                var attributeValueCopy = new ProductAttributeValue
                {
                    ProductAttributeMappingId = productAttributeMappingCopy.Id,
                    AttributeValueTypeId = productAttributeValue.AttributeValueTypeId,
                    AssociatedProductId = productAttributeValue.AssociatedProductId,
                    Name = productAttributeValue.Name,
                    ColorSquaresRgb = productAttributeValue.ColorSquaresRgb,
                    PriceAdjustment = productAttributeValue.PriceAdjustment,
                    PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage,
                    WeightAdjustment = productAttributeValue.WeightAdjustment,
                    Cost = productAttributeValue.Cost,
                    CustomerEntersQty = productAttributeValue.CustomerEntersQty,
                    Quantity = productAttributeValue.Quantity,
                    IsPreSelected = productAttributeValue.IsPreSelected,
                    DisplayOrder = productAttributeValue.DisplayOrder
                };

                //picture
                var oldValuePictures = await _productAttributeService.GetProductAttributeValuePicturesAsync(productAttributeValue.Id);
				_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): oldValuePictures → {OldValuePictures}", product, productCopy, originalNewPictureIdentifiers, oldValuePictures);
                foreach (var oldValuePicture in oldValuePictures)
                {
                    if (!originalNewPictureIdentifiers.TryGetValue(oldValuePicture.PictureId, out var valuePictureId))
                        continue;

                    await _productAttributeService.InsertProductAttributeValuePictureAsync(new ProductAttributeValuePicture
                    {
                        ProductAttributeValueId = attributeValueCopy.Id,
                        PictureId = valuePictureId
                    });
                }

                //picture associated to "iamge square" attribute type (if exists)
                if (productAttributeValue.ImageSquaresPictureId > 0)
                {
                    var origImageSquaresPicture =
                        await _pictureService.GetPictureByIdAsync(productAttributeValue.ImageSquaresPictureId);
                    if (origImageSquaresPicture != null)
                    {
                        //copy the picture
                        var imageSquaresPictureCopy = await _pictureService.InsertPictureAsync(
                            await _pictureService.LoadPictureBinaryAsync(origImageSquaresPicture),
                            origImageSquaresPicture.MimeType,
                            origImageSquaresPicture.SeoFilename,
                            origImageSquaresPicture.AltAttribute,
                            origImageSquaresPicture.TitleAttribute);
                        attributeValueCopy.ImageSquaresPictureId = imageSquaresPictureCopy.Id;
						_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): attributeValueCopy.ImageSquaresPictureId → {Arg3}", product, productCopy, originalNewPictureIdentifiers, attributeValueCopy.ImageSquaresPictureId);
                    }
                }

                await _productAttributeService.InsertProductAttributeValueAsync(attributeValueCopy);

                //save associated value (used for combinations copying)
                associatedAttributeValues.Add(productAttributeValue.Id, attributeValueCopy.Id);

                //localization
                foreach (var lang in languages)
                {
                    var name = await _localizationService.GetLocalizedAsync(productAttributeValue, x => x.Name, lang.Id, false, false);
					_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): name → {Name}", product, productCopy, originalNewPictureIdentifiers, name);
                    if (!string.IsNullOrEmpty(name))
                        await _localizedEntityService.SaveLocalizedValueAsync(attributeValueCopy, x => x.Name, name, lang.Id);
                }
            }
        }

        //copy attribute conditions
        foreach (var productAttributeMapping in oldCopyWithConditionAttributes)
        {
            var oldConditionAttributeMapping = (await _productAttributeParser
                .ParseProductAttributeMappingsAsync(productAttributeMapping.ConditionAttributeXml)).FirstOrDefault();

            if (oldConditionAttributeMapping == null)
                continue;

            var oldConditionValues = await _productAttributeParser.ParseProductAttributeValuesAsync(
                productAttributeMapping.ConditionAttributeXml,
                oldConditionAttributeMapping.Id);

            if (!oldConditionValues.Any())
                continue;

            var newAttributeMappingId = associatedAttributes[oldConditionAttributeMapping.Id];
            var newConditionAttributeMapping = productAttributeMappingCopies[newAttributeMappingId];

            var newConditionAttributeXml = string.Empty;

            foreach (var oldConditionValue in oldConditionValues)
            {
                newConditionAttributeXml = _productAttributeParser.AddProductAttribute(newConditionAttributeXml,
                    newConditionAttributeMapping, associatedAttributeValues[oldConditionValue.Id].ToString());
            }

            var attributeMappingId = associatedAttributes[productAttributeMapping.Id];
            var conditionAttribute = productAttributeMappingCopies[attributeMappingId];
            conditionAttribute.ConditionAttributeXml = newConditionAttributeXml;
			_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): conditionAttribute.ConditionAttributeXml → {Arg3}", product, productCopy, originalNewPictureIdentifiers, conditionAttribute.ConditionAttributeXml);

            await _productAttributeService.UpdateProductAttributeMappingAsync(conditionAttribute);
        }

        //attribute combinations
        foreach (var combination in await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
        {
            //generate new AttributesXml according to new value IDs
            var newAttributesXml = string.Empty;
            var parsedProductAttributes = await _productAttributeParser.ParseProductAttributeMappingsAsync(combination.AttributesXml);
			_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): parsedProductAttributes → {ParsedProductAttributes}", product, productCopy, originalNewPictureIdentifiers, parsedProductAttributes);
            foreach (var oldAttribute in parsedProductAttributes)
            {
                if (!associatedAttributes.ContainsKey(oldAttribute.Id))
                    continue;

                var newAttribute = await _productAttributeService.GetProductAttributeMappingByIdAsync(associatedAttributes[oldAttribute.Id]);
				_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): newAttribute → {NewAttribute}", product, productCopy, originalNewPictureIdentifiers, newAttribute);

                if (newAttribute == null)
                    continue;

                var oldAttributeValuesStr = _productAttributeParser.ParseValues(combination.AttributesXml, oldAttribute.Id);
				_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): oldAttributeValuesStr → {OldAttributeValuesStr}", product, productCopy, originalNewPictureIdentifiers, oldAttributeValuesStr);

                foreach (var oldAttributeValueStr in oldAttributeValuesStr)
                {
                    if (newAttribute.ShouldHaveValues())
                    {
                        //attribute values
                        var oldAttributeValue = int.Parse(oldAttributeValueStr);
						_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): oldAttributeValue → {OldAttributeValue}", product, productCopy, originalNewPictureIdentifiers, oldAttributeValue);
                        if (!associatedAttributeValues.ContainsKey(oldAttributeValue))
                            continue;

                        var newAttributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(associatedAttributeValues[oldAttributeValue]);
						_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): newAttributeValue → {NewAttributeValue}", product, productCopy, originalNewPictureIdentifiers, newAttributeValue);

                        if (newAttributeValue != null)
                        {
                            newAttributesXml = _productAttributeParser.AddProductAttribute(newAttributesXml,
                                newAttribute, newAttributeValue.Id.ToString());
                        }
                    }
                    else
                    {
                        //just a text
                        newAttributesXml = _productAttributeParser.AddProductAttribute(newAttributesXml,
                            newAttribute, oldAttributeValueStr);
                    }
                }
            }

            var combinationCopy = new ProductAttributeCombination
            {
                ProductId = productCopy.Id,
                AttributesXml = newAttributesXml,
                StockQuantity = combination.StockQuantity,
                MinStockQuantity = combination.MinStockQuantity,
                AllowOutOfStockOrders = combination.AllowOutOfStockOrders,
                Sku = combination.Sku,
                ManufacturerPartNumber = combination.ManufacturerPartNumber,
                Gtin = combination.Gtin,
                OverriddenPrice = combination.OverriddenPrice,
                NotifyAdminForQuantityBelow = combination.NotifyAdminForQuantityBelow
            };
            await _productAttributeService.InsertProductAttributeCombinationAsync(combinationCopy);

            //picture
            var oldCombinationPictures = await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id);
			_logger.LogDebug("CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers}): oldCombinationPictures → {OldCombinationPictures}", product, productCopy, originalNewPictureIdentifiers, oldCombinationPictures);
            foreach (var oldCombinationPicture in oldCombinationPictures)
            {
                if (!originalNewPictureIdentifiers.TryGetValue(oldCombinationPicture.PictureId, out var combinationPictureId))
                    continue;

                await _productAttributeService.InsertProductAttributeCombinationPictureAsync(new ProductAttributeCombinationPicture
                {
                    ProductAttributeCombinationId = combinationCopy.Id,
                    PictureId = combinationPictureId
                });
            }

            //quantity change history
            await _productService.AddStockQuantityHistoryEntryAsync(productCopy, combinationCopy.StockQuantity,
                combinationCopy.StockQuantity,
                message: string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyProduct"), product.Id), combinationId: combinationCopy.Id);
        }
		_logger.LogDebug("<<< Exiting CopyAttributesMappingAsync(product={Product},productCopy={ProductCopy},originalNewPictureIdentifiers={OriginalNewPictureIdentifiers})", product, productCopy, originalNewPictureIdentifiers);
    }

    /// <summary>
    /// Copy product specifications
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyProductSpecificationsAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyProductSpecificationsAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        var allLanguages = await _languageService.GetAllLanguagesAsync();

		_logger.LogDebug("CopyProductSpecificationsAsync(product={Product},productCopy={ProductCopy}): allLanguages → {AllLanguages}", product, productCopy, allLanguages);
        foreach (var productSpecificationAttribute in await _specificationAttributeService.GetProductSpecificationAttributesAsync(product.Id))
        {
            var psaCopy = new ProductSpecificationAttribute
            {
                ProductId = productCopy.Id,
                AttributeTypeId = productSpecificationAttribute.AttributeTypeId,
                SpecificationAttributeOptionId = productSpecificationAttribute.SpecificationAttributeOptionId,
                CustomValue = productSpecificationAttribute.CustomValue,
                AllowFiltering = productSpecificationAttribute.AllowFiltering,
                ShowOnProductPage = productSpecificationAttribute.ShowOnProductPage,
                DisplayOrder = productSpecificationAttribute.DisplayOrder
            };

            await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psaCopy);

            foreach (var language in allLanguages)
            {
                var customValue = await _localizationService.GetLocalizedAsync(productSpecificationAttribute, x => x.CustomValue, language.Id, false, false);
				_logger.LogDebug("CopyProductSpecificationsAsync(product={Product},productCopy={ProductCopy}): customValue → {CustomValue}", product, productCopy, customValue);
                if (!string.IsNullOrEmpty(customValue))
                    await _localizedEntityService.SaveLocalizedValueAsync(psaCopy, x => x.CustomValue, customValue, language.Id);
            }
        }
		_logger.LogDebug("<<< Exiting CopyProductSpecificationsAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy crosssell mapping
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyCrossSellsMappingAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyCrossSellsMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var csProduct in await _productService.GetCrossSellProductsByProductId1Async(product.Id, true))
            await _productService.InsertCrossSellProductAsync(
                new CrossSellProduct
                {
                    ProductId1 = productCopy.Id,
                    ProductId2 = csProduct.ProductId2
                });
				_logger.LogDebug("<<< Exiting CopyCrossSellsMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy related products mapping
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyRelatedProductsMappingAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyRelatedProductsMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var relatedProduct in await _productService.GetRelatedProductsByProductId1Async(product.Id, true))
            await _productService.InsertRelatedProductAsync(
                new RelatedProduct
                {
                    ProductId1 = productCopy.Id,
                    ProductId2 = relatedProduct.ProductId2,
                    DisplayOrder = relatedProduct.DisplayOrder
                });
				_logger.LogDebug("<<< Exiting CopyRelatedProductsMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy manufacturer mapping
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyManufacturersMappingAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyManufacturersMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var productManufacturers in await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true))
        {
            var productManufacturerCopy = new ProductManufacturer
            {
                ProductId = productCopy.Id,
                ManufacturerId = productManufacturers.ManufacturerId,
                IsFeaturedProduct = productManufacturers.IsFeaturedProduct,
                DisplayOrder = productManufacturers.DisplayOrder
            };

            await _manufacturerService.InsertProductManufacturerAsync(productManufacturerCopy);
        }
		_logger.LogDebug("<<< Exiting CopyManufacturersMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy category mapping
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyCategoriesMappingAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyCategoriesMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var productCategory in await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, showHidden: true))
        {
            var productCategoryCopy = new ProductCategory
            {
                ProductId = productCopy.Id,
                CategoryId = productCategory.CategoryId,
                IsFeaturedProduct = productCategory.IsFeaturedProduct,
                DisplayOrder = productCategory.DisplayOrder
            };

            await _categoryService.InsertProductCategoryAsync(productCategoryCopy);
        }
		_logger.LogDebug("<<< Exiting CopyCategoriesMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy warehouse mapping
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyWarehousesMappingAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyWarehousesMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        foreach (var pwi in await _productService.GetAllProductWarehouseInventoryRecordsAsync(product.Id))
        {
            await _productService.InsertProductWarehouseInventoryAsync(
                new ProductWarehouseInventory
                {
                    ProductId = productCopy.Id,
                    WarehouseId = pwi.WarehouseId,
                    StockQuantity = pwi.StockQuantity,
                    ReservedQuantity = 0
                });

            //quantity change history
            var message = $"{await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyProduct"), product.Id)}";
			_logger.LogDebug("CopyWarehousesMappingAsync(product={Product},productCopy={ProductCopy}): message → {Message}", product, productCopy, message);
            await _productService.AddStockQuantityHistoryEntryAsync(productCopy, pwi.StockQuantity, pwi.StockQuantity, pwi.WarehouseId, message);
        }
		_logger.LogDebug("<<< Exiting CopyWarehousesMappingAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy product pictures
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="newName">New product name</param>
    /// <param name="copyMultimedia"></param>
    /// <param name="productCopy">New product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the identifiers of old and new pictures
    /// </returns>
    protected virtual async Task<Dictionary<int, int>> CopyProductPicturesAsync(Product product, string newName, bool copyMultimedia, Product productCopy)
    {
        //variable to store original and new picture identifiers
        var originalNewPictureIdentifiers = new Dictionary<int, int>();
        if (!copyMultimedia)
            return originalNewPictureIdentifiers;

        foreach (var productPicture in await _productService.GetProductPicturesByProductIdAsync(product.Id))
        {
            var picture = await _pictureService.GetPictureByIdAsync(productPicture.PictureId);
            var pictureCopy = await _pictureService.InsertPictureAsync(
                await _pictureService.LoadPictureBinaryAsync(picture),
                picture.MimeType,
                await _pictureService.GetPictureSeNameAsync(newName),
                picture.AltAttribute,
                picture.TitleAttribute);
            await _productService.InsertProductPictureAsync(new ProductPicture
            {
                ProductId = productCopy.Id,
                PictureId = pictureCopy.Id,
                DisplayOrder = productPicture.DisplayOrder
            });
            originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
        }

        return originalNewPictureIdentifiers;
    }

    /// <summary>
    /// Copy product videos
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="copyVideos"></param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyProductVideosAsync(Product product, bool copyVideos, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyProductVideosAsync(product={Product},copyVideos={CopyVideos},productCopy={ProductCopy})", product, copyVideos, productCopy);
        if (copyVideos)
        {
            foreach (var productVideo in await _productService.GetProductVideosByProductIdAsync(product.Id))
            {
                var video = await _videoService.GetVideoByIdAsync(productVideo.VideoId);
				_logger.LogDebug("CopyProductVideosAsync(product={Product},copyVideos={CopyVideos},productCopy={ProductCopy}): video → {Video}", product, copyVideos, productCopy, video);
                var videoCopy = await _videoService.InsertVideoAsync(video);
				_logger.LogDebug("CopyProductVideosAsync(product={Product},copyVideos={CopyVideos},productCopy={ProductCopy}): videoCopy → {VideoCopy}", product, copyVideos, productCopy, videoCopy);
                await _productService.InsertProductVideoAsync(new ProductVideo
                {
                    ProductId = productCopy.Id,
                    VideoId = videoCopy.Id,
                    DisplayOrder = productVideo.DisplayOrder
                });
            }
        }
		_logger.LogDebug("<<< Exiting CopyProductVideosAsync(product={Product},copyVideos={CopyVideos},productCopy={ProductCopy})", product, copyVideos, productCopy);
    }

    /// <summary>
    /// Copy localization data
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productCopy">New product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CopyLocalizationDataAsync(Product product, Product productCopy)
    {
		_logger.LogDebug(">>> Entering CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
        var languages = await _languageService.GetAllLanguagesAsync(true);

		_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): languages → {Languages}", product, productCopy, languages);
        //localization
        foreach (var lang in languages)
        {
            var name = await _localizationService.GetLocalizedAsync(product, x => x.Name, lang.Id, false, false);
			_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): name → {Name}", product, productCopy, name);
            if (!string.IsNullOrEmpty(name))
                await _localizedEntityService.SaveLocalizedValueAsync(productCopy, x => x.Name, name, lang.Id);

            var shortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, lang.Id, false, false);
			_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): shortDescription → {ShortDescription}", product, productCopy, shortDescription);
            if (!string.IsNullOrEmpty(shortDescription))
                await _localizedEntityService.SaveLocalizedValueAsync(productCopy, x => x.ShortDescription, shortDescription, lang.Id);

            var fullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, lang.Id, false, false);
			_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): fullDescription → {FullDescription}", product, productCopy, fullDescription);
            if (!string.IsNullOrEmpty(fullDescription))
                await _localizedEntityService.SaveLocalizedValueAsync(productCopy, x => x.FullDescription, fullDescription, lang.Id);

            var metaKeywords = await _localizationService.GetLocalizedAsync(product, x => x.MetaKeywords, lang.Id, false, false);
			_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): metaKeywords → {MetaKeywords}", product, productCopy, metaKeywords);
            if (!string.IsNullOrEmpty(metaKeywords))
                await _localizedEntityService.SaveLocalizedValueAsync(productCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

            var metaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription, lang.Id, false, false);
			_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): metaDescription → {MetaDescription}", product, productCopy, metaDescription);
            if (!string.IsNullOrEmpty(metaDescription))
                await _localizedEntityService.SaveLocalizedValueAsync(productCopy, x => x.MetaDescription, metaDescription, lang.Id);

            var metaTitle = await _localizationService.GetLocalizedAsync(product, x => x.MetaTitle, lang.Id, false, false);
			_logger.LogDebug("CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy}): metaTitle → {MetaTitle}", product, productCopy, metaTitle);
            if (!string.IsNullOrEmpty(metaTitle))
                await _localizedEntityService.SaveLocalizedValueAsync(productCopy, x => x.MetaTitle, metaTitle, lang.Id);

            //search engine name
            await _urlRecordService.SaveSlugAsync(productCopy, await _urlRecordService.ValidateSeNameAsync(productCopy, string.Empty, name, false), lang.Id);
        }
		_logger.LogDebug("<<< Exiting CopyLocalizationDataAsync(product={Product},productCopy={ProductCopy})", product, productCopy);
    }

    /// <summary>
    /// Copy product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="newName">New product name</param>
    /// <param name="isPublished">A value indicating whether a new product is published</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    protected virtual async Task<Product> CopyBaseProductDataAsync(Product product, string newName, bool isPublished)
    {
		_logger.LogDebug(">>> Entering CopyBaseProductDataAsync(product={Product},newName={NewName},isPublished={IsPublished})", product, newName, isPublished);
        //product download & sample download
        var downloadId = product.DownloadId;
		_logger.LogDebug("CopyBaseProductDataAsync(product={Product},newName={NewName},isPublished={IsPublished}): downloadId → {DownloadId}", product, newName, isPublished, downloadId);
        var sampleDownloadId = product.SampleDownloadId;
		_logger.LogDebug("CopyBaseProductDataAsync(product={Product},newName={NewName},isPublished={IsPublished}): sampleDownloadId → {SampleDownloadId}", product, newName, isPublished, sampleDownloadId);
        if (product.IsDownload)
        {
            var download = await _downloadService.GetDownloadByIdAsync(product.DownloadId);
			_logger.LogDebug("CopyBaseProductDataAsync(product={Product},newName={NewName},isPublished={IsPublished}): download → {Download}", product, newName, isPublished, download);
            if (download != null)
            {
                var downloadCopy = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    UseDownloadUrl = download.UseDownloadUrl,
                    DownloadUrl = download.DownloadUrl,
                    DownloadBinary = download.DownloadBinary,
                    ContentType = download.ContentType,
                    Filename = download.Filename,
                    Extension = download.Extension,
                    IsNew = download.IsNew
                };
                await _downloadService.InsertDownloadAsync(downloadCopy);
                downloadId = downloadCopy.Id;
            }

            if (product.HasSampleDownload)
            {
                var sampleDownload = await _downloadService.GetDownloadByIdAsync(product.SampleDownloadId);
				_logger.LogDebug("CopyBaseProductDataAsync(product={Product},newName={NewName},isPublished={IsPublished}): sampleDownload → {SampleDownload}", product, newName, isPublished, sampleDownload);
                if (sampleDownload != null)
                {
                    var sampleDownloadCopy = new Download
                    {
                        DownloadGuid = Guid.NewGuid(),
                        UseDownloadUrl = sampleDownload.UseDownloadUrl,
                        DownloadUrl = sampleDownload.DownloadUrl,
                        DownloadBinary = sampleDownload.DownloadBinary,
                        ContentType = sampleDownload.ContentType,
                        Filename = sampleDownload.Filename,
                        Extension = sampleDownload.Extension,
                        IsNew = sampleDownload.IsNew
                    };
                    await _downloadService.InsertDownloadAsync(sampleDownloadCopy);
                    sampleDownloadId = sampleDownloadCopy.Id;
                }
            }
        }

        var newSku = !string.IsNullOrWhiteSpace(product.Sku)
            ? string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Copy.SKU.New"), product.Sku)
            : product.Sku;
        // product
        var productCopy = new Product
        {
            ProductTypeId = product.ProductTypeId,
            ParentGroupedProductId = product.ParentGroupedProductId,
            VisibleIndividually = product.VisibleIndividually,
            Name = newName,
            ShortDescription = product.ShortDescription,
            FullDescription = product.FullDescription,
            VendorId = product.VendorId,
            ProductTemplateId = product.ProductTemplateId,
            AdminComment = product.AdminComment,
            ShowOnHomepage = product.ShowOnHomepage,
            MetaKeywords = product.MetaKeywords,
            MetaDescription = product.MetaDescription,
            MetaTitle = product.MetaTitle,
            AllowCustomerReviews = product.AllowCustomerReviews,
            LimitedToStores = product.LimitedToStores,
            SubjectToAcl = product.SubjectToAcl,
            Sku = newSku,
            ManufacturerPartNumber = product.ManufacturerPartNumber,
            Gtin = product.Gtin,
            IsGiftCard = product.IsGiftCard,
            GiftCardType = product.GiftCardType,
            OverriddenGiftCardAmount = product.OverriddenGiftCardAmount,
            RequireOtherProducts = product.RequireOtherProducts,
            RequiredProductIds = product.RequiredProductIds,
            AutomaticallyAddRequiredProducts = product.AutomaticallyAddRequiredProducts,
            IsDownload = product.IsDownload,
            DownloadId = downloadId,
            UnlimitedDownloads = product.UnlimitedDownloads,
            MaxNumberOfDownloads = product.MaxNumberOfDownloads,
            DownloadExpirationDays = product.DownloadExpirationDays,
            DownloadActivationType = product.DownloadActivationType,
            HasSampleDownload = product.HasSampleDownload,
            SampleDownloadId = sampleDownloadId,
            HasUserAgreement = product.HasUserAgreement,
            UserAgreementText = product.UserAgreementText,
            IsRecurring = product.IsRecurring,
            RecurringCycleLength = product.RecurringCycleLength,
            RecurringCyclePeriod = product.RecurringCyclePeriod,
            RecurringTotalCycles = product.RecurringTotalCycles,
            IsRental = product.IsRental,
            RentalPriceLength = product.RentalPriceLength,
            RentalPricePeriod = product.RentalPricePeriod,
            IsShipEnabled = product.IsShipEnabled,
            IsFreeShipping = product.IsFreeShipping,
            ShipSeparately = product.ShipSeparately,
            AdditionalShippingCharge = product.AdditionalShippingCharge,
            DeliveryDateId = product.DeliveryDateId,
            IsTaxExempt = product.IsTaxExempt,
            TaxCategoryId = product.TaxCategoryId,
            ManageInventoryMethod = product.ManageInventoryMethod,
            ProductAvailabilityRangeId = product.ProductAvailabilityRangeId,
            UseMultipleWarehouses = product.UseMultipleWarehouses,
            WarehouseId = product.WarehouseId,
            StockQuantity = product.StockQuantity,
            DisplayStockAvailability = product.DisplayStockAvailability,
            DisplayStockQuantity = product.DisplayStockQuantity,
            MinStockQuantity = product.MinStockQuantity,
            LowStockActivityId = product.LowStockActivityId,
            NotifyAdminForQuantityBelow = product.NotifyAdminForQuantityBelow,
            BackorderMode = product.BackorderMode,
            AllowBackInStockSubscriptions = product.AllowBackInStockSubscriptions,
            OrderMinimumQuantity = product.OrderMinimumQuantity,
            OrderMaximumQuantity = product.OrderMaximumQuantity,
            AllowedQuantities = product.AllowedQuantities,
            AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations,
            NotReturnable = product.NotReturnable,
            DisableBuyButton = product.DisableBuyButton,
            DisableWishlistButton = product.DisableWishlistButton,
            AvailableForPreOrder = product.AvailableForPreOrder,
            PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
            CallForPrice = product.CallForPrice,
            Price = product.Price,
            OldPrice = product.OldPrice,
            ProductCost = product.ProductCost,
            CustomerEntersPrice = product.CustomerEntersPrice,
            MinimumCustomerEnteredPrice = product.MinimumCustomerEnteredPrice,
            MaximumCustomerEnteredPrice = product.MaximumCustomerEnteredPrice,
            BasepriceEnabled = product.BasepriceEnabled,
            BasepriceAmount = product.BasepriceAmount,
            BasepriceUnitId = product.BasepriceUnitId,
            BasepriceBaseAmount = product.BasepriceBaseAmount,
            BasepriceBaseUnitId = product.BasepriceBaseUnitId,
            MarkAsNew = product.MarkAsNew,
            MarkAsNewStartDateTimeUtc = product.MarkAsNewStartDateTimeUtc,
            MarkAsNewEndDateTimeUtc = product.MarkAsNewEndDateTimeUtc,
            Weight = product.Weight,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
            AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
            DisplayOrder = product.DisplayOrder,
            Published = isPublished,
            Deleted = product.Deleted,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            AgeVerification = product.AgeVerification,
            MinimumAgeToPurchase = product.MinimumAgeToPurchase
        };

        //validate search engine name
        await _productService.InsertProductAsync(productCopy);

        //search engine name
        await _urlRecordService.SaveSlugAsync(productCopy, await _urlRecordService.ValidateSeNameAsync(productCopy, string.Empty, productCopy.Name, true), 0);
		_logger.LogDebug("<<< Exiting CopyBaseProductDataAsync(product={Product},newName={NewName},isPublished={IsPublished})", product, newName, isPublished);
        return productCopy;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a copy of product with all depended data
    /// </summary>
    /// <param name="product">The product to copy</param>
    /// <param name="newName">The name of product duplicate</param>
    /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
    /// <param name="copyMultimedia">A value indicating whether the product images and videos should be copied</param>
    /// <param name="copyAssociatedProducts">A value indicating whether the copy associated products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product copy
    /// </returns>
    public virtual async Task<Product> CopyProductAsync(Product product, string newName,
        bool isPublished = true, bool copyMultimedia = true, bool copyAssociatedProducts = true)
    {
		_logger.LogDebug(">>> Entering CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts})", product, newName, isPublished, copyMultimedia, copyAssociatedProducts);
        ArgumentNullException.ThrowIfNull(product);

        if (string.IsNullOrEmpty(newName))
            throw new ArgumentException("Product name is required");

        var productCopy = await CopyBaseProductDataAsync(product, newName, isPublished);
		_logger.LogDebug("CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts}): productCopy → {ProductCopy}", product, newName, isPublished, copyMultimedia, copyAssociatedProducts, productCopy);

        //localization
        await CopyLocalizationDataAsync(product, productCopy);

        //copy product tags
        foreach (var productTag in await _productTagService.GetAllProductTagsByProductIdAsync(product.Id))
            await _productTagService.InsertProductProductTagMappingAsync(new ProductProductTagMapping { ProductTagId = productTag.Id, ProductId = productCopy.Id });

        //copy product pictures
        var originalNewPictureIdentifiers = await CopyProductPicturesAsync(product, newName, copyMultimedia, productCopy);
		_logger.LogDebug("CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts}): originalNewPictureIdentifiers → {OriginalNewPictureIdentifiers}", product, newName, isPublished, copyMultimedia, copyAssociatedProducts, originalNewPictureIdentifiers);

        //copy product videos
        await CopyProductVideosAsync(product, copyMultimedia, productCopy);

        //quantity change history
        await _productService.AddStockQuantityHistoryEntryAsync(productCopy, product.StockQuantity, product.StockQuantity, product.WarehouseId,
            string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyProduct"), product.Id));

        //product specifications
        await CopyProductSpecificationsAsync(product, productCopy);

        //product <-> warehouses mappings
        await CopyWarehousesMappingAsync(product, productCopy);
        //product <-> categories mappings
        await CopyCategoriesMappingAsync(product, productCopy);
        //product <-> manufacturers mappings
        await CopyManufacturersMappingAsync(product, productCopy);
        //product <-> related products mappings
        await CopyRelatedProductsMappingAsync(product, productCopy);
        //product <-> cross sells mappings
        await CopyCrossSellsMappingAsync(product, productCopy);
        //product <-> attributes mappings
        await CopyAttributesMappingAsync(product, productCopy, originalNewPictureIdentifiers);
        //product <-> discounts mapping
        await CopyDiscountsMappingAsync(product, productCopy);

        //store mapping
        var selectedStoreIds = await _storeMappingService.GetStoresIdsWithAccessAsync(product);
		_logger.LogDebug("CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts}): selectedStoreIds → {SelectedStoreIds}", product, newName, isPublished, copyMultimedia, copyAssociatedProducts, selectedStoreIds);
        foreach (var id in selectedStoreIds)
            await _storeMappingService.InsertStoreMappingAsync(productCopy, id);

        //customer role mapping
        var customerRoleIds = await _aclService.GetCustomerRoleIdsWithAccessAsync(product.Id, nameof(Product));
		_logger.LogDebug("CopyProductAsync(product={Product},newName={NewName},isPublished={IsPublished},copyMultimedia={CopyMultimedia},copyAssociatedProducts={CopyAssociatedProducts}): customerRoleIds → {CustomerRoleIds}", product, newName, isPublished, copyMultimedia, copyAssociatedProducts, customerRoleIds);

        foreach (var id in customerRoleIds)
            await _aclService.InsertAclRecordAsync(productCopy, id);

        //tier prices
        await CopyTierPricesAsync(product, productCopy);

        //associated products
        await CopyAssociatedProductsAsync(product, isPublished, copyMultimedia, copyAssociatedProducts, productCopy);

        return productCopy;
    }

    #endregion
}
