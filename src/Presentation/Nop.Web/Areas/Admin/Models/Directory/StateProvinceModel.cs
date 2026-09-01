using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a state and province model
/// </summary>
public partial record StateProvinceModel : BaseNopEntityModel, ILocalizedModel<StateProvinceLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public StateProvinceModel()
    {
		_logger.LogDebug(">>> Entering StateProvinceModel()");
        Locales = new List<StateProvinceLocalizedModel>();
		_logger.LogDebug("StateProvinceModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    public int CountryId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Abbreviation")]
    public string Abbreviation { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<StateProvinceLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record StateProvinceLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
    public string Name { get; set; }
}