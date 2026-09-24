using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Installation.SampleData;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Installation;

public partial class InstallationService
{
    #region Fields

    protected Dictionary<string, ProductTag> _tags = new(comparer: StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts the product picture
    /// </summary>
    /// <param name="product">Product to insert picture</param>
    /// <param name="fileName">Picture file name</param>
    /// <param name="displayOrder">Display order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the identifier of inserted picture
    /// </returns>
    protected virtual async Task<int> InsertProductPictureAsync(Product product, string fileName, int displayOrder = 1)
    {
		_logger.LogError(">>> Entering InsertProductPictureAsync(product={Product},fileName={FileName},displayOrder={DisplayOrder})", product, fileName, displayOrder);
        var pictureId = await InsertPictureAsync(fileName, product.Name);

		_logger.LogError("InsertProductPictureAsync(product={Product},fileName={FileName},displayOrder={DisplayOrder}): pictureId → {PictureId}", product, fileName, displayOrder, pictureId);
        await _dataProvider.InsertEntityAsync(
            new ProductPicture
            {
                ProductId = product.Id,
                PictureId = pictureId,
                DisplayOrder = displayOrder
            });
			_logger.LogDebug("InsertProductPictureAsync(product={Product},fileName={FileName},displayOrder={DisplayOrder}): DisplayOrder → {DisplayOrder1}", product, fileName, displayOrder, DisplayOrder);

		_logger.LogWarning("<<< Exiting InsertProductPictureAsync(product={Product},fileName={FileName},displayOrder={DisplayOrder})", product, fileName, displayOrder);
        return pictureId;
    }

    /// <summary>
    /// Gets a specification attribute option identifier
    /// </summary>
    /// <param name="specAttributeName">The spec attribute name</param>
    /// <param name="specAttributeOptionName">The spec attribute option name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> GetSpecificationAttributeOptionIdAsync(string specAttributeName, string specAttributeOptionName)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeOptionIdAsync(specAttributeName={SpecAttributeName},specAttributeOptionName={SpecAttributeOptionName})", specAttributeName, specAttributeOptionName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var specificationAttribute = await Table<SpecificationAttribute>()
            .SingleAsync(sa => sa.Name == specAttributeName);
			_logger.LogError("GetSpecificationAttributeOptionIdAsync(specAttributeName,specAttributeOptionName)={SpecAttributeName},{SpecAttributeOptionName}: database query executed in {Arg2} ms", specAttributeName, specAttributeOptionName, __logStart.ElapsedMilliseconds);

        var specificationAttributeOption = await Table<SpecificationAttributeOption>()
            .SingleAsync(sao => sao.Name == specAttributeOptionName && sao.SpecificationAttributeId == specificationAttribute.Id);

		_logger.LogWarning("<<< Exiting GetSpecificationAttributeOptionIdAsync(specAttributeName={SpecAttributeName},specAttributeOptionName={SpecAttributeOptionName})", specAttributeName, specAttributeOptionName);
        return specificationAttributeOption.Id;
    }

