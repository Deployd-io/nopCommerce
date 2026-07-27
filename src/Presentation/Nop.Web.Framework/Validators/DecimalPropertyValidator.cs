using FluentValidation;
using FluentValidation.Validators;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Decimal validator
/// </summary>
public partial class DecimalPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    protected readonly decimal _maxValue;

    public override string Name => "DecimalPropertyValidator";

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="maxValue">Maximum value</param>
    public DecimalPropertyValidator(decimal maxValue)
    {
		logger.LogDebug(">>> Entering DecimalPropertyValidator(maxValue={MaxValue})", maxValue);
        _maxValue = maxValue;
		logger.LogDebug("<<< Exiting DecimalPropertyValidator(maxValue={MaxValue})", maxValue);
		logger.LogDebug("DecimalPropertyValidator(maxValue={MaxValue}): _maxValue → {_maxValue}", maxValue, _maxValue);
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
		logger.LogDebug(">>> Entering IsValid(context={Context},value={Value})", context, value);
        if (decimal.TryParse(value.ToString(), out var propertyValue))
		logger.LogDebug("<<< Exiting IsValid(context={Context},value={Value})", context, value);
            return Math.Round(propertyValue, 3) < _maxValue;

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Decimal value is out of range";
}