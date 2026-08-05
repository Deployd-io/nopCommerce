using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class EmailSenderTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IEmailSender _emailSender;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _emailSender = GetService<IEmailSender>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _emailSender → {_emailSender}", _emailSender);
    }

    [Test]
    public async Task CanSendEmail()
    {
		_logger.LogDebug(">>> Entering CanSendEmail()");
        TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;

        var emailAccount = new EmailAccount
        {
            Id = 1,
            Email = NopTestsDefaults.AdminEmail,
            DisplayName = "Test name",
            Host = "smtp.test.com",
            Port = 25,
            Username = "test_user",
            Password = "test_password",
            EnableSsl = false
        };

        var subject = "Test subject";
        var body = "Test body";
		_logger.LogDebug("CanSendEmail(): subject → {Subject}", subject);
		_logger.LogDebug("CanSendEmail(): body → {Body}", body);
        var fromAddress = NopTestsDefaults.AdminEmail;
        var fromName = "From name";
        var toAddress = "test@test.com";
		_logger.LogDebug("CanSendEmail(): fromName → {FromName}", fromName);
		_logger.LogDebug("CanSendEmail(): toAddress → {ToAddress}", toAddress);
        var toName = "To name";
        var replyToAddress = NopTestsDefaults.AdminEmail;
		_logger.LogDebug("CanSendEmail(): toName → {ToName}", toName);
        var replyToName = "Reply to name";
        var bcc = new[] { NopTestsDefaults.AdminEmail };
		_logger.LogDebug("CanSendEmail(): replyToName → {ReplyToName}", replyToName);
        var cc = new[] { NopTestsDefaults.AdminEmail };

        await _emailSender.SendEmailAsync(emailAccount, subject, body,
            fromAddress, fromName, toAddress, toName,
            replyToAddress, replyToName, bcc, cc);

        TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanSendEmail()");
    }
}