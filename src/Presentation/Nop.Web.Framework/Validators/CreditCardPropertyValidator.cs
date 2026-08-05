using FluentValidation;
using FluentValidation.Validators;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Credit card validator
/// </summary>
public partial class CreditCardPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    public override string Name => "CreditCardPropertyValidator";

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
		logger.LogDebug(">>> Entering IsValid(context={Context},value={Value})", context, value);
        var ccValue = value as string;
        if (string.IsNullOrWhiteSpace(ccValue))
			logger.LogDebug("IsValid(context={Context},value={Value}): ccValue → {CcValue}", context, value, ccValue);
            return false;

        ccValue = ccValue.Replace(" ", "");
        ccValue = ccValue.Replace("-", "");

        var checksum = 0;
        var evenDigit = false;

		logger.LogDebug("IsValid(context={Context},value={Value}): evenDigit → {EvenDigit}", context, value, evenDigit);
        //http://www.beachnet.com/~hstiles/cardtype.html
        foreach (var digit in ccValue.Reverse())
        {
            if (!char.IsDigit(digit))
                return false;

            var digitValue = (digit - '0') * (evenDigit ? 2 : 1);
            evenDigit = !evenDigit;
			logger.LogDebug("IsValid(context={Context},value={Value}): digitValue → {DigitValue}", context, value, digitValue);

            while (digitValue > 0)
            {
                checksum += digitValue % 10;
                digitValue /= 10;
            }
        }

		logger.LogDebug("<<< Exiting IsValid(context={Context},value={Value})", context, value);
        return (checksum % 10) == 0;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Credit card number is not valid";
}