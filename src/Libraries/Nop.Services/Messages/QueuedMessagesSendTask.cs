using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Messages;

/// <summary>
/// Represents a task for sending queued message 
/// </summary>
public partial class QueuedMessagesSendTask : IScheduleTask
{
    #region Fields

    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IEmailSender _emailSender;
    protected readonly ILogger _logger;
    protected readonly IQueuedEmailService _queuedEmailService;
    private static readonly char[] _separator = [';'];

    #endregion

    #region Ctor

    public QueuedMessagesSendTask(IEmailAccountService emailAccountService,
        IEmailSender emailSender,
        ILogger logger,
        IQueuedEmailService queuedEmailService)
    {
        _emailAccountService = emailAccountService;
		_logger.LogDebug("QueuedMessagesSendTask(emailAccountService={EmailAccountService},emailSender={EmailSender},logger={Logger},queuedEmailService={QueuedEmailService}): _emailAccountService → {_emailAccountService}", emailAccountService, emailSender, logger, queuedEmailService, _emailAccountService);
        _emailSender = emailSender;
		_logger.LogDebug("QueuedMessagesSendTask(emailAccountService={EmailAccountService},emailSender={EmailSender},logger={Logger},queuedEmailService={QueuedEmailService}): _emailSender → {_emailSender}", emailAccountService, emailSender, logger, queuedEmailService, _emailSender);
        _logger = logger;
		_logger.LogDebug(">>> Entering QueuedMessagesSendTask(emailAccountService={EmailAccountService},emailSender={EmailSender},logger={Logger},queuedEmailService={QueuedEmailService})", emailAccountService, emailSender, logger, queuedEmailService);
		_logger.LogDebug("QueuedMessagesSendTask(emailAccountService={EmailAccountService},emailSender={EmailSender},logger={Logger},queuedEmailService={QueuedEmailService}): _logger → {_logger}", emailAccountService, emailSender, logger, queuedEmailService, _logger);
        _queuedEmailService = queuedEmailService;
		_logger.LogDebug("<<< Exiting QueuedMessagesSendTask(emailAccountService={EmailAccountService},emailSender={EmailSender},logger={Logger},queuedEmailService={QueuedEmailService})", emailAccountService, emailSender, logger, queuedEmailService);
		_logger.LogDebug("QueuedMessagesSendTask(emailAccountService={EmailAccountService},emailSender={EmailSender},logger={Logger},queuedEmailService={QueuedEmailService}): _queuedEmailService → {_queuedEmailService}", emailAccountService, emailSender, logger, queuedEmailService, _queuedEmailService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogDebug(">>> Entering ExecuteAsync()");
        var maxTries = 3;
        var queuedEmails = (await _queuedEmailService.SearchEmailsAsync(null, null, null, null,
            true, true, maxTries, false, 0, int.MaxValue)).GroupBy(qe => qe.EmailAccountId);

        foreach (var item in queuedEmails)
        {
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(item.Key);
			_logger.LogDebug("ExecuteAsync(): emailAccount → {EmailAccount}", emailAccount);

            foreach (var queuedEmail in item.Take(emailAccount.MaxNumberOfEmails))
            {
                var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                    ? null
                    : queuedEmail.Bcc.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                var cc = string.IsNullOrWhiteSpace(queuedEmail.CC)
                    ? null
                    : queuedEmail.CC.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    await _emailSender.SendEmailAsync(emailAccount,
                        queuedEmail.Subject,
                        queuedEmail.Body,
                        queuedEmail.From,
                        queuedEmail.FromName,
                        queuedEmail.To,
                        queuedEmail.ToName,
                        queuedEmail.ReplyTo,
                        queuedEmail.ReplyToName,
                        bcc,
                        cc,
                        queuedEmail.AttachmentFilePath,
                        queuedEmail.AttachmentFileName,
                        queuedEmail.AttachedDownloadId);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
					_logger.LogDebug("ExecuteAsync(): queuedEmail.SentOnUtc → {Arg0}", queuedEmail.SentOnUtc);
                }
                catch (Exception exc)
                {
                    await _logger.ErrorAsync($"Error sending email. {exc.Message}", exc);
                }
                finally
                {
                    queuedEmail.SentTries += 1;
                    await _queuedEmailService.UpdateQueuedEmailAsync(queuedEmail);
                }
            }
        }
		_logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}