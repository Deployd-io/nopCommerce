using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Email account service
/// </summary>
public partial class EmailAccountService : IEmailAccountService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<EmailAccount> _emailAccountRepository;

    #endregion

    #region Ctor

    public EmailAccountService(IRepository<EmailAccount> emailAccountRepository)
    {
		_logger.LogDebug(">>> Entering EmailAccountService(emailAccountRepository={EmailAccountRepository})", emailAccountRepository);
        _emailAccountRepository = emailAccountRepository;
		_logger.LogDebug("EmailAccountService(emailAccountRepository={EmailAccountRepository}): _emailAccountRepository → {_emailAccountRepository}", emailAccountRepository, _emailAccountRepository);
		_logger.LogDebug("<<< Exiting EmailAccountService(emailAccountRepository={EmailAccountRepository})", emailAccountRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts an email account
    /// </summary>
    /// <param name="emailAccount">Email account</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertEmailAccountAsync(EmailAccount emailAccount)
    {
		_logger.LogDebug(">>> Entering InsertEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
        ArgumentNullException.ThrowIfNull(emailAccount);

        emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
        emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
        emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
        emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
        emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

        emailAccount.Email = emailAccount.Email.Trim();
        emailAccount.DisplayName = emailAccount.DisplayName.Trim();
        emailAccount.Host = emailAccount.Host.Trim();
        emailAccount.Username = emailAccount.Username.Trim();
        emailAccount.Password = emailAccount.Password.Trim();

        emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
        emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
        emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
        emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
        emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

        await _emailAccountRepository.InsertAsync(emailAccount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("InsertEmailAccountAsync(emailAccount)={EmailAccount}: save query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting InsertEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
    }

    /// <summary>
    /// Updates an email account
    /// </summary>
    /// <param name="emailAccount">Email account</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateEmailAccountAsync(EmailAccount emailAccount)
    {
		_logger.LogDebug(">>> Entering UpdateEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
        ArgumentNullException.ThrowIfNull(emailAccount);

        emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
        emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
        emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
        emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
        emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

        emailAccount.Email = emailAccount.Email.Trim();
        emailAccount.DisplayName = emailAccount.DisplayName.Trim();
        emailAccount.Host = emailAccount.Host.Trim();
        emailAccount.Username = emailAccount.Username.Trim();
        emailAccount.Password = emailAccount.Password.Trim();

        emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
        emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
        emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
        emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
        emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

        await _emailAccountRepository.UpdateAsync(emailAccount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("UpdateEmailAccountAsync(emailAccount)={EmailAccount}: update query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
    }

    /// <summary>
    /// Deletes an email account
    /// </summary>
    /// <param name="emailAccount">Email account</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteEmailAccountAsync(EmailAccount emailAccount)
    {
		_logger.LogDebug(">>> Entering DeleteEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(emailAccount);

        if ((await GetAllEmailAccountsAsync()).Count == 1)
			_logger.LogDebug("<<< Exiting DeleteEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
			_logger.LogInformation("DeleteEmailAccountAsync(emailAccount)={EmailAccount}: external service call took {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
			_logger.LogInformation("DeleteEmailAccountAsync(emailAccount)={EmailAccount}: delete query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
			_logger.LogWarning("Throwing NopException from DeleteEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
            throw new NopException("You cannot delete this email account. At least one account is required.");

        await _emailAccountRepository.DeleteAsync(emailAccount);
		_logger.LogInformation("DeleteEmailAccountAsync(emailAccount)={EmailAccount}: delete query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets an email account by identifier
    /// </summary>
    /// <param name="emailAccountId">The email account identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email account
    /// </returns>
    public virtual async Task<EmailAccount> GetEmailAccountByIdAsync(int emailAccountId)
    {
		_logger.LogDebug(">>> Entering GetEmailAccountByIdAsync(emailAccountId={EmailAccountId})", emailAccountId);
		_logger.LogDebug("<<< Exiting GetEmailAccountByIdAsync(emailAccountId={EmailAccountId})", emailAccountId);
		_logger.LogDebug(">>> Entering GetAllEmailAccountsAsync()");
		_logger.LogInformation("GetAllEmailAccountsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAllEmailAccountsAsync()");
        return await _emailAccountRepository.GetByIdAsync(emailAccountId, cache => default);
    }

    /// <summary>
    /// Gets all email accounts
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email accounts list
    /// </returns>
    public virtual async Task<IList<EmailAccount>> GetAllEmailAccountsAsync()
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var emailAccounts = await _emailAccountRepository.GetAllAsync(query =>
        {
            return from ea in query
                orderby ea.Id
                select ea;
				_logger.LogInformation("GetAllEmailAccountsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

        return emailAccounts;
    }

    #endregion
}