using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer;

public partial record MultiFactorAuthenticationModel : BaseNopModel
{
    public MultiFactorAuthenticationModel()
    {
		logger.LogDebug(">>> Entering MultiFactorAuthenticationModel()");
        Providers = new List<MultiFactorAuthenticationProviderModel>();
		logger.LogDebug("<<< Exiting MultiFactorAuthenticationModel()");
		logger.LogDebug("MultiFactorAuthenticationModel(): Providers → {Providers}", Providers);
    }

    [NopResourceDisplayName("Account.MultiFactorAuthentication.Fields.IsEnabled")]
    public bool IsEnabled { get; set; }

    public List<MultiFactorAuthenticationProviderModel> Providers { get; set; }

    public string Message { get; set; }

}