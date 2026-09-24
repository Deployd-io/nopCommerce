using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Phohe number validator
/// </summary>
public partial class PhoneNumberPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CustomerSettings _customerSettings;

    public override string Name => "PhoneNumberPropertyValidator";

    /// <summary>
    /// Ctor
    /// </summary>
    public PhoneNumberPropertyValidator(CustomerSettings customerSettings)
    {
		_logger.LogError(">>> Entering PhoneNumberPropertyValidator(customerSettings={CustomerSettings})", customerSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("PhoneNumberPropertyValidator(customerSettings={CustomerSettings}): _customerSettings → {_customerSettings}", customerSettings, _customerSettings);
    _logger.LogWarning("<<< Exiting PhoneNumberPropertyValidator(customerSettings={CustomerSettings})", customerSettings);
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
		_logger.LogWarning("<<< Exiting IsValid(phoneNumber={PhoneNumber},customerSettings={CustomerSettings})", phoneNumber, customerSettings);
        return IsValid(value as string, _customerSettings);
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="customerSettings">Customer settings</param>
    /// <returns>Result</returns>
    public static bool IsValid(string phoneNumber, CustomerSettings customerSettings)
    {
        if (!customerSettings.PhoneNumberValidationEnabled || string.IsNullOrEmpty(customerSettings.PhoneNumberValidationRule))
            return true;

        if (string.IsNullOrEmpty(phoneNumber))
        {
            return !customerSettings.PhoneRequired;
        }

        return customerSettings.PhoneNumberValidationUseRegex
            ? Regex.IsMatch(phoneNumber, customerSettings.PhoneNumberValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            : phoneNumber.All(l => customerSettings.PhoneNumberValidationRule.Contains(l));
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        _logger.LogError(">>> Entering GetDefaultMessageTemplate(errorCode={ErrorCode})", errorCode);
        _logger.LogWarning("<<< Exiting GetDefaultMessageTemplate(errorCode={ErrorCode})", errorCode);
        return "Phone number is not valid";
    }
}