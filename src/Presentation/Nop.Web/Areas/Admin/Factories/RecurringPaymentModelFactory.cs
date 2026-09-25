using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the recurring payment model factory implementation
/// </summary>
public partial class RecurringPaymentModelFactory : IRecurringPaymentModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentService _paymentService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public RecurringPaymentModelFactory(IDateTimeHelper dateTimeHelper,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentService paymentService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext})", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext);
        _dateTimeHelper = dateTimeHelper;
        _customerService = customerService;
		_logger.LogDebug("RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext}): _customerService → {_customerService}", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext, _customerService);
		_logger.LogDebug("RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext, _dateTimeHelper);
    _logger.LogWarning("<<< Exiting RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext})", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext);
        _localizationService = localizationService;
		_logger.LogDebug("RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext}): _localizationService → {_localizationService}", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext, _localizationService);
        _orderProcessingService = orderProcessingService;
		_logger.LogDebug("RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext}): _orderProcessingService → {_orderProcessingService}", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext, _orderProcessingService);
        _orderService = orderService;
		_logger.LogDebug("RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext}): _orderService → {_orderService}", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext, _orderService);
        _paymentService = paymentService;
		_logger.LogDebug("RecurringPaymentModelFactory(dateTimeHelper={DateTimeHelper},customerService={CustomerService},localizationService={LocalizationService},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},workContext={WorkContext}): _paymentService → {_paymentService}", dateTimeHelper, customerService, localizationService, orderProcessingService, orderService, paymentService, workContext, _paymentService);
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare recurring payment history search model
    /// </summary>
    /// <param name="searchModel">Recurring payment history search model</param>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>Recurring payment history search model</returns>
    protected virtual RecurringPaymentHistorySearchModel PrepareRecurringPaymentHistorySearchModel(RecurringPaymentHistorySearchModel searchModel,
        RecurringPayment recurringPayment)
    {
		_logger.LogError(">>> Entering PrepareRecurringPaymentHistorySearchModel(searchModel={SearchModel},recurringPayment={RecurringPayment})", searchModel, recurringPayment);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(recurringPayment);

        searchModel.RecurringPaymentId = recurringPayment.Id;
		_logger.LogDebug("PrepareRecurringPaymentHistorySearchModel(searchModel={SearchModel},recurringPayment={RecurringPayment}): searchModel.RecurringPaymentId → {Arg2}", searchModel, recurringPayment, searchModel.RecurringPaymentId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareRecurringPaymentHistorySearchModel(searchModel={SearchModel},recurringPayment={RecurringPayment})", searchModel, recurringPayment);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare recurring payment search model
    /// </summary>
    /// <param name="searchModel">Recurring payment search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the recurring payment search model
    /// </returns>
    public virtual Task<RecurringPaymentSearchModel> PrepareRecurringPaymentSearchModelAsync(RecurringPaymentSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareRecurringPaymentSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareRecurringPaymentSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged recurring payment list model
    /// </summary>
    /// <param name="searchModel">Recurring payment search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the recurring payment list model
    /// </returns>
    public virtual async Task<RecurringPaymentListModel> PrepareRecurringPaymentListModelAsync(RecurringPaymentSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareRecurringPaymentListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get recurringPayments
        var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(showHidden: true,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new RecurringPaymentListModel().PrepareToGridAsync(searchModel, recurringPayments, () =>
        {
            return recurringPayments.SelectAwait(async recurringPayment =>
            {
                //fill in model values from the entity
                var recurringPaymentModel = recurringPayment.ToModel<RecurringPaymentModel>();
				_logger.LogError("PrepareRecurringPaymentListModelAsync(searchModel={SearchModel}): recurringPaymentModel → {RecurringPaymentModel}", searchModel, recurringPaymentModel);

                var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
				_logger.LogError("PrepareRecurringPaymentListModelAsync(searchModel={SearchModel}): order → {Order}", searchModel, order);
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
				_logger.LogError("PrepareRecurringPaymentListModelAsync(searchModel={SearchModel}): customer → {Customer}", searchModel, customer);

                //convert dates to the user time
                if ((await _orderProcessingService.GetNextPaymentDateAsync(recurringPayment)) is DateTime nextPaymentDate)
                {
                    recurringPaymentModel.NextPaymentDate = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(nextPaymentDate, DateTimeKind.Utc)).ToString(CultureInfo.InvariantCulture);
                    recurringPaymentModel.CyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(recurringPayment);
                }

                recurringPaymentModel.StartDate = (await _dateTimeHelper
                    .ConvertToUserTimeAsync(recurringPayment.StartDateUtc, DateTimeKind.Utc)).ToString(CultureInfo.InvariantCulture);

                //fill in additional values (not existing in the entity)
                recurringPaymentModel.CustomerId = customer.Id;
				_logger.LogDebug("PrepareRecurringPaymentListModelAsync(searchModel={SearchModel}): recurringPaymentModel.CustomerId → {Arg1}", searchModel, recurringPaymentModel.CustomerId);
                recurringPaymentModel.InitialOrderId = order.Id;
				_logger.LogDebug("PrepareRecurringPaymentListModelAsync(searchModel={SearchModel}): recurringPaymentModel.InitialOrderId → {Arg1}", searchModel, recurringPaymentModel.InitialOrderId);

                recurringPaymentModel.CyclePeriodStr = await _localizationService.GetLocalizedEnumAsync(recurringPayment.CyclePeriod);
                recurringPaymentModel.CustomerEmail = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");

                return recurringPaymentModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareRecurringPaymentListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare recurring payment model
    /// </summary>
    /// <param name="model">Recurring payment model</param>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the recurring payment model
    /// </returns>
    public virtual async Task<RecurringPaymentModel> PrepareRecurringPaymentModelAsync(RecurringPaymentModel model,
        RecurringPayment recurringPayment, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties})", model, recurringPayment, excludeProperties);
        if (recurringPayment == null)
            return model;

        //fill in model values from the entity
        if (model == null)
            model = recurringPayment.ToModel<RecurringPaymentModel>();
			_logger.LogError("PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties}): model → {Model1}", model, recurringPayment, excludeProperties, model);

        var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
		_logger.LogError("PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties}): order → {Order}", model, recurringPayment, excludeProperties, order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
		_logger.LogError("PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties}): customer → {Customer}", model, recurringPayment, excludeProperties, customer);

        //convert dates to the user time
        if (await _orderProcessingService.GetNextPaymentDateAsync(recurringPayment) is DateTime nextPaymentDate)
        {
            model.NextPaymentDate = (await _dateTimeHelper.ConvertToUserTimeAsync(nextPaymentDate, DateTimeKind.Utc)).ToString(CultureInfo.InvariantCulture);
            model.CyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(recurringPayment);
        }
        model.StartDate = (await _dateTimeHelper.ConvertToUserTimeAsync(recurringPayment.StartDateUtc, DateTimeKind.Utc)).ToString(CultureInfo.InvariantCulture);

        model.CustomerId = customer.Id;
		_logger.LogDebug("PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties}): model.CustomerId → {Arg3}", model, recurringPayment, excludeProperties, model.CustomerId);
        model.InitialOrderId = order.Id;
		_logger.LogDebug("PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties}): model.InitialOrderId → {Arg3}", model, recurringPayment, excludeProperties, model.InitialOrderId);
        model.CustomerEmail = await _customerService.IsRegisteredAsync(customer)
            ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
        model.PaymentType = await _localizationService.GetLocalizedEnumAsync(await _paymentService
            .GetRecurringPaymentTypeAsync(order.PaymentMethodSystemName));

        model.CanCancelRecurringPayment = await _orderProcessingService.CanCancelRecurringPaymentAsync(await _workContext.GetCurrentCustomerAsync(), recurringPayment);

        //prepare nested search model
        PrepareRecurringPaymentHistorySearchModel(model.RecurringPaymentHistorySearchModel, recurringPayment);

		_logger.LogWarning("<<< Exiting PrepareRecurringPaymentModelAsync(model={Model},recurringPayment={RecurringPayment},excludeProperties={ExcludeProperties})", model, recurringPayment, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged recurring payment history list model
    /// </summary>
    /// <param name="searchModel">Recurring payment history search model</param>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the recurring payment history list model
    /// </returns>
    public virtual async Task<RecurringPaymentHistoryListModel> PrepareRecurringPaymentHistoryListModelAsync(RecurringPaymentHistorySearchModel searchModel,
        RecurringPayment recurringPayment)
    {
		_logger.LogError(">>> Entering PrepareRecurringPaymentHistoryListModelAsync(searchModel={SearchModel},recurringPayment={RecurringPayment})", searchModel, recurringPayment);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(recurringPayment);

        //get recurring payments history
        var recurringPayments = (await _orderService.GetRecurringPaymentHistoryAsync(recurringPayment))
            .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new RecurringPaymentHistoryListModel().PrepareToGridAsync(searchModel, recurringPayments, () =>
        {
            return recurringPayments.SelectAwait(async historyEntry =>
            {
                //fill in model values from the entity
                var historyModel = historyEntry.ToModel<RecurringPaymentHistoryModel>();
				_logger.LogError("PrepareRecurringPaymentHistoryListModelAsync(searchModel={SearchModel},recurringPayment={RecurringPayment}): historyModel → {HistoryModel}", searchModel, recurringPayment, historyModel);

                //convert dates to the user time
                historyModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                var order = await _orderService.GetOrderByIdAsync(historyEntry.OrderId);
				_logger.LogError("PrepareRecurringPaymentHistoryListModelAsync(searchModel={SearchModel},recurringPayment={RecurringPayment}): order → {Order}", searchModel, recurringPayment, order);
                if (order == null)
                    return historyModel;

                historyModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                historyModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                historyModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                historyModel.CustomOrderNumber = order.CustomOrderNumber;
				_logger.LogDebug("PrepareRecurringPaymentHistoryListModelAsync(searchModel={SearchModel},recurringPayment={RecurringPayment}): historyModel.CustomOrderNumber → {Arg2}", searchModel, recurringPayment, historyModel.CustomOrderNumber);

                return historyModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareRecurringPaymentHistoryListModelAsync(searchModel={SearchModel},recurringPayment={RecurringPayment})", searchModel, recurringPayment);
        return model;
    }

    #endregion
}