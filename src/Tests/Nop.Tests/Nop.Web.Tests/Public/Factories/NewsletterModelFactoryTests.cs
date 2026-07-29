using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class NewsletterModelFactoryTests : BaseNopTest
{
    private INewsLetterModelFactory _newsletterModelFactory;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _newsletterModelFactory = GetService<INewsLetterModelFactory>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _newsletterModelFactory → {_newsletterModelFactory}", _newsletterModelFactory);
    }

    [Test]
    public async Task CanPrepareNewsletterBoxModel()
    {
		logger.LogDebug(">>> Entering CanPrepareNewsletterBoxModel()");
        var model = await _newsletterModelFactory.PrepareNewsLetterBoxModelAsync();

		logger.LogDebug("CanPrepareNewsletterBoxModel(): model → {Model}", model);
        model.AllowToUnsubscribe.Should().Be(GetService<CustomerSettings>().NewsletterBlockAllowToUnsubscribe);
		logger.LogDebug("<<< Exiting CanPrepareNewsletterBoxModel()");
    }

    [Test]
    public async Task CanPrepareSubscriptionActivationModel()
    {
		logger.LogDebug(">>> Entering CanPrepareSubscriptionActivationModel()");
        var activated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(true)).Result;

		logger.LogDebug("CanPrepareSubscriptionActivationModel(): activated → {Activated}", activated);
        var deactivated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(false)).Result;

		logger.LogDebug("CanPrepareSubscriptionActivationModel(): deactivated → {Deactivated}", deactivated);
        activated.Should().NotBe(deactivated);
		logger.LogDebug("<<< Exiting CanPrepareSubscriptionActivationModel()");
    }
}