using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Credit card validator
/// </summary>
public partial class CreditCardPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override string Name => "CreditCardPropertyValidator";

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
		_logger.LogDebug(">>> Entering IsValid(context={Context},value={Value})", context, value);
        var ccValue = value as string;
        if (string.IsNullOrWhiteSpace(ccValue))
			_logger.LogDebug("IsValid(context={Context},value={Value}): ccValue → {CcValue}", context, value, ccValue);
            return false;

        ccValue = ccValue.Replace(" ", "");
        ccValue = ccValue.Replace("-", "");

        var checksum = 0;
        var evenDigit = false;
		_logger.LogDebug("IsValid(context={Context},value={Value}): evenDigit → {EvenDigit}", context, value, evenDigit);

        //http://www.beachnet.com/~hstiles/cardtype.html
        foreach (var digit in ccValue.Reverse())
        {
            if (!char.IsDigit(digit))
                return false;

            var digitValue = (digit - '0') * (evenDigit ? 2 : 1);
			_logger.LogDebug("IsValid(context={Context},value={Value}): digitValue → {DigitValue}", context, value, digitValue);
            evenDigit = !evenDigit;

            while (digitValue > 0)
            {
                checksum += digitValue % 10;
                digitValue /= 10;
            }
        }

		_logger.LogDebug("<<< Exiting IsValid(context={Context},value={Value})", context, value);
        return (checksum % 10) == 0;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Credit card number is not valid";
}