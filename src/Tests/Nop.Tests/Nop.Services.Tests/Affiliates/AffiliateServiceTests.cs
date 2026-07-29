using FluentAssertions;
using Nop.Core.Domain.Affiliates;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Affiliates;

[TestFixture]
public class AffiliateServiceTests : ServiceTest<Affiliate>
{
    private IAffiliateService _affiliateService;
    private IAddressService _addressService;
    private Affiliate _activeAffiliate1;
    private Affiliate _activeAffiliate2;
    private Affiliate _notActiveAffiliate;
    private Affiliate _activeDeletedAffiliate;
    private Affiliate _notActiveDeletedAffiliate;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _affiliateService = GetService<IAffiliateService>();
        _addressService = GetService<IAddressService>();
		logger.LogDebug("SetUp(): _affiliateService → {_affiliateService}", _affiliateService);
		logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
        _activeAffiliate1 = new Affiliate
        {
            Active = true,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestActiveAffiliate1"
			logger.LogDebug("SetUp(): _activeAffiliate1 → {_activeAffiliate1}", _activeAffiliate1);
        };
        _activeAffiliate2 = new Affiliate
		logger.LogDebug("SetUp(): _activeAffiliate2 → {_activeAffiliate2}", _activeAffiliate2);
        {
            Active = true,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestActiveAffiliate2"
        };
        _notActiveAffiliate = new Affiliate
		logger.LogDebug("SetUp(): _notActiveAffiliate → {_notActiveAffiliate}", _notActiveAffiliate);
        {
            Active = false,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestNotActiveAffiliate"
        };
        _activeDeletedAffiliate = new Affiliate
		logger.LogDebug("SetUp(): _activeDeletedAffiliate → {_activeDeletedAffiliate}", _activeDeletedAffiliate);
        {
            Active = true,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestActiveDeletedAffiliate",
            Deleted = true
        };
        _notActiveDeletedAffiliate = new Affiliate
		logger.LogDebug("SetUp(): _notActiveDeletedAffiliate → {_notActiveDeletedAffiliate}", _notActiveDeletedAffiliate);
        {
            Active = false,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestNotActiveDeletedAffiliate",
            Deleted = true
        };

        await _affiliateService.InsertAffiliateAsync(_activeAffiliate1);
        await _affiliateService.InsertAffiliateAsync(_notActiveAffiliate);
        await _affiliateService.InsertAffiliateAsync(_activeDeletedAffiliate);
        await _affiliateService.InsertAffiliateAsync(_notActiveDeletedAffiliate);
		logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _affiliateService.DeleteAffiliateAsync(_activeAffiliate1);
        await _affiliateService.DeleteAffiliateAsync(_notActiveAffiliate);
        await _affiliateService.DeleteAffiliateAsync(_activeDeletedAffiliate);
        await _affiliateService.DeleteAffiliateAsync(_notActiveDeletedAffiliate);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanGetAffiliateByFriendlyUrlName()
    {
		logger.LogDebug(">>> Entering CanGetAffiliateByFriendlyUrlName()");
        var affiliate = await _affiliateService.GetAffiliateByFriendlyUrlNameAsync(_activeAffiliate1.FriendlyUrlName);

		logger.LogDebug("CanGetAffiliateByFriendlyUrlName(): affiliate → {Affiliate}", affiliate);
        affiliate.Should().NotBeNull();
        affiliate.Active.Should().BeTrue();
        affiliate.FriendlyUrlName.Should().Be(_activeAffiliate1.FriendlyUrlName);
		logger.LogDebug("<<< Exiting CanGetAffiliateByFriendlyUrlName()");
    }

