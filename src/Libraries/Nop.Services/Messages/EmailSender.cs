using MimeKit;
using MimeKit.Text;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Email sender
/// </summary>
public partial class EmailSender : IEmailSender
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDownloadService _downloadService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly ISmtpBuilder _smtpBuilder;

    #endregion

    #region Ctor

    public EmailSender(IDownloadService downloadService, INopFileProvider fileProvider, ISmtpBuilder smtpBuilder)
    {
		_logger.LogDebug(">>> Entering EmailSender(downloadService={DownloadService},fileProvider={FileProvider},smtpBuilder={SmtpBuilder})", downloadService, fileProvider, smtpBuilder);
        _downloadService = downloadService;
		_logger.LogDebug("EmailSender(downloadService={DownloadService},fileProvider={FileProvider},smtpBuilder={SmtpBuilder}): _downloadService → {_downloadService}", downloadService, fileProvider, smtpBuilder, _downloadService);
        _fileProvider = fileProvider;
		_logger.LogDebug("EmailSender(downloadService={DownloadService},fileProvider={FileProvider},smtpBuilder={SmtpBuilder}): _fileProvider → {_fileProvider}", downloadService, fileProvider, smtpBuilder, _fileProvider);
        _smtpBuilder = smtpBuilder;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create an file attachment for the specific download object from DB
    /// </summary>
    /// <param name="download">Attachment download (another attachment)</param>
    /// <returns>A leaf-node MIME part that contains an attachment.</returns>
    protected MimePart CreateMimeAttachment(Download download)
    {
		_logger.LogDebug(">>> Entering CreateMimeAttachment(download={Download})", download);
        ArgumentNullException.ThrowIfNull(download);

        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
		_logger.LogDebug("CreateMimeAttachment(download={Download}): fileName → {FileName}", download, fileName);

		_logger.LogDebug("<<< Exiting CreateMimeAttachment(download={Download})", download);
		_logger.LogDebug("<<< Exiting CreateMimeAttachment(attachmentFileName={AttachmentFileName},binaryContent={BinaryContent},cDate={CDate},mDate={MDate},rDate={RDate})", attachmentFileName, binaryContent, cDate, mDate, rDate);
        return CreateMimeAttachment($"{fileName}{download.Extension}", download.DownloadBinary, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
    }

    /// <summary>
    /// Create an file attachment for the specific file path
    /// </summary>
    /// <param name="filePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a leaf-node MIME part that contains an attachment.
    /// </returns>
    protected virtual async Task<MimePart> CreateMimeAttachmentAsync(string filePath, string attachmentFileName = null)
    {
		_logger.LogDebug(">>> Entering CreateMimeAttachmentAsync(filePath={FilePath},attachmentFileName={AttachmentFileName})", filePath, attachmentFileName);
		_logger.LogDebug("CreateMimeAttachmentAsync(filePath={FilePath},attachmentFileName={AttachmentFileName}): attachmentFileName → {AttachmentFileName1}", filePath, attachmentFileName, attachmentFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (string.IsNullOrWhiteSpace(attachmentFileName))
            attachmentFileName = Path.GetFileName(filePath);

        return CreateMimeAttachment(
            attachmentFileName,
            await _fileProvider.ReadAllBytesAsync(filePath),
            _fileProvider.GetCreationTime(filePath),
            _fileProvider.GetLastWriteTime(filePath),
		_logger.LogDebug("<<< Exiting CreateMimeAttachmentAsync(filePath={FilePath},attachmentFileName={AttachmentFileName})", filePath, attachmentFileName);
            _fileProvider.GetLastAccessTime(filePath));
    }

    /// <summary>
    /// Create an file attachment for the binary data
    /// </summary>
    /// <param name="attachmentFileName">Attachment file name</param>
    /// <param name="binaryContent">The array of unsigned bytes from which to create the attachment stream.</param>
    /// <param name="cDate">Creation date and time for the specified file or directory</param>
    /// <param name="mDate">Date and time that the specified file or directory was last written to</param>
    /// <param name="rDate">Date and time that the specified file or directory was last access to.</param>
    /// <returns>A leaf-node MIME part that contains an attachment.</returns>
    protected MimePart CreateMimeAttachment(string attachmentFileName, byte[] binaryContent, DateTime cDate, DateTime mDate, DateTime rDate)
    {
        if (!ContentType.TryParse(MimeTypes.GetMimeType(attachmentFileName), out var mimeContentType))
            mimeContentType = new ContentType("application", "octet-stream");

        return new MimePart(mimeContentType)
        {
            FileName = attachmentFileName,
            Content = new MimeContent(new MemoryStream(binaryContent)),
            ContentDisposition = new ContentDisposition
            {
                CreationDate = cDate,
                ModificationDate = mDate,
                ReadDate = rDate
            },
            ContentTransferEncoding = ContentEncoding.Base64
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends an email
    /// </summary>
    /// <param name="emailAccount">Email account to use</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="fromAddress">From address</param>
    /// <param name="fromName">From display name</param>
    /// <param name="toAddress">To address</param>
    /// <param name="toName">To display name</param>
    /// <param name="replyTo">ReplyTo address</param>
    /// <param name="replyToName">ReplyTo display name</param>
    /// <param name="bcc">BCC addresses list</param>
    /// <param name="cc">CC addresses list</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <param name="attachedDownloadId">Attachment download ID (another attachment)</param>
    /// <param name="headers">Headers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SendEmailAsync(EmailAccount emailAccount, string subject, string body,
        string fromAddress, string fromName, string toAddress, string toName,
        string replyTo = null, string replyToName = null,
        IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
        string attachmentFilePath = null, string attachmentFileName = null,
        int attachedDownloadId = 0, IDictionary<string, string> headers = null)
    {
		_logger.LogDebug(">>> Entering SendEmailAsync(emailAccount={EmailAccount},subject={Subject},body={Body},fromAddress={FromAddress},fromName={FromName},toAddress={ToAddress},toName={ToName},replyTo={ReplyTo},replyToName={ReplyToName},bcc={Bcc},cc={Cc},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},attachedDownloadId={AttachedDownloadId},headers={Headers})", emailAccount, subject, body, fromAddress, fromName, toAddress, toName, replyTo, replyToName, bcc, cc, attachmentFilePath, attachmentFileName, attachedDownloadId, headers);
        var message = new MimeMessage();
		_logger.LogDebug("SendEmailAsync(emailAccount={EmailAccount},subject={Subject},body={Body},fromAddress={FromAddress},fromName={FromName},toAddress={ToAddress},toName={ToName},replyTo={ReplyTo},replyToName={ReplyToName},bcc={Bcc},cc={Cc},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},attachedDownloadId={AttachedDownloadId},headers={Headers}): message → {Message}", emailAccount, subject, body, fromAddress, fromName, toAddress, toName, replyTo, replyToName, bcc, cc, attachmentFilePath, attachmentFileName, attachedDownloadId, headers, message);

        message.From.Add(new MailboxAddress(fromName, fromAddress));
        message.To.Add(new MailboxAddress(toName, toAddress));

        if (!string.IsNullOrEmpty(replyTo))
        {
            message.ReplyTo.Add(new MailboxAddress(replyToName, replyTo));
        }

        //BCC
        if (bcc != null)
        {
            foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
            {
                message.Bcc.Add(new MailboxAddress("", address.Trim()));
            }
        }

        //CC
        if (cc != null)
        {
            foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
            {
                message.Cc.Add(new MailboxAddress("", address.Trim()));
            }
        }

        //content
        message.Subject = subject;
		_logger.LogDebug("SendEmailAsync(emailAccount={EmailAccount},subject={Subject},body={Body},fromAddress={FromAddress},fromName={FromName},toAddress={ToAddress},toName={ToName},replyTo={ReplyTo},replyToName={ReplyToName},bcc={Bcc},cc={Cc},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},attachedDownloadId={AttachedDownloadId},headers={Headers}): message.Subject → {Arg15}", emailAccount, subject, body, fromAddress, fromName, toAddress, toName, replyTo, replyToName, bcc, cc, attachmentFilePath, attachmentFileName, attachedDownloadId, headers, message.Subject);

        //headers
        if (headers != null)
            foreach (var header in headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

        var multipart = new Multipart("mixed")
        {
		_logger.LogDebug("<<< Exiting SendEmailAsync(emailAccount={EmailAccount},subject={Subject},body={Body},fromAddress={FromAddress},fromName={FromName},toAddress={ToAddress},toName={ToName},replyTo={ReplyTo},replyToName={ReplyToName},bcc={Bcc},cc={Cc},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},attachedDownloadId={AttachedDownloadId},headers={Headers})", emailAccount, subject, body, fromAddress, fromName, toAddress, toName, replyTo, replyToName, bcc, cc, attachmentFilePath, attachmentFileName, attachedDownloadId, headers);
            new TextPart(TextFormat.Html) { Text = body }
        };

        //create the file attachment for this email message
        if (!string.IsNullOrEmpty(attachmentFilePath) && _fileProvider.FileExists(attachmentFilePath))
        {
            multipart.Add(await CreateMimeAttachmentAsync(attachmentFilePath, attachmentFileName));
        }

        //another attachment?
        if (attachedDownloadId > 0)
        {
            var download = await _downloadService.GetDownloadByIdAsync(attachedDownloadId);
            //we do not support URLs as attachments
            if (!download?.UseDownloadUrl ?? false)
            {
                multipart.Add(CreateMimeAttachment(download));
            }
        }

        message.Body = multipart;

        //send email
        using var smtpClient = await _smtpBuilder.BuildAsync(emailAccount);
        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
    }

    #endregion
}