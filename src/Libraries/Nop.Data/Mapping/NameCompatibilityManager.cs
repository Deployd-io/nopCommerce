using Nop.Core.ComponentModel;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping;

/// <summary>
/// Helper class for maintaining backward compatibility of table naming
/// </summary>
public static partial class NameCompatibilityManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private static readonly Dictionary<Type, string> _tableNames = new();
    private static readonly Dictionary<(Type, string), string> _columnName = new();
    private static readonly IList<Type> _loadedFor = new List<Type>();
    private static bool _isInitialized;
    private static readonly ReaderWriterLockSlim _locker = new();

    #endregion

    #region Utilities

    private static void Initialize()
    {
		_logger.LogDebug(">>> Entering Initialize()");
        //perform with locked access to resources
        using (new ReaderWriteLockDisposable(_locker))
        {
            if (_isInitialized)
                return;

            var typeFinder = Singleton<ITypeFinder>.Instance;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var compatibilities = typeFinder.FindClassesOfType<INameCompatibility>()
                ?.Select(type => EngineContext.Current.ResolveUnregistered(type) as INameCompatibility).ToList() ?? new List<INameCompatibility>();
				_logger.LogInformation("Initialize(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

            compatibilities.AddRange(AdditionalNameCompatibilities.Select(type => EngineContext.Current.ResolveUnregistered(type) as INameCompatibility));

            foreach (var nameCompatibility in compatibilities.Distinct())
            {
                if (_loadedFor.Contains(nameCompatibility.GetType()))
                    continue;

                _loadedFor.Add(nameCompatibility.GetType());

                foreach (var (key, value) in nameCompatibility.TableNames.Where(tableName =>
                             !_tableNames.ContainsKey(tableName.Key)))
                    _tableNames.Add(key, value);

                foreach (var (key, value) in nameCompatibility.ColumnName.Where(columnName =>
                             !_columnName.ContainsKey(columnName.Key)))
                    _columnName.Add(key, value);
            }

            _isInitialized = true;
        }
		_logger.LogDebug("<<< Exiting Initialize()");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets table name for mapping with the type
    /// </summary>
    /// <param name="type">Type to get the table name</param>
    /// <returns>Table name</returns>
    public static string GetTableName(Type type)
    {
		_logger.LogDebug(">>> Entering GetTableName(type={Type})", type);
        if (!_isInitialized)
            Initialize();

		_logger.LogDebug("<<< Exiting GetTableName(type={Type})", type);
        return _tableNames.TryGetValue(type, out var value) ? value : type.Name;
    }

    /// <summary>
    /// Gets column name for mapping with the entity's property
    /// </summary>
    /// <param name="type">Type of entity</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>Column name</returns>
    public static string GetColumnName(Type type, string propertyName)
    {
		_logger.LogDebug(">>> Entering GetColumnName(type={Type},propertyName={PropertyName})", type, propertyName);
        if (!_isInitialized)
            Initialize();

		_logger.LogDebug("<<< Exiting GetColumnName(type={Type},propertyName={PropertyName})", type, propertyName);
        return _columnName.ContainsKey((type, propertyName)) ? _columnName[(type, propertyName)] : propertyName;
    }

    #endregion

    /// <summary>
    /// Additional name compatibility types
    /// </summary>
    public static List<Type> AdditionalNameCompatibilities { get; } = new List<Type>();
}