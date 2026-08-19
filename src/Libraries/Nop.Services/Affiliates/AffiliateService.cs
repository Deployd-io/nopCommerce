using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Seo;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Affiliates;

/// <summary>
/// Affiliate service
/// </summary>
public partial class AffiliateService : IAffiliateService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IRepository<Address> _addressRepository;
    protected readonly IRepository<Affiliate> _affiliateRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly SeoSettings _seoSettings;

    #endregion

    #region Ctor

    public AffiliateService(IAddressService addressService,
        IRepository<Address> addressRepository,
        IRepository<Affiliate> affiliateRepository,
        IRepository<Order> orderRepository,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        SeoSettings seoSettings)
    {
		_logger.LogDebug(">>> Entering AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings})", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings);
        _addressService = addressService;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _addressService → {_addressService}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _addressService);
        _addressRepository = addressRepository;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _addressRepository → {_addressRepository}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _addressRepository);
        _affiliateRepository = affiliateRepository;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _affiliateRepository → {_affiliateRepository}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _affiliateRepository);
        _orderRepository = orderRepository;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _orderRepository → {_orderRepository}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _orderRepository);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _urlRecordService → {_urlRecordService}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _urlRecordService);
        _webHelper = webHelper;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _webHelper → {_webHelper}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _webHelper);
        _seoSettings = seoSettings;
		_logger.LogDebug("AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings}): _seoSettings → {_seoSettings}", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings, _seoSettings);
		_logger.LogDebug("<<< Exiting AffiliateService(addressService={AddressService},addressRepository={AddressRepository},affiliateRepository={AffiliateRepository},orderRepository={OrderRepository},urlRecordService={UrlRecordService},webHelper={WebHelper},seoSettings={SeoSettings})", addressService, addressRepository, affiliateRepository, orderRepository, urlRecordService, webHelper, seoSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets an affiliate by affiliate identifier
    /// </summary>
    /// <param name="affiliateId">Affiliate identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    public virtual async Task<Affiliate> GetAffiliateByIdAsync(int affiliateId)
    {
		_logger.LogDebug(">>> Entering GetAffiliateByIdAsync(affiliateId={AffiliateId})", affiliateId);
		_logger.LogDebug("<<< Exiting GetAffiliateByIdAsync(affiliateId={AffiliateId})", affiliateId);
        return await _affiliateRepository.GetByIdAsync(affiliateId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets an affiliate by friendly URL name
    /// </summary>
    /// <param name="friendlyUrlName">Friendly URL name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    public virtual async Task<Affiliate> GetAffiliateByFriendlyUrlNameAsync(string friendlyUrlName)
    {
		_logger.LogDebug(">>> Entering GetAffiliateByFriendlyUrlNameAsync(friendlyUrlName={FriendlyUrlName})", friendlyUrlName);
        if (string.IsNullOrWhiteSpace(friendlyUrlName))
            return null;

        var query = from a in _affiliateRepository.Table
            orderby a.Id
            where a.FriendlyUrlName == friendlyUrlName
            select a;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var affiliate = await query.FirstOrDefaultAsync();

		_logger.LogDebug("GetAffiliateByFriendlyUrlNameAsync(friendlyUrlName={FriendlyUrlName}): affiliate → {Affiliate}", friendlyUrlName, affiliate);
		_logger.LogInformation("GetAffiliateByFriendlyUrlNameAsync(friendlyUrlName)={FriendlyUrlName}: database query executed in {Arg1} ms", friendlyUrlName, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAffiliateByFriendlyUrlNameAsync(friendlyUrlName={FriendlyUrlName})", friendlyUrlName);
        return affiliate;
    }

    /// <summary>
    /// Marks affiliate as deleted 
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAffiliateAsync(Affiliate affiliate)
    {
		_logger.LogDebug(">>> Entering DeleteAffiliateAsync(affiliate={Affiliate})", affiliate);
		_logger.LogInformation("DeleteAffiliateAsync(affiliate)={Affiliate}: external service call took {Arg1} ms", affiliate, __logStart.ElapsedMilliseconds);
        await _affiliateRepository.DeleteAsync(affiliate);
		_logger.LogDebug("<<< Exiting DeleteAffiliateAsync(affiliate={Affiliate})", affiliate);
    }

    /// <summary>
    /// Gets all affiliates
    /// </summary>
    /// <param name="friendlyUrlName">Friendly URL name; null to load all records</param>
    /// <param name="firstName">First name; null to load all records</param>
    /// <param name="lastName">Last name; null to load all records</param>
    /// <param name="loadOnlyWithOrders">Value indicating whether to load affiliates only with orders placed (by affiliated customers)</param>
    /// <param name="ordersCreatedFromUtc">Orders created date from (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
    /// <param name="ordersCreatedToUtc">Orders created date to (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliates
    /// </returns>
    public virtual async Task<IPagedList<Affiliate>> GetAllAffiliatesAsync(string friendlyUrlName = null,
        string firstName = null, string lastName = null,
        bool loadOnlyWithOrders = false,
        DateTime? ordersCreatedFromUtc = null, DateTime? ordersCreatedToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue,
        bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetAllAffiliatesAsync(friendlyUrlName={FriendlyUrlName},firstName={FirstName},lastName={LastName},loadOnlyWithOrders={LoadOnlyWithOrders},ordersCreatedFromUtc={OrdersCreatedFromUtc},ordersCreatedToUtc={OrdersCreatedToUtc},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", friendlyUrlName, firstName, lastName, loadOnlyWithOrders, ordersCreatedFromUtc, ordersCreatedToUtc, pageIndex, pageSize, showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _affiliateRepository.GetAllPagedAsync(query =>
        {
			_logger.LogDebug("<<< Exiting GetAllAffiliatesAsync(friendlyUrlName={FriendlyUrlName},firstName={FirstName},lastName={LastName},loadOnlyWithOrders={LoadOnlyWithOrders},ordersCreatedFromUtc={OrdersCreatedFromUtc},ordersCreatedToUtc={OrdersCreatedToUtc},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", friendlyUrlName, firstName, lastName, loadOnlyWithOrders, ordersCreatedFromUtc, ordersCreatedToUtc, pageIndex, pageSize, showHidden);
            if (!string.IsNullOrWhiteSpace(friendlyUrlName))
                query = query.Where(a => a.FriendlyUrlName.Contains(friendlyUrlName));
				_logger.LogInformation("GetAllAffiliatesAsync(friendlyUrlName,firstName,lastName,loadOnlyWithOrders,ordersCreatedFromUtc,ordersCreatedToUtc,pageIndex,pageSize,showHidden)={FriendlyUrlName},{FirstName},{LastName},{LoadOnlyWithOrders},{OrdersCreatedFromUtc},{OrdersCreatedToUtc},{PageIndex},{PageSize},{ShowHidden}: find query executed in {Arg9} ms", friendlyUrlName, firstName, lastName, loadOnlyWithOrders, ordersCreatedFromUtc, ordersCreatedToUtc, pageIndex, pageSize, showHidden, __logStart.ElapsedMilliseconds);

            if (!string.IsNullOrWhiteSpace(firstName))
                query = from aff in query
                    join addr in _addressRepository.Table on aff.AddressId equals addr.Id
                    where addr.FirstName.Contains(firstName)
                    select aff;

            if (!string.IsNullOrWhiteSpace(lastName))
                query = from aff in query
                    join addr in _addressRepository.Table on aff.AddressId equals addr.Id
                    where addr.LastName.Contains(lastName)
                    select aff;

            if (!showHidden)
                query = query.Where(a => a.Active);
            query = query.Where(a => !a.Deleted);

            if (loadOnlyWithOrders)
            {
                var ordersQuery = _orderRepository.Table;
                if (ordersCreatedFromUtc.HasValue)
                    ordersQuery = ordersQuery.Where(o => ordersCreatedFromUtc.Value <= o.CreatedOnUtc);
                if (ordersCreatedToUtc.HasValue)
                    ordersQuery = ordersQuery.Where(o => ordersCreatedToUtc.Value >= o.CreatedOnUtc);
                ordersQuery = ordersQuery.Where(o => !o.Deleted);

                query = from a in query
                    join o in ordersQuery on a.Id equals o.AffiliateId
                    select a;
            }

            query = query.Distinct().OrderByDescending(a => a.Id);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Inserts an affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAffiliateAsync(Affiliate affiliate)
    {
		_logger.LogDebug(">>> Entering InsertAffiliateAsync(affiliate={Affiliate})", affiliate);
        await _affiliateRepository.InsertAsync(affiliate);
		_logger.LogDebug("<<< Exiting InsertAffiliateAsync(affiliate={Affiliate})", affiliate);
    }

    /// <summary>
    /// Updates the affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAffiliateAsync(Affiliate affiliate)
    {
		_logger.LogDebug(">>> Entering UpdateAffiliateAsync(affiliate={Affiliate})", affiliate);
        await _affiliateRepository.UpdateAsync(affiliate);
		_logger.LogDebug("<<< Exiting UpdateAffiliateAsync(affiliate={Affiliate})", affiliate);
    }

    /// <summary>
    /// Get full name
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate full name
    /// </returns>
    public virtual async Task<string> GetAffiliateFullNameAsync(Affiliate affiliate)
    {
		_logger.LogDebug(">>> Entering GetAffiliateFullNameAsync(affiliate={Affiliate})", affiliate);
        ArgumentNullException.ThrowIfNull(affiliate);

        var affiliateAddress = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
		_logger.LogDebug("GetAffiliateFullNameAsync(affiliate={Affiliate}): affiliateAddress → {AffiliateAddress}", affiliate, affiliateAddress);

        if (affiliateAddress == null)
            return string.Empty;

        var fullName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";
		_logger.LogDebug("GetAffiliateFullNameAsync(affiliate={Affiliate}): fullName → {FullName}", affiliate, fullName);

		_logger.LogDebug("<<< Exiting GetAffiliateFullNameAsync(affiliate={Affiliate})", affiliate);
        return fullName;
    }

    /// <summary>
    /// Generate affiliate URL
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated affiliate URL
    /// </returns>
    public virtual Task<string> GenerateUrlAsync(Affiliate affiliate)
    {
		_logger.LogDebug(">>> Entering GenerateUrlAsync(affiliate={Affiliate})", affiliate);
        ArgumentNullException.ThrowIfNull(affiliate);

        var storeUrl = _webHelper.GetStoreLocation();
		_logger.LogDebug("GenerateUrlAsync(affiliate={Affiliate}): storeUrl → {StoreUrl}", affiliate, storeUrl);
        var url = !string.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
            //use friendly URL
            _webHelper.ModifyQueryString(storeUrl, NopAffiliateDefaults.AffiliateQueryParameter, affiliate.FriendlyUrlName) :
            //use ID
            _webHelper.ModifyQueryString(storeUrl, NopAffiliateDefaults.AffiliateIdQueryParameter, affiliate.Id.ToString());

		_logger.LogDebug("<<< Exiting GenerateUrlAsync(affiliate={Affiliate})", affiliate);
        return Task.FromResult(url);
    }

    /// <summary>
    /// Validate friendly URL name
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <param name="friendlyUrlName">Friendly URL name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid friendly name
    /// </returns>
    public virtual async Task<string> ValidateFriendlyUrlNameAsync(Affiliate affiliate, string friendlyUrlName)
    {
		_logger.LogDebug(">>> Entering ValidateFriendlyUrlNameAsync(affiliate={Affiliate},friendlyUrlName={FriendlyUrlName})", affiliate, friendlyUrlName);
        ArgumentNullException.ThrowIfNull(affiliate);

        //ensure we have only valid chars
        friendlyUrlName = await _urlRecordService.GetSeNameAsync(friendlyUrlName, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);
		_logger.LogDebug("ValidateFriendlyUrlNameAsync(affiliate={Affiliate},friendlyUrlName={FriendlyUrlName}): friendlyUrlName → {FriendlyUrlName1}", affiliate, friendlyUrlName, friendlyUrlName);

        //max length
        //(consider a store URL + probably added {0}-{1} below)
        friendlyUrlName = CommonHelper.EnsureMaximumLength(friendlyUrlName, NopAffiliateDefaults.FriendlyUrlNameLength);

        //ensure this name is not reserved yet
        //empty? nothing to check
        if (string.IsNullOrEmpty(friendlyUrlName))
            return friendlyUrlName;
        //check whether such friendly URL name already exists (and that is not the current affiliate)
        var i = 2;
        var tempName = friendlyUrlName;
		_logger.LogDebug("ValidateFriendlyUrlNameAsync(affiliate={Affiliate},friendlyUrlName={FriendlyUrlName}): tempName → {TempName}", affiliate, friendlyUrlName, tempName);
        while (true)
        {
            var affiliateByFriendlyUrlName = await GetAffiliateByFriendlyUrlNameAsync(tempName);
			_logger.LogDebug("ValidateFriendlyUrlNameAsync(affiliate={Affiliate},friendlyUrlName={FriendlyUrlName}): affiliateByFriendlyUrlName → {AffiliateByFriendlyUrlName}", affiliate, friendlyUrlName, affiliateByFriendlyUrlName);
            var reserved = affiliateByFriendlyUrlName != null && affiliateByFriendlyUrlName.Id != affiliate.Id;
            if (!reserved)
                break;

            tempName = $"{friendlyUrlName}-{i}";
            i++;
        }

        friendlyUrlName = tempName;

		_logger.LogDebug("<<< Exiting ValidateFriendlyUrlNameAsync(affiliate={Affiliate},friendlyUrlName={FriendlyUrlName})", affiliate, friendlyUrlName);
        return friendlyUrlName;
    }

    #endregion
}