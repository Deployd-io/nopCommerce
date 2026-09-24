using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Username validator
/// </summary>
public partial class UsernamePropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CustomerSettings _customerSettings;

    public override string Name => "UsernamePropertyValidator";

    /// <summary>
    /// Ctor
    /// </summary>
    public UsernamePropertyValidator(CustomerSettings customerSettings)
    {
		_logger.LogError(">>> Entering UsernamePropertyValidator(customerSettings={CustomerSettings})", customerSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("UsernamePropertyValidator(customerSettings={CustomerSettings}): _customerSettings → {_customerSettings}", customerSettings, _customerSettings);
    _logger.LogWarning("<<< Exiting UsernamePropertyValidator(customerSettings={CustomerSettings})", customerSettings);
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
		_logger.LogError(">>> Entering IsValid(context={Context},value={Value})", context, value);
		_logger.LogWarning("<<< Exiting IsValid(context={Context},value={Value})", context, value);
		_logger.LogWarning("<<< Exiting IsValid(username={Username},customerSettings={CustomerSettings})", username, customerSettings);
        return IsValid(value as string, _customerSettings);
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="customerSettings">Customer settings</param>
    /// <returns>Result</returns>
    public static bool IsValid(string username, CustomerSettings customerSettings)
    {
        if (!customerSettings.UsernameValidationEnabled || string.IsNullOrEmpty(customerSettings.UsernameValidationRule))
            return true;

        if (string.IsNullOrEmpty(username))
            return false;

        return customerSettings.UsernameValidationUseRegex
            ? Regex.IsMatch(username, customerSettings.UsernameValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            : username.All(l => customerSettings.UsernameValidationRule.Contains(l));
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        _logger.LogError(">>> Entering GetDefaultMessageTemplate(errorCode={ErrorCode})", errorCode);
        _logger.LogWarning("<<< Exiting GetDefaultMessageTemplate(errorCode={ErrorCode})", errorCode);
        return "Username is not valid";
    }
}