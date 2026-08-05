using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Customer;

public partial record MultiFactorAuthenticationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public MultiFactorAuthenticationModel()
    {
		_logger.LogDebug(">>> Entering MultiFactorAuthenticationModel()");
        Providers = new List<MultiFactorAuthenticationProviderModel>();
		_logger.LogDebug("<<< Exiting MultiFactorAuthenticationModel()");
		_logger.LogDebug("MultiFactorAuthenticationModel(): Providers → {Providers}", Providers);
    }

    [NopResourceDisplayName("Account.MultiFactorAuthentication.Fields.IsEnabled")]
    public bool IsEnabled { get; set; }

    public List<MultiFactorAuthenticationProviderModel> Providers { get; set; }

    public string Message { get; set; }

}