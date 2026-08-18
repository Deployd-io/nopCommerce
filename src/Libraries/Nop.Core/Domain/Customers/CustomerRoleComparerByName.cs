using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Domain.Customers;

/// <summary>
/// Custom comparer for the CustomerRole class by Name and SystemName
/// </summary>
public partial class CustomerRoleComparerByName : IEqualityComparer<CustomerRole>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Customer roles are equal if their names and system names are equal.
    /// </summary>
    public bool Equals(CustomerRole x, CustomerRole y)
    {
		_logger.LogDebug(">>> Entering Equals(x={X},y={Y})", x, y);
        //Check whether the compared objects reference the same data.
        if (ReferenceEquals(x, y))
            return true;

        //Check whether any of the compared objects is null.
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
		_logger.LogDebug("<<< Exiting Equals(x={X},y={Y})", x, y);
            return false;

        //Check whether the customer role properties are equal.
        return x.Name == y.Name && x.SystemName == y.SystemName;
    }

    public int GetHashCode(CustomerRole customerRole)
    {
		_logger.LogDebug(">>> Entering GetHashCode(customerRole={CustomerRole})", customerRole);
        //Check whether the object is null
        if (ReferenceEquals(customerRole, null))
            return 0;

        //Get hash code for the Name field if it is not null.
        var hashCustomerRoleName = customerRole.Name == null ? 0 : customerRole.Name.GetHashCode();
		_logger.LogDebug("GetHashCode(customerRole={CustomerRole}): hashCustomerRoleName → {HashCustomerRoleName}", customerRole, hashCustomerRoleName);

        //Get hash code for the SystemName field.
        var hashCustomerRoleSystemName = customerRole.SystemName == null ? 0 : customerRole.SystemName.GetHashCode();
		_logger.LogDebug("GetHashCode(customerRole={CustomerRole}): hashCustomerRoleSystemName → {HashCustomerRoleSystemName}", customerRole, hashCustomerRoleSystemName);

        //Calculate the hash code for the CustomerRole.
		_logger.LogDebug("<<< Exiting GetHashCode(customerRole={CustomerRole})", customerRole);
        return hashCustomerRoleName ^ hashCustomerRoleSystemName;
    }
}