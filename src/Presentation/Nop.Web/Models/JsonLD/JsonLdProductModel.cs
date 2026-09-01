using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdProductModel : JsonLdModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public JsonLdProductModel()
    {
		_logger.LogDebug(">>> Entering JsonLdProductModel()");
        Brand = new List<JsonLdBrandModel>();
		_logger.LogDebug("JsonLdProductModel(): Brand → {Brand}", Brand);
        Review = new List<JsonLdReviewModel>();
		_logger.LogDebug("JsonLdProductModel(): Review → {Review}", Review);
        HasVariant = new List<JsonLdProductModel>();
    }

    #endregion

    #region Properties

    [JsonProperty("@context")]
    public static string Context => "https://schema.org";

    [JsonProperty("@type")]
    public static string Type => "Product";

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("sku")]
    public string Sku { get; set; }

    [JsonProperty("gtin")]
    public string Gtin { get; set; }

    [JsonProperty("mpn")]
    public string Mpn { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }

    [JsonProperty("brand")]
    public IList<JsonLdBrandModel> Brand { get; set; }

    [JsonProperty("offers")]
    public JsonLdOfferModel Offer { get; set; }

    [JsonProperty("aggregateRating")]
    public JsonLdAggregateRatingModel AggregateRating { get; set; }

    [JsonProperty("review")]
    public IList<JsonLdReviewModel> Review { get; set; }

    [JsonProperty("hasVariant")]
    public IList<JsonLdProductModel> HasVariant { get; set; }

    #endregion
}