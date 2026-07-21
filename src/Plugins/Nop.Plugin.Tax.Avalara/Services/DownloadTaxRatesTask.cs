using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents a schedule task to download tax rates
/// </summary>
public class DownloadTaxRatesTask : IScheduleTask
{
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;

    #endregion

    #region Ctor

    public DownloadTaxRatesTask(AvalaraTaxManager avalaraTaxManager)
    {
		logger.LogDebug(">>> Entering DownloadTaxRatesTask(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
        _avalaraTaxManager = avalaraTaxManager;
		logger.LogDebug("<<< Exiting DownloadTaxRatesTask(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
		logger.LogDebug("DownloadTaxRatesTask(avalaraTaxManager={AvalaraTaxManager}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, _avalaraTaxManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {
		logger.LogDebug(">>> Entering ExecuteAsync()");
        await _avalaraTaxManager.DownloadTaxRatesAsync();
		logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}
