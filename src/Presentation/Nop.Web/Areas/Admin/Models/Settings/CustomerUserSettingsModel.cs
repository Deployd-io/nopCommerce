using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a customer user settings model
/// </summary>
public partial record CustomerUserSettingsModel : BaseNopModel, ISettingsModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerUserSettingsModel()
    {
		_logger.LogDebug(">>> Entering CustomerUserSettingsModel()");
        CustomerSettings = new CustomerSettingsModel();
        AddressSettings = new AddressSettingsModel();
		_logger.LogDebug("CustomerUserSettingsModel(): AddressSettings → {AddressSettings}", AddressSettings);
		_logger.LogDebug("CustomerUserSettingsModel(): CustomerSettings → {CustomerSettings}", CustomerSettings);
        DateTimeSettings = new DateTimeSettingsModel();
		_logger.LogDebug("CustomerUserSettingsModel(): DateTimeSettings → {DateTimeSettings}", DateTimeSettings);
        ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
		_logger.LogDebug("CustomerUserSettingsModel(): ExternalAuthenticationSettings → {ExternalAuthenticationSettings}", ExternalAuthenticationSettings);
        MultiFactorAuthenticationSettings = new MultiFactorAuthenticationSettingsModel();
		_logger.LogDebug("CustomerUserSettingsModel(): MultiFactorAuthenticationSettings → {MultiFactorAuthenticationSettings}", MultiFactorAuthenticationSettings);
        CustomerAttributeSearchModel = new CustomerAttributeSearchModel();
		_logger.LogDebug("CustomerUserSettingsModel(): CustomerAttributeSearchModel → {CustomerAttributeSearchModel}", CustomerAttributeSearchModel);
        AddressAttributeSearchModel = new AddressAttributeSearchModel();
		_logger.LogDebug("CustomerUserSettingsModel(): AddressAttributeSearchModel → {AddressAttributeSearchModel}", AddressAttributeSearchModel);
		_logger.LogDebug("<<< Exiting CustomerUserSettingsModel()");
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