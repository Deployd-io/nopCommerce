using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.News;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ArtificialIntelligenceController : BaseAdminController
{
    #region Fields

    protected readonly IArtificialIntelligenceService _artificialIntelligenceService;
    protected readonly IBlogService _blogService;
    protected readonly ICategoryService _categoryService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INewsService _newsService;
    protected readonly IProductService _productService;
    protected readonly ITopicService _topicService;
    protected readonly IVendorService _vendorService;

    #endregion

    #region Ctor

    public ArtificialIntelligenceController(IArtificialIntelligenceService artificialIntelligenceService,
        IBlogService blogService,
        ICategoryService categoryService,
        IManufacturerService manufacturerService,
        INewsService newsService,
        IProductService productService,
        ITopicService topicService,
        IVendorService vendorService)
    {
        _artificialIntelligenceService = artificialIntelligenceService;
        _blogService = blogService;
        _categoryService = categoryService;
        _manufacturerService = manufacturerService;
        _newsService = newsService;
        _productService = productService;
        _topicService = topicService;
        _vendorService = vendorService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> GenerateMetaTags(MetaTagsGeneratorModel metaTagsGeneratorModel)
    {
		logger.LogDebug(">>> Entering GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel})", metaTagsGeneratorModel);
        var metaTitle = string.Empty;
        var metaKeywords = string.Empty;
        var metaDescription = string.Empty;

        try
        {
            switch (metaTagsGeneratorModel.EntityType)
            {
                case nameof(Product):
                    var product = await _productService.GetProductByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(product, metaTagsGeneratorModel.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): product → {Product}", metaTagsGeneratorModel, product);
                    break;
                case nameof(Category):
                    var category = await _categoryService.GetCategoryByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(category, metaTagsGeneratorModel.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): category → {Category}", metaTagsGeneratorModel, category);
                    break;
                case nameof(BlogPost):
                    var blogPost = await _blogService.GetBlogPostByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsAsync(blogPost, blogPost.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): blogPost → {BlogPost}", metaTagsGeneratorModel, blogPost);
                    break;
                case nameof(Manufacturer):
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(manufacturer, metaTagsGeneratorModel.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): manufacturer → {Manufacturer}", metaTagsGeneratorModel, manufacturer);
                    break;
                case nameof(NewsItem):
                    var newsItem = await _newsService.GetNewsByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsAsync(newsItem, newsItem.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): newsItem → {NewsItem}", metaTagsGeneratorModel, newsItem);
                    break;
                case nameof(Topic):
                    var topic = await _topicService.GetTopicByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(topic, metaTagsGeneratorModel.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): topic → {Topic}", metaTagsGeneratorModel, topic);
                    break;
                case nameof(Vendor):
                    var vendor = await _vendorService.GetVendorByIdAsync(metaTagsGeneratorModel.EntityId);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(vendor, metaTagsGeneratorModel.LanguageId);
					logger.LogDebug("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): vendor → {Vendor}", metaTagsGeneratorModel, vendor);
                    break;
            }

            return Json(new { Title = metaTitle, Keywords = metaKeywords, Description = metaDescription });
        }
        catch (NopException e)
        {
			logger.LogError("Exception in GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): {Arg1}", metaTagsGeneratorModel, ex.Message);
			logger.LogDebug("<<< Exiting GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel})", metaTagsGeneratorModel);
            return ErrorJson(e.Message);
        }
    }

    #endregion
}