namespace Nop.Plugin.Shipping.UPS.Domain;

/// <summary>
/// Represents custom attribute for UPS code
/// </summary>
public class UPSCodeAttribute : Attribute
{
    #region Ctor

    public UPSCodeAttribute(string codeValue)
    {
		logger.LogDebug(">>> Entering UPSCodeAttribute(codeValue={CodeValue})", codeValue);
        Code = codeValue;
		logger.LogDebug("UPSCodeAttribute(codeValue={CodeValue}): Code → {Code}", codeValue, Code);
		logger.LogDebug("<<< Exiting UPSCodeAttribute(codeValue={CodeValue})", codeValue);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a code value
    /// </summary>
    public string Code { get; }

    #endregion
}