using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class NewsLetterBoxViewComponent : NopViewComponent
{
    protected readonly CustomerSettings _customerSettings;
    protected readonly INewsLetterModelFactory _newsLetterModelFactory;

    public NewsLetterBoxViewComponent(CustomerSettings customerSettings, INewsLetterModelFactory newsLetterModelFactory)
    {
		logger.LogDebug(">>> Entering NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory})", customerSettings, newsLetterModelFactory);
        _customerSettings = customerSettings;
        _newsLetterModelFactory = newsLetterModelFactory;
		logger.LogDebug("<<< Exiting NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory})", customerSettings, newsLetterModelFactory);
		logger.LogDebug("NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory}): _newsLetterModelFactory → {_newsLetterModelFactory}", customerSettings, newsLetterModelFactory, _newsLetterModelFactory);
		logger.LogDebug("NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory}): _customerSettings → {_customerSettings}", customerSettings, newsLetterModelFactory, _customerSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        if (_customerSettings.HideNewsletterBlock)
            return Content("");

        var model = await _newsLetterModelFactory.PrepareNewsLetterBoxModelAsync();
		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}
