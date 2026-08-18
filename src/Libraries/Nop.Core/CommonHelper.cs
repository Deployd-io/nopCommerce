using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Nop.Core.Infrastructure;

namespace Nop.Core;

/// <summary>
/// Represents a common helper
/// </summary>
public partial class CommonHelper
{
    #region Fields

    //we use regular expression based on RFC 5322 Official Standard (see https://emailregex.com/)
    private const string EMAIL_EXPRESSION = @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";

    #endregion

    #region Methods

    /// <summary>
    /// Get email validation regex
    /// </summary>
    /// <returns>Regular expression</returns>
    [GeneratedRegex(EMAIL_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture, "en-US")]
    public static partial Regex GetEmailRegex();

    /// <summary>
    /// Ensures the subscriber email or throw.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <returns></returns>
    public static string EnsureSubscriberEmailOrThrow(string email)
    {
		_logger.LogDebug(">>> Entering EnsureSubscriberEmailOrThrow(email={Email})", email);
        var output = EnsureNotNull(email);
        output = output.Trim();
		_logger.LogDebug("EnsureSubscriberEmailOrThrow(email={Email}): output → {Output}", email, output);
        output = EnsureMaximumLength(output, 255);

        if (!IsValidEmail(output))
			_logger.LogWarning("Throwing NopException from EnsureSubscriberEmailOrThrow(email={Email})", email);
		_logger.LogDebug("<<< Exiting EnsureSubscriberEmailOrThrow(email={Email})", email);
            throw new NopException("Email is not valid.");

        return output;
    }

    /// <summary>
    /// Verifies that a string is in valid email format
    /// </summary>
    /// <param name="email">Email to verify</param>
    /// <returns>true if the string is a valid email address and false if it's not</returns>
    public static bool IsValidEmail(string email)
    {
		_logger.LogDebug(">>> Entering IsValidEmail(email={Email})", email);
        if (string.IsNullOrEmpty(email))
            return false;

        email = email.Trim();
		_logger.LogDebug("IsValidEmail(email={Email}): email → {Email1}", email, email);

		_logger.LogDebug("<<< Exiting IsValidEmail(email={Email})", email);
        return GetEmailRegex().IsMatch(email);
    }

    /// <summary>
    /// Verifies that string is an valid IP-Address
    /// </summary>
    /// <param name="ipAddress">IPAddress to verify</param>
    /// <returns>true if the string is a valid IpAddress and false if it's not</returns>
    public static bool IsValidIpAddress(string ipAddress)
    {
		_logger.LogDebug(">>> Entering IsValidIpAddress(ipAddress={IpAddress})", ipAddress);
		_logger.LogDebug("<<< Exiting IsValidIpAddress(ipAddress={IpAddress})", ipAddress);
        return IPAddress.TryParse(ipAddress, out var _);
    }

    /// <summary>
    /// Generate random digit code
    /// </summary>
    /// <param name="length">Length</param>
    /// <returns>Result string</returns>
    public static string GenerateRandomDigitCode(int length)
    {
		_logger.LogDebug(">>> Entering GenerateRandomDigitCode(length={Length})", length);
        using var random = new SecureRandomNumberGenerator();
        var str = string.Empty;
		_logger.LogDebug("GenerateRandomDigitCode(length={Length}): str → {Str}", length, str);
		_logger.LogDebug("GenerateRandomDigitCode(length={Length}): random → {Random}", length, random);
        for (var i = 0; i < length; i++)
            str = string.Concat(str, random.Next(10).ToString());
		_logger.LogDebug("<<< Exiting GenerateRandomDigitCode(length={Length})", length);
        return str;
    }

