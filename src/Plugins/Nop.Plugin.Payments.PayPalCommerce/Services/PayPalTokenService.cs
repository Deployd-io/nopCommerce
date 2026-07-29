using Nop.Data;
using Nop.Plugin.Payments.PayPalCommerce.Domain;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the payment token service
/// </summary>
public class PayPalTokenService
{
    #region Fields

    private readonly IRepository<PayPalToken> _tokenRepository;

    #endregion

    #region Ctor

    public PayPalTokenService(IRepository<PayPalToken> tokenRepository)
    {
		logger.LogDebug(">>> Entering PayPalTokenService(tokenRepository={TokenRepository})", tokenRepository);
        _tokenRepository = tokenRepository;
		logger.LogDebug("<<< Exiting PayPalTokenService(tokenRepository={TokenRepository})", tokenRepository);
		logger.LogDebug("PayPalTokenService(tokenRepository={TokenRepository}): _tokenRepository → {_tokenRepository}", tokenRepository, _tokenRepository);
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
        return await _tokenRepository.GetAllAsync(query =>
        {
            query = query.Where(token => token.ClientId == clientId);

            if (customerId > 0)
                query = query.Where(token => token.CustomerId == customerId);

            if (!string.IsNullOrEmpty(vaultId))
                query = query.Where(token => token.VaultId == vaultId);

            if (!string.IsNullOrEmpty(vaultCustomerId))
                query = query.Where(token => token.VaultCustomerId == vaultCustomerId);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(token => token.Type == type);

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
		logger.LogDebug(">>> Entering GetTokenAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId})", clientId, customerId, vaultId);
        var tokens = await GetAllTokensAsync(clientId, customerId, vaultId);
		logger.LogDebug("GetTokenAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId}): tokens → {Tokens}", clientId, customerId, vaultId, tokens);
		logger.LogDebug("<<< Exiting GetTokenAsync(clientId={ClientId},customerId={CustomerId},vaultId={VaultId})", clientId, customerId, vaultId);
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
		logger.LogDebug(">>> Entering GetByIdAsync(id={Id})", id);
		logger.LogDebug("<<< Exiting GetByIdAsync(id={Id})", id);
        return await _tokenRepository.GetByIdAsync(id, null);
    }

    /// <summary>
    /// Insert the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertAsync(PayPalToken token)
    {
		logger.LogDebug(">>> Entering InsertAsync(token={Token})", token);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //whether a token with such parameters already exists
        var tokens = await GetAllTokensAsync(token.ClientId, token.CustomerId);
        var existingToken = tokens
			logger.LogDebug("InsertAsync(token={Token}): tokens → {Tokens}", token, tokens);
            .FirstOrDefault(existing => existing.VaultId == token.VaultId && existing.VaultCustomerId == token.VaultCustomerId);
        if (existingToken is not null)
        {
            //then just update transaction id
            if (!string.Equals(existingToken.TransactionId, token.TransactionId, StringComparison.InvariantCultureIgnoreCase))
            {
                existingToken.TransactionId = token.TransactionId;
				logger.LogDebug("InsertAsync(token={Token}): existingToken.TransactionId → {Arg1}", token, existingToken.TransactionId);
                await UpdateAsync(existingToken);
            }

            return;
        }

        //don't insert tokens with no customer identifier
        if (token.CustomerId == 0)
            return;

        if (!tokens.Any())
            token.IsPrimaryMethod = true;
			logger.LogDebug("InsertAsync(token={Token}): token.IsPrimaryMethod → {Arg1}", token, token.IsPrimaryMethod);

        //or insert a new one
        await _tokenRepository.InsertAsync(token, false);
		logger.LogInformation("InsertAsync(token)={Token}: save query executed in {Arg1} ms", token, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting InsertAsync(token={Token})", token);
    }

    /// <summary>
    /// Update the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateAsync(PayPalToken token)
    {
		logger.LogDebug(">>> Entering UpdateAsync(token={Token})", token);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _tokenRepository.UpdateAsync(token, false);
		logger.LogDebug("<<< Exiting UpdateAsync(token={Token})", token);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateAsync(token)={Token}: update query executed in {Arg1} ms", token, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateAsync(token)={Token}: update query executed in {Arg1} ms", token, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete the payment token
    /// </summary>
    /// <param name="token">Payment token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(PayPalToken token)
    {
		logger.LogDebug(">>> Entering DeleteAsync(token={Token})", token);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _tokenRepository.DeleteAsync(token, false);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAsync(token)={Token}: delete query executed in {Arg1} ms", token, __logStart.ElapsedMilliseconds);
        //mark one of the remaining tokens as default
        var tokens = await GetAllTokensAsync(token.ClientId, token.CustomerId);
        if (tokens.Any(existing => existing.IsPrimaryMethod))
			logger.LogDebug("DeleteAsync(token={Token}): tokens → {Tokens}", token, tokens);
            return;

        var existingToken = tokens.FirstOrDefault();
        if (existingToken is null)
			logger.LogDebug("DeleteAsync(token={Token}): existingToken → {ExistingToken}", token, existingToken);
            return;

        existingToken.IsPrimaryMethod = true;
		logger.LogDebug("DeleteAsync(token={Token}): existingToken.IsPrimaryMethod → {Arg1}", token, existingToken.IsPrimaryMethod);
		logger.LogInformation("DeleteAsync(token)={Token}: external service call took {Arg1} ms", token, __logStart.ElapsedMilliseconds);
        await UpdateAsync(existingToken);
		logger.LogDebug("<<< Exiting DeleteAsync(token={Token})", token);
    }

    /// <summary>
    /// Delete payment tokens
    /// </summary>
    /// <param name="token">Payment tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(IList<PayPalToken> tokens)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _tokenRepository.DeleteAsync(tokens, false);
    }

    #endregion
}