using Nop.Services.Authentication.External;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the external authentication method model factory implementation
/// </summary>
public partial class ExternalAuthenticationMethodModelFactory : IExternalAuthenticationMethodModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAuthenticationPluginManager _authenticationPluginManager;

    #endregion

    #region Ctor

    public ExternalAuthenticationMethodModelFactory(IAuthenticationPluginManager authenticationPluginManager)
    {
		_logger.LogError(">>> Entering ExternalAuthenticationMethodModelFactory(authenticationPluginManager={AuthenticationPluginManager})", authenticationPluginManager);
        _authenticationPluginManager = authenticationPluginManager;
		_logger.LogDebug("ExternalAuthenticationMethodModelFactory(authenticationPluginManager={AuthenticationPluginManager}): _authenticationPluginManager → {_authenticationPluginManager}", authenticationPluginManager, _authenticationPluginManager);
    _logger.LogWarning("<<< Exiting ExternalAuthenticationMethodModelFactory(authenticationPluginManager={AuthenticationPluginManager})", authenticationPluginManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare external authentication method search model
    /// </summary>
    /// <param name="searchModel">External authentication method search model</param>
    /// <returns>External authentication method search model</returns>
    public virtual ExternalAuthenticationMethodSearchModel PrepareExternalAuthenticationMethodSearchModel(
        ExternalAuthenticationMethodSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareExternalAuthenticationMethodSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareExternalAuthenticationMethodSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged external authentication method list model
    /// </summary>
    /// <param name="searchModel">External authentication method search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the external authentication method list model
    /// </returns>
    public virtual async Task<ExternalAuthenticationMethodListModel> PrepareExternalAuthenticationMethodListModelAsync(
        ExternalAuthenticationMethodSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareExternalAuthenticationMethodListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get external authentication methods
        var externalAuthenticationMethods = (await _authenticationPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareExternalAuthenticationMethodListModelAsync(searchModel={SearchModel}): externalAuthenticationMethods → {ExternalAuthenticationMethods}", searchModel, externalAuthenticationMethods);

        //prepare grid model
        var model = new ExternalAuthenticationMethodListModel().PrepareToGrid(searchModel, externalAuthenticationMethods, () =>
        {
            //fill in model values from the entity
            return externalAuthenticationMethods.Select(method =>
            {
                //fill in model values from the entity
                var externalAuthenticationMethodModel = method.ToPluginModel<ExternalAuthenticationMethodModel>();
				_logger.LogError("PrepareExternalAuthenticationMethodListModelAsync(searchModel={SearchModel}): externalAuthenticationMethodModel → {ExternalAuthenticationMethodModel}", searchModel, externalAuthenticationMethodModel);

                //fill in additional values (not existing in the entity)
                externalAuthenticationMethodModel.IsActive = _authenticationPluginManager.IsPluginActive(method);
                externalAuthenticationMethodModel.ConfigurationUrl = method.GetConfigurationPageUrl();

                return externalAuthenticationMethodModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareExternalAuthenticationMethodListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}