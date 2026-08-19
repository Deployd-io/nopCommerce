using Microsoft.Extensions.Logging;
﻿using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers;

/// <summary>
/// Customer registration request
/// </summary>
public partial class CustomerRegistrationRequest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="email">Email</param>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="passwordFormat">Password format</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="isApproved">Is approved</param>
    public CustomerRegistrationRequest(Customer customer, string email, string username,
        string password,
        PasswordFormat passwordFormat,
        int storeId,
        bool isApproved = true)
    {
		_logger.LogDebug(">>> Entering CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved})", customer, email, username, storeId, isApproved);
        Customer = customer;
		_logger.LogDebug("CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved}): Customer → {Customer1}", customer, email, username, storeId, isApproved, Customer);
        Email = email;
        Username = username;
        Password = password;
		_logger.LogDebug("CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved}): Email → {Email1}", customer, email, username, storeId, isApproved, Email);
		_logger.LogDebug("CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved}): Username → {Username1}", customer, email, username, storeId, isApproved, Username);
		_logger.LogWarning("CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved}): Password → {Password1}", customer, email, username, storeId, isApproved, Password);
        PasswordFormat = passwordFormat;
        StoreId = storeId;
        IsApproved = isApproved;
		_logger.LogDebug("CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved}): PasswordFormat → {PasswordFormat1}", customer, email, username, storeId, isApproved, PasswordFormat);
		_logger.LogDebug("CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved}): StoreId → {StoreId1}", customer, email, username, storeId, isApproved, StoreId);
		_logger.LogDebug("<<< Exiting CustomerRegistrationRequest(customer={Customer},email={Email},username={Username},password=<redacted>,passwordFormat=<redacted>,storeId={StoreId},isApproved={IsApproved})", customer, email, username, storeId, isApproved);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Password format
    /// </summary>
    public PasswordFormat PasswordFormat { get; set; }

    /// <summary>
    /// Store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Is approved
    /// </summary>
    public bool IsApproved { get; set; }
}