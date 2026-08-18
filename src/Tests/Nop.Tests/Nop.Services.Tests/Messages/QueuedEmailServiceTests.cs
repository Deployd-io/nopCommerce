using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class QueuedEmailServiceTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IQueuedEmailService _queuedEmailService;
    private IRepository<QueuedEmail> _queuedEmailRepository;
    private List<QueuedEmail> _emails;
    private string _testEmail;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
		_logger.LogDebug(">>> Entering OneTimeSetUp()");
        _queuedEmailService = GetService<IQueuedEmailService>();
        _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();
		_logger.LogDebug("OneTimeSetUp(): _queuedEmailRepository → {_queuedEmailRepository}", _queuedEmailRepository);

        await _queuedEmailRepository.TruncateAsync();
        _testEmail = "test@test.com";
		_logger.LogDebug("OneTimeSetUp(): _testEmail → {_testEmail}", _testEmail);
		_logger.LogDebug("OneTimeSetUp(): _queuedEmailService → {_queuedEmailService}", _queuedEmailService);
		_logger.LogDebug("<<< Exiting OneTimeSetUp()");
    }

    [TearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _queuedEmailRepository.TruncateAsync();
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [SetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _emails =
        [
            new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentTries = 5},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentOnUtc = DateTime.UtcNow},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentOnUtc = DateTime.UtcNow}
		_logger.LogDebug("<<< Exiting SetUp()");
            ];

        foreach (var queuedEmail in _emails)
            await _queuedEmailService.InsertQueuedEmailAsync(queuedEmail);
    }

    [Test]
    public async Task CanCRUD()
    {
		_logger.LogDebug(">>> Entering CanCRUD()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count);

		_logger.LogDebug("CanCRUD(): queuedEmails → {QueuedEmails}", queuedEmails);
		_logger.LogInformation("CanCRUD(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        var email = await _queuedEmailService.GetQueuedEmailByIdAsync(_emails[0].Id);
		_logger.LogDebug("CanCRUD(): email → {Email}", email);
		_logger.LogDebug("CanCRUD(): email.Body → {Arg0}", email.Body);
        email.Body = "test";

        await _queuedEmailService.UpdateQueuedEmailAsync(email);
        (await _queuedEmailRepository.GetByIdAsync(_emails[0].Id)).Body.Should().Be(email.Body);

        await _queuedEmailService.DeleteQueuedEmailAsync(email);

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count - 1);

        await _queuedEmailService.DeleteQueuedEmailsAsync(_emails.Take(3).ToList());

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count - 3);

        await _queuedEmailService.DeleteAllEmailsAsync();

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanCRUD()");
    }


    [Test]
    public async Task CanGetQueuedEmailsByIds()
    {
		_logger.LogDebug(">>> Entering CanGetQueuedEmailsByIds()");
        var queuedEmails =
            await _queuedEmailService.GetQueuedEmailsByIdsAsync(_emails.Take(3).Select(e => e.Id).ToArray());
        queuedEmails.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanGetQueuedEmailsByIds()");
    }

    [Test]
    public async Task CanSearchEmails()
    {
		_logger.LogDebug(">>> Entering CanSearchEmails()");
        var loadNotSentItemsOnly = true;
        var loadOnlyItemsToBeSent = true;
        var maxSendTries = int.MaxValue;
        var loadNewest = false;

        async Task<int> getCountAsync()
        {
			_logger.LogDebug(">>> Entering getCountAsync()");
            var emails = await _queuedEmailService.SearchEmailsAsync(NopTestsDefaults.AdminEmail, _testEmail, null,
                null, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest);

			_logger.LogDebug("<<< Exiting getCountAsync()");
            return emails.Count;
        }

        (await getCountAsync()).Should().Be(3);
        loadNotSentItemsOnly = false;
        (await getCountAsync()).Should().Be(5);
        loadOnlyItemsToBeSent = false;
        (await getCountAsync()).Should().Be(5);
        loadNotSentItemsOnly = true;
        (await getCountAsync()).Should().Be(3);
        loadNotSentItemsOnly = false;
        maxSendTries = 1;
        (await getCountAsync()).Should().Be(4);
		_logger.LogDebug("<<< Exiting CanSearchEmails()");
    }

    [Test]
    public async Task CanDeleteAlreadySentEmails()
    {
		_logger.LogDebug(">>> Entering CanDeleteAlreadySentEmails()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _queuedEmailService.DeleteAlreadySentEmailsAsync(null, null);
        var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count(e => !e.SentOnUtc.HasValue));
		_logger.LogDebug("CanDeleteAlreadySentEmails(): queuedEmails → {QueuedEmails}", queuedEmails);
		_logger.LogInformation("CanDeleteAlreadySentEmails(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CanDeleteAlreadySentEmails()");
    }
}