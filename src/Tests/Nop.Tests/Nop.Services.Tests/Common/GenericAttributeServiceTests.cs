using Nop.Services.Common;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class GenericAttributeServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IGenericAttributeService _genericAttributeService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _genericAttributeService = GetService<IGenericAttributeService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _genericAttributeService → {_genericAttributeService}", _genericAttributeService);
    }

    [Test]
    public async Task ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()
    {
		_logger.LogDebug(">>> Entering ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()");
        var attribute = new global::Nop.Core.Domain.Common.GenericAttribute
        {
            Key = "test",
            KeyGroup = "test",
            Value = "test",
            CreatedOrUpdatedDateUTC = null
			_logger.LogWarning("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): Key → {Key}", Key);
			_logger.LogDebug("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): KeyGroup → {KeyGroup}", KeyGroup);
			_logger.LogDebug("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): Value → {Value}", Value);
			_logger.LogDebug("ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute(): CreatedOrUpdatedDateUTC → {CreatedOrUpdatedDateUTC}", CreatedOrUpdatedDateUTC);
        };

        await _genericAttributeService.InsertAttributeAsync(attribute);

        var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

        await _genericAttributeService.DeleteAttributeAsync(attribute);

        Assert.That(createdOrUpdatedDate,
            Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
			_logger.LogDebug("<<< Exiting ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()");
    }

    [Test]
    public async Task ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()
    {
		_logger.LogDebug(">>> Entering ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()");
        var attribute = new global::Nop.Core.Domain.Common.GenericAttribute { Key = "test", KeyGroup = "test", Value = "test" };

		_logger.LogDebug("ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute(): attribute → {Attribute}", attribute);
        await _genericAttributeService.InsertAttributeAsync(attribute);
        attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow.AddDays(-30);
        await _genericAttributeService.UpdateAttributeAsync(attribute);

        var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

        await _genericAttributeService.DeleteAttributeAsync(attribute);

        Assert.That(createdOrUpdatedDate,
            Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
			_logger.LogDebug("<<< Exiting ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()");
    }
}