using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Decimal validator
/// </summary>
public partial class DecimalPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly decimal _maxValue;

    public override string Name => "DecimalPropertyValidator";

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="maxValue">Maximum value</param>
    public DecimalPropertyValidator(decimal maxValue)
    {
		_logger.LogDebug(">>> Entering DecimalPropertyValidator(maxValue={MaxValue})", maxValue);
        _maxValue = maxValue;
		_logger.LogDebug("DecimalPropertyValidator(maxValue={MaxValue}): _maxValue → {_maxValue}", maxValue, _maxValue);
		_logger.LogDebug("<<< Exiting DecimalPropertyValidator(maxValue={MaxValue})", maxValue);
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
		_logger.LogDebug(">>> Entering IsValid(context={Context},value={Value})", context, value);
        if (decimal.TryParse(value.ToString(), out var propertyValue))
		_logger.LogDebug("<<< Exiting IsValid(context={Context},value={Value})", context, value);
            return Math.Round(propertyValue, 3) < _maxValue;

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Decimal value is out of range";
}