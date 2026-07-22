using FluentAssertions;
using Nop.Services.Localization;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

public class CountryModelFactoryTests : WebTest
{
    private ICountryModelFactory _countryModelFactory;
    private ILocalizationService _localizationService;
    private int _countryId;
    private int _invalidCountryId;
    private int _countryWithoutStatesId;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _countryModelFactory = GetService<ICountryModelFactory>();
        _localizationService = GetService<ILocalizationService>();
		logger.LogDebug("SetUp(): _countryModelFactory → {_countryModelFactory}", _countryModelFactory);
		logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);

        _countryId = 41; //Canada
        _countryWithoutStatesId = 3; //Albania
        _invalidCountryId = 0;
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _invalidCountryId → {_invalidCountryId}", _invalidCountryId);
    }

    [Test]
    public async Task CanGetStatesByValidCountryId()
    {
		logger.LogDebug(">>> Entering CanGetStatesByValidCountryId()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);
        models.Should().NotBeNullOrEmpty();
		logger.LogDebug("CanGetStatesByValidCountryId(): models → {Models}", models);
        models.Count.Should().BeGreaterThan(1);

        models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryWithoutStatesId, false);
        models.Should().NotBeNullOrEmpty();
        models.Count.Should().Be(1);
		logger.LogDebug("<<< Exiting CanGetStatesByValidCountryId()");
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryIdWithoutSelectState()
    {
		logger.LogDebug(">>> Entering CanGetStatesByInvalidCountryIdWithoutSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, false);
        models.Should().NotBeNullOrEmpty();
		logger.LogDebug("CanGetStatesByInvalidCountryIdWithoutSelectState(): models → {Models}", models);

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.Other")
        );
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetStatesByInvalidCountryIdWithoutSelectState()");
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryIdWithSelectState()
    {
		logger.LogDebug(">>> Entering CanGetStatesByInvalidCountryIdWithSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, true);
        models.Should().NotBeNullOrEmpty();
		logger.LogDebug("CanGetStatesByInvalidCountryIdWithSelectState(): models → {Models}", models);

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetStatesByInvalidCountryIdWithSelectState()");
    }

    [Test]
    public async Task CanGetStatesByCountryIdWithoutSelectState()
    {
		logger.LogDebug(">>> Entering CanGetStatesByCountryIdWithoutSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);

		logger.LogDebug("CanGetStatesByCountryIdWithoutSelectState(): models → {Models}", models);
        var result = await models.AllAwaitAsync(
            async m => m.name != await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetStatesByCountryIdWithoutSelectState()");
    }

    [Test]
    public async Task CanGetStatesByCountryIdWithSelectState()
    {
		logger.LogDebug(">>> Entering CanGetStatesByCountryIdWithSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, true);

		logger.LogDebug("CanGetStatesByCountryIdWithSelectState(): models → {Models}", models);
        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetStatesByCountryIdWithSelectState()");
    }
}
