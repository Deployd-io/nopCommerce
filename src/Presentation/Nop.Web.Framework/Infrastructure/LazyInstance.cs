using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Provides support for lazy initialization
/// </summary>
/// <typeparam name="T">Specifies the type of element being lazily initialized.</typeparam>
public partial class LazyInstance<T> : Lazy<T> where T : class
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public LazyInstance()
        : base(() => EngineContext.Current.Resolve<T>())
    {
	_logger.LogError(">>> Entering LazyInstance()");

	_logger.LogWarning("<<< Exiting LazyInstance()");
    }
}