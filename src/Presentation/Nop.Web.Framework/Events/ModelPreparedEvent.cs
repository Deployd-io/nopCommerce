using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after the model is prepared for view
/// </summary>
/// <typeparam name="T">Type of the model</typeparam>
public partial class ModelPreparedEvent<T>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="model">Model</param>
    public ModelPreparedEvent(T model)
    {
		_logger.LogError(">>> Entering ModelPreparedEvent(model={Model})", model);
        Model = model;
		_logger.LogDebug("ModelPreparedEvent(model={Model}): Model → {Model1}", model, Model);
    _logger.LogWarning("<<< Exiting ModelPreparedEvent(model={Model})", model);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a model
    /// </summary>
    public T Model { get; protected set; }

    #endregion
}