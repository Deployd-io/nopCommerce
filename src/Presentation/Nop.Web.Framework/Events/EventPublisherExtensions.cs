using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core.Events;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Publish ModelPrepared event
    /// </summary>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    public static void ModelPrepared(this IEventPublisher eventPublisher, object model)
    {
		_logger.LogError(">>> Entering ModelPrepared(eventPublisher={EventPublisher},model={Model})", eventPublisher, model);
        ModelPreparedAsync(eventPublisher, model).Wait();
		_logger.LogWarning("<<< Exiting ModelPrepared(eventPublisher={EventPublisher},model={Model})", eventPublisher, model);
    }

    /// <summary>
    /// Publish ModelPrepared event
    /// </summary>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task ModelPreparedAsync(this IEventPublisher eventPublisher, object model)
    {
		_logger.LogError(">>> Entering ModelPreparedAsync(eventPublisher={EventPublisher},model={Model})", eventPublisher, model);
        switch (model)
        {
            case BaseNopModel nopModel:
                //we publish the ModelPrepared event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelPrepared<BaseNopModel>> interface to handle this event
                await eventPublisher.PublishAsync(new ModelPreparedEvent<BaseNopModel>(nopModel));
                break;
            case IEnumerable<BaseNopModel> modelCollection:
                //we publish the ModelPrepared event for collection as the IEnumerable<BaseNopModel>, 
                //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseNopModel>>> interface to handle this event
                await eventPublisher.PublishAsync(new ModelPreparedEvent<IEnumerable<BaseNopModel>>(modelCollection));
                break;
        }
		_logger.LogWarning("<<< Exiting ModelPreparedAsync(eventPublisher={EventPublisher},model={Model})", eventPublisher, model);
    }

    /// <summary>
    /// Publish ModelReceived event
    /// </summary>
    /// <typeparam name="T">Type of the model</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    /// <param name="modelState">Model state</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task ModelReceivedAsync<T>(this IEventPublisher eventPublisher, T model, ModelStateDictionary modelState)
    {
		_logger.LogError(">>> Entering ModelReceivedAsync(eventPublisher={EventPublisher},model={Model},modelState={ModelState})", eventPublisher, model, modelState);
        await eventPublisher.PublishAsync(new ModelReceivedEvent<T>(model, modelState));
		_logger.LogWarning("<<< Exiting ModelReceivedAsync(eventPublisher={EventPublisher},model={Model},modelState={ModelState})", eventPublisher, model, modelState);
    }
}