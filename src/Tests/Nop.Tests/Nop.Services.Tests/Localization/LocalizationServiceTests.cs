using FluentAssertions;
using Humanizer;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Localization;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Localization;

[TestFixture]
public class LocalizationServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ILocalizationService _localizationService;
    private IRepository<LocaleStringResource> _lsrRepository;
    private const string PREFIX = "Nop.Tests.Nop.Services.Tests.Localization";
    private Dictionary<string, string> _resources;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _localizationService = GetService<ILocalizationService>();
        _lsrRepository = GetService<IRepository<LocaleStringResource>>();
		_logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);
		_logger.LogDebug("SetUp(): _lsrRepository → {_lsrRepository}", _lsrRepository);

        _resources = new Dictionary<string, string>
		_logger.LogDebug("SetUp(): _resources → {_resources}", _resources);
        {
            [$"{PREFIX}.Val1"] = "Value1",
            [$"{PREFIX}.Val2"] = "Value2",
            [$"{PREFIX}.Val3"] = "Value3"
        };
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [TearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _localizationService.DeleteLocaleResourcesAsync(PREFIX);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    protected IQueryable<LocaleStringResource> Filter(Dictionary<string, string> resources, IQueryable<LocaleStringResource> query)
    {
        query = query.Where(p =>
            resources.Keys.Select(k => k.ToLowerInvariant()).Contains(p.ResourceName));

        return query;
    }

    [Test]
    public async Task CanAddOrUpdateLocaleResource()
    {
		_logger.LogDebug(">>> Entering CanAddOrUpdateLocaleResource()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var localeStringResources = await _lsrRepository.GetAllAsync(query => Filter(_resources, query));

        localeStringResources.Any().Should().BeFalse();
		_logger.LogDebug("CanAddOrUpdateLocaleResource(): localeStringResources → {LocaleStringResources}", localeStringResources);
		_logger.LogInformation("CanAddOrUpdateLocaleResource(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        localeStringResources = await _lsrRepository.GetAllAsync(query => Filter(_resources, query));

        localeStringResources.Any().Should().BeTrue();
        localeStringResources.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanAddOrUpdateLocaleResource()");
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldIgnoreKeyCase()
    {
		_logger.LogDebug(">>> Entering AddOrUpdateLocaleResourceShouldIgnoreKeyCase()");
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Camelize(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Pascalize(), p => p.Value));

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting AddOrUpdateLocaleResourceShouldIgnoreKeyCase()");
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldNotIgnoreValueCase()
    {
		_logger.LogDebug(">>> Entering AddOrUpdateLocaleResourceShouldNotIgnoreValueCase()");
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);
        rez.Count(p => p.ResourceValue == p.ResourceValue.ToUpperInvariant()).Should().Be(0);

        rez.Count(p => _resources.ContainsValue(p.ResourceValue)).Should().Be(3);

        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToUpperInvariant()));

        rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);
        rez.Count(p => p.ResourceValue == p.ResourceValue.ToUpperInvariant()).Should().Be(3);
        rez.Count(p => _resources.ContainsValue(p.ResourceValue)).Should().Be(0);
		_logger.LogDebug("<<< Exiting AddOrUpdateLocaleResourceShouldNotIgnoreValueCase()");
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldSkipAlreadyExistsResources()
    {
		_logger.LogDebug(">>> Entering AddOrUpdateLocaleResourceShouldSkipAlreadyExistsResources()");
        LocaleResourceConsumer.UpdateCount = 0;

		_logger.LogDebug("AddOrUpdateLocaleResourceShouldSkipAlreadyExistsResources(): LocaleResourceConsumer.UpdateCount → {Arg0}", LocaleResourceConsumer.UpdateCount);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Camelize(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Pascalize(), p => p.Value));

        LocaleResourceConsumer.UpdateCount.Should().Be(0);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToUpperInvariant()));

        LocaleResourceConsumer.UpdateCount.Should().Be(3);
		_logger.LogDebug("<<< Exiting AddOrUpdateLocaleResourceShouldSkipAlreadyExistsResources()");
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldApdateAllResorcesIfLangIdIsNull()
    {
		_logger.LogDebug(">>> Entering AddOrUpdateLocaleResourceShouldApdateAllResorcesIfLangIdIsNull()");
        var languageService = GetService<ILanguageService>();
        var language = new Language
		_logger.LogDebug("AddOrUpdateLocaleResourceShouldApdateAllResorcesIfLangIdIsNull(): languageService → {LanguageService}", languageService);
        {
            Name = "test lang",
            LanguageCulture = "en"
        };
        await languageService.InsertLanguageAsync(language);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources, 1);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources, language.Id);
        LocaleResourceConsumer.UpdateCount = 0;
		_logger.LogDebug("AddOrUpdateLocaleResourceShouldApdateAllResorcesIfLangIdIsNull(): LocaleResourceConsumer.UpdateCount → {Arg0}", LocaleResourceConsumer.UpdateCount);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key, p => p.Value.ToUpperInvariant()), null);
        await languageService.DeleteLanguageAsync(language);
        LocaleResourceConsumer.UpdateCount.Should().Be(6);
		_logger.LogDebug("<<< Exiting AddOrUpdateLocaleResourceShouldApdateAllResorcesIfLangIdIsNull()");
    }

    [Test]
    public async Task CanDeleteLocaleResources()
    {
		_logger.LogDebug(">>> Entering CanDeleteLocaleResources()");
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);

        await _localizationService.DeleteLocaleResourcesAsync(PREFIX);

        rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanDeleteLocaleResources()");
    }

    [Test]
    public async Task DeleteLocaleResourcesShuoldIgnoreCase()
    {
		_logger.LogDebug(">>> Entering DeleteLocaleResourcesShuoldIgnoreCase()");
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);

        await _localizationService.DeleteLocaleResourcesAsync(PREFIX.ToUpperInvariant());

        rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting DeleteLocaleResourcesShuoldIgnoreCase()");
    }

    public class LocaleResourceConsumer : IConsumer<EntityUpdatedEvent<LocaleStringResource>>
    {
        public static int UpdateCount { get; set; }

        public Task HandleEventAsync(EntityUpdatedEvent<LocaleStringResource> eventMessage)
        {
			_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
            UpdateCount += 1;

			_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
            return Task.CompletedTask;
        }
    }
}