using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after the model is received from the view
/// </summary>
/// <typeparam name="T">Type of the model</typeparam>
public partial class ModelReceivedEvent<T>
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="model">Model</param>
    /// <param name="modelState">Model state</param>
    public ModelReceivedEvent(T model, ModelStateDictionary modelState)
    {
		logger.LogDebug(">>> Entering ModelReceivedEvent(model={Model},modelState={ModelState})", model, modelState);
        Model = model;
        ModelState = modelState;
		logger.LogDebug("<<< Exiting ModelReceivedEvent(model={Model},modelState={ModelState})", model, modelState);
		logger.LogDebug("ModelReceivedEvent(model={Model},modelState={ModelState}): ModelState → {ModelState1}", model, modelState, ModelState);
		logger.LogDebug("ModelReceivedEvent(model={Model},modelState={ModelState}): Model → {Model1}", model, modelState, Model);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a model
    /// </summary>
    public T Model { get; protected set; }

    /// <summary>
    /// Gets a model state
    /// </summary>
    public ModelStateDictionary ModelState { get; protected set; }

    #endregion
}