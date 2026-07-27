using System.Linq.Expressions;
using FluentAssertions;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Tax;
using Nop.Data;
using NUnit.Framework;

namespace Nop.Tests.Nop.Data.Tests;

[TestFixture]
public class EntityRepositoryTests : BaseNopTest
{
    private IStaticCacheManager _cacheManager;
    private CacheKey _cacheKey;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _cacheManager = GetService<IStaticCacheManager>();
        _cacheKey = new CacheKey("EntityRepositoryTestsCacheKey");
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _cacheManager → {_cacheManager}", _cacheManager);
		logger.LogDebug("SetUp(): _cacheKey → {_cacheKey}", _cacheKey);
    }

    [TearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var productRepository = GetService<IRepository<Product>>();
            var product = await productRepository.GetByIdAsync(2);
            product.Deleted = false;
			logger.LogDebug("TearDown(): productRepository → {ProductRepository}", productRepository);
			logger.LogInformation("TearDown(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
			logger.LogDebug("TearDown(): product → {Product}", product);
			logger.LogDebug("TearDown(): product.Deleted → {Arg0}", product.Deleted);
			logger.LogInformation("TearDown(): update query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
			logger.LogError("Exception in TearDown(): {Arg0}", ex.Message);
            await productRepository.UpdateAsync(product);
            await _cacheManager.ClearAsync();
        }
        catch
        {
            //ignore 
        }

        SetDataProviderType(DataProviderType.Unknown);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanGetById(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetById(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var productRepository = GetService<IRepository<Product>>();

		logger.LogDebug("CanGetById(type={Type}): productRepository → {ProductRepository}", type, productRepository);
        var product = await productRepository.GetByIdAsync(1);
        product.Should().NotBeNull();
		logger.LogDebug("CanGetById(type={Type}): product → {Product}", type, product);
		logger.LogInformation("CanGetById(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);

        product = await productRepository.GetByIdAsync(2);
        product.Deleted = true;
        await productRepository.UpdateAsync(product);

		logger.LogDebug("CanGetById(type={Type}): product.Deleted → {Arg1}", type, product.Deleted);
		logger.LogInformation("CanGetById(type)={Type}: update query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        product = await productRepository.GetByIdAsync(2);
        product.Should().NotBeNull();
        product = await productRepository.GetByIdAsync(2, includeDeleted: false);
        product.Should().BeNull();

        product = await _cacheManager.GetAsync(_cacheKey, default(Product));
        product.Should().BeNull();

        await productRepository.GetByIdAsync(1, _ => _cacheKey);
        product = await _cacheManager.GetAsync(_cacheKey, default(Product));
        product.Should().NotBeNull();
		logger.LogInformation("CanGetById(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting CanGetById(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanGetByIds(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetByIds(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var productRepository = GetService<IRepository<Product>>();

		logger.LogDebug("CanGetByIds(type={Type}): productRepository → {ProductRepository}", type, productRepository);
        var product = await productRepository.GetByIdAsync(2);
        product.Deleted = true;
		logger.LogDebug("CanGetByIds(type={Type}): product → {Product}", type, product);
		logger.LogInformation("CanGetByIds(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("CanGetByIds(type={Type}): product.Deleted → {Arg1}", type, product.Deleted);
		logger.LogInformation("CanGetByIds(type)={Type}: update query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await productRepository.UpdateAsync(product);

        var ids = new List<int> { 1, 2, 3 };

        var products = await productRepository.GetByIdsAsync(ids);
        products.Count.Should().Be(3);
		logger.LogDebug("CanGetByIds(type={Type}): products → {Products}", type, products);

        products = await productRepository.GetByIdsAsync(ids, includeDeleted: false);
        products.Count.Should().Be(2);

        products = await _cacheManager.GetAsync(_cacheKey, default(IList<Product>));
        products.Should().BeNull();

        await productRepository.GetByIdsAsync(ids, _ => _cacheKey);
        products = await _cacheManager.GetAsync(_cacheKey, default(IList<Product>));
        products.Count.Should().Be(3);
		logger.LogInformation("CanGetByIds(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting CanGetByIds(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanGetAll(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetAll(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var productRepository = GetService<IRepository<Product>>();

		logger.LogDebug("CanGetAll(type={Type}): productRepository → {ProductRepository}", type, productRepository);
        var product = await productRepository.GetByIdAsync(2);
        product.Deleted = true;
		logger.LogDebug("CanGetAll(type={Type}): product → {Product}", type, product);
		logger.LogInformation("CanGetAll(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("CanGetAll(type={Type}): product.Deleted → {Arg1}", type, product.Deleted);
		logger.LogInformation("CanGetAll(type)={Type}: update query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await productRepository.UpdateAsync(product);

        var asyncRez = await productRepository.GetAllAsync(query => query, null);
        asyncRez.Count.Should().BeGreaterThan(0);
		logger.LogDebug("CanGetAll(type={Type}): asyncRez → {AsyncRez}", type, asyncRez);

        var taskRez = await productRepository.GetAllAsync(Task.FromResult);
        taskRez.Count.Should().BeGreaterThan(0);
		logger.LogDebug("CanGetAll(type={Type}): taskRez → {TaskRez}", type, taskRez);

        var rez = productRepository.GetAll(query => query, manager => _cacheKey);
        rez.Count.Should().BeGreaterThan(0);
		logger.LogDebug("CanGetAll(type={Type}): rez → {Rez}", type, rez);

        rez = productRepository.GetAll(query => query);
        rez.Count.Should().BeGreaterThan(0);

        asyncRez.Count.Should().Be(rez.Count);
        rez.Count.Should().Be(taskRez.Count);

        rez = await productRepository.GetAllAsync(Task.FromResult, manager => Task.FromResult(_cacheKey));
        rez.Count.Should().BeGreaterThan(0);
        var fullCount = rez.Count;

        await _cacheManager.RemoveAsync(_cacheKey);

        rez = await productRepository.GetAllAsync(Task.FromResult, manager => Task.FromResult(default(CacheKey)), false);
        rez.Count.Should().BeGreaterThan(0);
        rez.Count.Should().BeLessThan(fullCount);
		logger.LogDebug("<<< Exiting CanGetAll(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanGetAllPaged(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetAllPaged(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var productRepository = GetService<IRepository<Product>>();

		logger.LogDebug("CanGetAllPaged(type={Type}): productRepository → {ProductRepository}", type, productRepository);
        var product = await productRepository.GetByIdAsync(2);
        product.Deleted = true;
		logger.LogDebug("CanGetAllPaged(type={Type}): product → {Product}", type, product);
		logger.LogInformation("CanGetAllPaged(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("CanGetAllPaged(type={Type}): product.Deleted → {Arg1}", type, product.Deleted);
		logger.LogInformation("CanGetAllPaged(type)={Type}: update query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await productRepository.UpdateAsync(product);

        var rez = await productRepository.GetAllPagedAsync(query => query);
        rez.Count.Should().BeGreaterThan(0);
        var fullCount = rez.Count;

        rez = await productRepository.GetAllPagedAsync(query => query, includeDeleted: false);
        rez.Count.Should().BeGreaterThan(0);
		logger.LogDebug("CanGetAllPaged(type={Type}): rez → {Rez}", type, rez);
        rez.Count.Should().BeLessThan(fullCount);
		logger.LogDebug("<<< Exiting CanGetAllPaged(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanInsert(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanInsert(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var taxCategoryRepository = GetService<IRepository<TaxCategory>>();
        var taxCategory = new TaxCategory { DisplayOrder = 10, Name = "test tax category" };
		logger.LogDebug("CanInsert(type={Type}): taxCategoryRepository → {TaxCategoryRepository}", type, taxCategoryRepository);
		logger.LogDebug("CanInsert(type={Type}): taxCategory → {TaxCategory}", type, taxCategory);
        taxCategory.Id.Should().Be(0);

        await taxCategoryRepository.InsertAsync(taxCategory);
		logger.LogInformation("CanInsert(type)={Type}: save query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogInformation("CanInsert(type)={Type}: delete query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await taxCategoryRepository.DeleteAsync(taxCategory);

        taxCategory.Id.Should().BeGreaterThan(0);

        Assert.Throws<AggregateException>(() => taxCategoryRepository.InsertAsync(default(TaxCategory)).Wait());
		logger.LogInformation("CanInsert(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting CanInsert(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanUpdate(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanUpdate(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var taxCategoryRepository = GetService<IRepository<TaxCategory>>();
        var taxCategory = new TaxCategory { DisplayOrder = 10, Name = "test tax category" };
		logger.LogDebug("CanUpdate(type={Type}): taxCategoryRepository → {TaxCategoryRepository}", type, taxCategoryRepository);
		logger.LogDebug("CanUpdate(type={Type}): taxCategory → {TaxCategory}", type, taxCategory);
        taxCategory.Id.Should().Be(0);

        await taxCategoryRepository.InsertAsync(taxCategory);
		logger.LogInformation("CanUpdate(type)={Type}: save query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await taxCategoryRepository.UpdateAsync(new TaxCategory
        {
            Id = taxCategory.Id,
            Name = "Updated test tax category"
        });
		logger.LogInformation("CanUpdate(type)={Type}: update query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogInformation("CanUpdate(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        var updatedTaxCategory = await taxCategoryRepository.GetByIdAsync(taxCategory.Id);
        await taxCategoryRepository.DeleteAsync(taxCategory);

		logger.LogDebug("CanUpdate(type={Type}): updatedTaxCategory → {UpdatedTaxCategory}", type, updatedTaxCategory);
		logger.LogInformation("CanUpdate(type)={Type}: delete query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogInformation("CanUpdate(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        taxCategory.Id.Should().BeGreaterThan(0);
        updatedTaxCategory.Name.Should().NotBeEquivalentTo(taxCategory.Name);
		logger.LogDebug("<<< Exiting CanUpdate(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanDelete(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanDelete(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var taxCategoryRepository = GetService<IRepository<TaxCategory>>();
        var taxCategory = new TaxCategory { DisplayOrder = 10, Name = "test tax category" };
		logger.LogDebug("CanDelete(type={Type}): taxCategoryRepository → {TaxCategoryRepository}", type, taxCategoryRepository);
		logger.LogInformation("CanDelete(type)={Type}: save query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await taxCategoryRepository.InsertAsync(taxCategory);
        await taxCategoryRepository.DeleteAsync(taxCategory);
        taxCategory.Id.Should().BeGreaterThan(0);
		logger.LogDebug("CanDelete(type={Type}): taxCategory → {TaxCategory}", type, taxCategory);
		logger.LogInformation("CanDelete(type)={Type}: delete query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogInformation("CanDelete(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        taxCategory = await taxCategoryRepository.GetByIdAsync(taxCategory.Id);
        taxCategory.Should().BeNull();

        Assert.Throws<AggregateException>(() => taxCategoryRepository.DeleteAsync(default(TaxCategory)).Wait());
        Assert.Throws<AggregateException>(() => taxCategoryRepository.DeleteAsync(default(IList<TaxCategory>)).Wait());
        Assert.Throws<AggregateException>(() => taxCategoryRepository.DeleteAsync((Expression<Func<TaxCategory, bool>>)null).Wait());
		logger.LogInformation("CanDelete(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting CanDelete(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanLoadOriginalCopy(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanLoadOriginalCopy(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var productRepository = GetService<IRepository<Product>>();
        var product = await productRepository.GetByIdAsync(1, _ => default);
        product.Name = "test name";
		logger.LogDebug("CanLoadOriginalCopy(type={Type}): productRepository → {ProductRepository}", type, productRepository);
		logger.LogDebug("CanLoadOriginalCopy(type={Type}): product → {Product}", type, product);
		logger.LogDebug("CanLoadOriginalCopy(type={Type}): product.Name → {Arg1}", type, product.Name);
		logger.LogInformation("CanLoadOriginalCopy(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        var productNew = await productRepository.GetByIdAsync(1, _ => default);
        var productOld = await productRepository.LoadOriginalCopyAsync(product);
		logger.LogDebug("CanLoadOriginalCopy(type={Type}): productNew → {ProductNew}", type, productNew);
		logger.LogDebug("CanLoadOriginalCopy(type={Type}): productOld → {ProductOld}", type, productOld);

        productOld.Name.Should().NotBeEquivalentTo(productNew.Name);
		logger.LogDebug("<<< Exiting CanLoadOriginalCopy(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanTruncate(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanTruncate(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var gdprConsentRepository = GetService<IRepository<GdprConsent>>();

		logger.LogDebug("CanTruncate(type={Type}): gdprConsentRepository → {GdprConsentRepository}", type, gdprConsentRepository);
		logger.LogInformation("CanTruncate(type)={Type}: save query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        await gdprConsentRepository.InsertAsync(new List<GdprConsent>
        {
            new() {Message = "Test message 1"},
            new() {Message = "Test message 2"},
            new() {Message = "Test message 3"},
            new() {Message = "Test message 4"},
            new() {Message = "Test message 5"}
        });

        var rezWithContent = await gdprConsentRepository.GetAllAsync(query => query);
        await gdprConsentRepository.TruncateAsync();
        var rezWithoutContent = await gdprConsentRepository.GetAllAsync(query => query);

        var gdprConsent1 = new GdprConsent { Message = "Test message 1" };
        var gdprConsent2 = new GdprConsent { Message = "Test message 2" };

        await gdprConsentRepository.InsertAsync(gdprConsent1);
        await gdprConsentRepository.TruncateAsync(true);
        await gdprConsentRepository.InsertAsync(gdprConsent2);
        await gdprConsentRepository.TruncateAsync(true);

        rezWithContent.Count.Should().Be(5);
        rezWithoutContent.Count.Should().Be(0);
        gdprConsent1.Id.Should().NotBe(1);
        gdprConsent2.Id.Should().Be(1);
		logger.LogDebug("<<< Exiting CanTruncate(type={Type})", type);
    }
}