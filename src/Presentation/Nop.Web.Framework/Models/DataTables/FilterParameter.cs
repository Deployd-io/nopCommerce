using Microsoft.Extensions.Logging;
﻿namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represent DataTables filter parameter
/// </summary>
public partial class FilterParameter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the FilterParameter class by default as string type parameter
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    public FilterParameter(string name)
    {
		_logger.LogDebug(">>> Entering FilterParameter(name={Name})", name);
        Name = name;
        Type = typeof(string);
		_logger.LogDebug("FilterParameter(name={Name}): Type → {Type}", name, Type);
		_logger.LogDebug("FilterParameter(name={Name}): Name → {Name1}", name, Name);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    /// <param name="modelName">Filter parameter model name</param>
    public FilterParameter(string name, string modelName)
    {
		_logger.LogDebug(">>> Entering FilterParameter(name={Name},modelName={ModelName})", name, modelName);
        Name = name;
        ModelName = modelName;
		_logger.LogDebug("FilterParameter(name={Name},modelName={ModelName}): Name → {Name1}", name, modelName, Name);
		_logger.LogDebug("FilterParameter(name={Name},modelName={ModelName}): ModelName → {ModelName1}", name, modelName, ModelName);
        Type = typeof(string);
		_logger.LogDebug("<<< Exiting FilterParameter(name={Name},modelName={ModelName})", name, modelName);
		_logger.LogDebug("FilterParameter(name={Name},modelName={ModelName}): Type → {Type}", name, modelName, Type);
		_logger.LogDebug("<<< Exiting FilterParameter(name={Name})", name);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class
    /// </summary>
	_logger.LogDebug("<<< Exiting FilterParameter(name={Name})", name);
    /// <param name="name">Filter parameter name</param>
    /// <param name="type">Filter parameter type</param>
    public FilterParameter(string name, Type type)
    {
		_logger.LogDebug(">>> Entering FilterParameter(name={Name},type={Type})", name, type);
        Name = name;
        Type = type;
		_logger.LogDebug("<<< Exiting FilterParameter(name={Name},type={Type})", name, type);
		_logger.LogDebug("FilterParameter(name={Name},type={Type}): Name → {Name1}", name, type, Name);
		_logger.LogDebug("FilterParameter(name={Name},type={Type}): Type → {Type1}", name, type, Type);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    /// <param name="value">Filter parameter value</param>
    public FilterParameter(string name, object value)
    {
		_logger.LogDebug(">>> Entering FilterParameter(name={Name},value={Value})", name, value);
        Name = name;
        Type = value.GetType();
		_logger.LogDebug("FilterParameter(name={Name},value={Value}): Type → {Type}", name, value, Type);
		_logger.LogDebug("FilterParameter(name={Name},value={Value}): Name → {Name1}", name, value, Name);
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class for linking "parent-child" tables
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    /// <param name="parentName">Filter parameter parent name</param>
    /// <param name="isParentChildParameter">Parameter indicator for linking "parent-child" tables</param>
    public FilterParameter(string name, string parentName, bool isParentChildParameter = true)
    {
		_logger.LogDebug(">>> Entering FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter})", name, parentName, isParentChildParameter);
        Name = name;
        ParentName = parentName;
		_logger.LogDebug("FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter}): Name → {Name1}", name, parentName, isParentChildParameter, Name);
		_logger.LogDebug("FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter}): ParentName → {ParentName1}", name, parentName, isParentChildParameter, ParentName);
        Type = typeof(string);
		_logger.LogDebug("<<< Exiting FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter})", name, parentName, isParentChildParameter);
		_logger.LogDebug("FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter}): Type → {Type}", name, parentName, isParentChildParameter, Type);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Filter field name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Filter model name
    /// </summary>
    public string ModelName { get; }

    /// <summary>
    /// Filter field type
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Filter field value
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Filter field parent name
    /// </summary>
    public string ParentName { get; set; }

    #endregion
}