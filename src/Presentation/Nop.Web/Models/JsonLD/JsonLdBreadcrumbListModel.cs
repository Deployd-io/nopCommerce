using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdBreadcrumbListModel : JsonLdModel
{
    #region Ctor

    public JsonLdBreadcrumbListModel()
    {
		logger.LogDebug(">>> Entering JsonLdBreadcrumbListModel()");
        ItemListElement = new List<JsonLdBreadcrumbListItemModel>();
		logger.LogDebug("<<< Exiting JsonLdBreadcrumbListModel()");
		logger.LogDebug("JsonLdBreadcrumbListModel(): ItemListElement → {ItemListElement}", ItemListElement);
    }

    #endregion

    #region Properties

    [JsonProperty("@context")]
    public static string Context => "https://schema.org";

    [JsonProperty("@type")]
    public static string Type => "BreadcrumbList";

    [JsonProperty("itemListElement")]
    public IList<JsonLdBreadcrumbListItemModel> ItemListElement { get; set; }

    #endregion
}