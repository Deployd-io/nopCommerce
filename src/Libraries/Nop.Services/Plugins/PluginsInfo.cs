using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Plugins;

/// <summary>
/// Represents an information about plugins
/// </summary>
public partial class PluginsInfo : IPluginsInfo
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected const string OBSOLETE_FIELD = "Obsolete field, using only for compatibility";
    protected List<string> _installedPluginNames = new();
    protected IList<PluginDescriptorBaseInfo> _installedPlugins = new List<PluginDescriptorBaseInfo>();

    protected readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    public PluginsInfo(INopFileProvider fileProvider)
    {
		_logger.LogError(">>> Entering PluginsInfo(fileProvider={FileProvider})", fileProvider);
        _fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
		_logger.LogDebug("PluginsInfo(fileProvider={FileProvider}): _fileProvider → {_fileProvider}", fileProvider, _fileProvider);
    _logger.LogWarning("<<< Exiting PluginsInfo(fileProvider={FileProvider})", fileProvider);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get system names of installed plugins from obsolete file
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of plugin system names
    /// </returns>
    protected virtual IList<string> GetObsoleteInstalledPluginNames()
    {
		_logger.LogError(">>> Entering GetObsoleteInstalledPluginNames()");
        //check whether file exists
        var filePath = _fileProvider.MapPath(NopPluginDefaults.InstalledPluginsFilePath);
		_logger.LogError("GetObsoleteInstalledPluginNames(): filePath → {FilePath}", filePath);
        if (!_fileProvider.FileExists(filePath))
        {
            //if not, try to parse the file that was used in previous nopCommerce versions
            filePath = _fileProvider.MapPath(NopPluginDefaults.ObsoleteInstalledPluginsFilePath);
            if (!_fileProvider.FileExists(filePath))
                return new List<string>();

            //get plugin system names from the old txt file
            var pluginSystemNames = new List<string>();
			_logger.LogError("GetObsoleteInstalledPluginNames(): pluginSystemNames → {PluginSystemNames}", pluginSystemNames);
            using (var reader = new StringReader(_fileProvider.ReadAllText(filePath, Encoding.UTF8)))
            {
                string pluginName;
                while ((pluginName = reader.ReadLine()) != null)
                    if (!string.IsNullOrWhiteSpace(pluginName))
                        pluginSystemNames.Add(pluginName.Trim());
            }

            //and delete the old one
            _fileProvider.DeleteFile(filePath);

            return pluginSystemNames;
        }

        var text = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
		_logger.LogError("GetObsoleteInstalledPluginNames(): text → {Text}", text);
        if (string.IsNullOrEmpty(text))
            return new List<string>();

        //delete the old file
        _fileProvider.DeleteFile(filePath);

        //get plugin system names from the JSON file
		_logger.LogWarning("<<< Exiting GetObsoleteInstalledPluginNames()");
        return JsonConvert.DeserializeObject<IList<string>>(text);
    }

    /// <summary>
    /// Deserialize PluginInfo from json
    /// </summary>
    /// <param name="json">Json data of PluginInfo</param>
    /// <returns>True if data are loaded, otherwise False</returns>
    protected virtual void DeserializePluginInfo(string json)
    {
		_logger.LogError(">>> Entering DeserializePluginInfo(json={Json})", json);
        if (string.IsNullOrEmpty(json))
            return;

        var pluginsInfo = JsonConvert.DeserializeObject<PluginsInfo>(json);
		_logger.LogError("DeserializePluginInfo(json={Json}): pluginsInfo → {PluginsInfo}", json, pluginsInfo);

        if (pluginsInfo == null)
            return;

        InstalledPluginNames = pluginsInfo.InstalledPluginNames;
		_logger.LogDebug("DeserializePluginInfo(json={Json}): InstalledPluginNames → {InstalledPluginNames}", json, InstalledPluginNames);
        InstalledPlugins = pluginsInfo.InstalledPlugins;
		_logger.LogDebug("DeserializePluginInfo(json={Json}): InstalledPlugins → {InstalledPlugins}", json, InstalledPlugins);
        PluginNamesToUninstall = pluginsInfo.PluginNamesToUninstall;
		_logger.LogDebug("DeserializePluginInfo(json={Json}): PluginNamesToUninstall → {PluginNamesToUninstall}", json, PluginNamesToUninstall);
        PluginNamesToDelete = pluginsInfo.PluginNamesToDelete;
		_logger.LogDebug("DeserializePluginInfo(json={Json}): PluginNamesToDelete → {PluginNamesToDelete}", json, PluginNamesToDelete);
        PluginNamesToInstall = pluginsInfo.PluginNamesToInstall;
		_logger.LogDebug("DeserializePluginInfo(json={Json}): PluginNamesToInstall → {PluginNamesToInstall}", json, PluginNamesToInstall);
		_logger.LogWarning("<<< Exiting DeserializePluginInfo(json={Json})", json);
    }

    /// <summary>
    /// Check whether the directory is a plugin directory
    /// </summary>
    /// <param name="directoryName">Directory name</param>
    /// <returns>Result of check</returns>
    protected bool IsPluginDirectory(string directoryName)
    {
		_logger.LogError(">>> Entering IsPluginDirectory(directoryName={DirectoryName})", directoryName);
        if (string.IsNullOrEmpty(directoryName))
            return false;

        //get parent directory
        var parent = _fileProvider.GetParentDirectory(directoryName);
		_logger.LogError("IsPluginDirectory(directoryName={DirectoryName}): parent → {Parent}", directoryName, parent);
        if (string.IsNullOrEmpty(parent))
            return false;

        //directory is directly in plugins directory
		_logger.LogWarning("<<< Exiting IsPluginDirectory(directoryName={DirectoryName})", directoryName);
        if (!_fileProvider.GetDirectoryNameOnly(parent).Equals(NopPluginDefaults.PathName, StringComparison.InvariantCultureIgnoreCase)) {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get list of description files-plugin descriptors pairs
    /// </summary>
    /// <param name="directoryName">Plugin directory name</param>
    /// <returns>Original and parsed description files</returns>
    protected IList<(string DescriptionFile, PluginDescriptor PluginDescriptor)> GetDescriptionFilesAndDescriptors(string directoryName)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryName);

        var result = new List<(string DescriptionFile, PluginDescriptor PluginDescriptor)>();

        //try to find description files in the plugin directory
        var files = _fileProvider.GetFiles(directoryName, NopPluginDefaults.DescriptionFileName, false);

        //populate result list
        foreach (var descriptionFile in files)
        {
            //skip files that are not in the plugin directory
            if (!IsPluginDirectory(_fileProvider.GetDirectoryName(descriptionFile)))
                continue;

            //load plugin descriptor from the file
            var text = _fileProvider.ReadAllText(descriptionFile, Encoding.UTF8);
            var pluginDescriptor = PluginDescriptor.GetPluginDescriptorFromText(text);

            result.Add((descriptionFile, pluginDescriptor));
        }

        //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
        //it's required: https://www.nopcommerce.com/boards/topic/17455/load-plugins-based-on-their-displayorder-on-startup
        result = result.OrderBy(item => item.PluginDescriptor.DisplayOrder).ToList();

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get plugins info
    /// </summary>
    /// <returns>
    /// The true if data are loaded, otherwise False
    /// </returns>
    public virtual void LoadPluginInfo()
    {
		_logger.LogError(">>> Entering LoadPluginInfo()");
        //check whether plugins info file exists
        var filePath = _fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
		_logger.LogError("LoadPluginInfo(): filePath → {FilePath}", filePath);
        if (!_fileProvider.FileExists(filePath))
        {
            //file doesn't exist, so try to get only installed plugin names from the obsolete file
            _installedPluginNames.AddRange(GetObsoleteInstalledPluginNames());

            //and save info into a new file if need
            if (_installedPluginNames.Any())
                Save();
        }

        //try to get plugin info from the JSON file
        var text = _fileProvider.FileExists(filePath)
            ? _fileProvider.ReadAllText(filePath, Encoding.UTF8)
            : string.Empty;

        DeserializePluginInfo(text);

        var pluginDescriptors = new List<(PluginDescriptor pluginDescriptor, bool needToDeploy)>();
		_logger.LogError("LoadPluginInfo(): pluginDescriptors → {PluginDescriptors}", pluginDescriptors);
        var incompatiblePlugins = new Dictionary<string, PluginIncompatibleType>();
		_logger.LogError("LoadPluginInfo(): incompatiblePlugins → {IncompatiblePlugins}", incompatiblePlugins);

        //ensure plugins directory is created
        var pluginsDirectory = _fileProvider.MapPath(NopPluginDefaults.Path);
		_logger.LogError("LoadPluginInfo(): pluginsDirectory → {PluginsDirectory}", pluginsDirectory);
        _fileProvider.CreateDirectory(pluginsDirectory);

        //load plugin descriptors from the plugin directory
        foreach (var item in GetDescriptionFilesAndDescriptors(pluginsDirectory))
        {
            var descriptionFile = item.DescriptionFile;
            var pluginDescriptor = item.PluginDescriptor;

            //skip descriptor of plugin that is going to be deleted
            if (PluginNamesToDelete.Contains(pluginDescriptor.SystemName))
                continue;

            //ensure that plugin is compatible with the current version
            if (!pluginDescriptor.SupportedVersions.Contains(NopVersion.CURRENT_VERSION, StringComparer.InvariantCultureIgnoreCase))
            {
                incompatiblePlugins.Add(pluginDescriptor.SystemName, PluginIncompatibleType.NotCompatibleWithCurrentVersion);
                continue;
            }

            //some more validation
				_logger.LogError("Throwing Exception from LoadPluginInfo()");
            if (string.IsNullOrEmpty(pluginDescriptor.SystemName?.Trim())) {
                throw new Exception($"A plugin '{descriptionFile}' has no system name. Try assigning the plugin a unique name and recompiling.");
            }

            if (pluginDescriptors.Any(p => p.pluginDescriptor.Equals(pluginDescriptor)))
                throw new Exception($"A plugin with '{pluginDescriptor.SystemName}' system name is already defined");

            //set 'Installed' property
            pluginDescriptor.Installed = InstalledPlugins.Select(pd => pd.SystemName)
                .Any(pluginName => pluginName.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase));

            try
            {
                //try to get plugin directory
                var pluginDirectory = _fileProvider.GetDirectoryName(descriptionFile);
				_logger.LogError("LoadPluginInfo(): pluginDirectory → {PluginDirectory}", pluginDirectory);
                if (string.IsNullOrEmpty(pluginDirectory))
                    throw new Exception($"Directory cannot be resolved for '{_fileProvider.GetFileName(descriptionFile)}' description file");

                //get list of all library files in the plugin directory (not in the bin one)
                pluginDescriptor.PluginFiles = _fileProvider.GetFiles(pluginDirectory, "*.dll", false)
                    .Where(file => IsPluginDirectory(_fileProvider.GetDirectoryName(file)))
                    .ToList();

                //try to find a main plugin assembly file
                var mainPluginFile = pluginDescriptor.PluginFiles.FirstOrDefault(file =>
                {
                    var fileName = _fileProvider.GetFileName(file);
					_logger.LogError("LoadPluginInfo(): fileName → {FileName}", fileName);
                    return fileName.Equals(pluginDescriptor.AssemblyFileName, StringComparison.InvariantCultureIgnoreCase);
                });

                //file with the specified name not found
                if (mainPluginFile == null)
                {
                    //so plugin is incompatible
                    incompatiblePlugins.Add(pluginDescriptor.SystemName, PluginIncompatibleType.MainAssemblyNotFound);
                    continue;
                }

                var pluginName = pluginDescriptor.SystemName;

                //if it's found, set it as original assembly file
                pluginDescriptor.OriginalAssemblyFile = mainPluginFile;
				_logger.LogDebug("LoadPluginInfo(): pluginDescriptor.OriginalAssemblyFile → {Arg0}", pluginDescriptor.OriginalAssemblyFile);

                //need to deploy if plugin is already installed
                var needToDeploy = InstalledPlugins.Select(pd => pd.SystemName).Contains(pluginName);
				_logger.LogError("LoadPluginInfo(): needToDeploy → {NeedToDeploy}", needToDeploy);

                //also, deploy if the plugin is only going to be installed now
                needToDeploy = needToDeploy || PluginNamesToInstall.Any(pluginInfo => pluginInfo.SystemName.Equals(pluginName));

                //finally, exclude from deploying the plugin that is going to be deleted
                needToDeploy = needToDeploy && !PluginNamesToDelete.Contains(pluginName);

                //mark plugin as successfully deployed
                pluginDescriptors.Add((pluginDescriptor, needToDeploy));
            }
            catch (ReflectionTypeLoadException exception)
            {
				_logger.LogError("Exception in LoadPluginInfo(): {Arg0}", exception.Message);
                //get all loader exceptions
                var error = exception.LoaderExceptions.Aggregate($"Plugin '{pluginDescriptor.FriendlyName}'. ",
                    (message, nextMessage) => $"{message}{nextMessage?.Message ?? string.Empty}{Environment.NewLine}");

                throw new Exception(error, exception);
            }
            catch (Exception exception)
            {
                //add a plugin name, this way we can easily identify a problematic plugin
                throw new Exception($"Plugin '{pluginDescriptor.FriendlyName}'. {exception.Message}", exception);
            }
        }

        IncompatiblePlugins = incompatiblePlugins;
		_logger.LogDebug("LoadPluginInfo(): IncompatiblePlugins → {IncompatiblePlugins}", IncompatiblePlugins);
        PluginDescriptors = pluginDescriptors;
		_logger.LogDebug("LoadPluginInfo(): PluginDescriptors → {PluginDescriptors}", PluginDescriptors);
		_logger.LogWarning("<<< Exiting LoadPluginInfo()");
    }


    /// <summary>
    /// Save plugins info to the file
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveAsync()
    {
		_logger.LogError(">>> Entering SaveAsync()");
        //save the file
        var filePath = _fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
		_logger.LogError("SaveAsync(): filePath → {FilePath}", filePath);
        var text = JsonConvert.SerializeObject(this, Formatting.Indented);
		_logger.LogError("SaveAsync(): text → {Text}", text);
        await _fileProvider.WriteAllTextAsync(filePath, text, Encoding.UTF8);
		_logger.LogWarning("<<< Exiting SaveAsync()");
    }

    /// <summary>
    /// Save plugins info to the file
    /// </summary>
    public virtual void Save()
    {
		_logger.LogError(">>> Entering Save()");
        //save the file
        var filePath = _fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
		_logger.LogError("Save(): filePath → {FilePath}", filePath);
        var text = JsonConvert.SerializeObject(this, Formatting.Indented);
		_logger.LogError("Save(): text → {Text}", text);
        _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
		_logger.LogWarning("<<< Exiting Save()");
    }

    /// <summary>
    /// Create copy from another instance of IPluginsInfo interface
    /// </summary>
    /// <param name="pluginsInfo">Plugins info</param>
    public virtual void CopyFrom(IPluginsInfo pluginsInfo)
    {
		_logger.LogError(">>> Entering CopyFrom(pluginsInfo={PluginsInfo})", pluginsInfo);
        InstalledPlugins = pluginsInfo.InstalledPlugins?.ToList() ?? new List<PluginDescriptorBaseInfo>();
        PluginNamesToUninstall = pluginsInfo.PluginNamesToUninstall?.ToList() ?? new List<string>();
		_logger.LogError("CopyFrom(pluginsInfo={PluginsInfo}): PluginNamesToUninstall → {PluginNamesToUninstall}", pluginsInfo, PluginNamesToUninstall);
		_logger.LogDebug("CopyFrom(pluginsInfo={PluginsInfo}): InstalledPlugins → {InstalledPlugins}", pluginsInfo, InstalledPlugins);
		_logger.LogDebug("CopyFrom(pluginsInfo={PluginsInfo}): PluginNamesToUninstall → {PluginNamesToUninstall}", pluginsInfo, PluginNamesToUninstall);
        PluginNamesToDelete = pluginsInfo.PluginNamesToDelete?.ToList() ?? new List<string>();
		_logger.LogError("CopyFrom(pluginsInfo={PluginsInfo}): PluginNamesToDelete → {PluginNamesToDelete}", pluginsInfo, PluginNamesToDelete);
        PluginNamesToInstall = pluginsInfo.PluginNamesToInstall?.ToList() ??
                               new List<(string SystemName, Guid? CustomerGuid)>();
        AssemblyLoadedCollision = pluginsInfo.AssemblyLoadedCollision?.ToList();
		_logger.LogError("CopyFrom(pluginsInfo={PluginsInfo}): AssemblyLoadedCollision → {AssemblyLoadedCollision}", pluginsInfo, AssemblyLoadedCollision);
		_logger.LogDebug("CopyFrom(pluginsInfo={PluginsInfo}): PluginNamesToDelete → {PluginNamesToDelete}", pluginsInfo, PluginNamesToDelete);
		_logger.LogError("CopyFrom(pluginsInfo={PluginsInfo}): IncompatiblePlugins → {IncompatiblePlugins}", pluginsInfo, IncompatiblePlugins);
        PluginDescriptors = pluginsInfo.PluginDescriptors;
        IncompatiblePlugins = pluginsInfo.IncompatiblePlugins?.ToDictionary(item => item.Key, item => item.Value);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the list of all installed plugin names
    /// </summary>
    public virtual IList<string> InstalledPluginNames
    {
        get
        {
            if (_installedPlugins.Any())
                _installedPluginNames.Clear();

            return _installedPluginNames.Any() ? _installedPluginNames : [OBSOLETE_FIELD];
        }
        set
        {
            if (value?.Any() ?? false)
                _installedPluginNames = value.ToList();
        }
    }

    /// <summary>
    /// Gets or sets the list of all installed plugin
    /// </summary>
    public virtual IList<PluginDescriptorBaseInfo> InstalledPlugins
    {
        get
        {
            if ((_installedPlugins?.Any() ?? false) || !_installedPluginNames.Any())
                return _installedPlugins;

            if (PluginDescriptors?.Any() ?? false)
                _installedPlugins = PluginDescriptors
                    .Where(pd => _installedPluginNames.Any(pn =>
                        pn.Equals(pd.pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(pd => pd.pluginDescriptor as PluginDescriptorBaseInfo).ToList();
            else
                return _installedPluginNames
                    .Where(name => !name.Equals(OBSOLETE_FIELD, StringComparison.InvariantCultureIgnoreCase))
                    .Select(systemName => new PluginDescriptorBaseInfo { SystemName = systemName }).ToList();

            return _installedPlugins;
        }
        set => _installedPlugins = value;
    }

    /// <summary>
    /// Gets or sets the list of plugin names which will be uninstalled
    /// </summary>
    public virtual IList<string> PluginNamesToUninstall { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the list of plugin names which will be deleted
    /// </summary>
    public virtual IList<string> PluginNamesToDelete { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the list of plugin names which will be installed
    /// </summary>
    public virtual IList<(string SystemName, Guid? CustomerGuid)> PluginNamesToInstall { get; set; } =
        new List<(string SystemName, Guid? CustomerGuid)>();


    /// <summary>
    /// Gets or sets the list of plugin which are not compatible with the current version
    /// </summary>
    /// <remarks>
    /// Key - the system name of plugin.
    /// Value - the reason of incompatibility.
    /// </remarks>
    [JsonIgnore]
    public virtual IDictionary<string, PluginIncompatibleType> IncompatiblePlugins { get; set; }

    /// <summary>
    /// Gets or sets the list of assembly loaded collisions
    /// </summary>
    [JsonIgnore]
    public virtual IList<PluginLoadedAssemblyInfo> AssemblyLoadedCollision { get; set; }

    /// <summary>
    /// Gets or sets a collection of plugin descriptors of all deployed plugins
    /// </summary>
    [JsonIgnore]
    public virtual IList<(PluginDescriptor pluginDescriptor, bool needToDeploy)> PluginDescriptors { get; set; }

    #endregion
}