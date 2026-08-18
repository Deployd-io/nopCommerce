using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Plugins;

/// <summary>
/// Represents a plugin model
/// </summary>
public partial record PluginModel : BaseNopModel, IAclSupportedModel, ILocalizedModel<PluginLocalizedModel>, IPluginModel, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PluginModel()
    {
		_logger.LogDebug(">>> Entering PluginModel()");
        Locales = new List<PluginLocalizedModel>();

		_logger.LogDebug("PluginModel(): Locales → {Locales}", Locales);
        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("PluginModel(): AvailableStores → {AvailableStores}", AvailableStores);
		_logger.LogDebug("PluginModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        SelectedCustomerRoleIds = new List<int>();
		_logger.LogDebug("PluginModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("PluginModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
		_logger.LogDebug("<<< Exiting PluginModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
    public string Group { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
    public string FriendlyName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
    public string SystemName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
    public string Version { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
    public string Author { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
    public string ConfigurationUrl { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
    public bool Installed { get; set; }

    public string Description { get; set; }

    public bool CanChangeEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.IsEnabled")]
    public bool IsEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Logo")]
    public string LogoUrl { get; set; }

    public IList<PluginLocalizedModel> Locales { get; set; }

    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public bool IsActive { get; set; }

    #endregion
}

public partial record PluginLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
    public string FriendlyName { get; set; }
}