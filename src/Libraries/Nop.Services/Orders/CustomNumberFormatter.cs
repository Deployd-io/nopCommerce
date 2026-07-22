using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders;

/// <summary>
/// Custom number formatter
/// </summary>
public partial class CustomNumberFormatter : ICustomNumberFormatter
{
    #region Fields

    protected readonly OrderSettings _orderSettings;

    #endregion

    #region Ctor

    public CustomNumberFormatter(OrderSettings orderSettings)
    {
		logger.LogDebug(">>> Entering CustomNumberFormatter(orderSettings={OrderSettings})", orderSettings);
        _orderSettings = orderSettings;
		logger.LogDebug("<<< Exiting CustomNumberFormatter(orderSettings={OrderSettings})", orderSettings);
		logger.LogDebug("CustomNumberFormatter(orderSettings={OrderSettings}): _orderSettings → {_orderSettings}", orderSettings, _orderSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate return request custom number
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <returns>Custom number</returns>
    public virtual string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest)
    {
		logger.LogDebug(">>> Entering GenerateReturnRequestCustomNumber(returnRequest={ReturnRequest})", returnRequest);
        string customNumber;

        if (string.IsNullOrEmpty(_orderSettings.ReturnRequestNumberMask))
        {
            customNumber = returnRequest.Id.ToString();
			logger.LogDebug("GenerateReturnRequestCustomNumber(returnRequest={ReturnRequest}): customNumber → {CustomNumber}", returnRequest, customNumber);
        }
        else
        {
            customNumber = _orderSettings.ReturnRequestNumberMask
                .Replace("{ID}", returnRequest.Id.ToString())
                .Replace("{YYYY}", returnRequest.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", returnRequest.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", returnRequest.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", returnRequest.CreatedOnUtc.ToString("dd"));

            ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
            ////mask for Id example {#:00000000}
            //var rgx = new Regex(@"{#:\d+}");
            //var match = rgx.Match(customNumber);
            //var maskForReplase = match.Value;
            //
            //rgx = new Regex(@"\d+");
            //match = rgx.Match(maskForReplase);
            //
            //var formatValue = match.Value;
            //if(!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
            //    customNumber = customNumber.Replace(maskForReplase, returnRequest.Id.ToString(formatValue));
            //else
            //    customNumber = customNumber.Insert(0, $"{returnRequest.Id}-");
        }

		logger.LogDebug("<<< Exiting GenerateReturnRequestCustomNumber(returnRequest={ReturnRequest})", returnRequest);
        return customNumber;
    }

    /// <summary>
    /// Generate order custom number
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>Custom number</returns>
    public virtual string GenerateOrderCustomNumber(Order order)
    {
		logger.LogDebug(">>> Entering GenerateOrderCustomNumber(order={Order})", order);
        if (string.IsNullOrEmpty(_orderSettings.CustomOrderNumberMask))
            return order.Id.ToString();

        var customNumber = _orderSettings.CustomOrderNumberMask
            .Replace("{ID}", order.Id.ToString())
            .Replace("{YYYY}", order.CreatedOnUtc.ToString("yyyy"))
            .Replace("{YY}", order.CreatedOnUtc.ToString("yy"))
            .Replace("{MM}", order.CreatedOnUtc.ToString("MM"))
            .Replace("{DD}", order.CreatedOnUtc.ToString("dd")).Trim();

        ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
        ////mask for Id example {#:00000000}
        //var rgx = new Regex(@"{#:\d+}");
        //var match = rgx.Match(customNumber);
        //var maskForReplase = match.Value;

        //rgx = new Regex(@"\d+");
        //match = rgx.Match(maskForReplase);

        //var formatValue = match.Value;
        //if (!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
        //    customNumber = customNumber.Replace(maskForReplase, order.Id.ToString(formatValue));
        //else
        //    customNumber = customNumber.Insert(0, $"{order.Id}-");

		logger.LogDebug("<<< Exiting GenerateOrderCustomNumber(order={Order})", order);
        return customNumber;
    }

    #endregion
}
