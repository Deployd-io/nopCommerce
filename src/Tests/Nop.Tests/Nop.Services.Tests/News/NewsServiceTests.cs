using FluentAssertions;
using Nop.Core.Domain.News;
using Nop.Services.News;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.News;

[TestFixture]
public class NewsServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private INewsService _newsService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _newsService = GetService<INewsService>();
		_logger.LogDebug("SetUp(): _newsService → {_newsService}", _newsService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public void ShouldBeAvailableWhenStartDateIsNotSet()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenStartDateIsNotSet()");
        var newsItem = new NewsItem
        {
            StartDateUtc = null
        };

        _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenStartDateIsNotSet()");
    }

    [Test]
    public void ShouldBeAvailableWhenStartDateIsLessThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenStartDateIsLessThanSomeDate()");
        var newsItem = new NewsItem
        {
            StartDateUtc = new DateTime(2010, 01, 02)
        };

        _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenStartDateIsLessThanSomeDate()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()");
        var newsItem = new NewsItem
        {
            StartDateUtc = new DateTime(2010, 01, 02)
        };

        _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 01)).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()");
    }

    [Test]
    public void ShouldBeAvailableWhenEndDateIsNotSet()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenEndDateIsNotSet()");
        var newsItem = new NewsItem
        {
            EndDateUtc = null
        };

        _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenEndDateIsNotSet()");
    }

    [Test]
    public void ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()");
        var newsItem = new NewsItem
        {
            EndDateUtc = new DateTime(2010, 01, 02)
        };

        _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 01)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()");
        var newsItem = new NewsItem
        {
            EndDateUtc = new DateTime(2010, 01, 02)
        };

        _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()");
    }
}