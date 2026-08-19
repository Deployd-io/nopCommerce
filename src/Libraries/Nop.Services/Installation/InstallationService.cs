using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Seo;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Installation;

/// <summary>
/// Installation service
/// </summary>
public partial class InstallationService : IInstallationService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IWebHelper _webHelper;

    protected InstallationSettings _installationSettings;

    #endregion

    #region Ctor

    public InstallationService(IHttpClientFactory httpClientFactory,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering InstallationService(httpClientFactory={HttpClientFactory},dataProvider={DataProvider},fileProvider={FileProvider},webHelper={WebHelper})", httpClientFactory, dataProvider, fileProvider, webHelper);
        _httpClientFactory = httpClientFactory;
        _dataProvider = dataProvider;
        _fileProvider = fileProvider;
		_logger.LogDebug("InstallationService(httpClientFactory={HttpClientFactory},dataProvider={DataProvider},fileProvider={FileProvider},webHelper={WebHelper}): _dataProvider → {_dataProvider}", httpClientFactory, dataProvider, fileProvider, webHelper, _dataProvider);
		_logger.LogDebug("InstallationService(httpClientFactory={HttpClientFactory},dataProvider={DataProvider},fileProvider={FileProvider},webHelper={WebHelper}): _fileProvider → {_fileProvider}", httpClientFactory, dataProvider, fileProvider, webHelper, _fileProvider);
        _webHelper = webHelper;
		_logger.LogDebug("InstallationService(httpClientFactory={HttpClientFactory},dataProvider={DataProvider},fileProvider={FileProvider},webHelper={WebHelper}): _webHelper → {_webHelper}", httpClientFactory, dataProvider, fileProvider, webHelper, _webHelper);
		_logger.LogDebug("<<< Exiting InstallationService(httpClientFactory={HttpClientFactory},dataProvider={DataProvider},fileProvider={FileProvider},webHelper={WebHelper})", httpClientFactory, dataProvider, fileProvider, webHelper);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets default customer identifier
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the identifier of default customer</returns>
    protected virtual async Task<int> GetDefaultCustomerIdAsync()
    {
		_logger.LogDebug(">>> Entering GetDefaultCustomerIdAsync()");
        if (_defaultCustomerId.HasValue)
            return _defaultCustomerId.Value;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await Table<Customer>().FirstOrDefaultAsync(x => x.Email == _installationSettings.AdminEmail) ?? throw new Exception("Cannot load default customer");

		_logger.LogDebug("GetDefaultCustomerIdAsync(): customer → {Customer}", customer);
		_logger.LogInformation("GetDefaultCustomerIdAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        _defaultCustomerId = customer.Id;
		_logger.LogDebug("GetDefaultCustomerIdAsync(): _defaultCustomerId → {_defaultCustomerId}", _defaultCustomerId);

		_logger.LogDebug("<<< Exiting GetDefaultCustomerIdAsync()");
        return customer.Id;
    }

    /// <summary>
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    protected virtual IQueryable<TEntity> Table<TEntity>() where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering Table()");
		_logger.LogDebug("<<< Exiting Table()");
        return _dataProvider.GetTable<TEntity>();
    }

    /// <summary>
    /// Gets the first entity identifier
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the entity identifier or null, if entity is not exists</returns>
    protected virtual async Task<int?> GetFirstEntityIdAsync<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering GetFirstEntityIdAsync(predicate={Predicate})", predicate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var entity = await Table<TEntity>().FirstOrDefaultAsync(predicate ?? (_ => true));

		_logger.LogDebug("GetFirstEntityIdAsync(predicate={Predicate}): entity → {Entity}", predicate, entity);
		_logger.LogInformation("GetFirstEntityIdAsync(predicate)={Predicate}: database query executed in {Arg1} ms", predicate, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetFirstEntityIdAsync(predicate={Predicate})", predicate);
        return entity?.Id;
    }

    /// <summary>
    /// Get the entity entry by identifier
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entry
    /// </returns>
    protected virtual async Task<TEntity> GetByIdAsync<TEntity>(int? id) where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering GetByIdAsync(id={Id})", id);
        if (!id.HasValue || id == 0)
            return null;

		_logger.LogInformation("GetByIdAsync(id)={Id}: database query executed in {Arg1} ms", id, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetByIdAsync(id={Id})", id);
        return await Table<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id.Value);
    }

    /// <summary>
    /// Validate search engine name
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="seName">Search engine name to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid seName
    /// </returns>
    protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : BaseEntity
    {
		_logger.LogDebug(">>> Entering ValidateSeNameAsync(entity={Entity},seName={SeName})", entity, seName);
        //duplicate of ValidateSeName method of \Nop.Services\Seo\UrlRecordService.cs (we cannot inject it here)
        ArgumentNullException.ThrowIfNull(entity);

        //validation
        var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
		_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): okChars → {OkChars}", entity, seName, okChars);
        seName = seName.Trim().ToLowerInvariant();
		_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): seName → {SeName1}", entity, seName, seName);

        var sb = new StringBuilder();
		_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): sb → {Sb}", entity, seName, sb);
        foreach (var c in seName.ToCharArray())
        {
            var c2 = c.ToString();
			_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): c2 → {C2}", entity, seName, c2);
            if (okChars.Contains(c2))
                sb.Append(c2);
        }

        seName = sb.ToString();
        seName = seName.Replace(" ", "-");
        while (seName.Contains("--"))
            seName = seName.Replace("--", "-");
        while (seName.Contains("__"))
            seName = seName.Replace("__", "_");

        //max length
        seName = CommonHelper.EnsureMaximumLength(seName, NopSeoDefaults.SearchEngineNameLength);

        //ensure this seName is not reserved yet
        var i = 2;
        var tempSeName = seName;
		_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): tempSeName → {TempSeName}", entity, seName, tempSeName);
        while (true)
        {
            //check whether such slug already exists (and that is not the current entity)

            var currentSeName = tempSeName;
            var query = Table<UrlRecord>().Where(ur => currentSeName != null && ur.Slug == currentSeName);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var urlRecord = await query.FirstOrDefaultAsync();

			_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): query → {Query}", entity, seName, query);
			_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): urlRecord → {UrlRecord}", entity, seName, urlRecord);
			_logger.LogInformation("ValidateSeNameAsync(entity,seName)={Entity},{SeName}: database query executed in {Arg2} ms", entity, seName, __logStart.ElapsedMilliseconds);
            var entityName = entity.GetType().Name;
            var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
			_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): entityName → {EntityName}", entity, seName, entityName);
			_logger.LogDebug("ValidateSeNameAsync(entity={Entity},seName={SeName}): reserved → {Reserved}", entity, seName, reserved);
            if (!reserved)
                break;

            tempSeName = $"{seName}-{i}";
            i++;
        }

        seName = tempSeName;

		_logger.LogDebug("<<< Exiting ValidateSeNameAsync(entity={Entity},seName={SeName})", entity, seName);
        return seName;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Install
    /// </summary>
    /// <param name="installationSettings">Installation settings</param>
    public virtual async Task InstallAsync(InstallationSettings installationSettings)
    {
		_logger.LogDebug(">>> Entering InstallAsync(installationSettings={InstallationSettings})", installationSettings);
        _installationSettings = installationSettings;
        _defaultLanguageId = null;
		_logger.LogDebug("InstallAsync(installationSettings={InstallationSettings}): _defaultLanguageId → {_defaultLanguageId}", installationSettings, _defaultLanguageId);
		_logger.LogDebug("InstallAsync(installationSettings={InstallationSettings}): _defaultCustomerId → {_defaultCustomerId}", installationSettings, _defaultCustomerId);
		_logger.LogDebug("InstallAsync(installationSettings={InstallationSettings}): _installationSettings → {_installationSettings}", installationSettings, _installationSettings);
        _defaultStoreId = null;
		_logger.LogDebug("InstallAsync(installationSettings={InstallationSettings}): _defaultStoreId → {_defaultStoreId}", installationSettings, _defaultStoreId);
        _defaultCustomerId = null;

        await InstallStoresAsync();
        await InstallMeasuresAsync();
        await InstallTaxCategoriesAsync();
        await InstallLanguagesAsync();
        await InstallCurrenciesAsync();
        await InstallCountriesAndStatesAsync();
        await InstallShippingMethodsAsync();
        await InstallDeliveryDatesAsync();
        await InstallProductAvailabilityRangesAsync();
        await InstallEmailAccountsAsync();
        await InstallMessageTemplatesAsync();
        await InstallNewsLetterSubscriptionTypeAsync();
        await InstallTopicTemplatesAsync();
        await InstallSettingsAsync();
        await InstallCustomersAndUsersAsync();
        await InstallTopicsAsync();
        await InstallActivityLogTypesAsync();
        await InstallProductTemplatesAsync();
        await InstallCategoryTemplatesAsync();
        await InstallManufacturerTemplatesAsync();
        await InstallScheduleTasksAsync();
        await InstallReturnRequestReasonsAsync();
        await InstallReturnRequestActionsAsync();
        await InstallMenusAsync();

        if (!installationSettings.InstallSampleData)
            return;

        //save setting to install plugin sample data
        var setting = new Setting
        {
            Name = NopInstallationDefaults.InstallPluginSampleDataSettingName,
            Value = "true",
            StoreId = 0
        };

        await _dataProvider.InsertEntityAsync(setting);

        var sampleData = JsonConvert.DeserializeObject<SampleData.SampleData>(await _fileProvider.ReadAllTextAsync(_fileProvider.MapPath(NopInstallationDefaults.SampleDataPath), Encoding.UTF8));
		_logger.LogDebug("InstallAsync(installationSettings={InstallationSettings}): sampleData → {SampleData}", installationSettings, sampleData);

        await InstallSampleCustomersAsync(sampleData.Customers);
        await InstallCheckoutAttributesAsync(sampleData.CheckoutAttributes);
        await InstallSpecificationAttributesAsync(sampleData.SpecificationAttributes);
        await InstallProductAttributesAsync(sampleData.ProductAttributes);
        await InstallCategoriesAsync(sampleData.Categories);
        await InstallManufacturersAsync(sampleData.Manufacturers);
        await InstallProductsAsync(sampleData.Products);
        await InstallForumsAsync(sampleData.ForumGroups);
        await InstallDiscountsAsync(sampleData.Discounts);
        await InstallBlogPostsAsync(sampleData.BlogPosts);
        await InstallNewsAsync(sampleData.NewsItems);
        await InstallPollsAsync(sampleData.Polls);
        await InstallWarehousesAsync(sampleData.Warehouses);
        await InstallVendorsAsync(sampleData.Vendors);
        await InstallAffiliatesAsync(sampleData.Affiliates);
        await InstallOrdersAsync(sampleData.Orders);
        await InstallActivityLogsAsync(sampleData.ActivityLogs);
        await InstallSearchTermsAsync(sampleData.SearchTerms);
		_logger.LogDebug("<<< Exiting InstallAsync(installationSettings={InstallationSettings})", installationSettings);
    }

    #endregion
}