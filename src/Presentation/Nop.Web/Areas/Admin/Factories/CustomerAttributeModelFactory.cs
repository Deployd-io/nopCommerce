using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer attribute model factory implementation
/// </summary>
public partial class CustomerAttributeModelFactory : ICustomerAttributeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;

    #endregion

    #region Ctor

    public CustomerAttributeModelFactory(IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory)
    {
        _customerAttributeService = customerAttributeService;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare customer attribute value search model
    /// </summary>
    /// <param name="searchModel">Customer attribute value search model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <returns>Customer attribute value search model</returns>
    protected virtual CustomerAttributeValueSearchModel PrepareCustomerAttributeValueSearchModel(CustomerAttributeValueSearchModel searchModel,
        CustomerAttribute customerAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerAttributeValueSearchModel(searchModel={SearchModel},customerAttribute={CustomerAttribute})", searchModel, customerAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customerAttribute);

        searchModel.CustomerAttributeId = customerAttribute.Id;
		_logger.LogDebug("PrepareCustomerAttributeValueSearchModel(searchModel={SearchModel},customerAttribute={CustomerAttribute}): searchModel.CustomerAttributeId → {Arg2}", searchModel, customerAttribute, searchModel.CustomerAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareCustomerAttributeValueSearchModel(searchModel={SearchModel},customerAttribute={CustomerAttribute})", searchModel, customerAttribute);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare customer attribute search model
    /// </summary>
    /// <param name="searchModel">Customer attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute search model
    /// </returns>
    public virtual Task<CustomerAttributeSearchModel> PrepareCustomerAttributeSearchModelAsync(CustomerAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareCustomerAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged customer attribute list model
    /// </summary>
    /// <param name="searchModel">Customer attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute list model
    /// </returns>
    public virtual async Task<CustomerAttributeListModel> PrepareCustomerAttributeListModelAsync(CustomerAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customer attributes
        var customerAttributes = (await _customerAttributeService.GetAllAttributesAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareCustomerAttributeListModelAsync(searchModel={SearchModel}): customerAttributes → {CustomerAttributes}", searchModel, customerAttributes);

        //prepare list model
        var model = await new CustomerAttributeListModel().PrepareToGridAsync(searchModel, customerAttributes, () =>
        {
            return customerAttributes.SelectAwait(async attribute =>
            {
                //fill in model values from the entity
                var attributeModel = attribute.ToModel<CustomerAttributeModel>();
				_logger.LogDebug("PrepareCustomerAttributeListModelAsync(searchModel={SearchModel}): attributeModel → {AttributeModel}", searchModel, attributeModel);

                //fill in additional values (not existing in the entity)
                attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

                return attributeModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareCustomerAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare customer attribute model
    /// </summary>
    /// <param name="model">Customer attribute model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute model
    /// </returns>
    public virtual async Task<CustomerAttributeModel> PrepareCustomerAttributeModelAsync(CustomerAttributeModel model,
        CustomerAttribute customerAttribute, bool excludeProperties = false)
    {
        Func<CustomerAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (customerAttribute != null)
        {
            //fill in model values from the entity
            model ??= customerAttribute.ToModel<CustomerAttributeModel>();

            //prepare nested search model
            PrepareCustomerAttributeValueSearchModel(model.CustomerAttributeValueSearchModel, customerAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(customerAttribute, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    /// <summary>
    /// Prepare paged customer attribute value list model
    /// </summary>
    /// <param name="searchModel">Customer attribute value search model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute value list model
    /// </returns>
    public virtual async Task<CustomerAttributeValueListModel> PrepareCustomerAttributeValueListModelAsync(CustomerAttributeValueSearchModel searchModel,
        CustomerAttribute customerAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerAttributeValueListModelAsync(searchModel={SearchModel},customerAttribute={CustomerAttribute})", searchModel, customerAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customerAttribute);

        //get customer attribute values
        var customerAttributeValues = (await _customerAttributeService
            .GetAttributeValuesAsync(customerAttribute.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = new CustomerAttributeValueListModel().PrepareToGrid(searchModel, customerAttributeValues, () =>
        {
            //fill in model values from the entity
            return customerAttributeValues.Select(value => value.ToModel<CustomerAttributeValueModel>());
        });

		_logger.LogDebug("<<< Exiting PrepareCustomerAttributeValueListModelAsync(searchModel={SearchModel},customerAttribute={CustomerAttribute})", searchModel, customerAttribute);
        return model;
    }

    /// <summary>
    /// Prepare customer attribute value model
    /// </summary>
    /// <param name="model">Customer attribute value model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <param name="customerAttributeValue">Customer attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute value model
    /// </returns>
    public virtual async Task<CustomerAttributeValueModel> PrepareCustomerAttributeValueModelAsync(CustomerAttributeValueModel model,
        CustomerAttribute customerAttribute, CustomerAttributeValue customerAttributeValue, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(customerAttribute);

        Func<CustomerAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (customerAttributeValue != null)
        {
            //fill in model values from the entity
            model ??= customerAttributeValue.ToModel<CustomerAttributeValueModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(customerAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.AttributeId = customerAttribute.Id;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    #endregion
}