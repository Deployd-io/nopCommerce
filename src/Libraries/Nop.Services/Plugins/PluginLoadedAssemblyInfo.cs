using Microsoft.Extensions.Logging;
﻿namespace Nop.Services.Plugins;

/// <summary>
/// Represents an information about assembly which loaded by plugins
/// </summary>
public partial class PluginLoadedAssemblyInfo
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shortName">Assembly short name</param>
    /// <param name="assemblyInMemory">Assembly version</param>
    public PluginLoadedAssemblyInfo(string shortName, Version assemblyInMemory)
    {
		_logger.LogDebug(">>> Entering PluginLoadedAssemblyInfo(shortName={ShortName},assemblyInMemory={AssemblyInMemory})", shortName, assemblyInMemory);
        ShortName = shortName;
        References = new List<(string PluginName, Version AssemblyVersion)>();
		_logger.LogDebug("PluginLoadedAssemblyInfo(shortName={ShortName},assemblyInMemory={AssemblyInMemory}): References → {References}", shortName, assemblyInMemory, References);
        AssemblyInMemory = assemblyInMemory;
		_logger.LogDebug("<<< Exiting PluginLoadedAssemblyInfo(shortName={ShortName},assemblyInMemory={AssemblyInMemory})", shortName, assemblyInMemory);
		_logger.LogDebug("PluginLoadedAssemblyInfo(shortName={ShortName},assemblyInMemory={AssemblyInMemory}): AssemblyInMemory → {AssemblyInMemory1}", shortName, assemblyInMemory, AssemblyInMemory);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Assembly in memory
    /// </summary>
    public Version AssemblyInMemory { get; }

    /// <summary>
    /// Gets the short assembly name
    /// </summary>
    public string ShortName { get; }

    /// <summary>
    /// Gets a list of all mentioned plugin-assembly pairs
    /// </summary>
    public List<(string PluginName, Version AssemblyVersion)> References { get; }

    /// <summary>
    /// Gets a list of plugins that conflict with the loaded assembly version
    /// </summary>
    public IList<(string PluginName, Version AssemblyVersion)> Collisions =>
        References.Where(reference => !reference.AssemblyVersion.Equals(AssemblyInMemory)).ToList();

    #endregion
}