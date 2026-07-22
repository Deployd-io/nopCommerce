using FluentAssertions;
using Nop.Core.Domain.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Domain;

[TestFixture]
public class EntityEqualityTests
{
    [Test]
    public void TwoTransientEntitiesShouldNotBeEqual()
    {
		logger.LogDebug(">>> Entering TwoTransientEntitiesShouldNotBeEqual()");
        var p1 = new Product();
        var p2 = new Product();
		logger.LogDebug("TwoTransientEntitiesShouldNotBeEqual(): p1 → {P1}", p1);
		logger.LogDebug("TwoTransientEntitiesShouldNotBeEqual(): p2 → {P2}", p2);

        p1.Should().NotBe(p2, "Different transient entities should not be equal");
		logger.LogDebug("<<< Exiting TwoTransientEntitiesShouldNotBeEqual()");
    }

    [Test]
    public void TwoReferencesToSameTransientEntityShouldBeEqual()
    {
		logger.LogDebug(">>> Entering TwoReferencesToSameTransientEntityShouldBeEqual()");
        var p1 = new Product();
        var p2 = p1;
		logger.LogDebug("TwoReferencesToSameTransientEntityShouldBeEqual(): p1 → {P1}", p1);

        p1.Should().Be(p2, "Two references to the same transient entity should be equal");
		logger.LogDebug("<<< Exiting TwoReferencesToSameTransientEntityShouldBeEqual()");
    }

    [Test]
    public void EntitiesWithDifferentIdShouldNotBeEqual()
    {
		logger.LogDebug(">>> Entering EntitiesWithDifferentIdShouldNotBeEqual()");
        var p1 = new Product { Id = 2 };
        var p2 = new Product { Id = 5 };

        p1.Should().NotBe(p2, "Entities with different ids should not be equal");
		logger.LogDebug("<<< Exiting EntitiesWithDifferentIdShouldNotBeEqual()");
    }

    [Test]
    public void EntityShouldNotEqualTransientEntity()
    {
		logger.LogDebug(">>> Entering EntityShouldNotEqualTransientEntity()");
        var p1 = new Product { Id = 1 };
        var p2 = new Product();

		logger.LogDebug("EntityShouldNotEqualTransientEntity(): p2 → {P2}", p2);
        p1.Should().NotBe(p2, "Entity and transient entity should not be equal");
		logger.LogDebug("<<< Exiting EntityShouldNotEqualTransientEntity()");
    }

    [Test]
    public void EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()
    {
		logger.LogDebug(">>> Entering EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()");
        const int id = 10;
        var p1 = new Product { Id = id };
		logger.LogDebug("EntitiesWithSameIdButDifferentTypeShouldNotBeEqual(): id → {Id}", id);

        var c1 = new Category { Id = id };

        p1.Should().NotBe(c1, "Entities of different types should not be equal, even if they have the same id");
		logger.LogDebug("<<< Exiting EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()");
    }
}
