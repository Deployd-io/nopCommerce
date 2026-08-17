using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.ShoppingCart;

public partial record EstimateShippingModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public EstimateShippingModel()
    {
		_logger.LogDebug(">>> Entering EstimateShippingModel()");
        AvailableCountries = new List<SelectListItem>();
        AvailableStates = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting EstimateShippingModel()");
		_logger.LogDebug("EstimateShippingModel(): AvailableStates → {AvailableStates}", AvailableStates);
		_logger.LogDebug("EstimateShippingModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
    }

    public int RequestDelay { get; set; }

    public bool Enabled { get; set; }

    public int? CountryId { get; set; }
    public int? StateProvinceId { get; set; }
    public string ZipPostalCode { get; set; }
    public bool UseCity { get; set; }
    public string City { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }
    public IList<SelectListItem> AvailableStates { get; set; }
}

public partial record EstimateShippingResultModel : BaseNopModel
{
    public EstimateShippingResultModel()
    {
		_logger.LogDebug(">>> Entering EstimateShippingResultModel()");
        ShippingOptions = new List<ShippingOptionModel>();
        Errors = new List<string>();
		_logger.LogDebug("<<< Exiting EstimateShippingResultModel()");
		_logger.LogDebug("EstimateShippingResultModel(): Errors → {Errors}", Errors);
		_logger.LogDebug("EstimateShippingResultModel(): ShippingOptions → {ShippingOptions}", ShippingOptions);
    }

    public IList<ShippingOptionModel> ShippingOptions { get; set; }

    public bool Success => !Errors.Any();

    public IList<string> Errors { get; set; }

    #region Nested Classes

    public partial record ShippingOptionModel : BaseNopModel
    {
        public string Name { get; set; }

        public string ShippingRateComputationMethodSystemName { get; set; }

        public string Description { get; set; }

        public string Price { get; set; }

        public decimal Rate { get; set; }

        public string DeliveryDateFormat { get; set; }

        public int DisplayOrder { get; set; }

        public bool Selected { get; set; }
    }

    #endregion
}