using Google.Authenticator;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;

/// <summary>
/// Represents Google Authenticator service
/// </summary>
public class GoogleAuthenticatorService
{
    #region Fields

    protected readonly IRepository<GoogleAuthenticatorRecord> _repository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;
    protected readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
    protected TwoFactorAuthenticator _twoFactorAuthenticator;
        
    #endregion

    #region Ctr

    public GoogleAuthenticatorService(
        IRepository<GoogleAuthenticatorRecord> repository,
        IStaticCacheManager staticCacheManager,
        IWorkContext workContext,
        GoogleAuthenticatorSettings googleAuthenticatorSettings)
    {
        _repository = repository;
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
        _googleAuthenticatorSettings = googleAuthenticatorSettings;
    }
    #endregion

    #region Utilities

    /// <summary>
    /// Insert the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task InsertConfigurationAsync(GoogleAuthenticatorRecord configuration)
    {
		logger.LogDebug(">>> Entering InsertConfigurationAsync(configuration={Configuration})", configuration);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(configuration);

        await _repository.InsertAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
		logger.LogInformation("InsertConfigurationAsync(configuration)={Configuration}: save query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting InsertConfigurationAsync(configuration={Configuration})", configuration);
    }

    /// <summary>
    /// Update the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task UpdateConfigurationAsync(GoogleAuthenticatorRecord configuration)
    {
		logger.LogDebug(">>> Entering UpdateConfigurationAsync(configuration={Configuration})", configuration);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(configuration);

        await _repository.UpdateAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
		logger.LogInformation("UpdateConfigurationAsync(configuration)={Configuration}: update query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting UpdateConfigurationAsync(configuration={Configuration})", configuration);
    }

