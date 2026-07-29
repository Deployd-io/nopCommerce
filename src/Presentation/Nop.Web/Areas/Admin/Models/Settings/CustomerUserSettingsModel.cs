using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a customer user settings model
/// </summary>
public partial record CustomerUserSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public CustomerUserSettingsModel()
    {
		logger.LogDebug(">>> Entering CustomerUserSettingsModel()");
        CustomerSettings = new CustomerSettingsModel();
        AddressSettings = new AddressSettingsModel();
		logger.LogDebug("CustomerUserSettingsModel(): CustomerSettings → {CustomerSettings}", CustomerSettings);
		logger.LogDebug("CustomerUserSettingsModel(): AddressSettings → {AddressSettings}", AddressSettings);
        DateTimeSettings = new DateTimeSettingsModel();
        ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
		logger.LogDebug("CustomerUserSettingsModel(): DateTimeSettings → {DateTimeSettings}", DateTimeSettings);
		logger.LogDebug("CustomerUserSettingsModel(): ExternalAuthenticationSettings → {ExternalAuthenticationSettings}", ExternalAuthenticationSettings);
        MultiFactorAuthenticationSettings = new MultiFactorAuthenticationSettingsModel();
        CustomerAttributeSearchModel = new CustomerAttributeSearchModel();
		logger.LogDebug("CustomerUserSettingsModel(): MultiFactorAuthenticationSettings → {MultiFactorAuthenticationSettings}", MultiFactorAuthenticationSettings);
		logger.LogDebug("CustomerUserSettingsModel(): CustomerAttributeSearchModel → {CustomerAttributeSearchModel}", CustomerAttributeSearchModel);
        AddressAttributeSearchModel = new AddressAttributeSearchModel();
		logger.LogDebug("<<< Exiting CustomerUserSettingsModel()");
		logger.LogDebug("CustomerUserSettingsModel(): AddressAttributeSearchModel → {AddressAttributeSearchModel}", AddressAttributeSearchModel);
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    public CustomerSettingsModel CustomerSettings { get; set; }

    public AddressSettingsModel AddressSettings { get; set; }

    public DateTimeSettingsModel DateTimeSettings { get; set; }

    public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

    public MultiFactorAuthenticationSettingsModel MultiFactorAuthenticationSettings { get; set; }

    public CustomerAttributeSearchModel CustomerAttributeSearchModel { get; set; }

    public AddressAttributeSearchModel AddressAttributeSearchModel { get; set; }

    #endregion
}