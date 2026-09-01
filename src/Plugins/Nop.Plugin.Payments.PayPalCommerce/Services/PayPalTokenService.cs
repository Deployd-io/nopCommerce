using Nop.Data;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the payment token service
/// </summary>
public class PayPalTokenService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IRepository<PayPalToken> _tokenRepository;

    #endregion

    #region Ctor

    public PayPalTokenService(IRepository<PayPalToken> tokenRepository)
    {
		_logger.LogDebug(">>> Entering PayPalTokenService(tokenRepository=<redacted>)");
        _tokenRepository = tokenRepository;
		_logger.LogDebug("PayPalTokenService(tokenRepository=<redacted>): _tokenRepository → {_tokenRepository}", _tokenRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all payment tokens
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="customerId">Customer identifier; pass 0 to load all tokens</param>
    /// <param name="vaultId">Vault identifier; pass null to load all tokens</param>
    /// <param name="vaultCustomerId">Vault customer identifier; pass null to load all tokens</param>
    /// <param name="type">Token type; pass null to load all tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of payment tokens
    /// </returns>
    public async Task<IList<PayPalToken>> GetAllTokensAsync(string clientId, int customerId = 0,
        string vaultId = null, string vaultCustomerId = null, string type = null)
    {
		_logger.LogDebug(">>> Entering GetAllTokensAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId},vaultCustomerId={VaultCustomerId},type={Type})", clientId, customerId, vaultId, vaultCustomerId, type);
        return await _tokenRepository.GetAllAsync(query =>
        {
			_logger.LogInformation("GetAllTokensAsync(clientId,customerId,vaultId,vaultCustomerId,type)={ClientId},{CustomerId},{VaultId},{VaultCustomerId},{Type}: find query executing", clientId, customerId, vaultId, vaultCustomerId, type);
            query = query.Where(token => token.ClientId == clientId);

            if (customerId > 0)
                query = query.Where(token => token.CustomerId == customerId);

            if (!string.IsNullOrEmpty(vaultId))
                query = query.Where(token => token.VaultId == vaultId);

            if (!string.IsNullOrEmpty(vaultCustomerId))
                query = query.Where(token => token.VaultCustomerId == vaultCustomerId);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(token => token.Type == type);

		_logger.LogDebug("<<< Exiting GetAllTokensAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId},vaultCustomerId={VaultCustomerId},type={Type})", clientId, customerId, vaultId, vaultCustomerId, type);
            return query;
        }, null);
    }

    /// <summary>
    /// Get a customer payment token
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="customerId">Customer identifier; pass 0 to load all tokens</param>
    /// <param name="vaultId">Vault identifier; pass null to load all tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment token
    /// </returns>
    public async Task<PayPalToken> GetTokenAsync(string clientId, int customerId = 0, string vaultId = null)
    {
		_logger.LogDebug(">>> Entering GetTokenAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId})", clientId, customerId, vaultId);
        var tokens = await GetAllTokensAsync(clientId, customerId, vaultId);
		_logger.LogDebug("GetTokenAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId}): tokens → {Tokens}", clientId, customerId, vaultId, tokens);
		_logger.LogDebug("<<< Exiting GetTokenAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId})", clientId, customerId, vaultId);
        return tokens.FirstOrDefault();
    }

    /// <summary>
    /// Get a payment token by identifier
    /// </summary>
    /// <param name="id">Token identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment token
    /// </returns>
    public async Task<PayPalToken> GetByIdAsync(int id)
    {
		_logger.LogDebug(">>> Entering GetByIdAsync(id={Id})", id);
        return await _tokenRepository.GetByIdAsync(id, null);
    }

    /// <summary>
    /// Insert the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertAsync(PayPalToken token)
    {
		_logger.LogDebug(">>> Entering InsertAsync(token=<redacted>)");
        //whether a token with such parameters already exists
        var tokens = await GetAllTokensAsync(token.ClientId, token.CustomerId);
		_logger.LogDebug("InsertAsync(token=<redacted>): tokens → {Tokens}", tokens);
        var existingToken = tokens
            .FirstOrDefault(existing => existing.VaultId == token.VaultId && existing.VaultCustomerId == token.VaultCustomerId);
        if (existingToken is not null)
        {
            //then just update transaction id
            if (!string.Equals(existingToken.TransactionId, token.TransactionId, StringComparison.InvariantCultureIgnoreCase))
            {
                existingToken.TransactionId = token.TransactionId;
				_logger.LogDebug("InsertAsync(token=<redacted>): existingToken.TransactionId → {Arg1}", existingToken.TransactionId);
                await UpdateAsync(existingToken);
            }

            return;
        }

        //don't insert tokens with no customer identifier
        if (token.CustomerId == 0)
            return;

        if (!tokens.Any())
            token.IsPrimaryMethod = true;
			_logger.LogDebug("InsertAsync(token=<redacted>): token.IsPrimaryMethod → {Arg1}", token.IsPrimaryMethod);

        //or insert a new one
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("InsertAsync(token)=<redacted>: save query executed in {Arg1} ms", __logStart.ElapsedMilliseconds);
        await _tokenRepository.InsertAsync(token, false);
		_logger.LogDebug("<<< Exiting InsertAsync(token=<redacted>)");
    }

    /// <summary>
    /// Update the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateAsync(PayPalToken token)
    {
		_logger.LogDebug(">>> Entering UpdateAsync(token=<redacted>)");
        await _tokenRepository.UpdateAsync(token, false);
    }

    /// <summary>
    /// Delete the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(PayPalToken token)
    {
		_logger.LogDebug(">>> Entering DeleteAsync(token=<redacted>)");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _tokenRepository.DeleteAsync(token, false);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        //mark one of the remaining tokens as default
        var tokens = await GetAllTokensAsync(token.ClientId, token.CustomerId);
		_logger.LogInformation("DeleteAsync(token)=<redacted>: delete query executed in {Arg1} ms", __logStartDb.ElapsedMilliseconds);
		_logger.LogDebug("DeleteAsync(token=<redacted>): tokens → {Tokens}", tokens);
        if (tokens.Any(existing => existing.IsPrimaryMethod))
            return;

        var existingToken = tokens.FirstOrDefault();
		_logger.LogDebug("DeleteAsync(token=<redacted>): existingToken → {ExistingToken}", existingToken);
        if (existingToken is null)
            return;

        existingToken.IsPrimaryMethod = true;
		_logger.LogInformation("DeleteAsync(tokens)=<redacted>: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
		_logger.LogInformation("DeleteAsync(token)=<redacted>: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
        await UpdateAsync(existingToken);
		_logger.LogDebug("<<< Exiting DeleteAsync(token=<redacted>)");
    }

    /// <summary>
    /// Delete payment tokens
    /// </summary>
    /// <param name="token">Payment tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(IList<PayPalToken> tokens)
    {
		_logger.LogDebug(">>> Entering DeleteAsync(tokens=<redacted>)");
        await _tokenRepository.DeleteAsync(tokens, false);
    }

    #endregion
}