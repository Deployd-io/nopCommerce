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
		_logger.LogDebug("CanSendEmail(): subject → {Subject}", subject);
        var body = "Test body";
		_logger.LogDebug("CanSendEmail(): body → {Body}", body);
        var fromAddress = NopTestsDefaults.AdminEmail;
        var fromName = "From name";
		_logger.LogDebug("CanSendEmail(): fromName → {FromName}", fromName);
        var toAddress = "test@test.com";
		_logger.LogDebug("CanSendEmail(): toAddress → {ToAddress}", toAddress);
        var toName = "To name";
		_logger.LogDebug("CanSendEmail(): toName → {ToName}", toName);
        var replyToAddress = NopTestsDefaults.AdminEmail;
        var replyToName = "Reply to name";
		_logger.LogDebug("CanSendEmail(): replyToName → {ReplyToName}", replyToName);
        var bcc = new[] { NopTestsDefaults.AdminEmail };
        var cc = new[] { NopTestsDefaults.AdminEmail };

        await _emailSender.SendEmailAsync(emailAccount, subject, body,
            fromAddress, fromName, toAddress, toName,
            replyToAddress, replyToName, bcc, cc);

        TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanSendEmail()");
    }
}