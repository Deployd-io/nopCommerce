using Microsoft.Extensions.Logging;
using FluentValidation.AspNetCore;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Represents attribute that used to mark model for the forced validation. 
/// Without this attribute, the model passed in the parameter will not be validated. It's used to prevent auto-validation of child models.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed partial class ValidateAttribute : CustomizeValidatorAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ValidateAttribute()
    {
		_logger.LogError(">>> Entering ValidateAttribute()");
        //specify rule set
		_logger.LogWarning("<<< Exiting ValidateAttribute()");
        RuleSet = NopValidationDefaults.ValidationRuleSet;
		_logger.LogDebug("ValidateAttribute(): RuleSet → {RuleSet}", RuleSet);
    }
}