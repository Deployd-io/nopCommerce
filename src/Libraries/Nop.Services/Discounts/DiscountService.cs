using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Discounts;

/// <summary>
/// Discount service
/// </summary>
public partial class DiscountService : IDiscountService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IDiscountPluginManager _discountPluginManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly IProductService _productService;
    protected readonly IRepository<Discount> _discountRepository;
    protected readonly IRepository<DiscountRequirement> _discountRequirementRepository;
    protected readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public DiscountService(ICustomerService customerService,
        IDiscountPluginManager discountPluginManager,
        ILocalizationService localizationService,
        IProductService productService,
        IRepository<Discount> discountRepository,
        IRepository<DiscountRequirement> discountRequirementRepository,
        IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
        IRepository<Order> orderRepository,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext)
    {
		_logger.LogDebug(">>> Entering DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext})", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext);
        _customerService = customerService;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _customerService → {_customerService}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _customerService);
        _discountPluginManager = discountPluginManager;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _discountPluginManager → {_discountPluginManager}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _discountPluginManager);
        _localizationService = localizationService;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _localizationService → {_localizationService}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _localizationService);
        _productService = productService;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _productService → {_productService}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _productService);
        _discountRepository = discountRepository;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _discountRepository → {_discountRepository}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _discountRepository);
        _discountRequirementRepository = discountRequirementRepository;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _discountRequirementRepository → {_discountRequirementRepository}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _discountRequirementRepository);
        _discountUsageHistoryRepository = discountUsageHistoryRepository;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _discountUsageHistoryRepository → {_discountUsageHistoryRepository}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _discountUsageHistoryRepository);
        _orderRepository = orderRepository;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _orderRepository → {_orderRepository}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _orderRepository);
        _shortTermCacheManager = shortTermCacheManager;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _shortTermCacheManager → {_shortTermCacheManager}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _shortTermCacheManager);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _staticCacheManager → {_staticCacheManager}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("DiscountService(customerService={CustomerService},discountPluginManager={DiscountPluginManager},localizationService={LocalizationService},productService={ProductService},discountRepository={DiscountRepository},discountRequirementRepository={DiscountRequirementRepository},discountUsageHistoryRepository={DiscountUsageHistoryRepository},orderRepository={OrderRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext}): _storeContext → {_storeContext}", customerService, discountPluginManager, localizationService, productService, discountRepository, discountRequirementRepository, discountUsageHistoryRepository, orderRepository, shortTermCacheManager, staticCacheManager, storeContext, _storeContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get discount validation result
    /// </summary>
    /// <param name="requirementsToValidate">Collection of discount requirement to validate rules</param>
    /// <param name="allRequirements">Collection of all discount requirement related to discount</param>
    /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
    /// <param name="customer">Customer</param>
    /// <param name="errors">Errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if result is valid; otherwise false
    /// </returns>
    protected virtual async Task<bool> GetValidationResultAsync(IList<DiscountRequirement> requirementsToValidate, IList<DiscountRequirement> allRequirements,
        RequirementGroupInteractionType groupInteractionType, Customer customer, List<string> errors)
    {
		_logger.LogDebug(">>> Entering GetValidationResultAsync(requirementsToValidate={RequirementsToValidate},allRequirements={AllRequirements},groupInteractionType=<redacted>,customer={Customer},errors={Errors})", requirementsToValidate, allRequirements, customer, errors);
        var result = false;

		_logger.LogDebug("GetValidationResultAsync(requirementsToValidate={RequirementsToValidate},allRequirements={AllRequirements},groupInteractionType=<redacted>,customer={Customer},errors={Errors}): result → {Result}", requirementsToValidate, allRequirements, customer, errors, result);
        var requirementsForCheck = requirementsToValidate.Any(r => !r.ParentId.HasValue)
            ? requirementsToValidate.Where(r => !r.ParentId.HasValue)
            : requirementsToValidate;

        foreach (var requirement in requirementsForCheck)
        {
            if (requirement.IsGroup)
            {
                var childRequirements = allRequirements.Where(r => r.ParentId == requirement.Id).ToList();
				_logger.LogDebug("GetValidationResultAsync(requirementsToValidate={RequirementsToValidate},allRequirements={AllRequirements},groupInteractionType=<redacted>,customer={Customer},errors={Errors}): childRequirements → {ChildRequirements}", requirementsToValidate, allRequirements, customer, errors, childRequirements);

                //get child requirements for the group
                var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                result = await GetValidationResultAsync(childRequirements, allRequirements, interactionType, customer, errors);
            }
            else
            {
                //or try to get validation result for the requirement
                var store = await _storeContext.GetCurrentStoreAsync();
				_logger.LogDebug("GetValidationResultAsync(requirementsToValidate={RequirementsToValidate},allRequirements={AllRequirements},groupInteractionType=<redacted>,customer={Customer},errors={Errors}): store → {Store}", requirementsToValidate, allRequirements, customer, errors, store);
                var requirementRulePlugin = await _discountPluginManager
                    .LoadPluginBySystemNameAsync(requirement.DiscountRequirementRuleSystemName, customer, store.Id);

                if (requirementRulePlugin == null)
                    continue;

                var ruleResult = await requirementRulePlugin.CheckRequirementAsync(new DiscountRequirementValidationRequest
                {
                    DiscountRequirementId = requirement.Id,
                    Customer = customer,
                    Store = store
                });

                //add validation error
                if (!ruleResult.IsValid)
                {
                    var userError = !string.IsNullOrEmpty(ruleResult.UserError)
                        ? ruleResult.UserError
                        : await _localizationService.GetResourceAsync("ShoppingCart.Discount.CannotBeUsed");
                    errors.Add(userError);
                }

                result = ruleResult.IsValid;
            }

            //all requirements must be met, so return false
            if (!result && groupInteractionType == RequirementGroupInteractionType.And)
                return false;

            //any of requirements must be met, so return true
            if (result && groupInteractionType == RequirementGroupInteractionType.Or)
                return true;
        }

        return result;
    }
    
    /// <summary>
    /// Get discount validation result
    /// </summary>
    /// <param name="requirements">Collection of discount requirement to validate rules</param>
    /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
    /// <param name="customer">Customer</param>
    /// <param name="errors">Errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if result is valid; otherwise false
    /// </returns>
    protected virtual async Task<bool> GetValidationResultAsync(IList<DiscountRequirement> requirements, RequirementGroupInteractionType groupInteractionType, Customer customer, List<string> errors)
    {
        return await GetValidationResultAsync(requirements, requirements, groupInteractionType, customer, errors);
    }

    #endregion

    #region Methods

    #region Discounts

    /// <summary>
    /// Delete discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDiscountAsync(Discount discount)
    {
		_logger.LogDebug(">>> Entering DeleteDiscountAsync(discount={Discount})", discount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //first, delete related discount requirements
        await _discountRequirementRepository.DeleteAsync(await GetAllDiscountRequirementsAsync(discount.Id));
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteDiscountAsync(discount)={Discount}: delete query executed in {Arg1} ms", discount, __logStartDb.ElapsedMilliseconds);
		_logger.LogInformation("DeleteDiscountAsync(discount)={Discount}: external service call took {Arg1} ms", discount, __logStart.ElapsedMilliseconds);

        //then delete the discount
		_logger.LogDebug("<<< Exiting DeleteDiscountAsync(discount={Discount})", discount);
        await _discountRepository.DeleteAsync(discount);
    }

    /// <summary>
    /// Gets a discount
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount
    /// </returns>
    public virtual async Task<Discount> GetDiscountByIdAsync(int discountId)
    {
		_logger.LogDebug(">>> Entering GetDiscountByIdAsync(discountId={DiscountId})", discountId);
		_logger.LogInformation("GetDiscountByIdAsync(discountId)={DiscountId}: find query executing", discountId);
		_logger.LogDebug(">>> Entering GetAllDiscountRequirementsAsync(discountId={DiscountId},topLevelOnly={TopLevelOnly})", discountId, topLevelOnly);
		_logger.LogInformation("GetAllDiscountRequirementsAsync(discountId,topLevelOnly)={DiscountId},{TopLevelOnly}: find query executing", discountId, topLevelOnly);
		_logger.LogDebug("<<< Exiting GetAllDiscountRequirementsAsync(discountId={DiscountId},topLevelOnly={TopLevelOnly})", discountId, topLevelOnly);
        return await _discountRepository.GetByIdAsync(discountId, cache => default);
    }

    /// <summary>
    /// Gets all discounts
    /// </summary>
    /// <param name="discountType">Discount type; pass null to load all records</param>
    /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
    /// <param name="discountName">Discount name; pass null or empty to load all records</param>
    /// <param name="showHidden">A value indicating whether to show expired and not started discounts</param>
    /// <param name="startDateUtc">Discount start date; pass null to load all records</param>
    /// <param name="endDateUtc">Discount end date; pass null to load all records</param>
    /// <param name="isActive">A value indicating whether to get active discounts; "null" to load all discounts; "false" to load only inactive discounts; "true" to load only active discounts</param>
    /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discounts
    /// </returns>
    public virtual async Task<IList<Discount>> GetAllDiscountsAsync(DiscountType? discountType = null,
        string couponCode = null, string discountName = null, bool showHidden = false,
        DateTime? startDateUtc = null, DateTime? endDateUtc = null, bool? isActive = true, int vendorId = 0)
    {
		_logger.LogDebug(">>> Entering GetAllDiscountsAsync(discountType={DiscountType},couponCode={CouponCode},discountName={DiscountName},showHidden={ShowHidden},startDateUtc={StartDateUtc},endDateUtc={EndDateUtc},isActive={IsActive},vendorId={VendorId})", discountType, couponCode, discountName, showHidden, startDateUtc, endDateUtc, isActive, vendorId);
        //we load all discounts, and filter them using "discountType" and dates later (in memory)
        //we do it because we know that this method is invoked several times per HTTP request with distinct "discountType" parameter and date filters
        //that's why let's access the database only once
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var discounts = (await _discountRepository.GetAllAsync(query =>
            {
				_logger.LogDebug("GetAllDiscountsAsync(discountType={DiscountType},couponCode={CouponCode},discountName={DiscountName},showHidden={ShowHidden},startDateUtc={StartDateUtc},endDateUtc={EndDateUtc},isActive={IsActive},vendorId={VendorId}): discounts → {Discounts}", discountType, couponCode, discountName, showHidden, startDateUtc, endDateUtc, isActive, vendorId, discounts);
                //filter by coupon code
                if (!string.IsNullOrEmpty(couponCode))
                    query = query.Where(discount => discount.CouponCode == couponCode);

				_logger.LogInformation("GetAllDiscountsAsync(discountType,couponCode,discountName,showHidden,startDateUtc,endDateUtc,isActive,vendorId)={DiscountType},{CouponCode},{DiscountName},{ShowHidden},{StartDateUtc},{EndDateUtc},{IsActive},{VendorId}: find query executed in {Arg8} ms", discountType, couponCode, discountName, showHidden, startDateUtc, endDateUtc, isActive, vendorId, __logStart.ElapsedMilliseconds);
				_logger.LogDebug("GetAllDiscountsAsync(discountType={DiscountType},couponCode={CouponCode},discountName={DiscountName},showHidden={ShowHidden},startDateUtc={StartDateUtc},endDateUtc={EndDateUtc},isActive={IsActive},vendorId={VendorId}): query → {Query}", discountType, couponCode, discountName, showHidden, startDateUtc, endDateUtc, isActive, vendorId, query);
                //filter by name
                if (!string.IsNullOrEmpty(discountName))
                    query = query.Where(discount => discount.Name.Contains(discountName));

                //filter by is active
                if (isActive.HasValue)
                    query = query.Where(discount => discount.IsActive == isActive.Value);

                query = query.OrderBy(discount => discount.Name).ThenBy(discount => discount.Id);

                return query;
            }, cache => cache.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountAllCacheKey,
                couponCode ?? string.Empty, discountName ?? string.Empty, isActive)))
            .AsQueryable();
        
        if (discountType.HasValue)
            discounts = discounts.Where(discount => discount.DiscountType == discountType.Value);

        //filter by dates
        if (!showHidden)
            discounts = discounts.Where(discount =>
                (!discount.StartDateUtc.HasValue || discount.StartDateUtc <= DateTime.UtcNow) &&
                (!discount.EndDateUtc.HasValue || discount.EndDateUtc >= DateTime.UtcNow));

        if (startDateUtc.HasValue)
            discounts = discounts.Where(discount =>
                !discount.StartDateUtc.HasValue || discount.StartDateUtc >= startDateUtc.Value);
        if (endDateUtc.HasValue)
            discounts = discounts.Where(discount =>
                !discount.EndDateUtc.HasValue || discount.EndDateUtc <= endDateUtc.Value);

        if (vendorId > 0)
            discounts = discounts.Where(discount => discount.VendorId == vendorId);

		_logger.LogDebug("<<< Exiting GetAllDiscountsAsync(discountType={DiscountType},couponCode={CouponCode},discountName={DiscountName},showHidden={ShowHidden},startDateUtc={StartDateUtc},endDateUtc={EndDateUtc},isActive={IsActive},vendorId={VendorId})", discountType, couponCode, discountName, showHidden, startDateUtc, endDateUtc, isActive, vendorId);
		_logger.LogInformation("GetAllDiscountsAsync(discountType,couponCode,discountName,showHidden,startDateUtc,endDateUtc,isActive,vendorId)={DiscountType},{CouponCode},{DiscountName},{ShowHidden},{StartDateUtc},{EndDateUtc},{IsActive},{VendorId}: database query executing", discountType, couponCode, discountName, showHidden, startDateUtc, endDateUtc, isActive, vendorId);
        return await discounts.ToListAsync();
    }

    /// <summary>
    /// Gets discounts applied to entity
    /// </summary>
    /// <typeparam name="T">Type based on <see cref="DiscountMapping" /></typeparam>
    /// <param name="entity">Entity which supports discounts (<see cref="IDiscountSupported{T}" />)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of discounts
    /// </returns>
    public virtual async Task<IList<Discount>> GetAppliedDiscountsAsync<T>(IDiscountSupported<T> entity) where T : DiscountMapping
    {
		_logger.LogDebug(">>> Entering GetAppliedDiscountsAsync(entity={Entity})", entity);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var discountMappingRepository = EngineContext.Current.Resolve<IRepository<T>>();

		_logger.LogDebug("GetAppliedDiscountsAsync(entity={Entity}): discountMappingRepository → {DiscountMappingRepository}", entity, discountMappingRepository);
        var appliedDiscounts = await _shortTermCacheManager.GetAsync(async () =>
        {
            return await (from d in _discountRepository.Table
                join ad in discountMappingRepository.Table on d.Id equals ad.DiscountId
                where ad.EntityId == entity.Id
                select d).ToListAsync();
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogInformation("GetAppliedDiscountsAsync(entity)={Entity}: find query executed in {Arg1} ms", entity, __logStartDb.ElapsedMilliseconds);
				_logger.LogInformation("GetAppliedDiscountsAsync(entity)={Entity}: external service call took {Arg1} ms", entity, __logStart.ElapsedMilliseconds);
        }, NopDiscountDefaults.AppliedDiscountsCacheKey, entity.GetType().Name, entity);

		_logger.LogDebug("<<< Exiting GetAppliedDiscountsAsync(entity={Entity})", entity);
        return appliedDiscounts;
    }

    /// <summary>
    /// Inserts a discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDiscountAsync(Discount discount)
    {
		_logger.LogDebug(">>> Entering InsertDiscountAsync(discount={Discount})", discount);
        await _discountRepository.InsertAsync(discount);
    }

    /// <summary>
    /// Updates the discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateDiscountAsync(Discount discount)
    {
		_logger.LogDebug(">>> Entering UpdateDiscountAsync(discount={Discount})", discount);
        await _discountRepository.UpdateAsync(discount);
    }

    #endregion

    #region Discounts (caching)

    /// <summary>
    /// Gets the discount amount for the specified value
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <param name="amount">Amount</param>
    /// <returns>The discount amount</returns>
    public virtual decimal GetDiscountAmount(Discount discount, decimal amount)
    {
		_logger.LogDebug(">>> Entering GetDiscountAmount(discount={Discount},amount={Amount})", discount, amount);
        ArgumentNullException.ThrowIfNull(discount);

        //calculate discount amount
        decimal result;
        if (discount.UsePercentage)
            result = (decimal)((float)amount * (float)discount.DiscountPercentage / 100f);
        else
            result = discount.DiscountAmount;

        //validate maximum discount amount
        if (discount.UsePercentage &&
            discount.MaximumDiscountAmount.HasValue &&
            result > discount.MaximumDiscountAmount.Value)
            result = discount.MaximumDiscountAmount.Value;

        if (result < decimal.Zero)
            result = decimal.Zero;

		_logger.LogDebug("<<< Exiting GetDiscountAmount(discount={Discount},amount={Amount})", discount, amount);
        return result;
    }

    /// <summary>
    /// Get preferred discount (with maximum discount value)
    /// </summary>
    /// <param name="discounts">A list of discounts to check</param>
    /// <param name="amount">Amount (initial value)</param>
    /// <param name="discountAmount">Discount amount</param>
    /// <returns>Preferred discount</returns>
    public virtual List<Discount> GetPreferredDiscount(IList<Discount> discounts,
        decimal amount, out decimal discountAmount)
    {
		_logger.LogDebug(">>> Entering GetPreferredDiscount(discounts={Discounts},amount={Amount})", discounts, amount);
        ArgumentNullException.ThrowIfNull(discounts);

        var result = new List<Discount>();
		_logger.LogDebug("GetPreferredDiscount(discounts={Discounts},amount={Amount}): result → {Result}", discounts, amount, result);
        discountAmount = decimal.Zero;
        if (!discounts.Any())
            return result;

        //first we check simple discounts
        foreach (var discount in discounts)
        {
            var currentDiscountValue = GetDiscountAmount(discount, amount);
			_logger.LogDebug("GetPreferredDiscount(discounts={Discounts},amount={Amount}): currentDiscountValue → {CurrentDiscountValue}", discounts, amount, currentDiscountValue);
            if (currentDiscountValue <= discountAmount)
                continue;

            discountAmount = currentDiscountValue;

            result.Clear();
            result.Add(discount);
        }
        //now let's check cumulative discounts
        //right now we calculate discount values based on the original amount value
        //please keep it in mind if you're going to use discounts with "percentage"
        var cumulativeDiscounts = discounts.Where(x => x.IsCumulative).OrderBy(x => x.Name).ToList();
		_logger.LogDebug("GetPreferredDiscount(discounts={Discounts},amount={Amount}): cumulativeDiscounts → {CumulativeDiscounts}", discounts, amount, cumulativeDiscounts);
        if (cumulativeDiscounts.Count <= 1)
            return result;

        var cumulativeDiscountAmount = cumulativeDiscounts.Sum(d => GetDiscountAmount(d, amount));
		_logger.LogDebug("GetPreferredDiscount(discounts={Discounts},amount={Amount}): cumulativeDiscountAmount → {CumulativeDiscountAmount}", discounts, amount, cumulativeDiscountAmount);
        if (cumulativeDiscountAmount <= discountAmount)
            return result;

        discountAmount = cumulativeDiscountAmount;

        result.Clear();
        result.AddRange(cumulativeDiscounts);

		_logger.LogDebug("<<< Exiting GetPreferredDiscount(discounts={Discounts},amount={Amount})", discounts, amount);
        return result;
    }

    /// <summary>
    /// Check whether a list of discounts already contains a certain discount instance
    /// </summary>
    /// <param name="discounts">A list of discounts</param>
    /// <param name="discount">Discount to check</param>
    /// <returns>Result</returns>
    public virtual bool ContainsDiscount(IList<Discount> discounts, Discount discount)
    {
		_logger.LogDebug(">>> Entering ContainsDiscount(discounts={Discounts},discount={Discount})", discounts, discount);
        ArgumentNullException.ThrowIfNull(discounts);

        ArgumentNullException.ThrowIfNull(discount);

		_logger.LogDebug("<<< Exiting ContainsDiscount(discounts={Discounts},discount={Discount})", discounts, discount);
        return discounts.Any(dis1 => discount.Id == dis1.Id);
    }

    #endregion

    #region Discount requirements

    /// <summary>
    /// Get all discount requirements
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <param name="topLevelOnly">Whether to load top-level requirements only (without parent identifier)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the requirements
    /// </returns>
    public virtual async Task<IList<DiscountRequirement>> GetAllDiscountRequirementsAsync(int discountId = 0, bool topLevelOnly = false)
    {
        return await _discountRequirementRepository.GetAllAsync(query =>
        {
			_logger.LogInformation("GetAllDiscountRequirementsAsync(discountId,topLevelOnly)={DiscountId},{TopLevelOnly}: find query executing", discountId, topLevelOnly);
            //filter by discount
            if (discountId > 0)
                query = query.Where(requirement => requirement.DiscountId == discountId);

            //filter by top-level
            if (topLevelOnly)
                query = query.Where(requirement => !requirement.ParentId.HasValue);

            query = query.OrderBy(requirement => requirement.Id);

            return query;
        });
    }

    /// <summary>
    /// Get a discount requirement
    /// </summary>
    /// <param name="discountRequirementId">Discount requirement identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<DiscountRequirement> GetDiscountRequirementByIdAsync(int discountRequirementId)
    {
		_logger.LogDebug(">>> Entering GetDiscountRequirementByIdAsync(discountRequirementId={DiscountRequirementId})", discountRequirementId);
		_logger.LogInformation("GetDiscountRequirementByIdAsync(discountRequirementId)={DiscountRequirementId}: find query executing", discountRequirementId);
        return await _discountRequirementRepository.GetByIdAsync(discountRequirementId, cache => default);
    }

    /// <summary>
    /// Gets child discount requirements
    /// </summary>
    /// <param name="discountRequirement">Parent discount requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<DiscountRequirement>> GetDiscountRequirementsByParentAsync(DiscountRequirement discountRequirement)
    {
		_logger.LogDebug(">>> Entering GetDiscountRequirementsByParentAsync(discountRequirement={DiscountRequirement})", discountRequirement);
        ArgumentNullException.ThrowIfNull(discountRequirement);

		_logger.LogDebug("<<< Exiting GetDiscountRequirementsByParentAsync(discountRequirement={DiscountRequirement})", discountRequirement);
        return await _discountRequirementRepository.GetAllAsync(
            query => query.Where(dr => dr.ParentId == discountRequirement.Id),
            cache => cache.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountRequirementsByParentCacheKey, discountRequirement));
    }

    /// <summary>
    /// Delete discount requirement
    /// </summary>
    /// <param name="discountRequirement">Discount requirement</param>
    /// <param name="recursive">A value indicating whether to recursively delete child requirements</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDiscountRequirementAsync(DiscountRequirement discountRequirement, bool recursive = false)
    {
		_logger.LogDebug(">>> Entering DeleteDiscountRequirementAsync(discountRequirement={DiscountRequirement},recursive={Recursive})", discountRequirement, recursive);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(discountRequirement);

        if (recursive && await GetDiscountRequirementsByParentAsync(discountRequirement) is IList<DiscountRequirement> children && children.Any())
            foreach (var child in children)
                await DeleteDiscountRequirementAsync(child, true);
				_logger.LogDebug("<<< Exiting DeleteDiscountRequirementAsync(discountRequirement={DiscountRequirement},recursive={Recursive})", discountRequirement, recursive);
				_logger.LogInformation("DeleteDiscountRequirementAsync(discountRequirement,recursive)={DiscountRequirement},{Recursive}: external service call took {Arg2} ms", discountRequirement, recursive, __logStart.ElapsedMilliseconds);
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();

        await _discountRequirementRepository.DeleteAsync(discountRequirement);
    }

    /// <summary>
    /// Inserts a discount requirement
    /// </summary>
    /// <param name="discountRequirement">Discount requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDiscountRequirementAsync(DiscountRequirement discountRequirement)
    {
		_logger.LogDebug(">>> Entering InsertDiscountRequirementAsync(discountRequirement={DiscountRequirement})", discountRequirement);
        await _discountRequirementRepository.InsertAsync(discountRequirement);
    }

    /// <summary>
    /// Updates a discount requirement
    /// </summary>
    /// <param name="discountRequirement">Discount requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateDiscountRequirementAsync(DiscountRequirement discountRequirement)
    {
		_logger.LogDebug(">>> Entering UpdateDiscountRequirementAsync(discountRequirement={DiscountRequirement})", discountRequirement);
        await _discountRequirementRepository.UpdateAsync(discountRequirement);
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validate discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <param name="customer">Customer</param>
    /// <param name="couponCodesToValidate">Coupon codes to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount validation result
    /// </returns>
    public virtual async Task<DiscountValidationResult> ValidateDiscountAsync(Discount discount, Customer customer, string[] couponCodesToValidate)
    {
		_logger.LogDebug(">>> Entering ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate})", discount, customer, couponCodesToValidate);
        ArgumentNullException.ThrowIfNull(discount);

        ArgumentNullException.ThrowIfNull(customer);

        //invalid by default
        var result = new DiscountValidationResult();
		_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): result → {Result}", discount, customer, couponCodesToValidate, result);

        //check discount is active
        if (!discount.IsActive)
            return result;

        //check coupon code
        if (discount.RequiresCouponCode)
        {
            if (string.IsNullOrEmpty(discount.CouponCode))
                return result;

            if (couponCodesToValidate == null)
                return result;

            if (!couponCodesToValidate.Any(x => x.Equals(discount.CouponCode, StringComparison.InvariantCultureIgnoreCase)))
                return result;
        }

        //Do not allow discounts applied to order subtotal or total when a customer has gift cards in the cart.
        //Otherwise, this customer can purchase gift cards with discount and get more than paid ("free money").
        if (discount.DiscountType == DiscountType.AssignedToOrderSubTotal ||
            discount.DiscountType == DiscountType.AssignedToOrderTotal)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): store → {Store}", discount, customer, couponCodesToValidate, store);

            //do not inject IShoppingCartService via constructor because it'll cause circular references
            var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): shoppingCartService → {ShoppingCartService}", discount, customer, couponCodesToValidate, shoppingCartService);
            var cart = await shoppingCartService.GetShoppingCartAsync(customer,
                ShoppingCartType.ShoppingCart, storeId: store.Id);

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): cartProductIds → {CartProductIds}", discount, customer, couponCodesToValidate, cartProductIds);

            if (await _productService.HasAnyGiftCardProductAsync(cartProductIds))
            {
                result.Errors = new List<string> { await _localizationService.GetResourceAsync("ShoppingCart.Discount.CannotBeUsedWithGiftCards") };
                return result;
            }
        }

        //check date range
        var now = DateTime.UtcNow;
        if (discount.StartDateUtc.HasValue)
        {
            var startDate = DateTime.SpecifyKind(discount.StartDateUtc.Value, DateTimeKind.Utc);
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): startDate → {StartDate}", discount, customer, couponCodesToValidate, startDate);
            if (startDate.CompareTo(now) > 0)
            {
                result.Errors = new List<string> { await _localizationService.GetResourceAsync("ShoppingCart.Discount.NotStartedYet") };
                return result;
            }
        }

        if (discount.EndDateUtc.HasValue)
        {
            var endDate = DateTime.SpecifyKind(discount.EndDateUtc.Value, DateTimeKind.Utc);
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): endDate → {EndDate}", discount, customer, couponCodesToValidate, endDate);
            if (endDate.CompareTo(now) < 0)
            {
                result.Errors = new List<string> { await _localizationService.GetResourceAsync("ShoppingCart.Discount.Expired") };
                return result;
            }
        }

        //discount limitation
        switch (discount.DiscountLimitation)
        {
            case DiscountLimitationType.NTimesOnly:
            {
                var usedTimes = (await GetAllDiscountUsageHistoryAsync(discount.Id, null, null, false, 0, 1)).TotalCount;
				_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): usedTimes → {UsedTimes}", discount, customer, couponCodesToValidate, usedTimes);
                if (usedTimes >= discount.LimitationTimes)
                    return result;
            }

                break;
            case DiscountLimitationType.NTimesPerCustomer:
            {
                if (await _customerService.IsRegisteredAsync(customer))
                {
                    var usedTimes = (await GetAllDiscountUsageHistoryAsync(discount.Id, customer.Id, null, false, 0, 1)).TotalCount;
                    if (usedTimes >= discount.LimitationTimes)
                    {
                        result.Errors = new List<string> { await _localizationService.GetResourceAsync("ShoppingCart.Discount.CannotBeUsedAnymore") };

                        return result;
                    }
                }
				_logger.LogDebug(">>> Entering GetAllDiscountUsageHistoryAsync(discountId={DiscountId},customerId={CustomerId},orderId={OrderId},includeCancelledOrders={IncludeCancelledOrders},pageIndex={PageIndex},pageSize={PageSize})", discountId, customerId, orderId, includeCancelledOrders, pageIndex, pageSize);
				_logger.LogInformation("GetAllDiscountUsageHistoryAsync(discountId,customerId,orderId,includeCancelledOrders,pageIndex,pageSize)={DiscountId},{CustomerId},{OrderId},{IncludeCancelledOrders},{PageIndex},{PageSize}: find query executing", discountId, customerId, orderId, includeCancelledOrders, pageIndex, pageSize);
				_logger.LogDebug("<<< Exiting GetAllDiscountUsageHistoryAsync(discountId={DiscountId},customerId={CustomerId},orderId={OrderId},includeCancelledOrders={IncludeCancelledOrders},pageIndex={PageIndex},pageSize={PageSize})", discountId, customerId, orderId, includeCancelledOrders, pageIndex, pageSize);
            }

                break;
            case DiscountLimitationType.Unlimited:
            default:
                break;
        }

        //discount requirements
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, discount);
		_logger.LogWarning("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): key → {Key}", discount, customer, couponCodesToValidate, key);

        var requirements = await _staticCacheManager.GetAsync(key, async () => await GetAllDiscountRequirementsAsync(discount.Id));
		_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): requirements → {Requirements}", discount, customer, couponCodesToValidate, requirements);

        //get top-level group
        var topLevelGroup = requirements.FirstOrDefault(r => !r.ParentId.HasValue);
		_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): topLevelGroup → {TopLevelGroup}", discount, customer, couponCodesToValidate, topLevelGroup);
        if (topLevelGroup == null || !topLevelGroup.InteractionType.HasValue || (topLevelGroup.IsGroup && requirements.All(r => r.ParentId != topLevelGroup.Id)))
        {
            //there are no requirements, so discount is valid
            result.IsValid = true;
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): result.IsValid → {Arg3}", discount, customer, couponCodesToValidate, result.IsValid);

            return result;
        }

        //requirements exist, let's check them
        var errors = new List<string>();
		_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): errors → {Errors}", discount, customer, couponCodesToValidate, errors);

        result.IsValid = await GetValidationResultAsync(requirements, requirements, topLevelGroup.InteractionType.Value, customer, errors);

        //set errors if result is not valid
        if (!result.IsValid)
            result.Errors = errors;
			_logger.LogDebug("ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate}): result.Errors → {Arg3}", discount, customer, couponCodesToValidate, result.Errors);

		_logger.LogInformation("ValidateDiscountAsync(discount,customer,couponCodesToValidate)={Discount},{Customer},{CouponCodesToValidate}: external service call took {Arg3} ms", discount, customer, couponCodesToValidate, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting ValidateDiscountAsync(discount={Discount},customer={Customer},couponCodesToValidate={CouponCodesToValidate})", discount, customer, couponCodesToValidate);
        return result;
    }

    #endregion

    #region Discount usage history

    /// <summary>
    /// Gets a discount usage history record
    /// </summary>
    /// <param name="discountUsageHistoryId">Discount usage history record identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount usage history
    /// </returns>
    public virtual async Task<DiscountUsageHistory> GetDiscountUsageHistoryByIdAsync(int discountUsageHistoryId)
    {
		_logger.LogDebug(">>> Entering GetDiscountUsageHistoryByIdAsync(discountUsageHistoryId={DiscountUsageHistoryId})", discountUsageHistoryId);
        return await _discountUsageHistoryRepository.GetByIdAsync(discountUsageHistoryId);
    }

    /// <summary>
    /// Gets all discount usage history records
    /// </summary>
    /// <param name="discountId">Discount identifier; null to load all records</param>
    /// <param name="customerId">Customer identifier; null to load all records</param>
    /// <param name="orderId">Order identifier; null to load all records</param>
    /// <param name="includeCancelledOrders">Include cancelled orders</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount usage history records
    /// </returns>
    public virtual async Task<IPagedList<DiscountUsageHistory>> GetAllDiscountUsageHistoryAsync(int? discountId = null,
        int? customerId = null, int? orderId = null, bool includeCancelledOrders = true, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return await _discountUsageHistoryRepository.GetAllPagedAsync(query =>
        {
			_logger.LogInformation("GetAllDiscountUsageHistoryAsync(discountId,customerId,orderId,includeCancelledOrders,pageIndex,pageSize)={DiscountId},{CustomerId},{OrderId},{IncludeCancelledOrders},{PageIndex},{PageSize}: find query executing", discountId, customerId, orderId, includeCancelledOrders, pageIndex, pageSize);
            //filter by discount
            if (discountId.HasValue && discountId.Value > 0)
                query = query.Where(historyRecord => historyRecord.DiscountId == discountId.Value);

            //filter by customer
            if (customerId.HasValue && customerId.Value > 0)
                query = from duh in query
                    join order in _orderRepository.Table on duh.OrderId equals order.Id
                    where order.CustomerId == customerId
                    select duh;

            //filter by order
            if (orderId.HasValue && orderId.Value > 0)
                query = query.Where(historyRecord => historyRecord.OrderId == orderId.Value);

            //ignore invalid orders
            query = from duh in query
                join order in _orderRepository.Table on duh.OrderId equals order.Id
                where !order.Deleted && (includeCancelledOrders || order.OrderStatusId != (int)OrderStatus.Cancelled)
                select duh;

            //order
            query = query.OrderByDescending(historyRecord => historyRecord.CreatedOnUtc)
                .ThenBy(historyRecord => historyRecord.Id);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Insert discount usage history record
    /// </summary>
    /// <param name="discountUsageHistory">Discount usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDiscountUsageHistoryAsync(DiscountUsageHistory discountUsageHistory)
    {
		_logger.LogDebug(">>> Entering InsertDiscountUsageHistoryAsync(discountUsageHistory={DiscountUsageHistory})", discountUsageHistory);
        await _discountUsageHistoryRepository.InsertAsync(discountUsageHistory);
    }

    /// <summary>
    /// Delete discount usage history record
    /// </summary>
    /// <param name="discountUsageHistory">Discount usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDiscountUsageHistoryAsync(DiscountUsageHistory discountUsageHistory)
    {
		_logger.LogDebug(">>> Entering DeleteDiscountUsageHistoryAsync(discountUsageHistory={DiscountUsageHistory})", discountUsageHistory);
        await _discountUsageHistoryRepository.DeleteAsync(discountUsageHistory);
    }

    #endregion

    #endregion
}