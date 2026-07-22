using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;

namespace Nop.Services.Messages;

/// <summary>
/// Email account service
/// </summary>
public partial class EmailAccountService : IEmailAccountService
{
    #region Fields

    protected readonly IRepository<EmailAccount> _emailAccountRepository;

    #endregion

    #region Ctor

    public EmailAccountService(IRepository<EmailAccount> emailAccountRepository)
    {
		logger.LogDebug(">>> Entering EmailAccountService(emailAccountRepository={EmailAccountRepository})", emailAccountRepository);
        _emailAccountRepository = emailAccountRepository;
		logger.LogDebug("<<< Exiting EmailAccountService(emailAccountRepository={EmailAccountRepository})", emailAccountRepository);
		logger.LogDebug("EmailAccountService(emailAccountRepository={EmailAccountRepository}): _emailAccountRepository → {_emailAccountRepository}", emailAccountRepository, _emailAccountRepository);
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
		logger.LogDebug(">>> Entering InsertEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
		logger.LogDebug("<<< Exiting InsertEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
		logger.LogInformation("InsertEmailAccountAsync(emailAccount)={EmailAccount}: save query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Updates an email account
    /// </summary>
    /// <param name="emailAccount">Email account</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateEmailAccountAsync(EmailAccount emailAccount)
    {
		logger.LogDebug(">>> Entering UpdateEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
		logger.LogDebug("<<< Exiting UpdateEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
		logger.LogInformation("UpdateEmailAccountAsync(emailAccount)={EmailAccount}: update query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Deletes an email account
    /// </summary>
    /// <param name="emailAccount">Email account</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteEmailAccountAsync(EmailAccount emailAccount)
    {
		logger.LogDebug(">>> Entering DeleteEmailAccountAsync(emailAccount={EmailAccount})", emailAccount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(emailAccount);

        if ((await GetAllEmailAccountsAsync()).Count == 1)
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            throw new NopException("You cannot delete this email account. At least one account is required.");

        await _emailAccountRepository.DeleteAsync(emailAccount);
		logger.LogInformation("DeleteEmailAccountAsync(emailAccount)={EmailAccount}: external service call took {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
		logger.LogInformation("DeleteEmailAccountAsync(emailAccount)={EmailAccount}: delete query executed in {Arg1} ms", emailAccount, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetEmailAccountByIdAsync(emailAccountId={EmailAccountId})", emailAccountId);
		logger.LogDebug("<<< Exiting GetEmailAccountByIdAsync(emailAccountId={EmailAccountId})", emailAccountId);
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
		logger.LogDebug(">>> Entering GetAllEmailAccountsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var emailAccounts = await _emailAccountRepository.GetAllAsync(query =>
        {
            return from ea in query
                orderby ea.Id
                select ea;
				logger.LogInformation("GetAllEmailAccountsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllEmailAccountsAsync()");
        return emailAccounts;
    }

    #endregion
}
