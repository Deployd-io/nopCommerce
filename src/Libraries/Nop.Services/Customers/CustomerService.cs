using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers;

/// <summary>
/// Customer service
/// </summary>
public partial class CustomerService : ICustomerService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IRepository<Address> _customerAddressRepository;
    protected readonly IRepository<BlogComment> _blogCommentRepository;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;
    protected readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
    protected readonly IRepository<CustomerPassword> _customerPasswordRepository;
    protected readonly IRepository<CustomerRole> _customerRoleRepository;
    protected readonly IRepository<ForumPost> _forumPostRepository;
    protected readonly IRepository<ForumTopic> _forumTopicRepository;
    protected readonly IRepository<GenericAttribute> _gaRepository;
    protected readonly IRepository<NewsComment> _newsCommentRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IRepository<ProductReview> _productReviewRepository;
    protected readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
    protected readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
    protected readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly ShoppingCartSettings _shoppingCartSettings;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public CustomerService(CustomerSettings customerSettings,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        INopDataProvider dataProvider,
        IRepository<Address> customerAddressRepository,
        IRepository<BlogComment> blogCommentRepository,
        IRepository<Customer> customerRepository,
        IRepository<CustomerAddressMapping> customerAddressMappingRepository,
        IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
        IRepository<CustomerPassword> customerPasswordRepository,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<ForumPost> forumPostRepository,
        IRepository<ForumTopic> forumTopicRepository,
        IRepository<GenericAttribute> gaRepository,
        IRepository<NewsComment> newsCommentRepository,
        IRepository<Order> orderRepository,
        IRepository<ProductReview> productReviewRepository,
        IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
        IRepository<PollVotingRecord> pollVotingRecordRepository,
        IRepository<ShoppingCartItem> shoppingCartRepository,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        ShoppingCartSettings shoppingCartSettings,
        TaxSettings taxSettings)
    {
		_logger.LogError(">>> Entering CustomerService()");
        _customerSettings = customerSettings;
        _eventPublisher = eventPublisher;
		_logger.LogDebug("CustomerService(): _eventPublisher → {_eventPublisher}", _eventPublisher);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("CustomerService(): _genericAttributeService → {_genericAttributeService}", _genericAttributeService);
        _dataProvider = dataProvider;
		_logger.LogError("CustomerService(): _dataProvider → {_dataProvider}", _dataProvider);
		_logger.LogDebug("CustomerService(): _dataProvider → {_dataProvider}", _dataProvider);
        _customerAddressRepository = customerAddressRepository;
        _blogCommentRepository = blogCommentRepository;
		_logger.LogDebug("CustomerService(): _customerAddressRepository → {_customerAddressRepository}", _customerAddressRepository);
		_logger.LogDebug("CustomerService(): _blogCommentRepository → {_blogCommentRepository}", _blogCommentRepository);
        _customerRepository = customerRepository;
        _customerAddressMappingRepository = customerAddressMappingRepository;
		_logger.LogDebug("CustomerService(): _customerRepository → {_customerRepository}", _customerRepository);
		_logger.LogDebug("CustomerService(): _customerAddressMappingRepository → {_customerAddressMappingRepository}", _customerAddressMappingRepository);
        _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
        _customerPasswordRepository = customerPasswordRepository;
		_logger.LogDebug("CustomerService(): _customerCustomerRoleMappingRepository → {_customerCustomerRoleMappingRepository}", _customerCustomerRoleMappingRepository);
		_logger.LogDebug("CustomerService(): _customerPasswordRepository → {_customerPasswordRepository}", _customerPasswordRepository);
        _customerRoleRepository = customerRoleRepository;
        _forumPostRepository = forumPostRepository;
		_logger.LogDebug("CustomerService(): _customerRoleRepository → {_customerRoleRepository}", _customerRoleRepository);
		_logger.LogDebug("CustomerService(): _forumPostRepository → {_forumPostRepository}", _forumPostRepository);
        _forumTopicRepository = forumTopicRepository;
        _gaRepository = gaRepository;
		_logger.LogDebug("CustomerService(): _forumTopicRepository → {_forumTopicRepository}", _forumTopicRepository);
		_logger.LogDebug("CustomerService(): _gaRepository → {_gaRepository}", _gaRepository);
        _newsCommentRepository = newsCommentRepository;
        _orderRepository = orderRepository;
		_logger.LogDebug("CustomerService(): _newsCommentRepository → {_newsCommentRepository}", _newsCommentRepository);
		_logger.LogDebug("CustomerService(): _orderRepository → {_orderRepository}", _orderRepository);
        _productReviewRepository = productReviewRepository;
        _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
		_logger.LogDebug("CustomerService(): _productReviewRepository → {_productReviewRepository}", _productReviewRepository);
		_logger.LogDebug("CustomerService(): _productReviewHelpfulnessRepository → {_productReviewHelpfulnessRepository}", _productReviewHelpfulnessRepository);
        _pollVotingRecordRepository = pollVotingRecordRepository;
        _shoppingCartRepository = shoppingCartRepository;
		_logger.LogDebug("CustomerService(): _pollVotingRecordRepository → {_pollVotingRecordRepository}", _pollVotingRecordRepository);
		_logger.LogDebug("CustomerService(): _shoppingCartRepository → {_shoppingCartRepository}", _shoppingCartRepository);
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("CustomerService(): _shortTermCacheManager → {_shortTermCacheManager}", _shortTermCacheManager);
		_logger.LogDebug("CustomerService(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
        _storeContext = storeContext;
        _shoppingCartSettings = shoppingCartSettings;
		_logger.LogDebug("CustomerService(): _storeContext → {_storeContext}", _storeContext);
		_logger.LogDebug("CustomerService(): _shoppingCartSettings → {_shoppingCartSettings}", _shoppingCartSettings);
    _logger.LogWarning("<<< Exiting CustomerService()");
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a dictionary of all customer roles mapped by ID.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation and contains a dictionary of all customer roles mapped by ID.
    /// </returns>
    protected virtual async Task<IDictionary<int, CustomerRole>> GetAllCustomerRolesDictionaryAsync()
    {
        return await _staticCacheManager.GetAsync(
            _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<CustomerRole>.AllCacheKey),
            async () => await _customerRoleRepository.Table.ToDictionaryAsync(cr => cr.Id));
    }

    #endregion

    #region Methods

    #region Customers

    /// <summary>
    /// Gets all customers
    /// </summary>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="lastActivityFromUtc">Last activity date from (UTC); null to load all records</param>
    /// <param name="lastActivityToUtc">Last activity date to (UTC); null to load all records</param>
    /// <param name="affiliateId">Affiliate identifier</param>
    /// <param name="vendorId">Vendor identifier</param>
    /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
    /// <param name="email">Email; null to load all customers</param>
    /// <param name="username">Username; null to load all customers</param>
    /// <param name="firstName">First name; null to load all customers</param>
    /// <param name="lastName">Last name; null to load all customers</param>
    /// <param name="dayOfBirth">Day of birth; 0 to load all customers</param>
    /// <param name="monthOfBirth">Month of birth; 0 to load all customers</param>
    /// <param name="company">Company; null to load all customers</param>
    /// <param name="phone">Phone; null to load all customers</param>
    /// <param name="zipPostalCode">Phone; null to load all customers</param>
    /// <param name="ipAddress">IP address; null to load all customers</param>
    /// <param name="isActive">Customer is active; null to load all customers</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customers
    /// </returns>
    public virtual async Task<IPagedList<Customer>> GetAllCustomersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null,
        int affiliateId = 0, int vendorId = 0, int[] customerRoleIds = null,
        string email = null, string username = null, string firstName = null, string lastName = null,
        int dayOfBirth = 0, int monthOfBirth = 0,
        string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
        bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
		_logger.LogError(">>> Entering GetAllCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},lastActivityFromUtc={LastActivityFromUtc},lastActivityToUtc={LastActivityToUtc},affiliateId={AffiliateId},vendorId={VendorId},customerRoleIds={CustomerRoleIds},email={Email},username={Username},firstName={FirstName},lastName={LastName},dayOfBirth={DayOfBirth},monthOfBirth={MonthOfBirth},company={Company},phone={Phone},zipPostalCode={ZipPostalCode},ipAddress={IpAddress},isActive={IsActive},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", createdFromUtc, createdToUtc, lastActivityFromUtc, lastActivityToUtc, affiliateId, vendorId, customerRoleIds, email, username, firstName, lastName, dayOfBirth, monthOfBirth, company, phone, zipPostalCode, ipAddress, isActive, pageIndex, pageSize, getOnlyTotalCount);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customers = await _customerRepository.GetAllPagedAsync(query =>
        {
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
				_logger.LogError("GetAllCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},lastActivityFromUtc={LastActivityFromUtc},lastActivityToUtc={LastActivityToUtc},affiliateId={AffiliateId},vendorId={VendorId},customerRoleIds={CustomerRoleIds},email={Email},username={Username},firstName={FirstName},lastName={LastName},dayOfBirth={DayOfBirth},monthOfBirth={MonthOfBirth},company={Company},phone={Phone},zipPostalCode={ZipPostalCode},ipAddress={IpAddress},isActive={IsActive},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount}): query → {Query}", createdFromUtc, createdToUtc, lastActivityFromUtc, lastActivityToUtc, affiliateId, vendorId, customerRoleIds, email, username, firstName, lastName, dayOfBirth, monthOfBirth, company, phone, zipPostalCode, ipAddress, isActive, pageIndex, pageSize, getOnlyTotalCount, query);
            if (lastActivityFromUtc.HasValue)
                query = query.Where(c => lastActivityFromUtc.Value <= c.LastActivityDateUtc);
            if (lastActivityToUtc.HasValue)
                query = query.Where(c => lastActivityToUtc.Value >= c.LastActivityDateUtc);
            if (affiliateId > 0)
                query = query.Where(c => affiliateId == c.AffiliateId);
            if (vendorId > 0)
                query = query.Where(c => vendorId == c.VendorId);
            if (isActive.HasValue)
                query = query.Where(c => c.Active == isActive.Value);

            query = query.Where(c => !c.Deleted);

            if (customerRoleIds != null && customerRoleIds.Length > 0)
            {
                query = query.Join(_customerCustomerRoleMappingRepository.Table, x => x.Id, y => y.CustomerId,
                        (x, y) => new { Customer = x, Mapping = y })
                    .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
                    .Select(z => z.Customer)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(c => c.FirstName.Contains(firstName));
            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(c => c.LastName.Contains(lastName));
            if (!string.IsNullOrWhiteSpace(company))
                query = query.Where(c => c.Company.Contains(company));
            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(c => c.Phone.Contains(phone));
            if (!string.IsNullOrWhiteSpace(zipPostalCode))
                query = query.Where(c => c.ZipPostalCode.Contains(zipPostalCode));

            if (dayOfBirth > 0 && monthOfBirth > 0)
                query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dayOfBirth &&
                                         c.DateOfBirth.Value.Month == monthOfBirth);
            else if (dayOfBirth > 0)
                query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dayOfBirth);
            else if (monthOfBirth > 0)
                query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Month == monthOfBirth);

            //search by IpAddress
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize, getOnlyTotalCount);
		_logger.LogError("GetAllCustomersAsync(createdFromUtc,createdToUtc,lastActivityFromUtc,lastActivityToUtc,affiliateId,vendorId,customerRoleIds,email,username,firstName,lastName,dayOfBirth,monthOfBirth,company,phone,zipPostalCode,ipAddress,isActive,pageIndex,pageSize,getOnlyTotalCount)={CreatedFromUtc},{CreatedToUtc},{LastActivityFromUtc},{LastActivityToUtc},{AffiliateId},{VendorId},{CustomerRoleIds},{Email},{Username},{FirstName},{LastName},{DayOfBirth},{MonthOfBirth},{Company},{Phone},{ZipPostalCode},{IpAddress},{IsActive},{PageIndex},{PageSize},{GetOnlyTotalCount}: find query executed in {Arg21} ms", createdFromUtc, createdToUtc, lastActivityFromUtc, lastActivityToUtc, affiliateId, vendorId, customerRoleIds, email, username, firstName, lastName, dayOfBirth, monthOfBirth, company, phone, zipPostalCode, ipAddress, isActive, pageIndex, pageSize, getOnlyTotalCount, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},lastActivityFromUtc={LastActivityFromUtc},lastActivityToUtc={LastActivityToUtc},affiliateId={AffiliateId},vendorId={VendorId},customerRoleIds={CustomerRoleIds},email={Email},username={Username},firstName={FirstName},lastName={LastName},dayOfBirth={DayOfBirth},monthOfBirth={MonthOfBirth},company={Company},phone={Phone},zipPostalCode={ZipPostalCode},ipAddress={IpAddress},isActive={IsActive},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", createdFromUtc, createdToUtc, lastActivityFromUtc, lastActivityToUtc, affiliateId, vendorId, customerRoleIds, email, username, firstName, lastName, dayOfBirth, monthOfBirth, company, phone, zipPostalCode, ipAddress, isActive, pageIndex, pageSize, getOnlyTotalCount);
        return customers;
    }

    /// <summary>
    /// Gets online customers
    /// </summary>
    /// <param name="lastActivityFromUtc">Customer last activity date (from)</param>
    /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customers
    /// </returns>
    public virtual async Task<IPagedList<Customer>> GetOnlineCustomersAsync(DateTime lastActivityFromUtc,
        int[] customerRoleIds, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetOnlineCustomersAsync(lastActivityFromUtc={LastActivityFromUtc},customerRoleIds={CustomerRoleIds},pageIndex={PageIndex},pageSize={PageSize})", lastActivityFromUtc, customerRoleIds, pageIndex, pageSize);
        var query = _customerRepository.Table;
        query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
		_logger.LogError("GetOnlineCustomersAsync(lastActivityFromUtc={LastActivityFromUtc},customerRoleIds={CustomerRoleIds},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", lastActivityFromUtc, customerRoleIds, pageIndex, pageSize, query);
        query = query.Where(c => !c.Deleted);

        if (customerRoleIds != null && customerRoleIds.Length > 0)
            query = query.Where(c => _customerCustomerRoleMappingRepository.Table.Any(ccrm => ccrm.CustomerId == c.Id && customerRoleIds.Contains(ccrm.CustomerRoleId)));

        query = query.OrderByDescending(c => c.LastActivityDateUtc);
        var customers = await query.ToPagedListAsync(pageIndex, pageSize);
		_logger.LogError("GetOnlineCustomersAsync(lastActivityFromUtc={LastActivityFromUtc},customerRoleIds={CustomerRoleIds},pageIndex={PageIndex},pageSize={PageSize}): customers → {Customers}", lastActivityFromUtc, customerRoleIds, pageIndex, pageSize, customers);

		_logger.LogWarning("<<< Exiting GetOnlineCustomersAsync(lastActivityFromUtc={LastActivityFromUtc},customerRoleIds={CustomerRoleIds},pageIndex={PageIndex},pageSize={PageSize})", lastActivityFromUtc, customerRoleIds, pageIndex, pageSize);
        return customers;
    }

    /// <summary>
    /// Gets customers with shopping carts
    /// </summary>
    /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="productId">Product identifier; pass null to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
    /// <param name="countryId">Billing country identifier; pass null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customers
    /// </returns>
    public virtual async Task<IPagedList<Customer>> GetCustomersWithShoppingCartsAsync(ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetCustomersWithShoppingCartsAsync(shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},countryId={CountryId},pageIndex={PageIndex},pageSize={PageSize})", storeId, productId, createdFromUtc, createdToUtc, countryId, pageIndex, pageSize);
        //get all shopping cart items
        var items = _shoppingCartRepository.Table;
		_logger.LogError("GetCustomersWithShoppingCartsAsync(shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},countryId={CountryId},pageIndex={PageIndex},pageSize={PageSize}): items → {Items}", storeId, productId, createdFromUtc, createdToUtc, countryId, pageIndex, pageSize, items);

        //filter by type
        if (shoppingCartType.HasValue)
            items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

        //filter shopping cart items by store
        if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
            items = items.Where(item => item.StoreId == storeId);

        //filter shopping cart items by product
        if (productId > 0)
            items = items.Where(item => item.ProductId == productId);

        //filter shopping cart items by date
        if (createdFromUtc.HasValue)
            items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
        if (createdToUtc.HasValue)
            items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

        //get all active customers
        var customers = _customerRepository.Table.Where(customer => customer.Active && !customer.Deleted);
		_logger.LogError("GetCustomersWithShoppingCartsAsync(shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},countryId={CountryId},pageIndex={PageIndex},pageSize={PageSize}): customers → {Customers}", storeId, productId, createdFromUtc, createdToUtc, countryId, pageIndex, pageSize, customers);

        //filter customers by billing country
        if (countryId > 0)
            customers = from c in customers
                join a in _customerAddressRepository.Table on c.BillingAddressId equals a.Id
                where a.CountryId == countryId
                select c;

        var customersWithCarts = from c in customers
            join item in items on c.Id equals item.CustomerId
            //we change ordering for the MySQL engine to avoid problems with the ONLY_FULL_GROUP_BY server property that is set by default since the 5.7.5 version
            orderby _dataProvider.ConfigurationName == "MySql" ? c.CreatedOnUtc : item.CreatedOnUtc descending
            select c;

		_logger.LogWarning("<<< Exiting GetCustomersWithShoppingCartsAsync(shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},countryId={CountryId},pageIndex={PageIndex},pageSize={PageSize})", storeId, productId, createdFromUtc, createdToUtc, countryId, pageIndex, pageSize);
        return await customersWithCarts.Distinct().ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets customer for shopping cart
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<Customer> GetShoppingCartCustomerAsync(IList<ShoppingCartItem> shoppingCart)
    {
		_logger.LogError(">>> Entering GetShoppingCartCustomerAsync(shoppingCart=<redacted>)");
        var customerId = shoppingCart.FirstOrDefault()?.CustomerId;

		_logger.LogError("GetShoppingCartCustomerAsync(shoppingCart=<redacted>): customerId → {CustomerId}", customerId);
		_logger.LogWarning("<<< Exiting GetShoppingCartCustomerAsync(shoppingCart=<redacted>)");
        return customerId.HasValue && customerId != 0 ? await GetCustomerByIdAsync(customerId.Value) : null;
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCustomerAsync(Customer customer)
    {
		_logger.LogError(">>> Entering DeleteCustomerAsync(customer={Customer})", customer);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        if (customer.IsSystemAccount)
			_logger.LogError("Throwing NopException from DeleteCustomerAsync(customer={Customer})", customer);
			_logger.LogDebug("DeleteCustomerAsync(customer={Customer}): customer.Deleted → {Arg1}", customer, customer.Deleted);
		_logger.LogWarning("<<< Exiting DeleteCustomerAsync(customer={Customer})", customer);
            throw new NopException($"System customer account ({customer.SystemName}) could not be deleted");

        customer.Deleted = true;

        if (_customerSettings.SuffixDeletedCustomers)
        {
            if (!string.IsNullOrEmpty(customer.Email))
                customer.Email += NopCustomerServicesDefaults.CustomerDeletedSuffix;
            if (!string.IsNullOrEmpty(customer.Username))
                customer.Username += NopCustomerServicesDefaults.CustomerDeletedSuffix;
        }

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteCustomerAsync(customer)={Customer}: update query executed in {Arg1} ms", customer, __logStartDb.ElapsedMilliseconds);
        await _customerRepository.UpdateAsync(customer, false);
        await _customerRepository.DeleteAsync(customer);
		_logger.LogError("DeleteCustomerAsync(customer)={Customer}: external service call took {Arg1} ms", customer, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets a customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer
    /// </returns>
    public virtual async Task<Customer> GetCustomerByIdAsync(int customerId)
    {
		_logger.LogError(">>> Entering GetCustomerByIdAsync(customerId={CustomerId})", customerId);
		_logger.LogError("GetCustomerByIdAsync(customerId)={CustomerId}: find query executing", customerId);
    _logger.LogWarning("<<< Exiting GetCustomerByIdAsync(customerId={CustomerId})", customerId);
        return await _customerRepository.GetByIdAsync(customerId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get customers by identifiers
    /// </summary>
    /// <param name="customerIds">Customer identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customers
    /// </returns>
    public virtual async Task<IList<Customer>> GetCustomersByIdsAsync(int[] customerIds)
    {
		_logger.LogError(">>> Entering GetCustomersByIdsAsync(customerIds={CustomerIds})", customerIds);
        _logger.LogWarning("<<< Exiting GetCustomersByIdsAsync(customerIds={CustomerIds})", customerIds);
        return await _customerRepository.GetByIdsAsync(customerIds, includeDeleted: false);
    }

    /// <summary>
    /// Get customers by guids
    /// </summary>
    /// <param name="customerGuids">Customer guids</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customers
    /// </returns>
    public virtual async Task<IList<Customer>> GetCustomersByGuidsAsync(Guid[] customerGuids)
    {
		_logger.LogError(">>> Entering GetCustomersByGuidsAsync(customerGuids={CustomerGuids})", customerGuids);
        if (customerGuids == null)
            return null;

        var query = from c in _customerRepository.Table
            where customerGuids.Contains(c.CustomerGuid)
            select c;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customers = await query.ToListAsync();

		_logger.LogError("GetCustomersByGuidsAsync(customerGuids={CustomerGuids}): customers → {Customers}", customerGuids, customers);
		_logger.LogError("GetCustomersByGuidsAsync(customerGuids)={CustomerGuids}: database query executed in {Arg1} ms", customerGuids, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomersByGuidsAsync(customerGuids={CustomerGuids})", customerGuids);
        return customers;
    }

    /// <summary>
    /// Gets a customer by GUID
    /// </summary>
    /// <param name="customerGuid">Customer GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer
    /// </returns>
    public virtual async Task<Customer> GetCustomerByGuidAsync(Guid customerGuid)
    {
		_logger.LogError(">>> Entering GetCustomerByGuidAsync(customerGuid={CustomerGuid})", customerGuid);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (customerGuid == Guid.Empty)
            return null;

        var query = from c in _customerRepository.Table
            where c.CustomerGuid == customerGuid
            orderby c.Id
            select c;

		_logger.LogError("GetCustomerByGuidAsync(customerGuid)={CustomerGuid}: find query executing", customerGuid);
		_logger.LogError("GetCustomerByGuidAsync(customerGuid)={CustomerGuid}: external service call took {Arg1} ms", customerGuid, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomerByGuidAsync(customerGuid={CustomerGuid})", customerGuid);
        return await _shortTermCacheManager.GetAsync(async () => await query.FirstOrDefaultAsync(), NopCustomerServicesDefaults.CustomerByGuidCacheKey, customerGuid);
    }

    /// <summary>
    /// Get customer by email
    /// </summary>
    /// <param name="email">Email</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    public virtual async Task<Customer> GetCustomerByEmailAsync(string email)
    {
		_logger.LogError(">>> Entering GetCustomerByEmailAsync(email={Email})", email);
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var query = from c in _customerRepository.Table
            orderby c.Id
            where c.Email == email
            select c;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await query.FirstOrDefaultAsync();

		_logger.LogError("GetCustomerByEmailAsync(email={Email}): customer → {Customer}", email, customer);
		_logger.LogError("GetCustomerByEmailAsync(email)={Email}: database query executed in {Arg1} ms", email, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomerByEmailAsync(email={Email})", email);
        return customer;
    }

    /// <summary>
    /// Get customer by system name
    /// </summary>
    /// <param name="systemName">System name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    public virtual async Task<Customer> GetCustomerBySystemNameAsync(string systemName)
    {
		_logger.LogError(">>> Entering GetCustomerBySystemNameAsync(systemName={SystemName})", systemName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        var query = from c in _customerRepository.Table
            orderby c.Id
            where c.SystemName == systemName
            select c;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetCustomerBySystemNameAsync(systemName)={SystemName}: external service call took {Arg1} ms", systemName, __logStart.ElapsedMilliseconds);
		_logger.LogError("GetCustomerBySystemNameAsync(systemName)={SystemName}: find query executed in {Arg1} ms", systemName, __logStartDb.ElapsedMilliseconds);
        var customer = await _shortTermCacheManager.GetAsync(async () => await query.FirstOrDefaultAsync(), NopCustomerServicesDefaults.CustomerBySystemNameCacheKey, systemName);
		_logger.LogError("GetCustomerBySystemNameAsync(systemName={SystemName}): customer → {Customer}", systemName, customer);

		_logger.LogWarning("<<< Exiting GetCustomerBySystemNameAsync(systemName={SystemName})", systemName);
        return customer;
    }

    /// <summary>
    /// Gets built-in system record used for background tasks
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer object
    /// </returns>
    public virtual async Task<Customer> GetOrCreateBackgroundTaskUserAsync()
    {
		_logger.LogError(">>> Entering GetOrCreateBackgroundTaskUserAsync()");
        var backgroundTaskUser = await GetCustomerBySystemNameAsync(NopCustomerDefaults.BackgroundTaskCustomerName);

		_logger.LogError("GetOrCreateBackgroundTaskUserAsync(): backgroundTaskUser → {BackgroundTaskUser}", backgroundTaskUser);
        if (backgroundTaskUser is null)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("GetOrCreateBackgroundTaskUserAsync(): store → {Store}", store);
            //If for any reason the system user isn't in the database, then we add it
            backgroundTaskUser = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = NopCustomerDefaults.BackgroundTaskCustomerName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = store.Id
            };

            await InsertCustomerAsync(backgroundTaskUser);

            var guestRole = await GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName) ?? throw new NopException("'Guests' role could not be loaded");
			_logger.LogError("GetOrCreateBackgroundTaskUserAsync(): guestRole → {GuestRole}", guestRole);

            await AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerRoleId = guestRole.Id, CustomerId = backgroundTaskUser.Id });
        }

		_logger.LogWarning("<<< Exiting GetOrCreateBackgroundTaskUserAsync()");
        return backgroundTaskUser;
    }

    /// <summary>
    /// Gets built-in system guest record used for requests from search engines
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer object
    /// </returns>
    public virtual async Task<Customer> GetOrCreateSearchEngineUserAsync()
    {
		_logger.LogError(">>> Entering GetOrCreateSearchEngineUserAsync()");
        var searchEngineUser = await GetCustomerBySystemNameAsync(NopCustomerDefaults.SearchEngineCustomerName);

		_logger.LogError("GetOrCreateSearchEngineUserAsync(): searchEngineUser → {SearchEngineUser}", searchEngineUser);
        if (searchEngineUser is null)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("GetOrCreateSearchEngineUserAsync(): store → {Store}", store);
            //If for any reason the system user isn't in the database, then we add it
            searchEngineUser = new Customer
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = NopCustomerDefaults.SearchEngineCustomerName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = store.Id
            };

            await InsertCustomerAsync(searchEngineUser);

            var guestRole = await GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName) ?? throw new NopException("'Guests' role could not be loaded");
			_logger.LogError("GetOrCreateSearchEngineUserAsync(): guestRole → {GuestRole}", guestRole);

            await AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerRoleId = guestRole.Id, CustomerId = searchEngineUser.Id });
        }

		_logger.LogWarning("<<< Exiting GetOrCreateSearchEngineUserAsync()");
        return searchEngineUser;
    }

    /// <summary>
    /// Get customer by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    public virtual async Task<Customer> GetCustomerByUsernameAsync(string username)
    {
		_logger.LogError(">>> Entering GetCustomerByUsernameAsync(username={Username})", username);
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var query = from c in _customerRepository.Table
            orderby c.Id
            where c.Username == username
            select c;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await query.FirstOrDefaultAsync();

		_logger.LogError("GetCustomerByUsernameAsync(username={Username}): customer → {Customer}", username, customer);
		_logger.LogError("GetCustomerByUsernameAsync(username)={Username}: database query executed in {Arg1} ms", username, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomerByUsernameAsync(username={Username})", username);
        return customer;
    }

    /// <summary>
    /// Insert a guest customer
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    public virtual async Task<Customer> InsertGuestCustomerAsync()
    {
		_logger.LogError(">>> Entering InsertGuestCustomerAsync()");
        var customer = new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            Active = true,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityDateUtc = DateTime.UtcNow
        };

        //add to 'Guests' role
        var guestRole = await GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName) ?? throw new NopException("'Guests' role could not be loaded");
		_logger.LogError("InsertGuestCustomerAsync(): guestRole → {GuestRole}", guestRole);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _customerRepository.InsertAsync(customer);
		_logger.LogError("InsertGuestCustomerAsync(): save query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        await AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });

		_logger.LogWarning("<<< Exiting InsertGuestCustomerAsync()");
        return customer;
    }

    /// <summary>
    /// Insert a customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCustomerAsync(Customer customer)
    {
		_logger.LogError(">>> Entering InsertCustomerAsync(customer={Customer})", customer);
        await _customerRepository.InsertAsync(customer);
		_logger.LogWarning("<<< Exiting InsertCustomerAsync(customer={Customer})", customer);
    }

    /// <summary>
    /// Updates the customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCustomerAsync(Customer customer)
    {
		_logger.LogError(">>> Entering UpdateCustomerAsync(customer={Customer})", customer);
        await _customerRepository.UpdateAsync(customer);
		_logger.LogWarning("<<< Exiting UpdateCustomerAsync(customer={Customer})", customer);
    }

    /// <summary>
    /// Reset data required for checkout
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
    /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
    /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
    /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
    /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ResetCheckoutDataAsync(Customer customer, int storeId,
        bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
        bool clearRewardPoints = true, bool clearShippingMethod = true,
        bool clearPaymentMethod = true)
    {
		_logger.LogError(">>> Entering ResetCheckoutDataAsync(customer={Customer},storeId={StoreId},clearCouponCodes={ClearCouponCodes},clearCheckoutAttributes={ClearCheckoutAttributes},clearRewardPoints={ClearRewardPoints},clearShippingMethod=<redacted>,clearPaymentMethod={ClearPaymentMethod})", customer, storeId, clearCouponCodes, clearCheckoutAttributes, clearRewardPoints, clearPaymentMethod);
        ArgumentNullException.ThrowIfNull(customer);

        //clear entered coupon codes
        if (clearCouponCodes)
        {
            await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.DiscountCouponCodeAttribute, null);
            await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.GiftCardCouponCodesAttribute, null);
        }

        //clear checkout attributes
        if (clearCheckoutAttributes)
            await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, null, storeId);

        //clear reward points flag
        if (clearRewardPoints)
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, false, storeId);

        //clear selected shipping method
        if (clearShippingMethod)
        {
            await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, storeId);
            await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.OfferedShippingOptionsAttribute, null, storeId);
            await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, storeId);
        }

        //clear selected payment method
        if (clearPaymentMethod)
            await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, null, storeId);

        await _eventPublisher.PublishAsync(new ResetCheckoutDataEvent(customer, storeId));
		_logger.LogWarning("<<< Exiting ResetCheckoutDataAsync(customer={Customer},storeId={StoreId},clearCouponCodes={ClearCouponCodes},clearCheckoutAttributes={ClearCheckoutAttributes},clearRewardPoints={ClearRewardPoints},clearShippingMethod=<redacted>,clearPaymentMethod={ClearPaymentMethod})", customer, storeId, clearCouponCodes, clearCheckoutAttributes, clearRewardPoints, clearPaymentMethod);
    }

    /// <summary>
    /// Delete guest customer records
    /// </summary>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete customers only without shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted customers
    /// </returns>
    public virtual async Task<int> DeleteGuestCustomersAsync(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart)
    {
		_logger.LogError(">>> Entering DeleteGuestCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},onlyWithoutShoppingCart=<redacted>)", createdFromUtc, createdToUtc);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var guestRole = await GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);

		_logger.LogError("DeleteGuestCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},onlyWithoutShoppingCart=<redacted>): guestRole → {GuestRole}", createdFromUtc, createdToUtc, guestRole);
        var allGuestCustomers = from guest in _customerRepository.Table
            join ccm in _customerCustomerRoleMappingRepository.Table on guest.Id equals ccm.CustomerId
            where ccm.CustomerRoleId == guestRole.Id
            select guest;

        var guestsToDelete = from guest in _customerRepository.Table
            join g in allGuestCustomers on guest.Id equals g.Id
            from sCart in _shoppingCartRepository.Table.Where(sci => sci.CustomerId == guest.Id).DefaultIfEmpty()
            from order in _orderRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from blogComment in _blogCommentRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from newsComment in _newsCommentRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from productReview in _productReviewRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from productReviewHelpfulness in _productReviewHelpfulnessRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from pollVotingRecord in _pollVotingRecordRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from forumTopic in _forumTopicRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            from forumPost in _forumPostRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
            where (!onlyWithoutShoppingCart || sCart == null) &&
                  order == null && blogComment == null && newsComment == null && productReview == null && productReviewHelpfulness == null &&
                  pollVotingRecord == null && forumTopic == null && forumPost == null &&
                  !guest.IsSystemAccount &&
                  (createdFromUtc == null || guest.CreatedOnUtc > createdFromUtc) &&
                  (createdToUtc == null || guest.CreatedOnUtc < createdToUtc)
            select new { CustomerId = guest.Id };

        await using var tmpGuests = await _dataProvider.CreateTempDataStorageAsync("tmp_guestsToDelete", guestsToDelete);
		_logger.LogError("DeleteGuestCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},onlyWithoutShoppingCart=<redacted>): tmpGuests → {TmpGuests}", createdFromUtc, createdToUtc, tmpGuests);
        await using var tmpAddresses = await _dataProvider.CreateTempDataStorageAsync("tmp_guestsAddressesToDelete",
            _customerAddressMappingRepository.Table
                .Where(ca => tmpGuests.Any(c => c.CustomerId == ca.CustomerId))
                .Select(ca => new { AddressId = ca.AddressId }));

        //delete guests
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        var totalRecordsDeleted = await _customerRepository.DeleteAsync(c => tmpGuests.Any(tmp => tmp.CustomerId == c.Id));

		_logger.LogError("DeleteGuestCustomersAsync(createdFromUtc,createdToUtc,onlyWithoutShoppingCart)={CreatedFromUtc},{CreatedToUtc},<redacted>: delete query executed in {Arg3} ms", createdFromUtc, createdToUtc, __logStartDb.ElapsedMilliseconds);
		_logger.LogError("DeleteGuestCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},onlyWithoutShoppingCart=<redacted>): totalRecordsDeleted → {TotalRecordsDeleted}", createdFromUtc, createdToUtc, totalRecordsDeleted);
        //delete attributes
        await _gaRepository.DeleteAsync(ga => tmpGuests.Any(c => c.CustomerId == ga.EntityId) && ga.KeyGroup == nameof(Customer));

        //delete m -> m addresses
        await _customerAddressRepository.DeleteAsync(a => tmpAddresses.Any(tmp => tmp.AddressId == a.Id));

		_logger.LogError("DeleteGuestCustomersAsync(createdFromUtc,createdToUtc,onlyWithoutShoppingCart)={CreatedFromUtc},{CreatedToUtc},<redacted>: external service call took {Arg3} ms", createdFromUtc, createdToUtc, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting DeleteGuestCustomersAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},onlyWithoutShoppingCart=<redacted>)", createdFromUtc, createdToUtc);
        return totalRecordsDeleted;
    }

    /// <summary>
    /// Gets a tax display type for the customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tax display type
    /// </returns>
    public virtual async Task<TaxDisplayType> GetCustomerTaxDisplayTypeAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerTaxDisplayTypeAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        //default tax display type
        var taxDisplayType = _taxSettings.TaxDisplayType;
		_logger.LogError("GetCustomerTaxDisplayTypeAsync(customer={Customer}): taxDisplayType → {TaxDisplayType}", customer, taxDisplayType);

        //whether customers are allowed to select tax display type and the customer has previously saved one
        if (_taxSettings.AllowCustomersToSelectTaxDisplayType && customer.TaxDisplayTypeId.HasValue)
            taxDisplayType = (TaxDisplayType)customer.TaxDisplayTypeId.Value;
        else
        {
            //default tax type by customer roles
            var defaultRoleTaxDisplayType = await GetCustomerDefaultTaxDisplayTypeAsync(customer);
			_logger.LogError("GetCustomerTaxDisplayTypeAsync(customer={Customer}): defaultRoleTaxDisplayType → {DefaultRoleTaxDisplayType}", customer, defaultRoleTaxDisplayType);
            if (defaultRoleTaxDisplayType.HasValue)
                taxDisplayType = defaultRoleTaxDisplayType.Value;
        }

		_logger.LogWarning("<<< Exiting GetCustomerTaxDisplayTypeAsync(customer={Customer})", customer);
        return taxDisplayType;
    }

    /// <summary>
    /// Gets a default tax display type (if configured)
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<TaxDisplayType?> GetCustomerDefaultTaxDisplayTypeAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerDefaultTaxDisplayTypeAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        var roleWithOverriddenTaxType = (await GetCustomerRolesAsync(customer)).FirstOrDefault(cr => cr.Active && cr.OverrideTaxDisplayType);
		_logger.LogError("GetCustomerDefaultTaxDisplayTypeAsync(customer={Customer}): roleWithOverriddenTaxType → {RoleWithOverriddenTaxType}", customer, roleWithOverriddenTaxType);
        if (roleWithOverriddenTaxType == null)
            return null;

        return (TaxDisplayType)roleWithOverriddenTaxType.DefaultTaxDisplayTypeId;
    }

    /// <summary>
    /// Get full name
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer full name
    /// </returns>
    public virtual async Task<string> GetCustomerFullNameAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerFullNameAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        var firstName = customer.FirstName;
        var lastName = customer.LastName;

        var fullName = string.Empty;
		_logger.LogError("GetCustomerFullNameAsync(customer={Customer}): fullName → {FullName}", customer, fullName);
        if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
        {
            //do not inject ILocalizationService via constructor because it'll cause circular references
            var format = await EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("Customer.FullNameFormat");
			_logger.LogError("GetCustomerFullNameAsync(customer={Customer}): format → {Format}", customer, format);

            fullName = string.Format(format, firstName, lastName);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(firstName))
                fullName = firstName;

            if (!string.IsNullOrWhiteSpace(lastName))
                fullName = lastName;
        }

		_logger.LogWarning("<<< Exiting GetCustomerFullNameAsync(customer={Customer})", customer);
		_logger.LogError(">>> Entering GetCustomerRolesAsync(customer={Customer},showHidden={ShowHidden})", customer, showHidden);
		_logger.LogError("GetCustomerRolesAsync(customer,showHidden)={Customer},{ShowHidden}: find query executed in {Arg2} ms", customer, showHidden, __logStartDb.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting GetCustomerRolesAsync(customer={Customer},showHidden={ShowHidden})", customer, showHidden);
        return fullName;
    }

    /// <summary>
    /// Formats the customer name
    /// </summary>
    /// <param name="customer">Source</param>
    /// <param name="stripTooLong">Strip too long customer name</param>
    /// <param name="maxLength">Maximum customer name length</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the formatted text
    /// </returns>
    public virtual async Task<string> FormatUsernameAsync(Customer customer, bool stripTooLong = false, int maxLength = 0)
    {
		_logger.LogError(">>> Entering FormatUsernameAsync(customer={Customer},stripTooLong={StripTooLong},maxLength={MaxLength})", customer, stripTooLong, maxLength);
        if (customer == null)
            return string.Empty;

        if (await IsGuestAsync(customer))
            //do not inject ILocalizationService via constructor because it'll cause circular references
            return await EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("Customer.Guest");

        var result = string.Empty;
        switch (_customerSettings.CustomerNameFormat)
        {
            case CustomerNameFormat.ShowEmails:
                result = customer.Email;
                break;
            case CustomerNameFormat.ShowUsernames:
                result = customer.Username;
                break;
            case CustomerNameFormat.ShowFullNames:
                result = await GetCustomerFullNameAsync(customer);
                break;
            case CustomerNameFormat.ShowFirstName:
                result = customer.FirstName;
                break;
            default:
                break;
        }

        if (stripTooLong && maxLength > 0)
            result = CommonHelper.EnsureMaximumLength(result, maxLength);

		_logger.LogWarning("<<< Exiting FormatUsernameAsync(customer={Customer},stripTooLong={StripTooLong},maxLength={MaxLength})", customer, stripTooLong, maxLength);
        return result ?? string.Empty;
    }

    /// <summary>
    /// Gets coupon codes
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the coupon codes
    /// </returns>
    public virtual async Task<string[]> ParseAppliedDiscountCouponCodesAsync(Customer customer)
    {
		_logger.LogError(">>> Entering ParseAppliedDiscountCouponCodesAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.DiscountCouponCodeAttribute);
		_logger.LogError("ParseAppliedDiscountCouponCodesAsync(customer={Customer}): existingCouponCodes → {ExistingCouponCodes}", customer, existingCouponCodes);

        var couponCodes = new List<string>();
		_logger.LogError("ParseAppliedDiscountCouponCodesAsync(customer={Customer}): couponCodes → {CouponCodes}", customer, couponCodes);
        if (string.IsNullOrEmpty(existingCouponCodes))
            return couponCodes.ToArray();

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("ParseAppliedDiscountCouponCodesAsync(customer={Customer}): xmlDoc → {XmlDoc}", customer, xmlDoc);
            xmlDoc.LoadXml(existingCouponCodes);

            var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
			_logger.LogError("ParseAppliedDiscountCouponCodesAsync(customer={Customer}): nodeList1 → {NodeList1}", customer, nodeList1);
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["Code"] == null)
                    continue;
                var code = node1.Attributes["Code"].InnerText.Trim();
				_logger.LogError("ParseAppliedDiscountCouponCodesAsync(customer={Customer}): code → {Code}", customer, code);
                couponCodes.Add(code);
            }
        }
        catch
        {
			_logger.LogError("Exception in ParseAppliedDiscountCouponCodesAsync(customer={Customer}): {Arg1}", customer, ex.Message);
            // ignored
        }

		_logger.LogWarning("<<< Exiting ParseAppliedDiscountCouponCodesAsync(customer={Customer})", customer);
        return couponCodes.ToArray();
    }

    /// <summary>
    /// Adds a coupon code
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="couponCode">Coupon code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new coupon codes document
    /// </returns>
    public virtual async Task ApplyDiscountCouponCodeAsync(Customer customer, string couponCode)
    {
		_logger.LogError(">>> Entering ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
        ArgumentNullException.ThrowIfNull(customer);

        var result = string.Empty;
		_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): result → {Result}", customer, couponCode, result);
        try
        {
            var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.DiscountCouponCodeAttribute);
			_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): existingCouponCodes → {ExistingCouponCodes}", customer, couponCode, existingCouponCodes);

            couponCode = couponCode.Trim().ToLowerInvariant();
			_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): couponCode → {CouponCode1}", customer, couponCode, couponCode);

            var xmlDoc = new XmlDocument();
			_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): xmlDoc → {XmlDoc}", customer, couponCode, xmlDoc);
            if (string.IsNullOrEmpty(existingCouponCodes))
            {
                var element1 = xmlDoc.CreateElement("DiscountCouponCodes");
				_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): element1 → {Element1}", customer, couponCode, element1);
                xmlDoc.AppendChild(element1);
            }
            else
                xmlDoc.LoadXml(existingCouponCodes);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//DiscountCouponCodes");
			_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): rootElement → {RootElement}", customer, couponCode, rootElement);

            XmlElement gcElement = null;
			_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): gcElement → {GcElement}", customer, couponCode, gcElement);
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
			_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): nodeList1 → {NodeList1}", customer, couponCode, nodeList1);
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["Code"] == null)
                    continue;

                var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();
				_logger.LogError("ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): couponCodeAttribute → {CouponCodeAttribute}", customer, couponCode, couponCodeAttribute);

                if (couponCodeAttribute.ToLowerInvariant() != couponCode.ToLowerInvariant())
                    continue;

                gcElement = (XmlElement)node1;
                break;
            }

            //create new one if not found
            if (gcElement == null)
            {
                gcElement = xmlDoc.CreateElement("CouponCode");
                gcElement.SetAttribute("Code", couponCode);
                rootElement.AppendChild(gcElement);
            }

            result = xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): {Arg2}", customer, couponCode, ex.Message);
            // ignored
        }

        //apply new value
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DiscountCouponCodeAttribute, result);
		_logger.LogWarning("<<< Exiting ApplyDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
    }

    /// <summary>
    /// Removes a coupon code
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="couponCode">Coupon code to remove</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new coupon codes document
    /// </returns>
    public virtual async Task RemoveDiscountCouponCodeAsync(Customer customer, string couponCode)
    {
		_logger.LogError(">>> Entering RemoveDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
        ArgumentNullException.ThrowIfNull(customer);

        //get applied coupon codes
        var existingCouponCodes = await ParseAppliedDiscountCouponCodesAsync(customer);
		_logger.LogError("RemoveDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode}): existingCouponCodes → {ExistingCouponCodes}", customer, couponCode, existingCouponCodes);

        //clear them
        await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.DiscountCouponCodeAttribute, null);

        //save again except removed one
        foreach (var existingCouponCode in existingCouponCodes)
            if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                await ApplyDiscountCouponCodeAsync(customer, existingCouponCode);
				_logger.LogWarning("<<< Exiting RemoveDiscountCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
    }

    /// <summary>
    /// Gets coupon codes
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the coupon codes
    /// </returns>
    public virtual async Task<string[]> ParseAppliedGiftCardCouponCodesAsync(Customer customer)
    {
		_logger.LogError(">>> Entering ParseAppliedGiftCardCouponCodesAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.GiftCardCouponCodesAttribute);
		_logger.LogError("ParseAppliedGiftCardCouponCodesAsync(customer={Customer}): existingCouponCodes → {ExistingCouponCodes}", customer, existingCouponCodes);

        var couponCodes = new List<string>();
		_logger.LogError("ParseAppliedGiftCardCouponCodesAsync(customer={Customer}): couponCodes → {CouponCodes}", customer, couponCodes);
        if (string.IsNullOrEmpty(existingCouponCodes))
            return couponCodes.ToArray();

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("ParseAppliedGiftCardCouponCodesAsync(customer={Customer}): xmlDoc → {XmlDoc}", customer, xmlDoc);
            xmlDoc.LoadXml(existingCouponCodes);

            var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
			_logger.LogError("ParseAppliedGiftCardCouponCodesAsync(customer={Customer}): nodeList1 → {NodeList1}", customer, nodeList1);
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["Code"] == null)
                    continue;

                var code = node1.Attributes["Code"].InnerText.Trim();
				_logger.LogError("ParseAppliedGiftCardCouponCodesAsync(customer={Customer}): code → {Code}", customer, code);
                couponCodes.Add(code);
            }
        }
        catch
        {
			_logger.LogError("Exception in ParseAppliedGiftCardCouponCodesAsync(customer={Customer}): {Arg1}", customer, ex.Message);
            // ignored
        }

		_logger.LogWarning("<<< Exiting ParseAppliedGiftCardCouponCodesAsync(customer={Customer})", customer);
        return couponCodes.ToArray();
    }

    /// <summary>
    /// Adds a coupon code
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="couponCode">Coupon code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new coupon codes document
    /// </returns>
    public virtual async Task ApplyGiftCardCouponCodeAsync(Customer customer, string couponCode)
    {
		_logger.LogError(">>> Entering ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
        ArgumentNullException.ThrowIfNull(customer);

        var result = string.Empty;
		_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): result → {Result}", customer, couponCode, result);
        try
        {
            var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.GiftCardCouponCodesAttribute);
			_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): existingCouponCodes → {ExistingCouponCodes}", customer, couponCode, existingCouponCodes);

            couponCode = couponCode.Trim().ToLowerInvariant();
			_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): couponCode → {CouponCode1}", customer, couponCode, couponCode);

            var xmlDoc = new XmlDocument();
			_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): xmlDoc → {XmlDoc}", customer, couponCode, xmlDoc);
            if (string.IsNullOrEmpty(existingCouponCodes))
            {
                var element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
				_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): element1 → {Element1}", customer, couponCode, element1);
                xmlDoc.AppendChild(element1);
            }
            else
                xmlDoc.LoadXml(existingCouponCodes);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");
			_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): rootElement → {RootElement}", customer, couponCode, rootElement);

            XmlElement gcElement = null;
			_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): gcElement → {GcElement}", customer, couponCode, gcElement);
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
			_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): nodeList1 → {NodeList1}", customer, couponCode, nodeList1);
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["Code"] == null)
                    continue;

                var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();
				_logger.LogError("ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): couponCodeAttribute → {CouponCodeAttribute}", customer, couponCode, couponCodeAttribute);
                if (couponCodeAttribute.ToLowerInvariant() != couponCode.ToLowerInvariant())
                    continue;

                gcElement = (XmlElement)node1;
                break;
            }

            //create new one if not found
            if (gcElement == null)
            {
                gcElement = xmlDoc.CreateElement("CouponCode");
                gcElement.SetAttribute("Code", couponCode);
                rootElement.AppendChild(gcElement);
            }

            result = xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): {Arg2}", customer, couponCode, ex.Message);
            // ignored
        }

        //apply new value
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.GiftCardCouponCodesAttribute, result);
		_logger.LogWarning("<<< Exiting ApplyGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
    }

    /// <summary>
    /// Removes a coupon code
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="couponCode">Coupon code to remove</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new coupon codes document
    /// </returns>
    public virtual async Task RemoveGiftCardCouponCodeAsync(Customer customer, string couponCode)
    {
		_logger.LogError(">>> Entering RemoveGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
        ArgumentNullException.ThrowIfNull(customer);

        //get applied coupon codes
        var existingCouponCodes = await ParseAppliedGiftCardCouponCodesAsync(customer);
		_logger.LogError("RemoveGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode}): existingCouponCodes → {ExistingCouponCodes}", customer, couponCode, existingCouponCodes);

        //clear them
        await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.GiftCardCouponCodesAttribute, null);

        //save again except removed one
        foreach (var existingCouponCode in existingCouponCodes)
            if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                await ApplyGiftCardCouponCodeAsync(customer, existingCouponCode);
				_logger.LogWarning("<<< Exiting RemoveGiftCardCouponCodeAsync(customer={Customer},couponCode={CouponCode})", customer, couponCode);
    }

    /// <summary>
    /// Returns a list of guids of not existing customers
    /// </summary>
    /// <param name="guids">The guids of the customers to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of guids not existing customers
    /// </returns>
    public virtual async Task<Guid[]> GetNotExistingCustomersAsync(Guid[] guids)
    {
		_logger.LogError(">>> Entering GetNotExistingCustomersAsync(guids={Guids})", guids);
        ArgumentNullException.ThrowIfNull(guids);

        var query = _customerRepository.Table;
		_logger.LogError("GetNotExistingCustomersAsync(guids={Guids}): query → {Query}", guids, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queryFilter = guids.Distinct().ToArray();
		_logger.LogError("GetNotExistingCustomersAsync(guids={Guids}): queryFilter → {QueryFilter}", guids, queryFilter);
        //filtering by guid
        var filter = await query.Select(c => c.CustomerGuid)
            .Where(c => queryFilter.Contains(c))
            .ToListAsync();
			_logger.LogError("GetNotExistingCustomersAsync(guids)={Guids}: database query executed in {Arg1} ms", guids, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetNotExistingCustomersAsync(guids={Guids})", guids);
        return queryFilter.Except(filter).ToArray();
    }

    #endregion

    #region Customer roles

    /// <summary>
    /// Add a customer-customer role mapping
    /// </summary>
    /// <param name="roleMapping">Customer-customer role mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AddCustomerRoleMappingAsync(CustomerCustomerRoleMapping roleMapping)
    {
		_logger.LogError(">>> Entering AddCustomerRoleMappingAsync(roleMapping=<redacted>)");
        await _customerCustomerRoleMappingRepository.InsertAsync(roleMapping);
		_logger.LogWarning("<<< Exiting AddCustomerRoleMappingAsync(roleMapping=<redacted>)");
    }

    /// <summary>
    /// Remove a customer-customer role mapping
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="role">Customer role</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveCustomerRoleMappingAsync(Customer customer, CustomerRole role)
    {
		_logger.LogError(">>> Entering RemoveCustomerRoleMappingAsync(customer={Customer},role={Role})", customer, role);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(role);

        var mapping = await _customerCustomerRoleMappingRepository.Table
            .SingleOrDefaultAsync(ccrm => ccrm.CustomerId == customer.Id && ccrm.CustomerRoleId == role.Id);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("RemoveCustomerRoleMappingAsync(customer,role)={Customer},{Role}: database query executed in {Arg2} ms", customer, role, __logStartDb.ElapsedMilliseconds);

        if (mapping != null)
            await _customerCustomerRoleMappingRepository.DeleteAsync(mapping);
			_logger.LogError("RemoveCustomerRoleMappingAsync(customer,role)={Customer},{Role}: external service call took {Arg2} ms", customer, role, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting RemoveCustomerRoleMappingAsync(customer={Customer},role={Role})", customer, role);
    }

    /// <summary>
    /// Delete a customer role
    /// </summary>
    /// <param name="customerRole">Customer role</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCustomerRoleAsync(CustomerRole customerRole)
    {
		_logger.LogError(">>> Entering DeleteCustomerRoleAsync(customerRole={CustomerRole})", customerRole);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customerRole);
		_logger.LogWarning("<<< Exiting DeleteCustomerRoleAsync(customerRole={CustomerRole})", customerRole);

		_logger.LogError("Throwing NopException from DeleteCustomerRoleAsync(customerRole={CustomerRole})", customerRole);
        if (customerRole.IsSystemRole)
            throw new NopException("System role could not be deleted");
			_logger.LogError("DeleteCustomerRoleAsync(customerRole)={CustomerRole}: external service call took {Arg1} ms", customerRole, __logStart.ElapsedMilliseconds);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("DeleteCustomerRoleAsync(customerRole)={CustomerRole}: delete query executed in {Arg1} ms", customerRole, __logStartDb.ElapsedMilliseconds);

        await _customerRoleRepository.DeleteAsync(customerRole);
    }

    /// <summary>
    /// Gets a customer role
    /// </summary>
    /// <param name="customerRoleId">Customer role identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role
    /// </returns>
    public virtual async Task<CustomerRole> GetCustomerRoleByIdAsync(int customerRoleId)
    {
		_logger.LogError(">>> Entering GetCustomerRoleByIdAsync(customerRoleId={CustomerRoleId})", customerRoleId);
        var allRolesById = await GetAllCustomerRolesDictionaryAsync();

		_logger.LogError("GetCustomerRoleByIdAsync(customerRoleId={CustomerRoleId}): allRolesById → {AllRolesById}", customerRoleId, allRolesById);
		_logger.LogWarning("<<< Exiting GetCustomerRoleByIdAsync(customerRoleId={CustomerRoleId})", customerRoleId);
        return allRolesById.TryGetValue(customerRoleId, out var role) ? role : null;
    }

    /// <summary>
    /// Gets a customer role
    /// </summary>
    /// <param name="systemName">Customer role system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role
    /// </returns>
    public virtual async Task<CustomerRole> GetCustomerRoleBySystemNameAsync(string systemName)
    {
		_logger.LogError(">>> Entering GetCustomerRoleBySystemNameAsync(systemName={SystemName})", systemName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerServicesDefaults.CustomerRolesBySystemNameCacheKey, systemName);
		_logger.LogError("GetCustomerRoleBySystemNameAsync(systemName={SystemName}): key → {Key}", systemName, key);

        var query = from cr in _customerRoleRepository.Table
            orderby cr.Id
            where cr.SystemName == systemName
            select cr;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetCustomerRoleBySystemNameAsync(systemName)={SystemName}: external service call took {Arg1} ms", systemName, __logStart.ElapsedMilliseconds);
		_logger.LogError("GetCustomerRoleBySystemNameAsync(systemName)={SystemName}: find query executed in {Arg1} ms", systemName, __logStartDb.ElapsedMilliseconds);
        var customerRole = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
		_logger.LogError("GetCustomerRoleBySystemNameAsync(systemName={SystemName}): customerRole → {CustomerRole}", systemName, customerRole);

		_logger.LogWarning("<<< Exiting GetCustomerRoleBySystemNameAsync(systemName={SystemName})", systemName);
        return customerRole;
    }

    /// <summary>
    /// Get customer role identifiers
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="showHidden">A value indicating whether to load hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role identifiers
    /// </returns>
    public virtual async Task<int[]> GetCustomerRoleIdsAsync(Customer customer, bool showHidden = false)
    {
		_logger.LogError(">>> Entering GetCustomerRoleIdsAsync(customer={Customer},showHidden={ShowHidden})", customer, showHidden);
        ArgumentNullException.ThrowIfNull(customer);

		_logger.LogWarning("<<< Exiting GetCustomerRoleIdsAsync(customer={Customer},showHidden={ShowHidden})", customer, showHidden);
        return (await GetCustomerRolesAsync(customer, showHidden: showHidden))
            .Select(cr => cr.Id)
            .ToArray();
    }

    /// <summary>
    /// Gets list of customer roles
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="showHidden">A value indicating whether to load hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<CustomerRole>> GetCustomerRolesAsync(Customer customer, bool showHidden = false)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var allRolesById = await GetAllCustomerRolesDictionaryAsync();
		_logger.LogError("GetCustomerRolesAsync(customer={Customer},showHidden={ShowHidden}): allRolesById → {AllRolesById}", customer, showHidden, allRolesById);

        var mappings = await _shortTermCacheManager.GetAsync(
            async () => await _customerCustomerRoleMappingRepository.GetAllAsync(query => query.Where(crm => crm.CustomerId == customer.Id)), NopCustomerServicesDefaults.CustomerRolesCacheKey, customer);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();

        return mappings.Select(mapping => allRolesById.TryGetValue(mapping.CustomerRoleId, out var role) ? role : null)
            .Where(cr => cr != null && (showHidden || cr.Active))
            .ToList();
    }

    /// <summary>
    /// Gets all customer roles
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer roles
    /// </returns>
    public virtual async Task<IList<CustomerRole>> GetAllCustomerRolesAsync(bool showHidden = false)
    {
		_logger.LogError(">>> Entering GetAllCustomerRolesAsync(showHidden={ShowHidden})", showHidden);
        var allRolesById = await GetAllCustomerRolesDictionaryAsync();

		_logger.LogError("GetAllCustomerRolesAsync(showHidden={ShowHidden}): allRolesById → {AllRolesById}", showHidden, allRolesById);
		_logger.LogWarning("<<< Exiting GetAllCustomerRolesAsync(showHidden={ShowHidden})", showHidden);
        return allRolesById.Values
            .Where(cr => showHidden || cr.Active)
            .ToList();
    }

    /// <summary>
    /// Inserts a customer role
    /// </summary>
    /// <param name="customerRole">Customer role</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCustomerRoleAsync(CustomerRole customerRole)
    {
		_logger.LogError(">>> Entering InsertCustomerRoleAsync(customerRole={CustomerRole})", customerRole);
        await _customerRoleRepository.InsertAsync(customerRole);
		_logger.LogWarning("<<< Exiting InsertCustomerRoleAsync(customerRole={CustomerRole})", customerRole);
    }

    /// <summary>
    /// Gets a value indicating whether customer is in a certain customer role
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="customerRoleSystemName">Customer role system name</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsInCustomerRoleAsync(Customer customer,
        string customerRoleSystemName, bool onlyActiveCustomerRoles = true)
    {
		_logger.LogError(">>> Entering IsInCustomerRoleAsync(customer={Customer},customerRoleSystemName={CustomerRoleSystemName},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, customerRoleSystemName, onlyActiveCustomerRoles);
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentException.ThrowIfNullOrEmpty(customerRoleSystemName);

        var customerRoles = await GetCustomerRolesAsync(customer, !onlyActiveCustomerRoles);
		_logger.LogError("IsInCustomerRoleAsync(customer={Customer},customerRoleSystemName={CustomerRoleSystemName},onlyActiveCustomerRoles={OnlyActiveCustomerRoles}): customerRoles → {CustomerRoles}", customer, customerRoleSystemName, onlyActiveCustomerRoles, customerRoles);

		_logger.LogWarning("<<< Exiting IsInCustomerRoleAsync(customer={Customer},customerRoleSystemName={CustomerRoleSystemName},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, customerRoleSystemName, onlyActiveCustomerRoles);
        return customerRoles?.Any(cr => cr.SystemName == customerRoleSystemName) ?? false;
    }

    /// <summary>
    /// Gets a value indicating whether customer is administrator
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsAdminAsync(Customer customer, bool onlyActiveCustomerRoles = true)
    {
		_logger.LogError(">>> Entering IsAdminAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        _logger.LogWarning("<<< Exiting IsAdminAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        return await IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName, onlyActiveCustomerRoles);
    }

    /// <summary>
    /// Gets a value indicating whether customer is a forum moderator
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsForumModeratorAsync(Customer customer, bool onlyActiveCustomerRoles = true)
    {
		_logger.LogError(">>> Entering IsForumModeratorAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        _logger.LogWarning("<<< Exiting IsForumModeratorAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        return await IsInCustomerRoleAsync(customer, NopCustomerDefaults.ForumModeratorsRoleName, onlyActiveCustomerRoles);
    }

    /// <summary>
    /// Gets a value indicating whether customer is registered
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsRegisteredAsync(Customer customer, bool onlyActiveCustomerRoles = true)
    {
		_logger.LogError(">>> Entering IsRegisteredAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        _logger.LogWarning("<<< Exiting IsRegisteredAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        return await IsInCustomerRoleAsync(customer, NopCustomerDefaults.RegisteredRoleName, onlyActiveCustomerRoles);
    }

    /// <summary>
    /// Gets a value indicating whether customer is guest
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsGuestAsync(Customer customer, bool onlyActiveCustomerRoles = true)
    {
		_logger.LogError(">>> Entering IsGuestAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        _logger.LogWarning("<<< Exiting IsGuestAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        return await IsInCustomerRoleAsync(customer, NopCustomerDefaults.GuestsRoleName, onlyActiveCustomerRoles);
    }

    /// <summary>
    /// Gets a value indicating whether customer is vendor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsVendorAsync(Customer customer, bool onlyActiveCustomerRoles = true)
    {
		_logger.LogError(">>> Entering IsVendorAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        _logger.LogWarning("<<< Exiting IsVendorAsync(customer={Customer},onlyActiveCustomerRoles={OnlyActiveCustomerRoles})", customer, onlyActiveCustomerRoles);
        return await IsInCustomerRoleAsync(customer, NopCustomerDefaults.VendorsRoleName, onlyActiveCustomerRoles);
    }

    /// <summary>
    /// Updates the customer role
    /// </summary>
    /// <param name="customerRole">Customer role</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCustomerRoleAsync(CustomerRole customerRole)
    {
		_logger.LogError(">>> Entering UpdateCustomerRoleAsync(customerRole={CustomerRole})", customerRole);
        await _customerRoleRepository.UpdateAsync(customerRole);
		_logger.LogWarning("<<< Exiting UpdateCustomerRoleAsync(customerRole={CustomerRole})", customerRole);
    }

    #endregion

    #region Customer passwords

    /// <summary>
    /// Gets customer passwords
    /// </summary>
    /// <param name="customerId">Customer identifier; pass null to load all records</param>
    /// <param name="passwordFormat">Password format; pass null to load all records</param>
    /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of customer passwords
    /// </returns>
    public virtual async Task<IList<CustomerPassword>> GetCustomerPasswordsAsync(int? customerId = null,
        PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
    {
		_logger.LogError(">>> Entering GetCustomerPasswordsAsync(customerId={CustomerId},passwordFormat=<redacted>,passwordsToReturn=<redacted>)", customerId);
        var query = _customerPasswordRepository.Table;

		_logger.LogError("GetCustomerPasswordsAsync(customerId={CustomerId},passwordFormat=<redacted>,passwordsToReturn=<redacted>): query → {Query}", customerId, query);
        //filter by customer
        if (customerId.HasValue)
            query = query.Where(password => password.CustomerId == customerId.Value);

        //filter by password format
        if (passwordFormat.HasValue)
            query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

        //get the latest passwords
        if (passwordsToReturn.HasValue)
            query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get current customer password
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer password
    /// </returns>
    public virtual async Task<CustomerPassword> GetCurrentPasswordAsync(int customerId)
    {
		_logger.LogError(">>> Entering GetCurrentPasswordAsync(customerId={CustomerId})", customerId);
        if (customerId == 0)
            return null;

        //return the latest password
        return (await GetCustomerPasswordsAsync(customerId, passwordsToReturn: 1)).FirstOrDefault();
    }

    /// <summary>
    /// Insert a customer password
    /// </summary>
    /// <param name="customerPassword">Customer password</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCustomerPasswordAsync(CustomerPassword customerPassword)
    {
		_logger.LogError(">>> Entering InsertCustomerPasswordAsync(customerPassword=<redacted>)");
        await _customerPasswordRepository.InsertAsync(customerPassword);
		_logger.LogWarning("<<< Exiting InsertCustomerPasswordAsync(customerPassword=<redacted>)");
    }

    /// <summary>
    /// Update a customer password
    /// </summary>
    /// <param name="customerPassword">Customer password</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCustomerPasswordAsync(CustomerPassword customerPassword)
    {
		_logger.LogError(">>> Entering UpdateCustomerPasswordAsync(customerPassword=<redacted>)");
        await _customerPasswordRepository.UpdateAsync(customerPassword);
		_logger.LogWarning("<<< Exiting UpdateCustomerPasswordAsync(customerPassword=<redacted>)");
    }

    /// <summary>
    /// Check whether password recovery token is valid
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="token">Token to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPasswordRecoveryTokenValidAsync(Customer customer, string token)
    {
		_logger.LogError(">>> Entering IsPasswordRecoveryTokenValidAsync(customer={Customer},token=<redacted>)", customer);
        ArgumentNullException.ThrowIfNull(customer);

        var cPrt = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute);
		_logger.LogError("IsPasswordRecoveryTokenValidAsync(customer={Customer},token=<redacted>): cPrt → {CPrt}", customer, cPrt);
        if (string.IsNullOrEmpty(cPrt))
            return false;

        if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
            return false;

        return true;
    }

    /// <summary>
    /// Check whether password recovery link is expired
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPasswordRecoveryLinkExpiredAsync(Customer customer)
    {
		_logger.LogError(">>> Entering IsPasswordRecoveryLinkExpiredAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        if (_customerSettings.PasswordRecoveryLinkDaysValid == 0)
            return false;

        var generatedDate = await _genericAttributeService.GetAttributeAsync<DateTime?>(customer, NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
		_logger.LogError("IsPasswordRecoveryLinkExpiredAsync(customer={Customer}): generatedDate → {GeneratedDate}", customer, generatedDate);
        if (!generatedDate.HasValue)
            return false;

        var daysPassed = (DateTime.UtcNow - generatedDate.Value).TotalDays;
		_logger.LogError("IsPasswordRecoveryLinkExpiredAsync(customer={Customer}): daysPassed → {DaysPassed}", customer, daysPassed);
        if (daysPassed > _customerSettings.PasswordRecoveryLinkDaysValid)
            return true;

        return false;
    }

    /// <summary>
    /// Check whether customer password is expired 
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if password is expired; otherwise false
    /// </returns>
    public virtual async Task<bool> IsPasswordExpiredAsync(Customer customer)
    {
		_logger.LogError(">>> Entering IsPasswordExpiredAsync(customer={Customer})", customer);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        //the guests don't have a password
        if (await IsGuestAsync(customer))
            return false;

        //password lifetime is disabled for user
        if (!(await GetCustomerRolesAsync(customer)).Any(role => role.Active && role.EnablePasswordLifetime))
            return false;

        //setting disabled for all
        if (_customerSettings.PasswordLifetime == 0)
            return false;

        //get current password usage time
        var currentLifetime = await _shortTermCacheManager.GetAsync(async () =>
        {
            var customerPassword = await GetCurrentPasswordAsync(customer.Id);
			_logger.LogError("IsPasswordExpiredAsync(customer={Customer}): customerPassword → {CustomerPassword}", customer, customerPassword);
            //password is not found, so return max value to force customer to change password
            if (customerPassword == null)
                return int.MaxValue;

            return (DateTime.UtcNow - customerPassword.CreatedOnUtc).Days;
        }, NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, customer);

		_logger.LogError("IsPasswordExpiredAsync(customer)={Customer}: external service call took {Arg1} ms", customer, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting IsPasswordExpiredAsync(customer={Customer})", customer);
        return currentLifetime >= _customerSettings.PasswordLifetime;
    }

    #endregion

    #region Customer address mapping

    /// <summary>
    /// Remove a customer-address mapping record
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveCustomerAddressAsync(Customer customer, Address address)
    {
		_logger.LogError(">>> Entering RemoveCustomerAddressAsync(customer={Customer},address={Address})", customer, address);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

		_logger.LogWarning("<<< Exiting RemoveCustomerAddressAsync(customer={Customer},address={Address})", customer, address);
        if (await _customerAddressMappingRepository.Table
                .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.CustomerId == customer.Id)
            is CustomerAddressMapping mapping)
        {
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
            if (customer.BillingAddressId == address.Id)
                customer.BillingAddressId = null;
				_logger.LogDebug("RemoveCustomerAddressAsync(customer={Customer},address={Address}): customer.ShippingAddressId → {Arg2}", customer, address, customer.ShippingAddressId);
            if (customer.ShippingAddressId == address.Id)
                customer.ShippingAddressId = null;
				_logger.LogDebug("RemoveCustomerAddressAsync(customer={Customer},address={Address}): customer.BillingAddressId → {Arg2}", customer, address, customer.BillingAddressId);
				_logger.LogError("RemoveCustomerAddressAsync(customer,address)={Customer},{Address}: external service call took {Arg2} ms", customer, address, __logStart.ElapsedMilliseconds);
				_logger.LogError("RemoveCustomerAddressAsync(customer,address)={Customer},{Address}: delete query executed in {Arg2} ms", customer, address, __logStartDb.ElapsedMilliseconds);
				_logger.LogError("RemoveCustomerAddressAsync(customer,address)={Customer},{Address}: database query executed in {Arg2} ms", customer, address, __logStartDb.ElapsedMilliseconds);

            await _customerAddressMappingRepository.DeleteAsync(mapping);
        }
    }

    /// <summary>
    /// Inserts a customer-address mapping record
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCustomerAddressAsync(Customer customer, Address address)
    {
		_logger.LogError(">>> Entering InsertCustomerAddressAsync(customer={Customer},address={Address})", customer, address);
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(address);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (await _customerAddressMappingRepository.Table
                .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.CustomerId == customer.Id)
            is null)
        {
            var mapping = new CustomerAddressMapping
            {
                AddressId = address.Id,
                CustomerId = customer.Id
            };

			_logger.LogWarning("<<< Exiting InsertCustomerAddressAsync(customer={Customer},address={Address})", customer, address);
			_logger.LogError("InsertCustomerAddressAsync(customer,address)={Customer},{Address}: save query executed in {Arg2} ms", customer, address, __logStart.ElapsedMilliseconds);
			_logger.LogError("InsertCustomerAddressAsync(customer,address)={Customer},{Address}: database query executed in {Arg2} ms", customer, address, __logStart.ElapsedMilliseconds);
            await _customerAddressMappingRepository.InsertAsync(mapping);
        }
    }

    /// <summary>
    /// Gets a list of addresses mapped to customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<Address>> GetAddressesByCustomerIdAsync(int customerId)
    {
		_logger.LogError(">>> Entering GetAddressesByCustomerIdAsync(customerId={CustomerId})", customerId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from address in _customerAddressRepository.Table
            join cam in _customerAddressMappingRepository.Table on address.Id equals cam.AddressId
            where cam.CustomerId == customerId
            select address;

		_logger.LogError("GetAddressesByCustomerIdAsync(customerId)={CustomerId}: find query executing", customerId);
		_logger.LogError("GetAddressesByCustomerIdAsync(customerId)={CustomerId}: external service call took {Arg1} ms", customerId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetAddressesByCustomerIdAsync(customerId={CustomerId})", customerId);
        return await _shortTermCacheManager.GetAsync(async () => await query.ToListAsync(), NopCustomerServicesDefaults.CustomerAddressesCacheKey, customerId);
    }

    /// <summary>
    /// Gets a address mapped to customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="addressId">Address identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<Address> GetCustomerAddressAsync(int customerId, int addressId)
    {
		_logger.LogError(">>> Entering GetCustomerAddressAsync(customerId={CustomerId},addressId={AddressId})", customerId, addressId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (customerId == 0 || addressId == 0)
            return null;

        var query = from address in _customerAddressRepository.Table
            join cam in _customerAddressMappingRepository.Table on address.Id equals cam.AddressId
            where cam.CustomerId == customerId && address.Id == addressId
            select address;

		_logger.LogError("GetCustomerAddressAsync(customerId,addressId)={CustomerId},{AddressId}: find query executing", customerId, addressId);
		_logger.LogError("GetCustomerAddressAsync(customerId,addressId)={CustomerId},{AddressId}: external service call took {Arg2} ms", customerId, addressId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomerAddressAsync(customerId={CustomerId},addressId={AddressId})", customerId, addressId);
        return await _shortTermCacheManager.GetAsync(async () => await query.FirstOrDefaultAsync(), NopCustomerServicesDefaults.CustomerAddressCacheKey, customerId, addressId);
    }

    /// <summary>
    /// Gets a customer billing address
    /// </summary>
    /// <param name="customer">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<Address> GetCustomerBillingAddressAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerBillingAddressAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

		_logger.LogWarning("<<< Exiting GetCustomerBillingAddressAsync(customer={Customer})", customer);
        return await GetCustomerAddressAsync(customer.Id, customer.BillingAddressId ?? 0);
    }

    /// <summary>
    /// Gets a customer shipping address
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<Address> GetCustomerShippingAddressAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerShippingAddressAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

		_logger.LogWarning("<<< Exiting GetCustomerShippingAddressAsync(customer={Customer})", customer);
        return await GetCustomerAddressAsync(customer.Id, customer.ShippingAddressId ?? 0);
    }

    #endregion

    #endregion
}