    /// <summary>
    /// Insert product tags mappings
    /// </summary>
    /// <param name="product"></param>
    /// <param name="tags"></param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertProductTagMappingAsync(Product product, string[] tags)
    {
		_logger.LogError(">>> Entering InsertProductTagMappingAsync(product={Product},tags={Tags})", product, tags);
        if (!_tags.Any())
            _tags = Table<ProductTag>().AsEnumerable().GroupBy(p => p.Name, p => p)
                .ToDictionary(p => p.Key, p => p.FirstOrDefault());
				_logger.LogDebug("InsertProductTagMappingAsync(product={Product},tags={Tags}): _tags → {_tags}", product, tags, _tags);
				var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var newProductTags = await tags.Distinct().Where(tag => !_tags.ContainsKey(tag))
            .Select(item => new ProductTag { Name = item }).ToListAsync();
			_logger.LogError("InsertProductTagMappingAsync(product,tags)={Product},{Tags}: find query executed in {Arg2} ms", product, tags, __logStart.ElapsedMilliseconds);

        if (newProductTags.Any())
        {
            await _dataProvider.BulkInsertEntitiesAsync(newProductTags);
            await InsertSearchEngineNamesAsync(newProductTags, productTag => productTag.Name);

            foreach (var productTag in newProductTags)
                _tags.Add(productTag.Name, productTag);
        }

        await _dataProvider.BulkInsertEntitiesAsync(tags.Select(tag =>
            new ProductProductTagMapping { ProductTagId = _tags[tag].Id, ProductId = product.Id }));
			_logger.LogWarning("<<< Exiting InsertProductTagMappingAsync(product={Product},tags={Tags})", product, tags);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallProductsAsync(SampleProducts jsonData)
    {
		_logger.LogError(">>> Entering InstallProductsAsync(jsonData={JsonData})", jsonData);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //products
        var allProducts = new List<Product>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): allProducts → {AllProducts}", jsonData, allProducts);

        var productTemplates = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): productTemplates → {ProductTemplates}", jsonData, productTemplates);
        var taxCategories = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): taxCategories → {TaxCategories}", jsonData, taxCategories);
        var categories = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): categories → {Categories}", jsonData, categories);
        var manufacturers = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): manufacturers → {Manufacturers}", jsonData, manufacturers);
        var productAttributes = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): productAttributes → {ProductAttributes}", jsonData, productAttributes);
        var productAvailabilityRanges = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): productAvailabilityRanges → {ProductAvailabilityRanges}", jsonData, productAvailabilityRanges);
        var deliveryDates = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): deliveryDates → {DeliveryDates}", jsonData, deliveryDates);
        var products = new Dictionary<string, int>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): products → {Products}", jsonData, products);

        async Task<int> getAndSaveId(Dictionary<string, int> dict, string key, Func<string, Task<int>> foo)
        {
			_logger.LogError(">>> Entering getAndSaveId(dict={Dict},key={Key},foo={Foo})", dict, key, foo);
            if (string.IsNullOrEmpty(key))
                return 0;

            if (dict.TryGetValue(key, out var id))
                return id;

            id = await foo(key);
			_logger.LogError("InstallProductsAsync(jsonData={JsonData}): id → {Id}", jsonData, id);
            dict[key] = id;
			_logger.LogError("getAndSaveId(dict={Dict},key={Key},foo={Foo}): id → {Id}", dict, key, foo, id);

			_logger.LogWarning("<<< Exiting getAndSaveId(dict={Dict},key={Key},foo={Foo})", dict, key, foo);
            return id;
        }

        async Task<int> getProductTemplate(string templateName)
        {
			_logger.LogError(">>> Entering getProductTemplate(templateName={TemplateName})", templateName);
            return await getAndSaveId(productTemplates, templateName,
                async tName => await GetFirstEntityIdAsync<ProductTemplate>(pt => pt.Name == tName) ??
					_logger.LogError("Throwing Exception from InstallProductsAsync(jsonData={JsonData})", jsonData);
        _logger.LogError("Throwing Exception from getProductTemplate(templateName={TemplateName})", templateName);
        _logger.LogWarning("<<< Exiting getProductTemplate(templateName={TemplateName})", templateName);
                    throw new Exception($"\"{tName}\" template could not be loaded"));
        }

        async Task<int> getTaxCategoryId(string taxCategoryName)
        {
			_logger.LogError(">>> Entering getTaxCategoryId(taxCategoryName={TaxCategoryName})", taxCategoryName);
            return await getAndSaveId(taxCategories, taxCategoryName, async tcName => await GetFirstEntityIdAsync<TaxCategory>(tc => tc.Name == tcName) ??
		_logger.LogError("Throwing Exception from InstallProductsAsync(jsonData={JsonData})", jsonData);
                throw new Exception($"\"{tcName}\" tax category could not be loaded"));
        }

        async Task<int> getCategoryId(string categoryName)
        {
			_logger.LogError(">>> Entering getCategoryId(categoryName={CategoryName})", categoryName);
            return await getAndSaveId(categories, categoryName, async cName => await GetFirstEntityIdAsync<Category>(c => c.Name == cName) ??
                throw new Exception($"\"{cName}\" category could not be loaded"));
        }

        async Task<int> getManufacturerId(string manufacturerName)
        {
			_logger.LogError(">>> Entering getManufacturerId(manufacturerName={ManufacturerName})", manufacturerName);
            return await getAndSaveId(manufacturers, manufacturerName, async mName => await GetFirstEntityIdAsync<Manufacturer>(m => m.Name == mName) ??
                throw new Exception($"\"{mName}\" manufacturer could not be loaded"));
        }

        async Task<int> getProductAttributeId(string productAttributeName)
        {
			_logger.LogError(">>> Entering getProductAttributeId(productAttributeName={ProductAttributeName})", productAttributeName);
            return await getAndSaveId(productAttributes, productAttributeName, async paName => await GetFirstEntityIdAsync<ProductAttribute>(pa => pa.Name == paName) ??
                throw new Exception($"\"{paName}\" product attribute could not be loaded"));
        }

        async Task<int> getProductAvailabilityRangeId(string productAvailabilityRangeName)
        {
			_logger.LogError(">>> Entering getProductAvailabilityRangeId(productAvailabilityRangeName={ProductAvailabilityRangeName})", productAvailabilityRangeName);
            return await getAndSaveId(productAvailabilityRanges, productAvailabilityRangeName, async parName => await GetFirstEntityIdAsync<ProductAvailabilityRange>(par => par.Name == parName) ??
                throw new Exception($"\"{parName}\" product availability range could not be loaded"));
        }

        async Task<int> getDeliveryDateId(string deliveryDateName)
        {
			_logger.LogError(">>> Entering getDeliveryDateId(deliveryDateName={DeliveryDateName})", deliveryDateName);
            return await getAndSaveId(deliveryDates, deliveryDateName, async ddName => await GetFirstEntityIdAsync<DeliveryDate>(dd => dd.Name == ddName) ??
                throw new Exception($"\"{ddName}\" delivery date could not be loaded"));
        }

        async Task<int> getProductId(string productSku)
        {
			_logger.LogError(">>> Entering getProductId(productSku={ProductSku})", productSku);
            return await getAndSaveId(products, productSku, async sku => await GetFirstEntityIdAsync<Product>(p => p.Sku == sku) ??
                throw new Exception($"Product with SKU = \"{sku}\" could not be loaded"));
        }

        //TODO: avoid using service
        var downloadService = EngineContext.Current.Resolve<IDownloadService>();
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): downloadService → {DownloadService}", jsonData, downloadService);

        var sampleDownloadsPath = _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);
		_logger.LogError("InstallProductsAsync(jsonData={JsonData}): sampleDownloadsPath → {SampleDownloadsPath}", jsonData, sampleDownloadsPath);

        async Task insertProduct(SampleProducts.SampleProduct sample, int parentGroupedProductId = 0)
        {
			_logger.LogError(">>> Entering insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId})", sample, parentGroupedProductId);
            var product = new Product
            {
                ProductType = sample.ProductType,
                VisibleIndividually = sample.VisibleIndividually,
                ParentGroupedProductId = parentGroupedProductId,
                Name = sample.Name,
                Sku = sample.Sku,
                ShortDescription = sample.ShortDescription,
                FullDescription = sample.FullDescription,
                ProductTemplateId = await getProductTemplate(sample.ProductTemplateName),
                AllowCustomerReviews = sample.AllowCustomerReviews,
                Price = sample.Price,
                OldPrice = sample.OldPrice,
                IsShipEnabled = sample.IsShipEnabled,
                IsFreeShipping = sample.IsFreeShipping,
                Weight = sample.Weight,
                Length = sample.Length,
                Width = sample.Width,
                Height = sample.Height,
                TaxCategoryId = await getTaxCategoryId(sample.TaxCategoryName),
                ManageInventoryMethod = sample.ManageInventoryMethod,
                StockQuantity = sample.StockQuantity,
                NotifyAdminForQuantityBelow = sample.NotifyAdminForQuantityBelow,
                AllowBackInStockSubscriptions = sample.AllowBackInStockSubscriptions,
                DisplayStockAvailability = sample.DisplayStockAvailability,
                LowStockActivity = sample.LowStockActivity,
                BackorderMode = sample.BackorderMode,
                OrderMinimumQuantity = sample.OrderMinimumQuantity,
                OrderMaximumQuantity = sample.OrderMaximumQuantity,
                Published = sample.Published,
                ShowOnHomepage = sample.ShowOnHomepage,
                MarkAsNew = sample.MarkAsNew,
                IsRecurring = sample.IsRecurring,
                RecurringCycleLength = sample.RecurringCycleLength,
                RecurringCyclePeriod = sample.RecurringCyclePeriod,
                RecurringTotalCycles = sample.RecurringTotalCycles,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IsRental = sample.IsRental,
                RentalPriceLength = sample.RentalPriceLength,
                RentalPricePeriod = sample.RentalPricePeriod,
                IsGiftCard = sample.IsGiftCard,
                GiftCardType = sample.GiftCardType,
                IsDownload = sample.IsDownload,
                DownloadActivationType = sample.DownloadActivationType,
                UnlimitedDownloads = sample.UnlimitedDownloads,
                HasUserAgreement = sample.HasUserAgreement,
                CustomerEntersPrice = sample.CustomerEntersPrice,
                MinimumCustomerEnteredPrice = sample.MinimumCustomerEnteredPrice,
                MaximumCustomerEnteredPrice = sample.MaximumCustomerEnteredPrice,
            };

            if (!string.IsNullOrEmpty(sample.ProductAvailabilityRange))
                product.ProductAvailabilityRangeId = await getProductAvailabilityRangeId(sample.ProductAvailabilityRange);

            if (!string.IsNullOrEmpty(sample.DeliveryDate))
                product.DeliveryDateId = await getDeliveryDateId(sample.DeliveryDate);

            if (sample.Download != null)
            {
                var download = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    ContentType = sample.Download.ContentType,
                    DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + sample.Download.DownloadFileName),
                    Extension = sample.Download.Extension,
                    Filename = sample.Download.Filename,
                    IsNew = sample.Download.IsNew
                };
                await downloadService.InsertDownloadAsync(download);

                product.DownloadId = download.Id;
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): product.DownloadId → {Arg1}", jsonData, product.DownloadId);
            }
			_logger.LogDebug("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): product.DownloadId → {Arg2}", sample, parentGroupedProductId, product.DownloadId);

            if (sample.SampleDownload != null)
            {
                var download = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    ContentType = sample.SampleDownload.ContentType,
                    DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + sample.SampleDownload.DownloadFileName),
                    Extension = sample.SampleDownload.Extension,
                    Filename = sample.SampleDownload.Filename,
                    IsNew = sample.SampleDownload.IsNew
                };
                await downloadService.InsertDownloadAsync(download);

                product.HasSampleDownload = true;
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): product.HasSampleDownload → {Arg1}", jsonData, product.HasSampleDownload);
                product.DownloadId = download.Id;
            }
			_logger.LogDebug("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): product.HasSampleDownload → {Arg2}", sample, parentGroupedProductId, product.HasSampleDownload);

            allProducts.Add(product);

            await _dataProvider.InsertEntityAsync(product);

            if (!string.IsNullOrEmpty(sample.CategoryName))
                await _dataProvider.InsertEntityAsync(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = await getCategoryId(sample.CategoryName),
                    DisplayOrder = 1
                });
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): DisplayOrder → {DisplayOrder}", jsonData, DisplayOrder);

			_logger.LogDebug("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): DisplayOrder → {DisplayOrder}", sample, parentGroupedProductId, DisplayOrder);
            if (!string.IsNullOrEmpty(sample.ManufacturerName))
                await _dataProvider.InsertEntityAsync(new ProductManufacturer
                {
                    ProductId = product.Id,
                    ManufacturerId = await getManufacturerId(sample.ManufacturerName),
                    DisplayOrder = 1
                });

            if (sample.ProductPictures.Any())
            {
                var productPictures = new List<ProductPicture>();
				_logger.LogError("InstallProductsAsync(jsonData={JsonData}): productPictures → {ProductPictures}", jsonData, productPictures);

				_logger.LogError("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): productPictures → {ProductPictures}", sample, parentGroupedProductId, productPictures);
                foreach (var pictureName in sample.ProductPictures)
                    productPictures.Add(new()
                    {
                        ProductId = product.Id,
                        PictureId = await InsertPictureAsync(pictureName, product.Name),
                        DisplayOrder = 1
                    });

                await _dataProvider.BulkInsertEntitiesAsync(productPictures);
            }

            foreach (var productAttributeMapping in sample.ProductAttributeMapping)
            {
                var attributeMapping = await _dataProvider.InsertEntityAsync(new ProductAttributeMapping
                {
                    ProductId = product.Id,
                    ProductAttributeId = await getProductAttributeId(productAttributeMapping.ProductAttributeName),
                    AttributeControlType = productAttributeMapping.AttributeControlType,
                    TextPrompt = productAttributeMapping.TextPrompt,
                    IsRequired = productAttributeMapping.IsRequired
                });
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): IsRequired → {IsRequired}", jsonData, IsRequired);

				_logger.LogDebug("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): IsRequired → {IsRequired}", sample, parentGroupedProductId, IsRequired);
                var productAttributeValues = new List<ProductAttributeValue>();
				_logger.LogError("InstallProductsAsync(jsonData={JsonData}): productAttributeValues → {ProductAttributeValues}", jsonData, productAttributeValues);

				_logger.LogError("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): productAttributeValues → {ProductAttributeValues}", sample, parentGroupedProductId, productAttributeValues);
                foreach (var sampleProductAttributeValue in productAttributeMapping.AttributeValues)
                {
                    var productAttributeValue = new ProductAttributeValue
                    {
                        ProductAttributeMappingId = attributeMapping.Id,
                        AttributeValueType = sampleProductAttributeValue.AttributeValueType,
                        Name = sampleProductAttributeValue.Name,
                        DisplayOrder = sampleProductAttributeValue.DisplayOrder,
                        PriceAdjustment = sampleProductAttributeValue.PriceAdjustment,
                        IsPreSelected = sampleProductAttributeValue.IsPreSelected,
                        ColorSquaresRgb = sampleProductAttributeValue.ColorSquaresRgb
                    };

                    if (!string.IsNullOrEmpty(sampleProductAttributeValue.ImageSquaresPictureName))
                        productAttributeValue.ImageSquaresPictureId = await InsertPictureAsync(sampleProductAttributeValue.ImageSquaresPictureName, productAttributeValue.Name);

                    if (sampleProductAttributeValue.AttributeValuePictures.Any())
                    {
                        await _dataProvider.InsertEntityAsync(productAttributeValue);

                        await _dataProvider.BulkInsertEntitiesAsync(await sampleProductAttributeValue.AttributeValuePictures
                            .SelectAwait(async p =>
                                new ProductAttributeValuePicture
                                {
                                    PictureId = await InsertProductPictureAsync(product, p),
                                    ProductAttributeValueId = productAttributeValue.Id
                                }).ToListAsync());
                    }
                    else
                        productAttributeValues.Add(productAttributeValue);
						_logger.LogError("InstallProductsAsync(jsonData)={JsonData}: database query executed in {Arg1} ms", jsonData, __logStart.ElapsedMilliseconds);
                }
				_logger.LogError("insertProduct(sample,parentGroupedProductId)={Sample},{ParentGroupedProductId}: database query executed in {Arg2} ms", sample, parentGroupedProductId, __logStart.ElapsedMilliseconds);

                await _dataProvider.BulkInsertEntitiesAsync(productAttributeValues);
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): ProductAttributeValueId → {ProductAttributeValueId}", jsonData, ProductAttributeValueId);

				_logger.LogDebug("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): ProductAttributeValueId → {ProductAttributeValueId}", sample, parentGroupedProductId, ProductAttributeValueId);
                if (productAttributeValues.Any(p => p.ImageSquaresPictureId != 0))
                    await _dataProvider.BulkInsertEntitiesAsync(productAttributeValues.Where(p => p.ImageSquaresPictureId != 0).Select(v =>
                        new ProductAttributeValuePicture
                        {
                            PictureId = v.ImageSquaresPictureId,
                            ProductAttributeValueId = v.Id
                        }));
            }

            if (sample.ProductTags.Any())
                await InsertProductTagMappingAsync(product, sample.ProductTags.ToArray());

            if (sample.ProductSpecificationAttribute.Any())
            {
                var productSpecificationAttributes = new List<ProductSpecificationAttribute>();
				_logger.LogError("InstallProductsAsync(jsonData={JsonData}): productSpecificationAttributes → {ProductSpecificationAttributes}", jsonData, productSpecificationAttributes);

				_logger.LogError("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): productSpecificationAttributes → {ProductSpecificationAttributes}", sample, parentGroupedProductId, productSpecificationAttributes);
                foreach (var specificationAttribute in sample.ProductSpecificationAttribute)
                    productSpecificationAttributes.Add(new ProductSpecificationAttribute
                    {
                        ProductId = product.Id,
                        AllowFiltering = specificationAttribute.AllowFiltering,
                        ShowOnProductPage = specificationAttribute.ShowOnProductPage,
                        DisplayOrder = specificationAttribute.DisplayOrder,
                        SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync(specificationAttribute.SpecAttributeName, specificationAttribute.SpecAttributeOptionName)
                    });
						_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): SpecificationAttributeOptionId → {SpecificationAttributeOptionId}", jsonData, SpecificationAttributeOptionId);

                await _dataProvider.BulkInsertEntitiesAsync(productSpecificationAttributes);
            }

            foreach (var sampleGroupedProduct in sample.GroupedProducts)
                await insertProduct(sampleGroupedProduct, product.Id);

			_logger.LogWarning("<<< Exiting insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId})", sample, parentGroupedProductId);
            if (sample.TierPrices.Any())
                await _dataProvider.BulkInsertEntitiesAsync(sample.TierPrices.Select(tp => new TierPrice
                {
					_logger.LogDebug("insertProduct(sample={Sample},parentGroupedProductId={ParentGroupedProductId}): ProductId → {ProductId}", sample, parentGroupedProductId, ProductId);
                    Quantity = tp.Quantity,
                    Price = tp.Price,
                    ProductId = product.Id
                }));
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): ProductId → {ProductId}", jsonData, ProductId);
        }

        foreach (var sample in jsonData.Products)
            await insertProduct(sample);

        //search engine names
        await InsertSearchEngineNamesAsync(allProducts, product => product.Name);

        //related products
        if (jsonData.RelatedProducts.Any())
            await _dataProvider.BulkInsertEntitiesAsync(await jsonData.RelatedProducts.SelectAwait(async rp =>
                new RelatedProduct
                {
                    ProductId1 = await getProductId(rp.FirstProductSku),
                    ProductId2 = await getProductId(rp.SecondProductSku)
                }).ToListAsync());
					_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): ProductId2 → {ProductId2}", jsonData, ProductId2);

        //reviews
        using (var random = new SecureRandomNumberGenerator())
            foreach (var product in allProducts)
            {
                if (product.ProductType != ProductType.SimpleProduct)
                    continue;

                //only 3 of 4 products will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);
				_logger.LogError("InstallProductsAsync(jsonData={JsonData}): rating → {Rating}", jsonData, rating);

                await _dataProvider.InsertEntityAsync(new ProductReview
                {
                    CustomerId = await GetDefaultCustomerIdAsync(),
                    ProductId = product.Id,
                    StoreId = await GetDefaultStoreIdAsync(),
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = $"This sample review is for the {product.Name}. I've been waiting for this product to be available. It is priced just right.",
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): CreatedOnUtc → {CreatedOnUtc}", jsonData, CreatedOnUtc);

                product.ApprovedRatingSum = rating;
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): product.ApprovedRatingSum → {Arg1}", jsonData, product.ApprovedRatingSum);
				_logger.LogDebug("InstallProductsAsync(jsonData={JsonData}): product.ApprovedTotalReviews → {Arg1}", jsonData, product.ApprovedTotalReviews);
                product.ApprovedTotalReviews = 1;
            }

        await _dataProvider.UpdateEntitiesAsync(allProducts);

        //stock quantity history
        foreach (var product in allProducts.Where(product => product.StockQuantity > 0))
            await _dataProvider.InsertEntityAsync(new StockQuantityHistory
            {
                ProductId = product.Id,
                WarehouseId = product.WarehouseId > 0 ? product.WarehouseId : null,
                QuantityAdjustment = product.StockQuantity,
                StockQuantity = product.StockQuantity,
                Message = "The stock quantity has been edited",
                CreatedOnUtc = DateTime.UtcNow
            });
			_logger.LogWarning("<<< Exiting InstallProductsAsync(jsonData={JsonData})", jsonData);
    }

    #endregion
}
