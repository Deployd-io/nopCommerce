using Microsoft.Extensions.Logging;
namespace Nop.Services.Security;

/// <summary>
/// Permission config
/// </summary>
public partial class PermissionConfig
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PermissionConfig(string name, string systemName, string category, params string[] defaultCustomerRoles)
    {
		_logger.LogError(">>> Entering PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles})", name, systemName, category, defaultCustomerRoles);
        Name = name;
		_logger.LogWarning("<<< Exiting PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles})", name, systemName, category, defaultCustomerRoles);
        SystemName = systemName;
		_logger.LogDebug("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): Name → {Name1}", name, systemName, category, defaultCustomerRoles, Name);
		_logger.LogDebug("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): SystemName → {SystemName1}", name, systemName, category, defaultCustomerRoles, SystemName);
		_logger.LogError("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): Name → {Name1}", name, systemName, category, defaultCustomerRoles, Name);
        Category = category;

		_logger.LogDebug("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): Category → {Category1}", name, systemName, category, defaultCustomerRoles, Category);
        DefaultCustomerRoles.AddRange(defaultCustomerRoles);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the permission name
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets or sets the permission system name
    /// </summary>
    public string SystemName { get; protected set; }

    /// <summary>
    /// Gets or sets the permission category
    /// </summary>
    public string Category { get; protected set; }

    /// <summary>
    /// Gets default list of customer roles system name
    /// </summary>
    public List<string> DefaultCustomerRoles { get; } = new();

    #endregion
}