using System.Text;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Validators;

public partial class PasswordPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>, IRegularExpressionValidator
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public PasswordPropertyValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
    {
		_logger.LogDebug(">>> Entering PasswordPropertyValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
        _localizationService = localizationService;
        _customerSettings = customerSettings;
		_logger.LogDebug("PasswordPropertyValidator(localizationService={LocalizationService},customerSettings={CustomerSettings}): _localizationService → {_localizationService}", localizationService, customerSettings, _localizationService);
		_logger.LogDebug("PasswordPropertyValidator(localizationService={LocalizationService},customerSettings={CustomerSettings}): _customerSettings → {_customerSettings}", localizationService, customerSettings, _customerSettings);
		_logger.LogDebug("<<< Exiting PasswordPropertyValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
    }

    #endregion

    #region Utilities

    private async Task<string> GetValidationMessageAsync()
    {
		_logger.LogDebug(">>> Entering GetValidationMessageAsync()");
        var regExpError = new StringBuilder()
            .AppendJoin(' ',
                await _localizationService.GetResourceAsync("Validation.Password.Rule"),
                string.Format(await _localizationService.GetResourceAsync("Validation.Password.LengthValidation"), _customerSettings.PasswordMinLength, _customerSettings.PasswordMaxLength));

        if (_customerSettings.PasswordRequireUppercase)
            regExpError.AppendFormat(", {0}", await _localizationService.GetResourceAsync("Validation.Password.RequireUppercase"));

        if (_customerSettings.PasswordRequireLowercase)
            regExpError.AppendFormat(", {0}", await _localizationService.GetResourceAsync("Validation.Password.RequireLowercase"));

        if (_customerSettings.PasswordRequireDigit)
            regExpError.AppendFormat(", {0}", await _localizationService.GetResourceAsync("Validation.Password.RequireDigit"));

        if (_customerSettings.PasswordRequireNonAlphanumeric)
            regExpError.AppendFormat(", {0}", await _localizationService.GetResourceAsync("Validation.Password.RequireNonAlphanumeric"));

		_logger.LogDebug("<<< Exiting GetValidationMessageAsync()");
        return regExpError.ToString();
    }

    #endregion

    /// <summary>
    /// Validates a specific property value
    /// </summary>
    /// <param name="context">The validation context. The parent object can be obtained from here</param>
    /// <param name="value">The current property value to validate</param>
    /// <returns>True if valid, otherwise false</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
		_logger.LogDebug(">>> Entering IsValid(context={Context},value={Value})", context, value);
        var password = value as string;

        if (string.IsNullOrEmpty(password))
        {
            context.AddFailure(_localizationService.GetResourceAsync("Validation.Password.IsNotEmpty").Result);
            return false;
        }

        if (!Regex.IsMatch(password, Expression))
        {
            context.AddFailure(GetDefaultMessageTemplate(string.Empty));
		_logger.LogDebug("<<< Exiting IsValid(context={Context},value={Value})", context, value);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the default error message template for this validator, when not overridden
    /// </summary>
    /// <param name="errorCode">The currently configured error code for the validator</param>
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
		_logger.LogDebug(">>> Entering GetDefaultMessageTemplate(errorCode={ErrorCode})", errorCode);
		_logger.LogDebug("<<< Exiting GetDefaultMessageTemplate(errorCode={ErrorCode})", errorCode);
        return GetValidationMessageAsync().Result;
    }

    #region Properties

    /// <summary>
    /// Gets regular expression for client side (pattern source: https://emailregex.com/)
    /// </summary>
    public string Expression
    {
        get
        {
            var regExp = new StringBuilder("^");

            // upper case (A-Z),
            if (_customerSettings.PasswordRequireUppercase)
                regExp.Append("(?=.*?[A-Z])");

            // lower case (a-z),
            if (_customerSettings.PasswordRequireLowercase)
                regExp.Append("(?=.*?[a-z])");

            // number (0-9)
            if (_customerSettings.PasswordRequireDigit)
                regExp.Append("(?=.*?[0-9])");

            // and special character (e.g. !@#$%^&*-)
            if (_customerSettings.PasswordRequireNonAlphanumeric)
                regExp.Append("(?=.*?[#?!@$%^&*-])");

            // password length must be in the range
            regExp.Append($".{{{_customerSettings.PasswordMinLength},{_customerSettings.PasswordMaxLength}}}$");

            return regExp.ToString();
        }
    }

    public override string Name => "PasswordPropertyValidator";

    #endregion
}