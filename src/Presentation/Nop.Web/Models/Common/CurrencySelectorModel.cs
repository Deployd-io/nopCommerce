using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record CurrencySelectorModel : BaseNopModel
{
    public CurrencySelectorModel()
    {
		logger.LogDebug(">>> Entering CurrencySelectorModel()");
        AvailableCurrencies = new List<CurrencyModel>();
		logger.LogDebug("<<< Exiting CurrencySelectorModel()");
		logger.LogDebug("CurrencySelectorModel(): AvailableCurrencies → {AvailableCurrencies}", AvailableCurrencies);
    }

    public IList<CurrencyModel> AvailableCurrencies { get; set; }

    public int CurrentCurrencyId { get; set; }

    public bool DisplayCurrencySymbol { get; set; }
}