using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class PrivateMessagesInboxViewComponent : NopViewComponent
{
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

    public PrivateMessagesInboxViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
    {
		logger.LogDebug(">>> Entering PrivateMessagesInboxViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
        _privateMessagesModelFactory = privateMessagesModelFactory;
		logger.LogDebug("<<< Exiting PrivateMessagesInboxViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory})", privateMessagesModelFactory);
		logger.LogDebug("PrivateMessagesInboxViewComponent(privateMessagesModelFactory={PrivateMessagesModelFactory}): _privateMessagesModelFactory → {_privateMessagesModelFactory}", privateMessagesModelFactory, _privateMessagesModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
    {
		logger.LogDebug(">>> Entering InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        var model = await _privateMessagesModelFactory.PrepareInboxModelAsync(pageNumber, tab);
		logger.LogDebug("InvokeAsync(pageNumber={PageNumber},tab={Tab}): model → {Model}", pageNumber, tab, model);
		logger.LogDebug("<<< Exiting InvokeAsync(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        return View(model);
    }
}