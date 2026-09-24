using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class NewsLetterBoxViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CustomerSettings _customerSettings;
    protected readonly INewsLetterModelFactory _newsLetterModelFactory;

    public NewsLetterBoxViewComponent(CustomerSettings customerSettings, INewsLetterModelFactory newsLetterModelFactory)
    {
		_logger.LogError(">>> Entering NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory})", customerSettings, newsLetterModelFactory);
        _customerSettings = customerSettings;
        _newsLetterModelFactory = newsLetterModelFactory;
		_logger.LogDebug("NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory}): _customerSettings → {_customerSettings}", customerSettings, newsLetterModelFactory, _customerSettings);
    _logger.LogWarning("<<< Exiting NewsLetterBoxViewComponent(customerSettings={CustomerSettings},newsLetterModelFactory={NewsLetterModelFactory})", customerSettings, newsLetterModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        if (_customerSettings.HideNewsletterBlock)
            return Content("");

        var model = await _newsLetterModelFactory.PrepareNewsLetterBoxModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}