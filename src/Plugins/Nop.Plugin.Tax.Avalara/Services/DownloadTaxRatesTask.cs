using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents a schedule task to download tax rates
/// </summary>
public class DownloadTaxRatesTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;

    #endregion

    #region Ctor

    public DownloadTaxRatesTask(AvalaraTaxManager avalaraTaxManager)
    {
		_logger.LogError(">>> Entering DownloadTaxRatesTask(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
        _avalaraTaxManager = avalaraTaxManager;
		_logger.LogDebug("DownloadTaxRatesTask(avalaraTaxManager={AvalaraTaxManager}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, _avalaraTaxManager);
    _logger.LogWarning("<<< Exiting DownloadTaxRatesTask(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {
		_logger.LogError(">>> Entering ExecuteAsync()");
        await _avalaraTaxManager.DownloadTaxRatesAsync();
		_logger.LogWarning("<<< Exiting ExecuteAsync()");
    }

    #endregion
}