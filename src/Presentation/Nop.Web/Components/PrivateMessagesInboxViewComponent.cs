using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class PrivateMessagesInboxViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

    public PrivateMessagesInboxViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
    {
		_logger.LogDebug(">>> Entering PrivateMessagesInboxViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
        _privateMessagesModelFactory = privateMessagesModelFactory;
		_logger.LogDebug("<<< Exiting PrivateMessagesInboxViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
		_logger.LogDebug("PrivateMessagesInboxViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory}): _privateMessagesModelFactory → {_privateMessagesModelFactory}", privateMessagesModelFactory, _privateMessagesModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        var model = await _privateMessagesModelFactory.PrepareInboxModelAsync(pageNumber, tab);
		_logger.LogDebug("InvokeAsync(pageNumber={PageNumber},tab={Tab}): model → {Model}", pageNumber, tab, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        return View(model);
    }
}