    /// <summary>
    /// Delete the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    internal async Task DeleteConfigurationAsync(GoogleAuthenticatorRecord configuration)
    {
		logger.LogDebug(">>> Entering DeleteConfigurationAsync(configuration={Configuration})", configuration);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _repository.DeleteAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
		logger.LogDebug("<<< Exiting DeleteConfigurationAsync(configuration={Configuration})", configuration);
		logger.LogInformation("DeleteConfigurationAsync(configuration)={Configuration}: external service call took {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
		logger.LogInformation("DeleteConfigurationAsync(configuration)={Configuration}: delete query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Get a configuration by the identifier
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <returns>Configuration</returns>
    internal async Task<GoogleAuthenticatorRecord> GetConfigurationByIdAsync(int configurationId)
    {
		logger.LogDebug(">>> Entering GetConfigurationByIdAsync(configurationId={ConfigurationId})", configurationId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (configurationId == 0)
            return null;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

		logger.LogInformation("GetConfigurationByIdAsync(configurationId)={ConfigurationId}: external service call took {Arg1} ms", configurationId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetConfigurationByIdAsync(configurationId={ConfigurationId})", configurationId);
        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(GoogleAuthenticatorDefaults.ConfigurationCacheKey, configurationId), async () =>
            await _repository.GetByIdAsync(configurationId));
    }

    internal GoogleAuthenticatorRecord GetConfigurationByCustomerEmail(string email)
    {
		logger.LogDebug(">>> Entering GetConfigurationByCustomerEmail(email={Email})", email);
        if (string.IsNullOrEmpty(email))
            return null;

        var query = _repository.Table;
		logger.LogDebug("<<< Exiting GetConfigurationByCustomerEmail(email={Email})", email);
        return query.FirstOrDefault(record => record.Customer == email);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get configurations
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of configurations
    /// </returns>
    public async Task<IPagedList<GoogleAuthenticatorRecord>> GetPagedConfigurationsAsync(string email = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		logger.LogDebug(">>> Entering GetPagedConfigurationsAsync(email={Email},pageIndex={PageIndex},pageSize={PageSize})", email, pageIndex, pageSize);
        var query = _repository.Table;
        if (!string.IsNullOrWhiteSpace(email))
			logger.LogDebug("GetPagedConfigurationsAsync(email={Email},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", email, pageIndex, pageSize, query);
            query = query.Where(c => c.Customer.Contains(email));
        query = query.OrderBy(configuration => configuration.Id);

		logger.LogDebug("<<< Exiting GetPagedConfigurationsAsync(email={Email},pageIndex={PageIndex},pageSize={PageSize})", email, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Check if the customer is registered  
    /// </summary>
    /// <param name="customerEmail"></param>
    /// <returns></returns>
    public bool IsRegisteredCustomer(string customerEmail)
    {
		logger.LogDebug(">>> Entering IsRegisteredCustomer(customerEmail={CustomerEmail})", customerEmail);
		logger.LogDebug("<<< Exiting IsRegisteredCustomer(customerEmail={CustomerEmail})", customerEmail);
        return GetConfigurationByCustomerEmail(customerEmail) != null;
    }

    /// <summary>
    /// Add configuration of GoogleAuthenticator
    /// </summary>
    /// <param name="customerEmail">Customer email</param>
    /// <param name="key">Secret key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AddGoogleAuthenticatorAccountAsync(string customerEmail, string key)
    {
		logger.LogDebug(">>> Entering AddGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
        var account = new GoogleAuthenticatorRecord
        {
            Customer = customerEmail,
            SecretKey = key,
        };

        await InsertConfigurationAsync(account);
		logger.LogDebug("<<< Exiting AddGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
    }

    /// <summary>
    /// Update configuration of GoogleAuthenticator
    /// </summary>
    /// <param name="customerEmail">Customer email</param>
    /// <param name="key">Secret key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateGoogleAuthenticatorAccountAsync(string customerEmail, string key)
    {
		logger.LogDebug(">>> Entering UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
        var account = GetConfigurationByCustomerEmail(customerEmail);
        if (account != null)
		logger.LogDebug("UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key}): account → {Account}", customerEmail, key, account);
        {
            account.SecretKey = key;
			logger.LogDebug("UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key}): account.SecretKey → {Arg2}", customerEmail, key, account.SecretKey);
            await UpdateConfigurationAsync(account);
        }
		logger.LogDebug("<<< Exiting UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
    }

    /// <summary>
    /// Generate a setup code for a Google Authenticator user to scan
    /// </summary>
    /// <param name="secretkey">Secret key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public async Task<SetupCode> GenerateSetupCode(string secretkey)
    {
		logger.LogDebug(">>> Entering GenerateSetupCode(secretkey={Secretkey})", secretkey);
        var customer = await _workContext.GetCurrentCustomerAsync();

		logger.LogDebug("GenerateSetupCode(secretkey={Secretkey}): customer → {Customer}", secretkey, customer);
		logger.LogDebug("<<< Exiting GenerateSetupCode(secretkey={Secretkey})", secretkey);
        return TwoFactorAuthenticator.GenerateSetupCode(
            _googleAuthenticatorSettings.BusinessPrefix,
            customer.Email,
            secretkey, false, _googleAuthenticatorSettings.QRPixelsPerModule);
    }

    /// <summary>
    /// Validate token auth
    /// </summary>
    /// <param name="secretkey">Secret key</param>
    /// <param name="token">Token</param>
    /// <returns></returns>
    public bool ValidateTwoFactorToken(string secretkey, string token)
    {
		logger.LogDebug(">>> Entering ValidateTwoFactorToken(secretkey={Secretkey},token={Token})", secretkey, token);
		logger.LogDebug("<<< Exiting ValidateTwoFactorToken(secretkey={Secretkey},token={Token})", secretkey, token);
        return TwoFactorAuthenticator.ValidateTwoFactorPIN(secretkey, token);
    }

    #endregion

    #region Properties

    protected TwoFactorAuthenticator TwoFactorAuthenticator
    {
        get
        {
            _twoFactorAuthenticator = new TwoFactorAuthenticator();
            return _twoFactorAuthenticator;
        }
    }

    #endregion
}
