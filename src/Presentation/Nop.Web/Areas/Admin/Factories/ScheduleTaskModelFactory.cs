using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Services.ScheduleTasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Models.Extensions;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the schedule task model factory implementation
/// </summary>
public partial class ScheduleTaskModelFactory : IScheduleTaskModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IScheduleTaskService _scheduleTaskService;

    #endregion

    #region Ctor

    public ScheduleTaskModelFactory(IDateTimeHelper dateTimeHelper,
        IScheduleTaskService scheduleTaskService)
    {
        _dateTimeHelper = dateTimeHelper;
        _scheduleTaskService = scheduleTaskService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare schedule task search model
    /// </summary>
    /// <param name="searchModel">Schedule task search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the schedule task search model
    /// </returns>
    public virtual Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareScheduleTaskSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareScheduleTaskSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged schedule task list model
    /// </summary>
    /// <param name="searchModel">Schedule task search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the schedule task list model
    /// </returns>
    public virtual async Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareScheduleTaskListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get schedule tasks
        var scheduleTasks = (await _scheduleTaskService.GetAllTasksAsync(true))
            .Where(task => !string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase))
            .ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new ScheduleTaskListModel().PrepareToGridAsync(searchModel, scheduleTasks, () =>
        {
            return scheduleTasks.SelectAwait(async scheduleTask =>
            {
                //fill in model values from the entity
                var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

				_logger.LogDebug("PrepareScheduleTaskListModelAsync(searchModel={SearchModel}): scheduleTaskModel → {ScheduleTaskModel}", searchModel, scheduleTaskModel);
                //convert dates to the user time
                if (scheduleTask.LastStartUtc.HasValue)
                {
                    scheduleTaskModel.LastStartUtc = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc)).ToString("G");
                }

                if (scheduleTask.LastEndUtc.HasValue)
                {
                    scheduleTaskModel.LastEndUtc = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc)).ToString("G");
                }

                if (scheduleTask.LastSuccessUtc.HasValue)
                {
                    scheduleTaskModel.LastSuccessUtc = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc)).ToString("G");
                }

                return scheduleTaskModel;
            });
        });
		_logger.LogDebug("<<< Exiting PrepareScheduleTaskListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}