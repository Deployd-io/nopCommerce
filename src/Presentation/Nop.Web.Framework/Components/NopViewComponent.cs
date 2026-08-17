using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Components;

/// <summary>
/// Base class for ViewComponent in nopCommerce
/// </summary>
public abstract partial class NopViewComponent : ViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Returns a result which will render the partial view with name <paramref name="viewName"/>.
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
    {
		_logger.LogDebug(">>> Entering View(viewName={ViewName},model={Model})", viewName, model);
        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        eventPublisher.ModelPrepared(model);
		_logger.LogDebug("View(viewName={ViewName},model={Model}): eventPublisher → {EventPublisher}", viewName, model, eventPublisher);

        //invoke the base method
        return base.View(viewName, model);
    }

    /// <summary>
    /// Returns a result which will render the partial view
    /// </summary>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View<TModel>(TModel model)
    {
        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        eventPublisher.ModelPrepared(model);
		_logger.LogDebug("View(model={Model}): eventPublisher → {EventPublisher}", model, eventPublisher);

        //invoke the base method
        return base.View(model);
    }

    /// <summary>
    ///  Returns a result which will render the partial view with name viewName
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View(string viewName)
    {
        //invoke the base method
		_logger.LogDebug("<<< Exiting View(viewName={ViewName},model={Model})", viewName, model);
        return base.View(viewName);
    }
}