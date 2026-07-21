namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represent DataTables filter parameter
/// </summary>
public partial class FilterParameter
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the FilterParameter class by default as string type parameter
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    public FilterParameter(string name)
    {
		logger.LogDebug(">>> Entering FilterParameter(name={Name})", name);
        Name = name;
        Type = typeof(string);
		logger.LogDebug("FilterParameter(name={Name}): Name → {Name1}", name, Name);
		logger.LogDebug("FilterParameter(name={Name}): Type → {Type}", name, Type);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    /// <param name="modelName">Filter parameter model name</param>
    public FilterParameter(string name, string modelName)
    {
		logger.LogDebug(">>> Entering FilterParameter(name={Name},modelName={ModelName})", name, modelName);
        Name = name;
        ModelName = modelName;
		logger.LogDebug("FilterParameter(name={Name},modelName={ModelName}): Name → {Name1}", name, modelName, Name);
		logger.LogDebug("FilterParameter(name={Name},modelName={ModelName}): ModelName → {ModelName1}", name, modelName, ModelName);
        Type = typeof(string);
		logger.LogDebug("<<< Exiting FilterParameter(name={Name},modelName={ModelName})", name, modelName);
		logger.LogDebug("FilterParameter(name={Name},modelName={ModelName}): Type → {Type}", name, modelName, Type);
		logger.LogDebug("<<< Exiting FilterParameter(name={Name})", name);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class
    /// </summary>
	logger.LogDebug("<<< Exiting FilterParameter(name={Name})", name);
    /// <param name="name">Filter parameter name</param>
    /// <param name="type">Filter parameter type</param>
    public FilterParameter(string name, Type type)
    {
		logger.LogDebug(">>> Entering FilterParameter(name={Name},type={Type})", name, type);
        Name = name;
        Type = type;
		logger.LogDebug("<<< Exiting FilterParameter(name={Name},type={Type})", name, type);
		logger.LogDebug("FilterParameter(name={Name},type={Type}): Name → {Name1}", name, type, Name);
		logger.LogDebug("FilterParameter(name={Name},type={Type}): Type → {Type1}", name, type, Type);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    /// <param name="value">Filter parameter value</param>
    public FilterParameter(string name, object value)
    {
		logger.LogDebug(">>> Entering FilterParameter(name={Name},value={Value})", name, value);
        Name = name;
        Type = value.GetType();
		logger.LogDebug("FilterParameter(name={Name},value={Value}): Name → {Name1}", name, value, Name);
		logger.LogDebug("FilterParameter(name={Name},value={Value}): Type → {Type}", name, value, Type);
        Value = value;
		logger.LogDebug("FilterParameter(name={Name},value={Value}): Value → {Value1}", name, value, Value);
    }

    /// <summary>
    /// Initializes a new instance of the FilterParameter class for linking "parent-child" tables
    /// </summary>
    /// <param name="name">Filter parameter name</param>
    /// <param name="parentName">Filter parameter parent name</param>
    /// <param name="isParentChildParameter">Parameter indicator for linking "parent-child" tables</param>
    public FilterParameter(string name, string parentName, bool isParentChildParameter = true)
    {
		logger.LogDebug(">>> Entering FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter})", name, parentName, isParentChildParameter);
        Name = name;
        ParentName = parentName;
		logger.LogDebug("FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter}): Name → {Name1}", name, parentName, isParentChildParameter, Name);
		logger.LogDebug("FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter}): ParentName → {ParentName1}", name, parentName, isParentChildParameter, ParentName);
        Type = typeof(string);
		logger.LogDebug("<<< Exiting FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter})", name, parentName, isParentChildParameter);
		logger.LogDebug("FilterParameter(name={Name},parentName={ParentName},isParentChildParameter={IsParentChildParameter}): Type → {Type}", name, parentName, isParentChildParameter, Type);
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
