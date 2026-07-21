using Nop.Services.ScheduleTasks;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

public class TestScheduleTask : IScheduleTask
{
    public TestScheduleTask()
    {
		logger.LogDebug(">>> Entering TestScheduleTask()");
        IsInit = true;
		logger.LogDebug("<<< Exiting TestScheduleTask()");
		logger.LogDebug("TestScheduleTask(): IsInit → {IsInit}", IsInit);
    }

    public Task ExecuteAsync()
    {
		logger.LogDebug(">>> Entering ExecuteAsync()");
        throw new System.NotImplementedException();
    }

    public static bool IsInit { get; protected set; }

    public static void ResetInitFlag()
    {
		logger.LogDebug(">>> Entering ResetInitFlag()");
        IsInit = false;
		logger.LogDebug("<<< Exiting ResetInitFlag()");
		logger.LogDebug("ResetInitFlag(): IsInit → {IsInit}", IsInit);
    }
}
