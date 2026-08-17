using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Tests;

public class TestSmtpBuilder : SmtpBuilder
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TestSmtpBuilder(EmailAccountSettings emailAccountSettings, 
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider) : base(emailAccountSettings, emailAccountService, localizationService, fileProvider)
    {
	_logger.LogDebug(">>> Entering TestSmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider})", emailAccountSettings, emailAccountService, localizationService, fileProvider);
	_logger.LogDebug("<<< Exiting TestSmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider})", emailAccountSettings, emailAccountService, localizationService, fileProvider);
    }

    public override Task<SmtpClient> BuildAsync(EmailAccount emailAccount = null)
    {
		_logger.LogDebug(">>> Entering BuildAsync(emailAccount={EmailAccount})", emailAccount);
		_logger.LogDebug("<<< Exiting BuildAsync(emailAccount={EmailAccount})", emailAccount);
        return Task.FromResult<SmtpClient>(new TestSmtpClient());
    }

    public class TestSmtpClient : SmtpClient
    {
        public override Task<string> SendAsync(MimeMessage message,
            CancellationToken cancellationToken = default,
            ITransferProgress progress = null)
        {
            MessageIsSent = true;
			_logger.LogDebug("<<< Exiting SendAsync(message={Message},cancellationToken=<redacted>,progress={Progress})", message, progress);
			_logger.LogDebug("SendAsync(message={Message},cancellationToken=<redacted>,progress={Progress}): MessageIsSent → {MessageIsSent}", message, progress, MessageIsSent);
            return Task.FromResult(string.Empty);
        }

        public static bool MessageIsSent { get; set; }
    }
}