using Microsoft.Extensions.Logging;
namespace Nop.Web.Models.JsonLD;

/// <summary>
/// Represents JSON-LD model created event
/// </summary>
public partial class JsonLdCreatedEvent<T> where T : JsonLdModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="model">Created model</param>
    public JsonLdCreatedEvent(T model)
    {
		_logger.LogDebug(">>> Entering JsonLdCreatedEvent(model={Model})", model);
        Model = model;
		_logger.LogDebug("JsonLdCreatedEvent(model={Model}): Model → {Model1}", model, Model);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Created model
    /// </summary>
    public T Model { get; }

    #endregion
}