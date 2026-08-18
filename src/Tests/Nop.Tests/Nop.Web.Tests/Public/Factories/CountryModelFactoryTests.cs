using FluentAssertions;
using Nop.Services.Localization;
using Nop.Web.Factories;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

public class CountryModelFactoryTests : WebTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICountryModelFactory _countryModelFactory;
    private ILocalizationService _localizationService;
    private int _countryId;
    private int _invalidCountryId;
    private int _countryWithoutStatesId;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _countryModelFactory = GetService<ICountryModelFactory>();
        _localizationService = GetService<ILocalizationService>();

		_logger.LogDebug("SetUp(): _countryModelFactory → {_countryModelFactory}", _countryModelFactory);
		_logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);
        _countryId = 41; //Canada
        _countryWithoutStatesId = 3; //Albania
        _invalidCountryId = 0;
		_logger.LogDebug("SetUp(): _invalidCountryId → {_invalidCountryId}", _invalidCountryId);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanGetStatesByValidCountryId()
    {
		_logger.LogDebug(">>> Entering CanGetStatesByValidCountryId()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);
        models.Should().NotBeNullOrEmpty();
		_logger.LogDebug("CanGetStatesByValidCountryId(): models → {Models}", models);
        models.Count.Should().BeGreaterThan(1);

        models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryWithoutStatesId, false);
        models.Should().NotBeNullOrEmpty();
        models.Count.Should().Be(1);
		_logger.LogDebug("<<< Exiting CanGetStatesByValidCountryId()");
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryIdWithoutSelectState()
    {
		_logger.LogDebug(">>> Entering CanGetStatesByInvalidCountryIdWithoutSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, false);
        models.Should().NotBeNullOrEmpty();
		_logger.LogDebug("CanGetStatesByInvalidCountryIdWithoutSelectState(): models → {Models}", models);

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.Other")
        );
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetStatesByInvalidCountryIdWithoutSelectState()");
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryIdWithSelectState()
    {
		_logger.LogDebug(">>> Entering CanGetStatesByInvalidCountryIdWithSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, true);
        models.Should().NotBeNullOrEmpty();
		_logger.LogDebug("CanGetStatesByInvalidCountryIdWithSelectState(): models → {Models}", models);

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetStatesByInvalidCountryIdWithSelectState()");
    }

    [Test]
    public async Task CanGetStatesByCountryIdWithoutSelectState()
    {
		_logger.LogDebug(">>> Entering CanGetStatesByCountryIdWithoutSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);

		_logger.LogDebug("CanGetStatesByCountryIdWithoutSelectState(): models → {Models}", models);
        var result = await models.AllAwaitAsync(
            async m => m.name != await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetStatesByCountryIdWithoutSelectState()");
    }

    [Test]
    public async Task CanGetStatesByCountryIdWithSelectState()
    {
		_logger.LogDebug(">>> Entering CanGetStatesByCountryIdWithSelectState()");
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, true);

		_logger.LogDebug("CanGetStatesByCountryIdWithSelectState(): models → {Models}", models);
        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetStatesByCountryIdWithSelectState()");
    }
}
