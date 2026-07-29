using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;

namespace Nop.Services.Messages;

/// <summary>
/// Queued email service
/// </summary>
public partial class QueuedEmailService : IQueuedEmailService
{
    #region Fields

    protected readonly IRepository<QueuedEmail> _queuedEmailRepository;

    #endregion

    #region Ctor

    public QueuedEmailService(IRepository<QueuedEmail> queuedEmailRepository)
    {
		logger.LogDebug(">>> Entering QueuedEmailService(queuedEmailRepository={QueuedEmailRepository})", queuedEmailRepository);
        _queuedEmailRepository = queuedEmailRepository;
		logger.LogDebug("<<< Exiting QueuedEmailService(queuedEmailRepository={QueuedEmailRepository})", queuedEmailRepository);
		logger.LogDebug("QueuedEmailService(queuedEmailRepository={QueuedEmailRepository}): _queuedEmailRepository → {_queuedEmailRepository}", queuedEmailRepository, _queuedEmailRepository);
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
		logger.LogDebug(">>> Entering InsertQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.InsertAsync(queuedEmail);
		logger.LogDebug("<<< Exiting InsertQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertQueuedEmailAsync(queuedEmail)={QueuedEmail}: save query executed in {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertQueuedEmailAsync(queuedEmail)={QueuedEmail}: save query executed in {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a queued email
    /// </summary>
    /// <param name="queuedEmail">Queued email</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateQueuedEmailAsync(QueuedEmail queuedEmail)
    {
		logger.LogDebug(">>> Entering UpdateQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.UpdateAsync(queuedEmail);
		logger.LogDebug("<<< Exiting UpdateQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateQueuedEmailAsync(queuedEmail)={QueuedEmail}: update query executed in {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateQueuedEmailAsync(queuedEmail)={QueuedEmail}: update query executed in {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deleted a queued email
    /// </summary>
    /// <param name="queuedEmail">Queued email</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteQueuedEmailAsync(QueuedEmail queuedEmail)
    {
		logger.LogDebug(">>> Entering DeleteQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.DeleteAsync(queuedEmail);
		logger.LogDebug("<<< Exiting DeleteQueuedEmailAsync(queuedEmail={QueuedEmail})", queuedEmail);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteQueuedEmailAsync(queuedEmail)={QueuedEmail}: external service call took {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteQueuedEmailAsync(queuedEmail)={QueuedEmail}: delete query executed in {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteQueuedEmailAsync(queuedEmail)={QueuedEmail}: external service call took {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteQueuedEmailAsync(queuedEmail)={QueuedEmail}: delete query executed in {Arg1} ms", queuedEmail, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deleted a queued emails
    /// </summary>
    /// <param name="queuedEmails">Queued emails</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.DeleteAsync(queuedEmails);
    }

	var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
		logger.LogDebug(">>> Entering GetQueuedEmailByIdAsync(queuedEmailId={QueuedEmailId})", queuedEmailId);
		logger.LogDebug("<<< Exiting GetQueuedEmailByIdAsync(queuedEmailId={QueuedEmailId})", queuedEmailId);
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
		logger.LogDebug(">>> Entering GetQueuedEmailsByIdsAsync(queuedEmailIds={QueuedEmailIds})", queuedEmailIds);
		logger.LogDebug("<<< Exiting GetQueuedEmailsByIdsAsync(queuedEmailIds={QueuedEmailIds})", queuedEmailIds);
        return await _queuedEmailRepository.GetByIdsAsync(queuedEmailIds);
    }

    /// <summary>
    /// Requeue a queued emails
    /// </summary>
    /// <param name="queuedEmails">Queued emails</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RequeueQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
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

        await _queuedEmailRepository.InsertAsync(newEmails);
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
        fromEmail = (fromEmail ?? string.Empty).Trim();
        toEmail = (toEmail ?? string.Empty).Trim();

        var query = _queuedEmailRepository.Table;
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
		logger.LogDebug(">>> Entering DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc})", createdFromUtc, createdToUtc);
        var query = _queuedEmailRepository.Table;

		logger.LogDebug("DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc}): query → {Query}", createdFromUtc, createdToUtc, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        // only sent emails
        query = query.Where(qe => qe.SentOnUtc.HasValue);

        if (createdFromUtc.HasValue)
            query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
        if (createdToUtc.HasValue)
            query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);

        var emails = await query.ToArrayAsync();

		logger.LogDebug("DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc}): emails → {Emails}", createdFromUtc, createdToUtc, emails);
		logger.LogInformation("DeleteAlreadySentEmailsAsync(createdFromUtc,createdToUtc)={CreatedFromUtc},{CreatedToUtc}: database query executed in {Arg2} ms", createdFromUtc, createdToUtc, __logStart.ElapsedMilliseconds);
        await DeleteQueuedEmailsAsync(emails);

		logger.LogDebug("<<< Exiting DeleteAlreadySentEmailsAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc})", createdFromUtc, createdToUtc);
        return emails.Length;
    }

    /// <summary>
    /// Delete all queued emails
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAllEmailsAsync()
    {
		logger.LogDebug(">>> Entering DeleteAllEmailsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailRepository.TruncateAsync();
		logger.LogDebug("<<< Exiting DeleteAllEmailsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAllEmailsAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteAllEmailsAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    #endregion
}