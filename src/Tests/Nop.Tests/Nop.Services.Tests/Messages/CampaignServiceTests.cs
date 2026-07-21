using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class CampaignServiceTests : ServiceTest<Campaign>
{
    private ICampaignService _campaignService;
    private IRepository<Campaign> _campaignRepository;
    private IRepository<QueuedEmail> _cueuedEmailRepository;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _campaignService = GetService<ICampaignService>();
        _campaignRepository = GetService<IRepository<Campaign>>();
		logger.LogDebug("SetUp(): _campaignService → {_campaignService}", _campaignService);
		logger.LogDebug("SetUp(): _campaignRepository → {_campaignRepository}", _campaignRepository);
        _cueuedEmailRepository = GetService<IRepository<QueuedEmail>>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _cueuedEmailRepository → {_cueuedEmailRepository}", _cueuedEmailRepository);
    }

    protected override CrudData<Campaign> CrudData
    {
        get
        {
            var insertItem = new Campaign
            {
                Name = "Test name",
                Subject = "Test subject",
                Body = "Test body"
            };

            var updateItem = new Campaign
            {
                Name = "Test name",
                Subject = "Test subject",
                Body = "Test body"
            };

            return new CrudData<Campaign>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _campaignService.InsertCampaignAsync,
                Update = _campaignService.UpdateCampaignAsync,
                GetById = _campaignService.GetCampaignByIdAsync,
                IsEqual = (item, other) => item.Subject.Equals(other.Subject) && item.Body.Equals(other.Body) && item.Name.Equals(other.Name),
                Delete = _campaignService.DeleteCampaignAsync
            };
        }
    }
    
    [Test]
    public async Task CanGetAllCampaigns()
    {
		logger.LogDebug(">>> Entering CanGetAllCampaigns()");
        var rez = await _campaignService.GetAllCampaignsAsync();
        rez.Count.Should().Be(0);
		logger.LogDebug("CanGetAllCampaigns(): rez → {Rez}", rez);
        await _campaignService.InsertCampaignAsync(new Campaign
        {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body"
        });
        await _campaignService.InsertCampaignAsync(new Campaign
        {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            StoreId = 2
        });
        await _campaignService.InsertCampaignAsync(new Campaign
        {
            Name = "Test name",
            Subject = "Test subject",
            Body = "Test body",
            StoreId = 1
        });

        rez = await _campaignService.GetAllCampaignsAsync();
        rez.Count.Should().Be(3);
        rez = await _campaignService.GetAllCampaignsAsync(1);
        rez.Count.Should().Be(1);
        rez = await _campaignService.GetAllCampaignsAsync(2);
        rez.Count.Should().Be(1);
		logger.LogDebug("<<< Exiting CanGetAllCampaigns()");
    }

    [Test]
    public async Task CanSendCampaign()
    {
		logger.LogDebug(">>> Entering CanSendCampaign()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _campaignRepository.TruncateAsync();
        var campaign = new Campaign { Name = "Test name", Subject = "Test subject", Body = "Test body" };
        await _campaignService.InsertCampaignAsync(campaign);
		logger.LogDebug("CanSendCampaign(): campaign → {Campaign}", campaign);
		logger.LogInformation("CanSendCampaign(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;
        await _cueuedEmailRepository.TruncateAsync();

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

        var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };

		logger.LogDebug("CanSendCampaign(): subscription → {Subscription}", subscription);
        await _campaignService.SendCampaignAsync(campaign, emailAccount, new[] { subscription });
        _cueuedEmailRepository.Table.Count().Should().Be(1);

        await _campaignService.SendCampaignAsync(campaign, emailAccount, NopTestsDefaults.AdminEmail, 1);
        TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanSendCampaign()");
    }
}
