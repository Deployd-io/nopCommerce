using FluentAssertions;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Web.Factories;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class TopicModelFactoryTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private readonly ITopicModelFactory _topicModelFactory;
    private readonly Topic _testTopic;

    public TopicModelFactoryTests()
    {
		_logger.LogDebug(">>> Entering TopicModelFactoryTests()");
        _topicModelFactory = GetService<ITopicModelFactory>();
        _testTopic = GetService<IRepository<Topic>>().GetById(1);
		_logger.LogDebug("TopicModelFactoryTests(): _testTopic → {_testTopic}", _testTopic);
		_logger.LogDebug("TopicModelFactoryTests(): _topicModelFactory → {_topicModelFactory}", _topicModelFactory);
		_logger.LogDebug("<<< Exiting TopicModelFactoryTests()");
    }

    [Test]
    public async Task CanPrepareTopicModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareTopicModel()");
        var modelByTopic = await _topicModelFactory.PrepareTopicModelAsync(_testTopic);
        modelByTopic.Should().NotBeNull();
		_logger.LogDebug("CanPrepareTopicModel(): modelByTopic → {ModelByTopic}", modelByTopic);
        var modelBySystemName = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(_testTopic.SystemName);
		_logger.LogDebug("CanPrepareTopicModel(): modelBySystemName → {ModelBySystemName}", modelBySystemName);
        modelBySystemName.Should().NotBeNull();
        modelByTopic.SeName.Should().NotBeNullOrEmpty();
        PropertiesShouldEqual(modelByTopic, modelBySystemName, "CustomProperties");
		_logger.LogDebug("<<< Exiting CanPrepareTopicModel()");
    }

    [Test]
    public async Task CanPrepareTemplateViewPath()
    {
		_logger.LogDebug(">>> Entering CanPrepareTemplateViewPath()");
        var model1 = await _topicModelFactory.PrepareTemplateViewPathAsync(1);
        var model2 = await _topicModelFactory.PrepareTemplateViewPathAsync(int.MaxValue);
		_logger.LogDebug("CanPrepareTemplateViewPath(): model2 → {Model2}", model2);
		_logger.LogDebug("CanPrepareTemplateViewPath(): model1 → {Model1}", model1);

        model1.Should().Be(model2);
		_logger.LogDebug("<<< Exiting CanPrepareTemplateViewPath()");
    }
}
