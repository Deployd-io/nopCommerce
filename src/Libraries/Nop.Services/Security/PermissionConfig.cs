namespace Nop.Services.Security;

/// <summary>
/// Permission config
/// </summary>
public partial class PermissionConfig
{
    #region Ctor

    public PermissionConfig(string name, string systemName, string category, params string[] defaultCustomerRoles)
    {
		logger.LogDebug(">>> Entering PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles})", name, systemName, category, defaultCustomerRoles);
        Name = name;
        SystemName = systemName;
		logger.LogDebug("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): Name → {Name1}", name, systemName, category, defaultCustomerRoles, Name);
		logger.LogDebug("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): SystemName → {SystemName1}", name, systemName, category, defaultCustomerRoles, SystemName);
        Category = category;

		logger.LogDebug("PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles}): Category → {Category1}", name, systemName, category, defaultCustomerRoles, Category);
        DefaultCustomerRoles.AddRange(defaultCustomerRoles);
		logger.LogDebug("<<< Exiting PermissionConfig(name={Name},systemName={SystemName},category={Category},defaultCustomerRoles={DefaultCustomerRoles})", name, systemName, category, defaultCustomerRoles);
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