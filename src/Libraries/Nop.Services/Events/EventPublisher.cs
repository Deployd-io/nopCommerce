using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Services.Events;

/// <summary>
/// Represents the event publisher implementation
/// </summary>
public partial class EventPublisher : IEventPublisher
{
    #region Methods

    /// <summary>
    /// Publish event to consumers
    /// </summary>
    /// <typeparam name="TEvent">Type of event</typeparam>
    /// <param name="event">Event object</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PublishAsync<TEvent>(TEvent @event)
    {
		logger.LogDebug(">>> Entering PublishAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //get all event consumers
        var consumers = EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList();

		logger.LogDebug("PublishAsync(): consumers → {Consumers}", consumers);
		logger.LogInformation("PublishAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        foreach (var consumer in consumers)
            try
            {
                //try to handle published event
                await consumer.HandleEventAsync(@event);

                if (@event is IStopProcessingEvent { StopProcessing: true })
                    break;
            }
            catch (Exception exception)
            {
                //log error, we put in to nested try-catch to prevent possible cyclic (if some error occurs)
                try
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
					logger.LogDebug("PublishAsync(): logger → {Logger}", logger);
                    if (logger == null)
                        return;

                    await logger.ErrorAsync(exception.Message, exception);
                }
                catch
                {
					logger.LogError("Exception in PublishAsync(): {Arg0}", exception.Message);
                    // ignored
                }
            }
			logger.LogDebug("<<< Exiting PublishAsync()");
    }

    /// <summary>
    /// Publish event to consumers
    /// </summary>
    /// <typeparam name="TEvent">Type of event</typeparam>
    /// <param name="event">Event object</param>
    public virtual void Publish<TEvent>(TEvent @event)
    {
		logger.LogDebug(">>> Entering Publish()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //get all event consumers
        var consumers = EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList();

		logger.LogDebug("Publish(): consumers → {Consumers}", consumers);
		logger.LogInformation("Publish(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        foreach (var consumer in consumers)
            try
            {
                //try to handle published event
                consumer.HandleEventAsync(@event).Wait();

                if (@event is IStopProcessingEvent { StopProcessing: true })
                    break;
            }
            catch (Exception exception)
            {
                //log error, we put in to nested try-catch to prevent possible cyclic (if some error occurs)
                try
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
					logger.LogDebug("Publish(): logger → {Logger}", logger);
                    if (logger == null)
                        return;

                    logger.Error(exception.Message, exception);
                }
                catch
                {
					logger.LogError("Exception in Publish(): {Arg0}", exception.Message);
                    // ignored
                }
            }
			logger.LogDebug("<<< Exiting Publish()");
    }

    #endregion
}