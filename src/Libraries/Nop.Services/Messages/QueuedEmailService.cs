using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Queued email service
/// </summary>
public partial class QueuedEmailService : IQueuedEmailService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<QueuedEmail> _queuedEmailRepository;

    #endregion

    #region Ctor

    public QueuedEmailService(IRepository<QueuedEmail> queuedEmailRepository)
    {
		_logger.LogError(">>> Entering QueuedEmailService(queuedEmailRepository={QueuedEmailRepository})", queuedEmailRepository);
        _queuedEmailRepository = queuedEmailRepository;
		_logger.LogDebug("QueuedEmailService(queuedEmailRepository={QueuedEmailRepository}): _queuedEmailRepository → {_queuedEmailRepository}", queuedEmailRepository, _queuedEmailRepository);
    _logger.LogWarning("<<< Exiting QueuedEmailService(queuedEmailRepository={QueuedEmailRepository})", queuedEmailRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts a queued email
    /// </summary>
    /// <param name="queuedEmail">Queued email</param>        
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertQueuedEmailAsync(QueuedEmail queuedEmail)
    {
		_logger.LogError(">>> Entering InsertQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
        await _queuedEmailRepository.InsertAsync(queuedEmail);
		_logger.LogWarning("<<< Exiting InsertQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
    }

    /// <summary>
    /// Updates a queued email
    /// </summary>
    /// <param name="queuedEmail">Queued email</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateQueuedEmailAsync(QueuedEmail queuedEmail)
    {
		_logger.LogError(">>> Entering UpdateQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
        await _queuedEmailRepository.UpdateAsync(queuedEmail);
		_logger.LogWarning("<<< Exiting UpdateQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
    }

    /// <summary>
    /// Deleted a queued email
    /// </summary>
    /// <param name="queuedEmail">Queued email</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteQueuedEmailAsync(QueuedEmail queuedEmail)
    {
		_logger.LogError(">>> Entering DeleteQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.DeleteAsync(queuedEmail);
		_logger.LogError("DeleteQueuedEmailAsync(queuedEmail)={QueuedEmail}: external service call took {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
    }

    /// <summary>
    /// Deleted a queued emails
    /// </summary>
    /// <param name="queuedEmails">Queued emails</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
    {
		_logger.LogError(">>> Entering DeleteQueuedEmailsAsync(queuedEmails={QueuedEmails})", queuedEmails);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.DeleteAsync(queuedEmails);
		_logger.LogError("DeleteQueuedEmailsAsync(queuedEmails)={QueuedEmails}: external service call took {Arg1} ms", queuedEmails, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteQueuedEmailsAsync(queuedEmails={QueuedEmails})", queuedEmails);
    }

    /// <summary>
    /// Gets a queued email by identifier
    /// </summary>
    /// <param name="queuedEmailId">Queued email identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email
    /// </returns>
    public virtual async Task<QueuedEmail> GetQueuedEmailByIdAsync(int queuedEmailId)
    {
		_logger.LogError(">>> Entering GetQueuedEmailByIdAsync(queuedEmailId={QueuedEmailId})", queuedEmailId);
		_logger.LogError("GetQueuedEmailByIdAsync(queuedEmailId)={QueuedEmailId}: find query executing", queuedEmailId);
    _logger.LogWarning("<<< Exiting GetQueuedEmailByIdAsync(queuedEmailId={QueuedEmailId})", queuedEmailId);
        return await _queuedEmailRepository.GetByIdAsync(queuedEmailId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get queued emails by identifiers
    /// </summary>
    /// <param name="queuedEmailIds">queued email identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued emails
    /// </returns>
    public virtual async Task<IList<QueuedEmail>> GetQueuedEmailsByIdsAsync(int[] queuedEmailIds)
    {
		_logger.LogError(">>> Entering GetQueuedEmailsByIdsAsync(queuedEmailIds={QueuedEmailIds})", queuedEmailIds);
        _logger.LogWarning("<<< Exiting GetQueuedEmailsByIdsAsync(queuedEmailIds={QueuedEmailIds})", queuedEmailIds);
        return await _queuedEmailRepository.GetByIdsAsync(queuedEmailIds);
    }

    /// <summary>
    /// Requeue a queued emails
    /// </summary>
    /// <param name="queuedEmails">Queued emails</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RequeueQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
    {
		_logger.LogError(">>> Entering RequeueQueuedEmailsAsync(queuedEmails={QueuedEmails})", queuedEmails);
        ArgumentNullException.ThrowIfNull(queuedEmails);

        var newEmails = queuedEmails.Select(queuedEmail => new QueuedEmail
        {
            PriorityId = queuedEmail.PriorityId,
            From = queuedEmail.From,
            FromName = queuedEmail.FromName,
            To = queuedEmail.To,
            ToName = queuedEmail.ToName,
            ReplyTo = queuedEmail.ReplyTo,
            ReplyToName = queuedEmail.ReplyToName,
            CC = queuedEmail.CC,
            Bcc = queuedEmail.Bcc,
            Subject = queuedEmail.Subject,
            Body = queuedEmail.Body,
            AttachmentFilePath = queuedEmail.AttachmentFilePath,
            AttachmentFileName = queuedEmail.AttachmentFileName,
            AttachedDownloadId = queuedEmail.AttachedDownloadId,
            CreatedOnUtc = DateTime.UtcNow,
            EmailAccountId = queuedEmail.EmailAccountId,
            DontSendBeforeDateUtc = queuedEmail.DontSendBeforeDateUtc
        }).ToList();
		_logger.LogDebug("RequeueQueuedEmailsAsync(queuedEmails={QueuedEmails}): DontSendBeforeDateUtc → {DontSendBeforeDateUtc}", queuedEmails, DontSendBeforeDateUtc);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _queuedEmailRepository.InsertAsync(newEmails);
		_logger.LogError("RequeueQueuedEmailsAsync(queuedEmails)={QueuedEmails}: save query executed in {Arg1} ms", queuedEmails, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting RequeueQueuedEmailsAsync(queuedEmails={QueuedEmails})", queuedEmails);
    }

    /// <summary>
    /// Gets all queued emails
    /// </summary>
    /// <param name="fromEmail">From Email</param>
    /// <param name="toEmail">To Email</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
    /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only emails for ready to be sent</param>
    /// <param name="maxSendTries">Maximum send tries</param>
    /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email item list
    /// </returns>
    public virtual async Task<IPagedList<QueuedEmail>> SearchEmailsAsync(string fromEmail,
        string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
        bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
        bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering SearchEmailsAsync(fromEmail={FromEmail},toEmail={ToEmail},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},loadNotSentItemsOnly={LoadNotSentItemsOnly},loadOnlyItemsToBeSent={LoadOnlyItemsToBeSent},maxSendTries={MaxSendTries},loadNewest={LoadNewest},pageIndex={PageIndex},pageSize={PageSize})", fromEmail, toEmail, createdFromUtc, createdToUtc, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest, pageIndex, pageSize);
        fromEmail = (fromEmail ?? string.Empty).Trim();
        toEmail = (toEmail ?? string.Empty).Trim();
		_logger.LogError("SearchEmailsAsync(fromEmail={FromEmail},toEmail={ToEmail},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},loadNotSentItemsOnly={LoadNotSentItemsOnly},loadOnlyItemsToBeSent={LoadOnlyItemsToBeSent},maxSendTries={MaxSendTries},loadNewest={LoadNewest},pageIndex={PageIndex},pageSize={PageSize}): toEmail → {ToEmail1}", fromEmail, toEmail, createdFromUtc, createdToUtc, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest, pageIndex, pageSize, toEmail);
		_logger.LogError("SearchEmailsAsync(fromEmail={FromEmail},toEmail={ToEmail},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},loadNotSentItemsOnly={LoadNotSentItemsOnly},loadOnlyItemsToBeSent={LoadOnlyItemsToBeSent},maxSendTries={MaxSendTries},loadNewest={LoadNewest},pageIndex={PageIndex},pageSize={PageSize}): fromEmail → {FromEmail1}", fromEmail, toEmail, createdFromUtc, createdToUtc, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest, pageIndex, pageSize, fromEmail);

        var query = _queuedEmailRepository.Table;
		_logger.LogError("SearchEmailsAsync(fromEmail={FromEmail},toEmail={ToEmail},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},loadNotSentItemsOnly={LoadNotSentItemsOnly},loadOnlyItemsToBeSent={LoadOnlyItemsToBeSent},maxSendTries={MaxSendTries},loadNewest={LoadNewest},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", fromEmail, toEmail, createdFromUtc, createdToUtc, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest, pageIndex, pageSize, query);
        if (!string.IsNullOrEmpty(fromEmail))
            query = query.Where(qe => qe.From.Contains(fromEmail));
        if (!string.IsNullOrEmpty(toEmail))
            query = query.Where(qe => qe.To.Contains(toEmail));
        if (createdFromUtc.HasValue)
            query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
        if (createdToUtc.HasValue)
            query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
        if (loadNotSentItemsOnly)
            query = query.Where(qe => !qe.SentOnUtc.HasValue);
        if (loadOnlyItemsToBeSent)
        {
            var nowUtc = DateTime.UtcNow;
            query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
        }

        query = query.Where(qe => qe.SentTries < maxSendTries);
        query = loadNewest ?
            //load the newest records
            query.OrderByDescending(qe => qe.CreatedOnUtc) :
            //load by priority
            query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedOnUtc);

        var queuedEmails = await query.ToPagedListAsync(pageIndex, pageSize);
		_logger.LogError("SearchEmailsAsync(fromEmail={FromEmail},toEmail={ToEmail},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},loadNotSentItemsOnly={LoadNotSentItemsOnly},loadOnlyItemsToBeSent={LoadOnlyItemsToBeSent},maxSendTries={MaxSendTries},loadNewest={LoadNewest},pageIndex={PageIndex},pageSize={PageSize}): queuedEmails → {QueuedEmails}", fromEmail, toEmail, createdFromUtc, createdToUtc, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest, pageIndex, pageSize, queuedEmails);

		_logger.LogWarning("<<< Exiting SearchEmailsAsync(fromEmail={FromEmail},toEmail={ToEmail},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},loadNotSentItemsOnly={LoadNotSentItemsOnly},loadOnlyItemsToBeSent={LoadOnlyItemsToBeSent},maxSendTries={MaxSendTries},loadNewest={LoadNewest},pageIndex={PageIndex},pageSize={PageSize})", fromEmail, toEmail, createdFromUtc, createdToUtc, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest, pageIndex, pageSize);
        return queuedEmails;
    }

    /// <summary>
    /// Deletes already sent emails
    /// </summary>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted emails
    /// </returns>
    public virtual async Task<int> DeleteAlreadySentEmailsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
    {
		_logger.LogError(">>> Entering DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc})", createdFromUtc, createdToUtc);
        var query = _queuedEmailRepository.Table;

		_logger.LogError("DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc}): query → {Query}", createdFromUtc, createdToUtc, query);
        // only sent emails
        query = query.Where(qe => qe.SentOnUtc.HasValue);

        if (createdFromUtc.HasValue)
            query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
        if (createdToUtc.HasValue)
            query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var emails = await query.ToArrayAsync();

		_logger.LogError("DeleteAlreadySentEmailsAsync(createdFromUtc,createdToUtc)={CreatedFromUtc},{CreatedToUtc}: database query executed in {Arg2} ms", createdFromUtc, createdToUtc, __logStart.ElapsedMilliseconds);
		_logger.LogError("DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc}): emails → {Emails}", createdFromUtc, createdToUtc, emails);
        await DeleteQueuedEmailsAsync(emails);

		_logger.LogWarning("<<< Exiting DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc})", createdFromUtc, createdToUtc);
        return emails.Length;
    }

    /// <summary>
    /// Delete all queued emails
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAllEmailsAsync()
    {
		_logger.LogError(">>> Entering DeleteAllEmailsAsync()");
        await _queuedEmailRepository.TruncateAsync();
		_logger.LogWarning("<<< Exiting DeleteAllEmailsAsync()");
    }

    #endregion
}