using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the model factory which supports access control list (ACL)
/// </summary>
public partial class AclSupportedModelFactory : IAclSupportedModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;

    #endregion

    #region Ctor

    public AclSupportedModelFactory(IAclService aclService,
        ICustomerService customerService)
    {
		_logger.LogDebug(">>> Entering AclSupportedModelFactory(aclService={AclService},customerService={CustomerService})", aclService, customerService);
        _aclService = aclService;
        _customerService = customerService;
		_logger.LogDebug("AclSupportedModelFactory(aclService={AclService},customerService={CustomerService}): _customerService → {_customerService}", aclService, customerService, _customerService);
		_logger.LogDebug("AclSupportedModelFactory(aclService={AclService},customerService={CustomerService}): _aclService → {_aclService}", aclService, customerService, _aclService);
		_logger.LogDebug("<<< Exiting AclSupportedModelFactory(aclService={AclService},customerService={CustomerService})", aclService, customerService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare selected and all available customer roles for the passed model
    /// </summary>
    /// <typeparam name="TModel">ACL supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareModelCustomerRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel
    {
		_logger.LogDebug(">>> Entering PrepareModelCustomerRolesAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
		_logger.LogDebug("PrepareModelCustomerRolesAsync(model={Model}): availableRoles → {AvailableRoles}", model, availableRoles);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();
		_logger.LogDebug("<<< Exiting PrepareModelCustomerRolesAsync(model={Model})", model);
    }

    /// <summary>
    /// Prepare selected and all available customer roles for the passed model by ACL mappings
    /// </summary>
    /// <typeparam name="TModel">ACL supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <param name="entityName">Entity name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareModelCustomerRolesAsync<TModel>(TModel model, string entityName)
        where TModel : BaseNopEntityModel, IAclSupportedModel
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare customer roles with granted access
        model.SelectedCustomerRoleIds = (await _aclService.GetCustomerRoleIdsWithAccessAsync(model.Id, entityName)).ToList();

        await PrepareModelCustomerRolesAsync(model);
    }

    #endregion
}