using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Nop.Core;
using Nop.Core.ComponentModel;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;
using Nop.Services.Plugins;

namespace Nop.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents application part manager extensions
/// </summary>
public static partial class ApplicationPartManagerExtensions
{
    #region Fields

    private static readonly INopFileProvider _fileProvider;
    private static readonly List<KeyValuePair<string, Version>> _baseAppLibraries;
    private static readonly Dictionary<string, Version> _pluginLibraries;
    private static readonly Dictionary<string, PluginLoadedAssemblyInfo> _loadedAssemblies = new();
    private static readonly ReaderWriterLockSlim _locker = new();

    #endregion

    #region Ctor

    static ApplicationPartManagerExtensions()
    {
		_logger.LogDebug(">>> Entering ApplicationPartManagerExtensions()");
        //we use the default file provider, since the DI isn't initialized yet
        _fileProvider = CommonHelper.DefaultFileProvider;
		_logger.LogDebug("ApplicationPartManagerExtensions(): _fileProvider → {_fileProvider}", _fileProvider);

        _baseAppLibraries = new List<KeyValuePair<string, Version>>();
        _pluginLibraries = new Dictionary<string, Version>();

		_logger.LogDebug("ApplicationPartManagerExtensions(): _baseAppLibraries → {_baseAppLibraries}", _baseAppLibraries);
		_logger.LogDebug("ApplicationPartManagerExtensions(): _pluginLibraries → {_pluginLibraries}", _pluginLibraries);
        //get all libraries from /bin/{version}/ directory
        foreach (var file in _fileProvider.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            _baseAppLibraries.Add(new KeyValuePair<string, Version>(_fileProvider.GetFileName(file), GetAssemblyVersion(file)));

        //get all libraries from base site directory
        if (!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
            foreach (var file in _fileProvider.GetFiles(Environment.CurrentDirectory, "*.dll"))
                _baseAppLibraries.Add(new KeyValuePair<string, Version>(_fileProvider.GetFileName(file), GetAssemblyVersion(file)));

        //get all libraries from refs directory
        var refsPathName = _fileProvider.Combine(Environment.CurrentDirectory, NopPluginDefaults.RefsPathName);
		_logger.LogDebug("ApplicationPartManagerExtensions(): refsPathName → {RefsPathName}", refsPathName);
        if (_fileProvider.DirectoryExists(refsPathName))
            foreach (var file in _fileProvider.GetFiles(refsPathName, "*.dll"))
                _baseAppLibraries.Add(new KeyValuePair<string, Version>(_fileProvider.GetFileName(file), GetAssemblyVersion(file)));
				_logger.LogDebug("<<< Exiting ApplicationPartManagerExtensions()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets access to information about plugins
    /// </summary>
    private static IPluginsInfo PluginsInfo
    {
        get => Singleton<IPluginsInfo>.Instance;
        set => Singleton<IPluginsInfo>.Instance = value;
    }

    #endregion

    #region Utilities

    private static Version GetAssemblyVersion(string filePath)
    {
		_logger.LogDebug(">>> Entering GetAssemblyVersion(filePath={FilePath})", filePath);
        try
        {
            return AssemblyName.GetAssemblyName(filePath).Version;
        }
        catch (BadImageFormatException)
        {
			_logger.LogError("Exception in GetAssemblyVersion(filePath={FilePath}): {Arg1}", filePath, BadImageFormatException.getMessage());
            //ignore
        }

		_logger.LogDebug("<<< Exiting GetAssemblyVersion(filePath={FilePath})", filePath);
        return null;
    }

    private static void CheckCompatible(PluginDescriptor pluginDescriptor, IDictionary<string, Version> assemblies)
    {
		_logger.LogDebug(">>> Entering CheckCompatible(pluginDescriptor={PluginDescriptor},assemblies={Assemblies})", pluginDescriptor, assemblies);
        var refFiles = pluginDescriptor.PluginFiles.Where(file =>
            !_fileProvider.GetFileName(file).Equals(_fileProvider.GetFileName(pluginDescriptor.OriginalAssemblyFile))).ToList();

        foreach (var refFile in refFiles.Where(file =>
                     assemblies.ContainsKey(_fileProvider.GetFileName(file).ToLower())))
            IsAlreadyLoaded(refFile, pluginDescriptor.SystemName);

        var hasCollisions = _loadedAssemblies.Where(p =>
                p.Value.References.Any(r => r.PluginName.Equals(pluginDescriptor.SystemName)))
            .Any(p => p.Value.Collisions.Any());

        if (hasCollisions)
        {
            PluginsInfo.IncompatiblePlugins.Add(pluginDescriptor.SystemName, PluginIncompatibleType.HasCollisions);
            PluginsInfo.PluginDescriptors.Remove((pluginDescriptor, false));
        }
		_logger.LogDebug("<<< Exiting CheckCompatible(pluginDescriptor={PluginDescriptor},assemblies={Assemblies})", pluginDescriptor, assemblies);
    }

    /// <summary>
    /// Load and register the assembly
    /// </summary>
    /// <param name="applicationPartManager">Application part manager</param>
    /// <param name="assemblyFile">Path to the assembly file</param>
    /// <param name="useUnsafeLoadAssembly">Indicating whether to load an assembly into the load-from context, bypassing some security checks</param>
    /// <returns>Assembly</returns>
    private static Assembly AddApplicationParts(ApplicationPartManager applicationPartManager, string assemblyFile, bool useUnsafeLoadAssembly)
    {
		_logger.LogDebug(">>> Entering AddApplicationParts(applicationPartManager={ApplicationPartManager},assemblyFile={AssemblyFile},useUnsafeLoadAssembly={UseUnsafeLoadAssembly})", applicationPartManager, assemblyFile, useUnsafeLoadAssembly);
        //try to load a assembly
        Assembly assembly;

        try
        {
            assembly = Assembly.LoadFrom(assemblyFile);
        }
        catch (FileLoadException)
        {
			_logger.LogError("Exception in AddApplicationParts(applicationPartManager={ApplicationPartManager},assemblyFile={AssemblyFile},useUnsafeLoadAssembly={UseUnsafeLoadAssembly}): {Arg3}", applicationPartManager, assemblyFile, useUnsafeLoadAssembly, FileLoadException.getMessage());
            if (useUnsafeLoadAssembly)
            {
                //if an application has been copied from the web, it is flagged by Windows as being a web application,
                //even if it resides on the local computer.You can change that designation by changing the file properties,
                //or you can use the<loadFromRemoteSources> element to grant the assembly full trust.As an alternative,
                //you can use the UnsafeLoadFrom method to load a local assembly that the operating system has flagged as
                //having been loaded from the web.
                //see http://go.microsoft.com/fwlink/?LinkId=155569 for more information.
                assembly = Assembly.UnsafeLoadFrom(assemblyFile);
            }
            else
                throw;
        }

        //register the plugin definition
        applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));

		_logger.LogDebug("<<< Exiting AddApplicationParts(applicationPartManager={ApplicationPartManager},assemblyFile={AssemblyFile},useUnsafeLoadAssembly={UseUnsafeLoadAssembly})", applicationPartManager, assemblyFile, useUnsafeLoadAssembly);
        return assembly;
    }

    /// <summary>
    /// Perform file deploy and return loaded assembly
    /// </summary>
    /// <param name="applicationPartManager">Application part manager</param>
    /// <param name="assemblyFile">Path to the plugin assembly file</param>
    /// <param name="pluginConfig">Plugin config</param>
    /// <param name="fileProvider">Nop file provider</param>
    /// <returns>Assembly</returns>
    private static Assembly PerformFileDeploy(this ApplicationPartManager applicationPartManager,
        string assemblyFile, PluginConfig pluginConfig, INopFileProvider fileProvider)
    {
		_logger.LogDebug(">>> Entering PerformFileDeploy(applicationPartManager={ApplicationPartManager},assemblyFile={AssemblyFile},pluginConfig={PluginConfig},fileProvider={FileProvider})", applicationPartManager, assemblyFile, pluginConfig, fileProvider);
        //ensure for proper directory structure
        if (string.IsNullOrEmpty(assemblyFile) ||
            string.IsNullOrEmpty(fileProvider.GetParentDirectory(assemblyFile)))
			_logger.LogWarning("Throwing InvalidOperationException from PerformFileDeploy(applicationPartManager={ApplicationPartManager},assemblyFile={AssemblyFile},pluginConfig={PluginConfig},fileProvider={FileProvider})", applicationPartManager, assemblyFile, pluginConfig, fileProvider);
            throw new InvalidOperationException(
                $"The plugin directory for the {fileProvider.GetFileName(assemblyFile)} file exists in a directory outside of the allowed nopCommerce directory hierarchy");

        var assembly =
            AddApplicationParts(applicationPartManager, assemblyFile, pluginConfig.UseUnsafeLoadAssembly);

        // delete the .deps file
        if (assemblyFile.EndsWith(".dll"))
            _fileProvider.DeleteFile(assemblyFile[0..^4] + ".deps.json");

        if (!_pluginLibraries.ContainsKey(fileProvider.GetFileName(assemblyFile)))
            _pluginLibraries.Add(fileProvider.GetFileName(assemblyFile), assembly.GetName().Version);

		_logger.LogDebug("<<< Exiting PerformFileDeploy(applicationPartManager={ApplicationPartManager},assemblyFile={AssemblyFile},pluginConfig={PluginConfig},fileProvider={FileProvider})", applicationPartManager, assemblyFile, pluginConfig, fileProvider);
        return assembly;
    }

    /// <summary>
    /// Check whether the assembly is already loaded
    /// </summary>
    /// <param name="filePath">Assembly file path</param>
    /// <param name="pluginName">Plugin system name</param>
    /// <returns>Result of check</returns>
    private static bool IsAlreadyLoaded(string filePath, string pluginName)
    {
		_logger.LogDebug(">>> Entering IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName})", filePath, pluginName);
        //ignore already loaded libraries
        //(we do it because not all libraries are loaded immediately after application start)
        var fileName = _fileProvider.GetFileName(filePath);
		_logger.LogDebug("IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName}): fileName → {FileName}", filePath, pluginName, fileName);
        if (_baseAppLibraries.Any(library => library.Key.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)))
            return true;

        try
        {
            //get filename without extension
            var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(filePath);
			_logger.LogDebug("IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName}): fileNameWithoutExtension → {FileNameWithoutExtension}", filePath, pluginName, fileNameWithoutExtension);
            if (string.IsNullOrEmpty(fileNameWithoutExtension))
				_logger.LogWarning("Throwing Exception from IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName})", filePath, pluginName);
                throw new Exception($"Cannot get file extension for {fileName}");

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                //compare assemblies by file names
                var assemblyName = (assembly.FullName ?? string.Empty).Split(',').FirstOrDefault();
				_logger.LogDebug("IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName}): assemblyName → {AssemblyName}", filePath, pluginName, assemblyName);
                if (!fileNameWithoutExtension.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                //loaded assembly not found
                if (!_loadedAssemblies.TryGetValue(assemblyName, out var pluginLoadedAssemblyInfo))
                {
                    //add it to the list to find collisions later
                    pluginLoadedAssemblyInfo = new PluginLoadedAssemblyInfo(assemblyName, GetAssemblyVersion(assembly.Location));
                    _loadedAssemblies.Add(assemblyName, pluginLoadedAssemblyInfo);
					_logger.LogDebug("IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName}): pluginLoadedAssemblyInfo → {PluginLoadedAssemblyInfo}", filePath, pluginName, pluginLoadedAssemblyInfo);
                }

                //set assembly name and plugin name for further using
                pluginLoadedAssemblyInfo.References.Add((pluginName, GetAssemblyVersion(filePath)));

                return true;
            }
        }
        catch
        {
			_logger.LogError("Exception in IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName}): {Arg2}", filePath, pluginName, FileLoadException.Message);
            // ignored
        }

        //nothing found
		_logger.LogDebug("<<< Exiting IsAlreadyLoaded(filePath={FilePath},pluginName={PluginName})", filePath, pluginName);
        return false;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize plugins system
    /// </summary>
    /// <param name="applicationPartManager">Application part manager</param>
    /// <param name="pluginConfig">Plugin config</param>
    public static void InitializePlugins(this ApplicationPartManager applicationPartManager, PluginConfig pluginConfig)
    {
		_logger.LogDebug(">>> Entering InitializePlugins(applicationPartManager={ApplicationPartManager},pluginConfig={PluginConfig})", applicationPartManager, pluginConfig);
        ArgumentNullException.ThrowIfNull(applicationPartManager);

        ArgumentNullException.ThrowIfNull(pluginConfig);

        //perform with locked access to resources
        using (new ReaderWriteLockDisposable(_locker))
		using Microsoft.Extensions.Logging;
        {
            try
            {
				private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
                //ensure plugins directory is created
                var pluginsDirectory = _fileProvider.MapPath(NopPluginDefaults.Path);
                _fileProvider.CreateDirectory(pluginsDirectory);

                //ensure uploaded directory is created
                var uploadedPath = _fileProvider.MapPath(NopPluginDefaults.UploadedPath);
                _fileProvider.CreateDirectory(uploadedPath);

                foreach (var directory in _fileProvider.GetDirectories(uploadedPath))
                {
                    var moveTo = _fileProvider.Combine(pluginsDirectory, _fileProvider.GetDirectoryNameOnly(directory));

                    if (_fileProvider.DirectoryExists(moveTo))
                        _fileProvider.DeleteDirectory(moveTo);

                    _fileProvider.DirectoryMove(directory, moveTo);
                }

                PluginsInfo = new PluginsInfo(_fileProvider);
                PluginsInfo.LoadPluginInfo();

                foreach (var pluginDescriptor in PluginsInfo.PluginDescriptors.Where(p => p.needToDeploy)
                             .Select(p => p.pluginDescriptor))
                {
                    var mainPluginFile = pluginDescriptor.OriginalAssemblyFile;

                    //try to deploy main plugin assembly 
                    pluginDescriptor.ReferencedAssembly =
                        applicationPartManager.PerformFileDeploy(mainPluginFile, pluginConfig, _fileProvider);

                    //and then deploy all other referenced assemblies
                    var filesToDeploy = pluginDescriptor.PluginFiles.Where(file =>
                        !_fileProvider.GetFileName(file).Equals(_fileProvider.GetFileName(mainPluginFile)) &&
                        !IsAlreadyLoaded(file, pluginDescriptor.SystemName)).ToList();

                    foreach (var file in filesToDeploy)
                        applicationPartManager.PerformFileDeploy(file, pluginConfig, _fileProvider);

                    //determine a plugin type (only one plugin per assembly is allowed)
                    var pluginType = pluginDescriptor.ReferencedAssembly.GetTypes().FirstOrDefault(type =>
                        typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && type.IsClass &&
                        !type.IsAbstract);

                    if (pluginType != default)
                        pluginDescriptor.PluginType = pluginType;
                }


                var assemblies = _baseAppLibraries.ToList();
                foreach (var pluginLoadedAssemblyInfo in _loadedAssemblies)
                    assemblies.Add(new KeyValuePair<string, Version>(pluginLoadedAssemblyInfo.Key, pluginLoadedAssemblyInfo.Value.AssemblyInMemory));

                foreach (var pluginLibrary in _pluginLibraries.Where(item => !assemblies.Any(p => p.Key.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase))).ToList())
                    assemblies.Add(new KeyValuePair<string, Version>(pluginLibrary.Key, pluginLibrary.Value));

                var inMemoryAssemblies = assemblies.GroupBy(p => p.Key).Select(p => p.First())
                    .ToDictionary(p => p.Key.ToLower(), p => p.Value);

                foreach (var pluginDescriptor in PluginsInfo.PluginDescriptors.Where(p => !p.needToDeploy)
                             .Select(p => p.pluginDescriptor).ToList())
                    CheckCompatible(pluginDescriptor, inMemoryAssemblies);
            }
            catch (Exception exception)
            {
				_logger.LogError("Exception in InitializePlugins(applicationPartManager={ApplicationPartManager},pluginConfig={PluginConfig}): {Arg2}", applicationPartManager, pluginConfig, exception.Message);
                //throw full exception
                var message = string.Empty;
                for (var inner = exception; inner != null; inner = inner.InnerException)
                    message = $"{message}{inner.Message}{Environment.NewLine}";

				_logger.LogWarning("Throwing Exception from InitializePlugins(applicationPartManager={ApplicationPartManager},pluginConfig={PluginConfig})", applicationPartManager, pluginConfig);
                throw new Exception(message, exception);
            }

            PluginsInfo.AssemblyLoadedCollision = _loadedAssemblies.Select(item => item.Value)
                .Where(loadedAssemblyInfo => loadedAssemblyInfo.Collisions.Any()).ToList();

            //add name compatibility types from plugins
            var nameCompatibilityList = PluginsInfo.PluginDescriptors.Where(pd => pd.pluginDescriptor.Installed).SelectMany(pd => pd
                .pluginDescriptor.ReferencedAssembly.GetTypes().Where(type =>
                    typeof(INameCompatibility).IsAssignableFrom(type) && !type.IsInterface && type.IsClass &&
                    !type.IsAbstract));
            NameCompatibilityManager.AdditionalNameCompatibilities.AddRange(nameCompatibilityList);
        }
		_logger.LogDebug("<<< Exiting InitializePlugins(applicationPartManager={ApplicationPartManager},pluginConfig={PluginConfig})", applicationPartManager, pluginConfig);
    }

    #endregion
}