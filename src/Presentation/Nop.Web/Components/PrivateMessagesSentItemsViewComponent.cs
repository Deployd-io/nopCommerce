using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class PrivateMessagesSentItemsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

    public PrivateMessagesSentItemsViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
    {
		_logger.LogDebug(">>> Entering PrivateMessagesSentItemsViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
        _privateMessagesModelFactory = privateMessagesModelFactory;
		_logger.LogDebug("PrivateMessagesSentItemsViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory}): _privateMessagesModelFactory → {_privateMessagesModelFactory}", privateMessagesModelFactory, _privateMessagesModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        var model = await _privateMessagesModelFactory.PrepareSentModelAsync(pageNumber, tab);
		_logger.LogDebug("InvokeAsync(pageNumber={PageNumber},tab={Tab}): model → {Model}", pageNumber, tab, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        return View(model);
    }
}