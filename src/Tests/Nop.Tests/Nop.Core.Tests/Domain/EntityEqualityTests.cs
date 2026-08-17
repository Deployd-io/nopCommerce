using FluentAssertions;
using Nop.Core.Domain.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Domain;

[TestFixture]
public class EntityEqualityTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void TwoTransientEntitiesShouldNotBeEqual()
    {
		_logger.LogDebug(">>> Entering TwoTransientEntitiesShouldNotBeEqual()");
        var p1 = new Product();
        var p2 = new Product();
		_logger.LogDebug("TwoTransientEntitiesShouldNotBeEqual(): p2 → {P2}", p2);
		_logger.LogDebug("TwoTransientEntitiesShouldNotBeEqual(): p1 → {P1}", p1);

        p1.Should().NotBe(p2, "Different transient entities should not be equal");
		_logger.LogDebug("<<< Exiting TwoTransientEntitiesShouldNotBeEqual()");
    }

    [Test]
    public void TwoReferencesToSameTransientEntityShouldBeEqual()
    {
		_logger.LogDebug(">>> Entering TwoReferencesToSameTransientEntityShouldBeEqual()");
        var p1 = new Product();
        var p2 = p1;
		_logger.LogDebug("TwoReferencesToSameTransientEntityShouldBeEqual(): p1 → {P1}", p1);

        p1.Should().Be(p2, "Two references to the same transient entity should be equal");
		_logger.LogDebug("<<< Exiting TwoReferencesToSameTransientEntityShouldBeEqual()");
    }

    [Test]
    public void EntitiesWithDifferentIdShouldNotBeEqual()
    {
		_logger.LogDebug(">>> Entering EntitiesWithDifferentIdShouldNotBeEqual()");
        var p1 = new Product { Id = 2 };
        var p2 = new Product { Id = 5 };

        p1.Should().NotBe(p2, "Entities with different ids should not be equal");
		_logger.LogDebug("<<< Exiting EntitiesWithDifferentIdShouldNotBeEqual()");
    }

    [Test]
    public void EntityShouldNotEqualTransientEntity()
    {
		_logger.LogDebug(">>> Entering EntityShouldNotEqualTransientEntity()");
        var p1 = new Product { Id = 1 };
        var p2 = new Product();
		_logger.LogDebug("EntityShouldNotEqualTransientEntity(): p2 → {P2}", p2);

        p1.Should().NotBe(p2, "Entity and transient entity should not be equal");
		_logger.LogDebug("<<< Exiting EntityShouldNotEqualTransientEntity()");
    }

    [Test]
    public void EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()
    {
		_logger.LogDebug(">>> Entering EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()");
        const int id = 10;
        var p1 = new Product { Id = id };
		_logger.LogDebug("EntitiesWithSameIdButDifferentTypeShouldNotBeEqual(): id → {Id}", id);

        var c1 = new Category { Id = id };

        p1.Should().NotBe(c1, "Entities of different types should not be equal, even if they have the same id");
		_logger.LogDebug("<<< Exiting EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()");
    }
}