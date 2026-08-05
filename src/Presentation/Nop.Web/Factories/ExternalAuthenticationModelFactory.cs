using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the external authentication model factory
/// </summary>
public partial class ExternalAuthenticationModelFactory : IExternalAuthenticationModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ExternalAuthenticationModelFactory(IAuthenticationPluginManager authenticationPluginManager,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _authenticationPluginManager = authenticationPluginManager;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the external authentication method model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the external authentication method model
    /// </returns>
    public virtual async Task<List<ExternalAuthenticationMethodModel>> PrepareExternalMethodsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareExternalMethodsModelAsync()");
        var store = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("PrepareExternalMethodsModelAsync(): store → {Store}", store);
		_logger.LogDebug("<<< Exiting PrepareExternalMethodsModelAsync()");
        return (await _authenticationPluginManager
                .LoadActivePluginsAsync(await _workContext.GetCurrentCustomerAsync(), store.Id))
            .Select(authenticationMethod => new ExternalAuthenticationMethodModel
            {
                ViewComponent = authenticationMethod.GetPublicViewComponent()
            }).Where(p => p.ViewComponent != null)
            .ToList();
    }

    #endregion
}