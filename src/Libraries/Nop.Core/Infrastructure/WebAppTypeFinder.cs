using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides information about types in the current web application. 
/// Optionally this class can look at all assemblies in the bin folder.
/// </summary>
public partial class WebAppTypeFinder : ITypeFinder
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    /// <summary>Gets the pattern for DLLs that we know don't need to be investigated.</summary>
    protected const string ASSEMBLY_SKIP_LOADING_PATTERN = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";

    #endregion

    #region Fields

    protected static readonly ConcurrentDictionary<string, Assembly> _assemblies = new(StringComparer.InvariantCultureIgnoreCase);

    protected static bool _loaded;
    protected static readonly object _locker = new();

    private readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    public WebAppTypeFinder()
    {
        _fileProvider = CommonHelper.DefaultFileProvider;
		_logger.LogError(">>> Entering WebAppTypeFinder()");
		_logger.LogWarning("<<< Exiting WebAppTypeFinder()");
    }

    static WebAppTypeFinder()
    {
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    #endregion

    #region Utilities

    private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
		_logger.LogError(">>> Entering OnAssemblyLoad(sender={Sender},args={Args})", sender, args);
        var assembly = args.LoadedAssembly;

        if (assembly.FullName == null)
            return;

        if (_assemblies.ContainsKey(assembly.FullName))
            return;

		_logger.LogWarning("<<< Exiting OnAssemblyLoad(sender={Sender},args={Args})", sender, args);
        if (!Matches(assembly.FullName)) {
            return;
        }

        _assemblies.TryAdd(assembly.FullName, assembly);
    }

    /// <summary>
    /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
    /// </summary>
    /// <param name="assemblyFullName">
    /// The name of the assembly to check.
    /// </param>
    /// <returns>
    /// True if the assembly should be loaded into Nop.
    /// </returns>
    protected static bool Matches(string assemblyFullName)
    {
		_logger.LogError(">>> Entering Matches(assemblyFullName={AssemblyFullName})", assemblyFullName);
        _logger.LogWarning("<<< Exiting Matches(assemblyFullName={AssemblyFullName})", assemblyFullName);
        return !Regex.IsMatch(assemblyFullName, ASSEMBLY_SKIP_LOADING_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    /// <summary>
    /// Does type implement generic?
    /// </summary>
    /// <param name="type"></param>
    /// <param name="openGeneric"></param>
    /// <returns></returns>
    protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
    {
		_logger.LogError(">>> Entering DoesTypeImplementOpenGeneric(type={Type},openGeneric={OpenGeneric})", type, openGeneric);
        try
        {
            var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
			_logger.LogError("DoesTypeImplementOpenGeneric(type={Type},openGeneric={OpenGeneric}): genericTypeDefinition → {GenericTypeDefinition}", type, openGeneric, genericTypeDefinition);
            return type.FindInterfaces((_, _) => true, null)
                .Where(implementedInterface => implementedInterface.IsGenericType).Any(implementedInterface =>
                    genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()));
        }
        catch
        {
			_logger.LogError("Exception in DoesTypeImplementOpenGeneric(type={Type},openGeneric={OpenGeneric}): {Arg2}", type, openGeneric, ex.Message);
			_logger.LogWarning("<<< Exiting DoesTypeImplementOpenGeneric(type={Type},openGeneric={OpenGeneric})", type, openGeneric);
            return false;
        }
    }

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <param name="assignTypeFrom">Assign type from</param>
    /// <param name="assemblies">Assemblies</param>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    protected virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
    {
		_logger.LogError(">>> Entering FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses})", assignTypeFrom, assemblies, onlyConcreteClasses);
        var result = new List<Type>();

		_logger.LogError("FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses}): result → {Result}", assignTypeFrom, assemblies, onlyConcreteClasses, result);
        try
        {
            foreach (var a in assemblies)
            {
                Type[] types = null;
                try
                {
                    types = a.GetTypes();
					_logger.LogError("FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses}): types → {Types}", assignTypeFrom, assemblies, onlyConcreteClasses, types);
                }
                catch
                {
					_logger.LogError("Exception in FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses}): {Arg3}", assignTypeFrom, assemblies, onlyConcreteClasses, ex.Message);
					_logger.LogDebug("FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses}): types → {Types}", assignTypeFrom, assemblies, onlyConcreteClasses, types);
                    //ignore
                }

                if (types == null)
                    continue;

                foreach (var t in types)
                {
                    if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        continue;

                    if (t.IsInterface)
                        continue;

                    if (onlyConcreteClasses)
                    {
                        if (t.IsClass && !t.IsAbstract)
                            result.Add(t);
                    }
                    else
                        result.Add(t);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            var msg = string.Empty;

            if (ex.LoaderExceptions.Any())
                msg = ex.LoaderExceptions.Where(e => e != null)
                    .Aggregate(msg, (current, e) => $"{current}{e.Message + Environment.NewLine}");

            var fail = new Exception(msg, ex);
			_logger.LogError("FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses}): fail → {Fail}", assignTypeFrom, assemblies, onlyConcreteClasses, fail);
            Debug.WriteLine(fail.Message, fail);

		_logger.LogWarning("<<< Exiting FindClassesOfType(assignTypeFrom={AssignTypeFrom},assemblies={Assemblies},onlyConcreteClasses={OnlyConcreteClasses})", assignTypeFrom, assemblies, onlyConcreteClasses);
            throw fail;
        }

        return result;
    }

    protected virtual void InitData()
    {
		_logger.LogError(">>> Entering InitData()");
        //data already loaded
        if (_loaded)
            return;

        //prevent multi loading data
        lock (_locker)
        {
            //data can be loaded while we waited
            if (_loaded)
                return;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == null)
                    continue;

                if (!Matches(assembly.FullName))
                    continue;

                _assemblies.TryAdd(assembly.FullName, assembly);
            }

            foreach (var dllPath in DirectoriesToLoadAssemblies
                         .Where(directoriesToLoadAssembly => _fileProvider.DirectoryExists(directoriesToLoadAssembly))
                         .SelectMany(directoriesToLoadAssembly =>
                             _fileProvider.GetFiles(directoriesToLoadAssembly, "*.dll")))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
					_logger.LogError("InitData(): an → {An}", an);

                    if (_assemblies.ContainsKey(an.FullName))
                        continue;

                    if (!Matches(an.FullName))
                        continue;

                    Assembly assembly;

                    try
                    {
                        assembly = AppDomain.CurrentDomain.Load(an);
						_logger.LogError("InitData(): assembly → {Assembly}", assembly);
                    }
                    catch
                    {
                        assembly = Assembly.LoadFrom(dllPath);
                    }

                    _assemblies.TryAdd(assembly.FullName, assembly);
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }

            _loaded = true;
			_logger.LogDebug("InitData(): _loaded → {_loaded}", _loaded);
        }
		_logger.LogWarning("<<< Exiting InitData()");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the assemblies related to the current implementation.
    /// </summary>
    /// <returns>A list of assemblies</returns>
    public virtual IList<Assembly> GetAssemblies()
    {
		_logger.LogError(">>> Entering GetAssemblies()");
        if (!_loaded)
            InitData();

		_logger.LogWarning("<<< Exiting GetAssemblies()");
        return _assemblies.Values.ToList();
    }

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    public virtual IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(typeof(T), onlyConcreteClasses);
    }

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <param name="assignTypeFrom">Assign type from</param>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    /// <returns></returns>
    public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
    }

    /// <summary>
    /// Gets the assembly by it full name
    /// </summary>
    /// <returns>A list of assemblies</returns>
    public virtual Assembly GetAssemblyByName(string assemblyFullName)
    {
		_logger.LogError(">>> Entering GetAssemblyByName(assemblyFullName={AssemblyFullName})", assemblyFullName);
        if (!_loaded)
            InitData();

        _assemblies.TryGetValue(assemblyFullName, out var assembly);

        if (assembly != null)
            return assembly;

        var assemblyName = new AssemblyName(assemblyFullName);
		_logger.LogError("GetAssemblyByName(assemblyFullName={AssemblyFullName}): assemblyName → {AssemblyName}", assemblyFullName, assemblyName);
        var key = _assemblies.Keys.FirstOrDefault(k => k.StartsWith(assemblyName.Name ?? assemblyFullName.Split(' ')[0], StringComparison.InvariantCultureIgnoreCase));
		_logger.LogError("GetAssemblyByName(assemblyFullName={AssemblyFullName}): key → {Key}", assemblyFullName, key);

		_logger.LogWarning("<<< Exiting GetAssemblyByName(assemblyFullName={AssemblyFullName})", assemblyFullName);
        return string.IsNullOrEmpty(key) ? null : _assemblies[key];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the list of directories to load assemblies on initialize 
    /// </summary>
    /// <remarks>
    /// For example, the web application's bin folder should be specifically checked for being loaded on the application load. This is needed in situations where plugins need to be loaded in the AppDomain after the application has been reloaded
    /// </remarks>
    public virtual List<string> DirectoriesToLoadAssemblies { get; set; } = new()
    {
        AppContext.BaseDirectory
    };

    #endregion
}