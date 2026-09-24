using Google.Authenticator;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;

/// <summary>
/// Represents Google Authenticator service
/// </summary>
public class GoogleAuthenticatorService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering GoogleAuthenticatorService(repository={Repository},staticCacheManager={StaticCacheManager},workContext={WorkContext},googleAuthenticatorSettings={GoogleAuthenticatorSettings})", repository, staticCacheManager, workContext, googleAuthenticatorSettings);
        _repository = repository;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("GoogleAuthenticatorService(repository={Repository},staticCacheManager={StaticCacheManager},workContext={WorkContext},googleAuthenticatorSettings={GoogleAuthenticatorSettings}): _staticCacheManager → {_staticCacheManager}", repository, staticCacheManager, workContext, googleAuthenticatorSettings, _staticCacheManager);
        _workContext = workContext;
		_logger.LogDebug("GoogleAuthenticatorService(repository={Repository},staticCacheManager={StaticCacheManager},workContext={WorkContext},googleAuthenticatorSettings={GoogleAuthenticatorSettings}): _workContext → {_workContext}", repository, staticCacheManager, workContext, googleAuthenticatorSettings, _workContext);
        _googleAuthenticatorSettings = googleAuthenticatorSettings;
		_logger.LogDebug("GoogleAuthenticatorService(repository={Repository},staticCacheManager={StaticCacheManager},workContext={WorkContext},googleAuthenticatorSettings={GoogleAuthenticatorSettings}): _googleAuthenticatorSettings → {_googleAuthenticatorSettings}", repository, staticCacheManager, workContext, googleAuthenticatorSettings, _googleAuthenticatorSettings);
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
		_logger.LogError(">>> Entering InsertConfigurationAsync(configuration={Configuration})", configuration);
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _repository.InsertAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
		_logger.LogError("InsertConfigurationAsync(configuration)={Configuration}: save query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting InsertConfigurationAsync(configuration={Configuration})", configuration);
    }

    /// <summary>
    /// Update the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task UpdateConfigurationAsync(GoogleAuthenticatorRecord configuration)
    {
		_logger.LogError(">>> Entering UpdateConfigurationAsync(configuration={Configuration})", configuration);
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _repository.UpdateAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
		_logger.LogError("UpdateConfigurationAsync(configuration)={Configuration}: update query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateConfigurationAsync(configuration={Configuration})", configuration);
    }

    /// <summary>
    /// Delete the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    internal async Task DeleteConfigurationAsync(GoogleAuthenticatorRecord configuration)
    {
		_logger.LogError(">>> Entering DeleteConfigurationAsync(configuration={Configuration})", configuration);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteConfigurationAsync(configuration)={Configuration}: delete query executed in {Arg1} ms", configuration, __logStartDb.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteConfigurationAsync(configuration={Configuration})", configuration);
        await _repository.DeleteAsync(configuration);
		_logger.LogError("DeleteConfigurationAsync(configuration)={Configuration}: external service call took {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
        await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
    }

    /// <summary>
    /// Get a configuration by the identifier
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <returns>Configuration</returns>
    internal async Task<GoogleAuthenticatorRecord> GetConfigurationByIdAsync(int configurationId)
    {
		_logger.LogError(">>> Entering GetConfigurationByIdAsync(configurationId={ConfigurationId})", configurationId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (configurationId == 0)
            return null;

		_logger.LogError("GetConfigurationByIdAsync(configurationId)={ConfigurationId}: external service call took {Arg1} ms", configurationId, __logStart.ElapsedMilliseconds);
        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(GoogleAuthenticatorDefaults.ConfigurationCacheKey, configurationId), async () =>
            await _repository.GetByIdAsync(configurationId));
    }

    internal GoogleAuthenticatorRecord GetConfigurationByCustomerEmail(string email)
    {
		_logger.LogError(">>> Entering GetConfigurationByCustomerEmail(email={Email})", email);
        if (string.IsNullOrEmpty(email))
            return null;

        var query = _repository.Table;
		_logger.LogWarning("<<< Exiting GetConfigurationByCustomerEmail(email={Email})", email);
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
		_logger.LogError(">>> Entering GetPagedConfigurationsAsync(email={Email},pageIndex={PageIndex},pageSize={PageSize})", email, pageIndex, pageSize);
        var query = _repository.Table;
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(c => c.Customer.Contains(email));
        query = query.OrderBy(configuration => configuration.Id);

		_logger.LogWarning("<<< Exiting GetPagedConfigurationsAsync(email={Email},pageIndex={PageIndex},pageSize={PageSize})", email, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Check if the customer is registered  
    /// </summary>
    /// <param name="customerEmail"></param>
    /// <returns></returns>
    public bool IsRegisteredCustomer(string customerEmail)
    {
		_logger.LogError(">>> Entering IsRegisteredCustomer(customerEmail={CustomerEmail})", customerEmail);
        _logger.LogWarning("<<< Exiting IsRegisteredCustomer(customerEmail={CustomerEmail})", customerEmail);
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
		_logger.LogError(">>> Entering AddGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
        var account = new GoogleAuthenticatorRecord
        {
            Customer = customerEmail,
            SecretKey = key,
        };

        await InsertConfigurationAsync(account);
		_logger.LogWarning("<<< Exiting AddGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
    }

    /// <summary>
    /// Update configuration of GoogleAuthenticator
    /// </summary>
    /// <param name="customerEmail">Customer email</param>
    /// <param name="key">Secret key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateGoogleAuthenticatorAccountAsync(string customerEmail, string key)
    {
		_logger.LogError(">>> Entering UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
        var account = GetConfigurationByCustomerEmail(customerEmail);
		_logger.LogError("UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key}): account → {Account}", customerEmail, key, account);
        if (account != null)
        {
            account.SecretKey = key;
			_logger.LogDebug("UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key}): account.SecretKey → {Arg2}", customerEmail, key, account.SecretKey);
            await UpdateConfigurationAsync(account);
        }
		_logger.LogWarning("<<< Exiting UpdateGoogleAuthenticatorAccountAsync(customerEmail={CustomerEmail},key={Key})", customerEmail, key);
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
		_logger.LogError(">>> Entering GenerateSetupCode(secretkey=<redacted>)");
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogError("GenerateSetupCode(secretkey=<redacted>): customer → {Customer}", customer);
		_logger.LogWarning("<<< Exiting GenerateSetupCode(secretkey=<redacted>)");
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
		_logger.LogError(">>> Entering ValidateTwoFactorToken(secretkey=<redacted>,token=<redacted>)");
        _logger.LogWarning("<<< Exiting ValidateTwoFactorToken(secretkey=<redacted>,token=<redacted>)");
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