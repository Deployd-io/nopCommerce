using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class PrivateMessagesSentItemsViewComponent : NopViewComponent
{
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

    public PrivateMessagesSentItemsViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
    {
		logger.LogDebug(">>> Entering PrivateMessagesSentItemsViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
        _privateMessagesModelFactory = privateMessagesModelFactory;
		logger.LogDebug("<<< Exiting PrivateMessagesSentItemsViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
		logger.LogDebug("PrivateMessagesSentItemsViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory}): _privateMessagesModelFactory → {_privateMessagesModelFactory}", privateMessagesModelFactory, _privateMessagesModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
    {
		logger.LogDebug(">>> Entering InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        var model = await _privateMessagesModelFactory.PrepareSentModelAsync(pageNumber, tab);
		logger.LogDebug("InvokeAsync(pageNumber={PageNumber},tab={Tab}): model → {Model}", pageNumber, tab, model);
		logger.LogDebug("<<< Exiting InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        return View(model);
    }
}