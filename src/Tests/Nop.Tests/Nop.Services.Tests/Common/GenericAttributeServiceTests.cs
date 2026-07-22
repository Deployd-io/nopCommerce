using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class GenericAttributeServiceTests : ServiceTest
{
    private IGenericAttributeService _genericAttributeService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _genericAttributeService = GetService<IGenericAttributeService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _genericAttributeService → {_genericAttributeService}", _genericAttributeService);
    }

    [Test]
    public async Task ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()
    {
		logger.LogDebug(">>> Entering ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()");
        var attribute = new global::Nop.Core.Domain.Common.GenericAttribute
        {
            Key = "test",
            KeyGroup = "test",
            Value = "test",
            CreatedOrUpdatedDateUTC = null
			logger.LogWarning("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): Key → {Key}", Key);
			logger.LogDebug("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): KeyGroup → {KeyGroup}", KeyGroup);
			logger.LogDebug("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): Value → {Value}", Value);
			logger.LogDebug("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): CreatedOrUpdatedDateUTC → {CreatedOrUpdatedDateUTC}", CreatedOrUpdatedDateUTC);
        };

        await _genericAttributeService.InsertAttributeAsync(attribute);

        var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

        await _genericAttributeService.DeleteAttributeAsync(attribute);

        Assert.That(createdOrUpdatedDate,
            Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
			logger.LogDebug("<<< Exiting ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()");
    }

    [Test]
    public async Task ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()
    {
		logger.LogDebug(">>> Entering ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()");
        var attribute = new global::Nop.Core.Domain.Common.GenericAttribute { Key = "test", KeyGroup = "test", Value = "test" };

		logger.LogDebug("ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute(): attribute → {Attribute}", attribute);
        await _genericAttributeService.InsertAttributeAsync(attribute);
        attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow.AddDays(-30);
        await _genericAttributeService.UpdateAttributeAsync(attribute);

        var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

        await _genericAttributeService.DeleteAttributeAsync(attribute);

        Assert.That(createdOrUpdatedDate,
            Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
			logger.LogDebug("<<< Exiting ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()");
    }
}
