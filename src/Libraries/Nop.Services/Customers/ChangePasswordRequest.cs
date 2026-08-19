using Microsoft.Extensions.Logging;
﻿using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers;

/// <summary>
/// Change password request
/// </summary>
public partial class ChangePasswordRequest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// A value indicating whether we should validate request
    /// </summary>
    public bool ValidateRequest { get; set; }

    /// <summary>
    /// Password format
    /// </summary>
    public PasswordFormat NewPasswordFormat { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    public string NewPassword { get; set; }

    /// <summary>
    /// Old password
    /// </summary>
    public string OldPassword { get; set; }

    /// <summary>
    /// Hashed password format (e.g. SHA1, SHA512)
    /// </summary>
    public string HashedPasswordFormat { get; set; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="validateRequest">A value indicating whether we should validate request</param>
    /// <param name="newPasswordFormat">Password format</param>
    /// <param name="newPassword">New password</param>
    /// <param name="oldPassword">Old password</param>
    /// <param name="hashedPasswordFormat">Hashed password format</param>
    public ChangePasswordRequest(string email, bool validateRequest,
        PasswordFormat newPasswordFormat, string newPassword, string oldPassword = "",
        string hashedPasswordFormat = null)
    {
		_logger.LogDebug(">>> Entering ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>)", email, validateRequest);
        Email = email;
        ValidateRequest = validateRequest;
		_logger.LogDebug("ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>): Email → {Email1}", email, validateRequest, Email);
		_logger.LogDebug("ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>): ValidateRequest → {ValidateRequest1}", email, validateRequest, ValidateRequest);
        NewPasswordFormat = newPasswordFormat;
        NewPassword = newPassword;
		_logger.LogDebug("ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>): NewPasswordFormat → {NewPasswordFormat1}", email, validateRequest, NewPasswordFormat);
		_logger.LogDebug("ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>): NewPassword → {NewPassword1}", email, validateRequest, NewPassword);
        OldPassword = oldPassword;
        HashedPasswordFormat = hashedPasswordFormat;
		_logger.LogDebug("<<< Exiting ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>)", email, validateRequest);
		_logger.LogDebug("ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>): OldPassword → {OldPassword1}", email, validateRequest, OldPassword);
    }
}
_logger.LogDebug("ChangePasswordRequest(email={Email},validateRequest={ValidateRequest},newPasswordFormat=<redacted>,newPassword=<redacted>,oldPassword=<redacted>,hashedPasswordFormat=<redacted>): HashedPasswordFormat → {HashedPasswordFormat1}", email, validateRequest, HashedPasswordFormat);