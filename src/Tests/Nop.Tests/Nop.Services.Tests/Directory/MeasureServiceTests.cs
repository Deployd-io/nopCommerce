using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Directory;

[TestFixture]
public class MeasureServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IMeasureService _measureService;

    private MeasureDimension _measureDimensionInches, _measureDimensionFeet, _measureDimensionMeters, _measureDimensionMillimetres;
    private MeasureWeight _measureWeightOunce, _measureWeightLb, _measureWeightKg, _measureWeightGrams;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _measureService = GetService<IMeasureService>();

		_logger.LogDebug("SetUp(): _measureService → {_measureService}", _measureService);
        _measureDimensionInches = await _measureService.GetMeasureDimensionBySystemKeywordAsync("inches");
		_logger.LogDebug("SetUp(): _measureDimensionInches → {_measureDimensionInches}", _measureDimensionInches);
        _measureDimensionFeet = await _measureService.GetMeasureDimensionBySystemKeywordAsync("feet");
		_logger.LogDebug("SetUp(): _measureDimensionFeet → {_measureDimensionFeet}", _measureDimensionFeet);
        _measureDimensionMeters = await _measureService.GetMeasureDimensionBySystemKeywordAsync("meters");
        _measureDimensionMillimetres = await _measureService.GetMeasureDimensionBySystemKeywordAsync("millimetres");
		_logger.LogDebug("SetUp(): _measureDimensionMeters → {_measureDimensionMeters}", _measureDimensionMeters);
		_logger.LogDebug("SetUp(): _measureDimensionMillimetres → {_measureDimensionMillimetres}", _measureDimensionMillimetres);

        _measureWeightOunce = await _measureService.GetMeasureWeightBySystemKeywordAsync("ounce");
		_logger.LogDebug("SetUp(): _measureWeightOunce → {_measureWeightOunce}", _measureWeightOunce);
        _measureWeightLb = await _measureService.GetMeasureWeightBySystemKeywordAsync("lb");
        _measureWeightKg = await _measureService.GetMeasureWeightBySystemKeywordAsync("kg");
		_logger.LogDebug("SetUp(): _measureWeightLb → {_measureWeightLb}", _measureWeightLb);
		_logger.LogDebug("SetUp(): _measureWeightKg → {_measureWeightKg}", _measureWeightKg);
        _measureWeightGrams = await _measureService.GetMeasureWeightBySystemKeywordAsync("grams");
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _measureWeightGrams → {_measureWeightGrams}", _measureWeightGrams);
    }

    [Test]
    public async Task CanConvertDimension()
    {
        //from meter(s) to feet
        var newDimension = await _measureService.ConvertDimensionAsync(10, _measureDimensionMeters, _measureDimensionFeet);
        newDimension.Should().Be(32.81M);
		_logger.LogDebug("CanConvertDimension(): newDimension → {NewDimension}", newDimension);
        //from inch(es) to meter(s)
        newDimension = await _measureService.ConvertDimensionAsync(10, _measureDimensionInches, _measureDimensionMeters);
        newDimension.Should().Be(0.25M);
        //from meter(s) to meter(s)
        newDimension = await _measureService.ConvertDimensionAsync(13.333M, _measureDimensionMeters, _measureDimensionMeters);
        newDimension.Should().Be(13.33M);
        //from meter(s) to millimeter(s)
        newDimension = await _measureService.ConvertDimensionAsync(10, _measureDimensionMeters, _measureDimensionMillimetres);
        newDimension.Should().Be(10000);
        //from millimeter(s) to meter(s)
        newDimension = await _measureService.ConvertDimensionAsync(10000, _measureDimensionMillimetres, _measureDimensionMeters);
        newDimension.Should().Be(10);
		_logger.LogDebug("<<< Exiting CanConvertDimension()");
    }

    [Test]
    public async Task CanConvertWeight()
    {
		_logger.LogDebug(">>> Entering CanConvertWeight()");
        //from ounce(s) to lb(s)
        var newWeight = await _measureService.ConvertWeightAsync(11, _measureWeightOunce, _measureWeightLb);
        newWeight.Should().Be(0.69M);
		_logger.LogDebug("CanConvertWeight(): newWeight → {NewWeight}", newWeight);
        //from lb(s) to ounce(s)
        newWeight = await _measureService.ConvertWeightAsync(11, _measureWeightLb, _measureWeightOunce);
        newWeight.Should().Be(176);
        //from ounce(s) to  ounce(s)
        newWeight = await _measureService.ConvertWeightAsync(13.333M, _measureWeightOunce, _measureWeightOunce);
        newWeight.Should().Be(13.33M);
        //from kg(s) to ounce(s)
        newWeight = await _measureService.ConvertWeightAsync(11, _measureWeightKg, _measureWeightOunce);
        newWeight.Should().Be(388.01M);
        //from kg(s) to gram(s)
        newWeight = await _measureService.ConvertWeightAsync(10, _measureWeightKg, _measureWeightGrams);
        newWeight.Should().Be(10000);
		_logger.LogDebug("<<< Exiting CanConvertWeight()");
    }

}

[TestFixture]
public class MeasureDimensionCrudTests : ServiceTest<MeasureDimension>
{
    private IMeasureService _measureService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _measureService = GetService<IMeasureService>();
    }

    protected override CrudData<MeasureDimension> CrudData
    {
        get
        {
            var insertItem = new MeasureDimension
            {
                Name = "Test name",
                SystemKeyword = "test"
            };

            var updateItem = new MeasureDimension
            {
                Name = "Test name 1",
                SystemKeyword = "test"
            };

            return new CrudData<MeasureDimension>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _measureService.InsertMeasureDimensionAsync,
                Update = _measureService.UpdateMeasureDimensionAsync,
                GetById = _measureService.GetMeasureDimensionByIdAsync,
                IsEqual = (item, other) => item.Name.Equals(other.Name),
                Delete = _measureService.DeleteMeasureDimensionAsync
            };
        }
    }
}

[TestFixture]
public class MeasureWeightCrudTests : ServiceTest<MeasureWeight>
{
    private IMeasureService _measureService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _measureService = GetService<IMeasureService>();
    }

    protected override CrudData<MeasureWeight> CrudData
    {
        get
        {
            var insertItem = new MeasureWeight
            {
                Name = "Test name",
                SystemKeyword = "test"
            };

            var updateItem = new MeasureWeight
            {
                Name = "Test name 1",
                SystemKeyword = "test"
            };

            return new CrudData<MeasureWeight>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _measureService.InsertMeasureWeightAsync,
                Update = _measureService.UpdateMeasureWeightAsync,
                GetById = _measureService.GetMeasureWeightByIdAsync,
                IsEqual = (item, other) => item.Name.Equals(other.Name),
                Delete = _measureService.DeleteMeasureWeightAsync
            };
        }
    }

}