using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class NewsletterModelFactoryTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private INewsLetterModelFactory _newsletterModelFactory;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _newsletterModelFactory = GetService<INewsLetterModelFactory>();
		_logger.LogDebug("SetUp(): _newsletterModelFactory → {_newsletterModelFactory}", _newsletterModelFactory);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanPrepareNewsletterBoxModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareNewsletterBoxModel()");
        var model = await _newsletterModelFactory.PrepareNewsLetterBoxModelAsync();

		_logger.LogDebug("CanPrepareNewsletterBoxModel(): model → {Model}", model);
        model.AllowToUnsubscribe.Should().Be(GetService<CustomerSettings>().NewsletterBlockAllowToUnsubscribe);
		_logger.LogDebug("<<< Exiting CanPrepareNewsletterBoxModel()");
    }

    [Test]
    public async Task CanPrepareSubscriptionActivationModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareSubscriptionActivationModel()");
        var activated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(true)).Result;

		_logger.LogDebug("CanPrepareSubscriptionActivationModel(): activated → {Activated}", activated);
        var deactivated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(false)).Result;
		_logger.LogDebug("CanPrepareSubscriptionActivationModel(): deactivated → {Deactivated}", deactivated);

        activated.Should().NotBe(deactivated);
		_logger.LogDebug("<<< Exiting CanPrepareSubscriptionActivationModel()");
    }
}