    [Test]
    public async Task CanGetAllAffiliates()
    {
		logger.LogDebug(">>> Entering CanGetAllAffiliates()");
        var affiliates = await _affiliateService.GetAllAffiliatesAsync();
        affiliates.TotalCount.Should().Be(2);
		logger.LogDebug("CanGetAllAffiliates(): affiliates → {Affiliates}", affiliates);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(showHidden: true);
        affiliates.TotalCount.Should().Be(3);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(_activeDeletedAffiliate.FriendlyUrlName);
        affiliates.TotalCount.Should().Be(0);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(_activeAffiliate1.FriendlyUrlName, showHidden: true);
        affiliates.TotalCount.Should().Be(1);

        var address = await _addressService.GetAddressByIdAsync(1);

		logger.LogDebug("CanGetAllAffiliates(): address → {Address}", address);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(firstName: address.FirstName, showHidden: true);
        affiliates.TotalCount.Should().Be(3);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(firstName: address.FirstName);
        affiliates.TotalCount.Should().Be(2);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(loadOnlyWithOrders: true, showHidden: true);
        affiliates.TotalCount.Should().Be(0);
		logger.LogDebug("<<< Exiting CanGetAllAffiliates()");
    }

    [Test]
    public async Task CanGetAffiliateFullName()
    {
		logger.LogDebug(">>> Entering CanGetAffiliateFullName()");
        var fullName = await _affiliateService.GetAffiliateFullNameAsync(_activeAffiliate1);
        var address = await _addressService.GetAddressByIdAsync(1);
		logger.LogDebug("CanGetAffiliateFullName(): fullName → {FullName}", fullName);
		logger.LogDebug("CanGetAffiliateFullName(): address → {Address}", address);

        fullName.Should().Be($"{address.FirstName} {address.LastName}");
		logger.LogDebug("<<< Exiting CanGetAffiliateFullName()");
    }

    [Test]
    public async Task CanGenerateUrl()
    {
		logger.LogDebug(">>> Entering CanGenerateUrl()");
        var url = await _affiliateService.GenerateUrlAsync(_activeAffiliate1);
        url.Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?{NopAffiliateDefaults.AffiliateQueryParameter}={_activeAffiliate1.FriendlyUrlName}");
		logger.LogDebug("CanGenerateUrl(): url → {Url}", url);

        _notActiveDeletedAffiliate.FriendlyUrlName = string.Empty;
		logger.LogDebug("CanGenerateUrl(): _notActiveDeletedAffiliate.FriendlyUrlName → {Arg0}", _notActiveDeletedAffiliate.FriendlyUrlName);
        url = await _affiliateService.GenerateUrlAsync(_notActiveDeletedAffiliate);
        url.Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?{NopAffiliateDefaults.AffiliateIdQueryParameter}={_notActiveDeletedAffiliate.Id}");
		logger.LogDebug("<<< Exiting CanGenerateUrl()");
    }

    [Test]
    public async Task CanValidateFriendlyUrlName()
    {
		logger.LogDebug(">>> Entering CanValidateFriendlyUrlName()");
        var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(_activeAffiliate1, _activeAffiliate2.FriendlyUrlName);
        friendlyUrlName.Should().Be(_activeAffiliate2.FriendlyUrlName.ToLowerInvariant());
		logger.LogDebug("CanValidateFriendlyUrlName(): friendlyUrlName → {FriendlyUrlName}", friendlyUrlName);

        friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(_activeAffiliate1, "not/valid/url*name");
        friendlyUrlName.Should().Be("notvalidurlname");
		logger.LogDebug("<<< Exiting CanValidateFriendlyUrlName()");
    }

    protected override CrudData<Affiliate> CrudData
    {
        get
        {
            var baseEntity = new Affiliate
            {
                Active = true,
                AddressId = 1,
                AdminComment = "Test admin comment",
                FriendlyUrlName = "TestActiveAffiliate"
            };

            var updatedEntity = new Affiliate
            {
                Active = true,
                AddressId = 1,
                AdminComment = "Test comment",
                FriendlyUrlName = "TestActiveAffiliate"
            };

            return new CrudData<Affiliate>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _affiliateService.InsertAffiliateAsync,
                Update = _affiliateService.UpdateAffiliateAsync,
                Delete = _affiliateService.DeleteAffiliateAsync,
                GetById = _affiliateService.GetAffiliateByIdAsync,
                IsEqual = (first, second) => first.Active == second.Active && first.AddressId == second.AddressId && first.AdminComment.Equals(second.AdminComment) && first.FriendlyUrlName.Equals(second.FriendlyUrlName) && first.Deleted == second.Deleted
            };
        }
    }

}