using FluentAssertions;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class TopicModelFactoryTests : BaseNopTest
{
    private readonly ITopicModelFactory _topicModelFactory;
    private readonly Topic _testTopic;

    public TopicModelFactoryTests()
    {
		logger.LogDebug(">>> Entering TopicModelFactoryTests()");
        _topicModelFactory = GetService<ITopicModelFactory>();
        _testTopic = GetService<IRepository<Topic>>().GetById(1);
		logger.LogDebug("<<< Exiting TopicModelFactoryTests()");
		logger.LogDebug("TopicModelFactoryTests(): _topicModelFactory → {_topicModelFactory}", _topicModelFactory);
		logger.LogDebug("TopicModelFactoryTests(): _testTopic → {_testTopic}", _testTopic);
    }

    [Test]
    public async Task CanPrepareTopicModel()
    {
		logger.LogDebug(">>> Entering CanPrepareTopicModel()");
        var modelByTopic = await _topicModelFactory.PrepareTopicModelAsync(_testTopic);
        modelByTopic.Should().NotBeNull();
		logger.LogDebug("CanPrepareTopicModel(): modelByTopic → {ModelByTopic}", modelByTopic);
        var modelBySystemName = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(_testTopic.SystemName);
        modelBySystemName.Should().NotBeNull();
		logger.LogDebug("CanPrepareTopicModel(): modelBySystemName → {ModelBySystemName}", modelBySystemName);
        modelByTopic.SeName.Should().NotBeNullOrEmpty();
        PropertiesShouldEqual(modelByTopic, modelBySystemName, "CustomProperties");
		logger.LogDebug("<<< Exiting CanPrepareTopicModel()");
    }

    [Test]
    public async Task CanPrepareTemplateViewPath()
    {
		logger.LogDebug(">>> Entering CanPrepareTemplateViewPath()");
        var model1 = await _topicModelFactory.PrepareTemplateViewPathAsync(1);
        var model2 = await _topicModelFactory.PrepareTemplateViewPathAsync(int.MaxValue);
		logger.LogDebug("CanPrepareTemplateViewPath(): model1 → {Model1}", model1);
		logger.LogDebug("CanPrepareTemplateViewPath(): model2 → {Model2}", model2);

        model1.Should().Be(model2);
		logger.LogDebug("<<< Exiting CanPrepareTemplateViewPath()");
    }
}
