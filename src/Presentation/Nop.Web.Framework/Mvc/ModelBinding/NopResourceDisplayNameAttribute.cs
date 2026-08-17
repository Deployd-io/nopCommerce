using System.ComponentModel;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.ModelBinding;

/// <summary>
/// Represents model property attribute that specifies the display name by passed key of the locale resource
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NopResourceDisplayNameAttribute : DisplayNameAttribute, IModelAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private string _resourceValue = string.Empty;

    #endregion

    #region Ctor

    /// <summary>
    /// Create instance of the attribute
    /// </summary>
    /// <param name="resourceKey">Key of the locale resource</param>
    public NopResourceDisplayNameAttribute(string resourceKey) : base(resourceKey)
    {
		_logger.LogDebug(">>> Entering NopResourceDisplayNameAttribute(resourceKey={ResourceKey})", resourceKey);
        ResourceKey = resourceKey;
		_logger.LogDebug("<<< Exiting NopResourceDisplayNameAttribute(resourceKey={ResourceKey})", resourceKey);
		_logger.LogDebug("NopResourceDisplayNameAttribute(resourceKey={ResourceKey}): ResourceKey → {ResourceKey1}", resourceKey, ResourceKey);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets key of the locale resource 
    /// </summary>
    public string ResourceKey { get; set; }

    /// <summary>
    /// Gets the display name
    /// </summary>
    public override string DisplayName
    {
        get
        {
            //get locale resource value
            _resourceValue = EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync(ResourceKey).Result;

            return _resourceValue;
        }
    }

    /// <summary>
    /// Gets name of the attribute
    /// </summary>
    public string Name => nameof(NopResourceDisplayNameAttribute);

    #endregion
}