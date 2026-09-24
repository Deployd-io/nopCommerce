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
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ArtificialIntelligenceController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService})", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService);
        _artificialIntelligenceService = artificialIntelligenceService;
        _blogService = blogService;
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _blogService → {_blogService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _blogService);
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _artificialIntelligenceService → {_artificialIntelligenceService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _artificialIntelligenceService);
    _logger.LogWarning("<<< Exiting ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService})", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService);
        _categoryService = categoryService;
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _categoryService → {_categoryService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _categoryService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _manufacturerService → {_manufacturerService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _manufacturerService);
        _newsService = newsService;
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _newsService → {_newsService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _newsService);
        _productService = productService;
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _productService → {_productService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _productService);
        _topicService = topicService;
		_logger.LogDebug("ArtificialIntelligenceController(artificialIntelligenceService={ArtificialIntelligenceService},blogService={BlogService},categoryService={CategoryService},manufacturerService={ManufacturerService},newsService={NewsService},productService={ProductService},topicService={TopicService},vendorService={VendorService}): _topicService → {_topicService}", artificialIntelligenceService, blogService, categoryService, manufacturerService, newsService, productService, topicService, vendorService, _topicService);
        _vendorService = vendorService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> GenerateMetaTags(MetaTagsGeneratorModel metaTagsGeneratorModel)
    {
		_logger.LogError(">>> Entering GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel})", metaTagsGeneratorModel);
        var metaTitle = string.Empty;
        var metaKeywords = string.Empty;
        var metaDescription = string.Empty;

        try
        {
            switch (metaTagsGeneratorModel.EntityType)
            {
                case nameof(Product):
                    var product = await _productService.GetProductByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): product → {Product}", metaTagsGeneratorModel, product);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(product, metaTagsGeneratorModel.LanguageId);
                    break;
                case nameof(Category):
                    var category = await _categoryService.GetCategoryByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): category → {Category}", metaTagsGeneratorModel, category);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(category, metaTagsGeneratorModel.LanguageId);
                    break;
                case nameof(BlogPost):
                    var blogPost = await _blogService.GetBlogPostByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): blogPost → {BlogPost}", metaTagsGeneratorModel, blogPost);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsAsync(blogPost, blogPost.LanguageId);
                    break;
                case nameof(Manufacturer):
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): manufacturer → {Manufacturer}", metaTagsGeneratorModel, manufacturer);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(manufacturer, metaTagsGeneratorModel.LanguageId);
                    break;
                case nameof(NewsItem):
                    var newsItem = await _newsService.GetNewsByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): newsItem → {NewsItem}", metaTagsGeneratorModel, newsItem);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsAsync(newsItem, newsItem.LanguageId);
                    break;
                case nameof(Topic):
                    var topic = await _topicService.GetTopicByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): topic → {Topic}", metaTagsGeneratorModel, topic);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(topic, metaTagsGeneratorModel.LanguageId);
                    break;
                case nameof(Vendor):
                    var vendor = await _vendorService.GetVendorByIdAsync(metaTagsGeneratorModel.EntityId);
					_logger.LogError("GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): vendor → {Vendor}", metaTagsGeneratorModel, vendor);
                    (metaTitle, metaKeywords, metaDescription) = await _artificialIntelligenceService.CreateMetaTagsForLocalizedEntityAsync(vendor, metaTagsGeneratorModel.LanguageId);
                    break;
            }

            return Json(new { Title = metaTitle, Keywords = metaKeywords, Description = metaDescription });
        }
        catch (NopException e)
        {
			_logger.LogError("Exception in GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel}): {Arg1}", metaTagsGeneratorModel, e.Message);
			_logger.LogWarning("<<< Exiting GenerateMetaTags(metaTagsGeneratorModel={MetaTagsGeneratorModel})", metaTagsGeneratorModel);
            return ErrorJson(e.Message);
        }
    }

    #endregion
}