    /// <summary>
    /// Returns an random integer number within a specified rage
    /// </summary>
    /// <param name="min">Minimum number</param>
    /// <param name="max">Maximum number</param>
    /// <returns>Result</returns>
    public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GenerateRandomInteger(min={Min},max={Max})", min, max);
        using var random = new SecureRandomNumberGenerator();
		using Microsoft.Extensions.Logging;
		_logger.LogDebug("GenerateRandomInteger(min={Min},max={Max}): random → {Random}", min, max, random);
		_logger.LogDebug("<<< Exiting GenerateRandomInteger(min={Min},max={Max})", min, max);
        return random.Next(min, max);
    }

    /// <summary>
    /// Ensure that a string doesn't exceed maximum allowed length
    /// </summary>
    /// <param name="str">Input string</param>
    /// <param name="maxLength">Maximum length</param>
    /// <param name="postfix">A string to add to the end if the original string was shorten</param>
    /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
    public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        if (string.IsNullOrEmpty(str))
			_logger.LogDebug(">>> Entering EnsureMaximumLength(str={Str},maxLength={MaxLength},postfix={Postfix})", str, maxLength, postfix);
			_logger.LogDebug("<<< Exiting EnsureMaximumLength(str={Str},maxLength={MaxLength},postfix={Postfix})", str, maxLength, postfix);
            return str;

        if (str.Length <= maxLength)
            return str;

        var pLen = postfix?.Length ?? 0;

        var result = str[0..(maxLength - pLen)];
		_logger.LogDebug("EnsureMaximumLength(str={Str},maxLength={MaxLength},postfix={Postfix}): result → {Result}", str, maxLength, postfix, result);
        if (!string.IsNullOrEmpty(postfix))
            result += postfix;

        return result;
    }

    /// <summary>
    /// Ensures that a string only contains numeric values
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
    public static string EnsureNumericOnly(string str)
    {
		_logger.LogDebug(">>> Entering EnsureNumericOnly(str={Str})", str);
		_logger.LogDebug("<<< Exiting EnsureNumericOnly(str={Str})", str);
        return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Ensure that a string is not null
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Result</returns>
    public static string EnsureNotNull(string str)
    {
		_logger.LogDebug(">>> Entering EnsureNotNull(str={Str})", str);
		_logger.LogDebug("<<< Exiting EnsureNotNull(str={Str})", str);
        return str ?? string.Empty;
    }

    /// <summary>
    /// Indicates whether the specified strings are null or empty strings
    /// </summary>
    /// <param name="stringsToValidate">Array of strings to validate</param>
    /// <returns>Boolean</returns>
    public static bool AreNullOrEmpty(params string[] stringsToValidate)
    {
		_logger.LogDebug(">>> Entering AreNullOrEmpty(stringsToValidate={StringsToValidate})", stringsToValidate);
		_logger.LogDebug("<<< Exiting AreNullOrEmpty(stringsToValidate={StringsToValidate})", stringsToValidate);
        return stringsToValidate.Any(string.IsNullOrEmpty);
    }

    /// <summary>
    /// Compare two arrays
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="a1">Array 1</param>
    /// <param name="a2">Array 2</param>
    /// <returns>Result</returns>
    public static bool ArraysEqual<T>(T[] a1, T[] a2)
    {
		_logger.LogDebug(">>> Entering ArraysEqual(a1={A1},a2={A2})", a1, a2);
        //also see Enumerable.SequenceEqual(a1, a2);
        if (ReferenceEquals(a1, a2))
            return true;

        if (a1 == null || a2 == null)
            return false;

        if (a1.Length != a2.Length)
            return false;

        var comparer = EqualityComparer<T>.Default;
		_logger.LogDebug("<<< Exiting ArraysEqual(a1={A1},a2={A2})", a1, a2);
        return !a1.Where((t, i) => !comparer.Equals(t, a2[i])).Any();
    }

    /// <summary>
    /// Sets a property on an object to a value.
    /// </summary>
    /// <param name="instance">The object whose property to set.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to set the property to.</param>
    public static void SetProperty(object instance, string propertyName, object value)
    {
		_logger.LogDebug(">>> Entering SetProperty(instance={Instance},propertyName={PropertyName},value={Value})", instance, propertyName, value);
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(propertyName);

        var instanceType = instance.GetType();
		_logger.LogDebug("SetProperty(instance={Instance},propertyName={PropertyName},value={Value}): instanceType → {InstanceType}", instance, propertyName, value, instanceType);
        var pi = instanceType.GetProperty(propertyName)
            ?? throw new NopException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);

        if (!pi.CanWrite)
			_logger.LogDebug("<<< Exiting SetProperty(instance={Instance},propertyName={PropertyName},value={Value})", instance, propertyName, value);
			_logger.LogWarning("Throwing NopException from SetProperty(instance={Instance},propertyName={PropertyName},value={Value})", instance, propertyName, value);
            throw new NopException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
        if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
            value = To(value, pi.PropertyType);
			_logger.LogDebug("SetProperty(instance={Instance},propertyName={PropertyName},value={Value}): value → {Value1}", instance, propertyName, value, value);
        pi.SetValue(instance, value, Array.Empty<object>());
    }

    /// <summary>
    /// Converts a value to a destination type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="destinationType">The type to convert the value to.</param>
    /// <returns>The converted value.</returns>
    public static object To(object value, Type destinationType)
    {
		_logger.LogDebug(">>> Entering To(value={Value},destinationType={DestinationType})", value, destinationType);
		_logger.LogDebug("<<< Exiting To(value={Value},destinationType={DestinationType})", value, destinationType);
		_logger.LogDebug("<<< Exiting To(value={Value})", value);
        return To(value, destinationType, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a value to a destination type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="destinationType">The type to convert the value to.</param>
    /// <param name="culture">Culture</param>
    /// <returns>The converted value.</returns>
    public static object To(object value, Type destinationType, CultureInfo culture)
    {
        if (value == null)
            return null;

        var sourceType = value.GetType();

        var destinationConverter = TypeDescriptor.GetConverter(destinationType);
        if (destinationConverter.CanConvertFrom(value.GetType()))
            return destinationConverter.ConvertFrom(null, culture, value);

        var sourceConverter = TypeDescriptor.GetConverter(sourceType);
        if (sourceConverter.CanConvertTo(destinationType))
            return sourceConverter.ConvertTo(null, culture, value, destinationType);

        if (destinationType.IsEnum && value is int)
            return Enum.ToObject(destinationType, (int)value);

        if (!destinationType.IsInstanceOfType(value))
            return Convert.ChangeType(value, destinationType, culture);

        return value;
    }

    /// <summary>
    /// Converts a value to a destination type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <returns>The converted value.</returns>
    public static T To<T>(object value)
    {
        //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        return (T)To(value, typeof(T));
    }

    /// <summary>
    /// Splits the camel-case word into separate one
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Splitted string</returns>
    public static string SplitCamelCaseWord(string str)
    {
		_logger.LogDebug(">>> Entering SplitCamelCaseWord(str={Str})", str);
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        var result = str.ToCharArray()
            .Select(p => p.ToString())
            .Aggregate(string.Empty, (current, c) => current + (c == c.ToUpperInvariant() ? $" {c}" : c));
			_logger.LogDebug("SplitCamelCaseWord(str={Str}): result → {Result}", str, result);

        //ensure no spaces (e.g. when the first letter is upper case)
        result = result.TrimStart();

		_logger.LogDebug("<<< Exiting SplitCamelCaseWord(str={Str})", str);
        return result;
    }

    /// <summary>
    /// Converts a string from <c>SNAKE_CASE</c> to <c>PascalCase</c>
    /// </summary>
    /// <param name="str">Input string</param>
    /// <returns>Splitted <c>PascalCase</c></returns>
    public static string SnakeCaseToPascalCase(string str)
    {
		_logger.LogDebug(">>> Entering SnakeCaseToPascalCase(str={Str})", str);
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        var tempString = str.ToLower().Replace("_", " ");
		_logger.LogDebug("SnakeCaseToPascalCase(str={Str}): tempString → {TempString}", str, tempString);

		_logger.LogDebug("<<< Exiting SnakeCaseToPascalCase(str={Str})", str);
        return CultureInfo.InvariantCulture.TextInfo
            .ToTitleCase(tempString)
            .Replace(" ", string.Empty);
    }

    /// <summary>
    /// Get difference in years
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
    {
		_logger.LogDebug(">>> Entering GetDifferenceInYears(startDate={StartDate},endDate={EndDate})", startDate, endDate);
        //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
        //this assumes you are looking for the western idea of age and not using East Asian reckoning.
        var age = endDate.Year - startDate.Year;
        if (startDate > endDate.AddYears(-age))
            age--;
		_logger.LogDebug("<<< Exiting GetDifferenceInYears(startDate={StartDate},endDate={EndDate})", startDate, endDate);
        return age;
    }

    /// <summary>
    /// Get DateTime to the specified year, month, and day using the conventions of the current thread culture
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    /// <param name="day">The day</param>
    /// <returns>An instance of the Nullable<System.DateTime></returns>
    public static DateTime? ParseDate(int? year, int? month, int? day)
    {
		_logger.LogDebug(">>> Entering ParseDate(year={Year},month={Month},day={Day})", year, month, day);
        if (!year.HasValue || !month.HasValue || !day.HasValue)
            return null;

        DateTime? date = null;
        try
        {
            date = new DateTime(year.Value, month.Value, day.Value, CultureInfo.CurrentCulture.Calendar);
        }
        catch { }
		_logger.LogError("Exception in ParseDate(year={Year},month={Month},day={Day}): {Arg3}", year, month, day, ex.Message);
		_logger.LogDebug("<<< Exiting ParseDate(year={Year},month={Month},day={Day})", year, month, day);
        return date;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the default file provider
    /// </summary>
    public static INopFileProvider DefaultFileProvider { get; set; }

    #endregion
}