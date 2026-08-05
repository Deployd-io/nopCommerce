using Microsoft.Extensions.Logging;
﻿using Nop.Services.ScheduleTasks;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

public class TestScheduleTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TestScheduleTask()
    {
		_logger.LogDebug(">>> Entering TestScheduleTask()");
        IsInit = true;
		_logger.LogDebug("<<< Exiting TestScheduleTask()");
		_logger.LogDebug("TestScheduleTask(): IsInit → {IsInit}", IsInit);
    }

    public Task ExecuteAsync()
    {
		_logger.LogDebug(">>> Entering ExecuteAsync()");
        throw new System.NotImplementedException();
    }

    public static bool IsInit { get; protected set; }

    public static void ResetInitFlag()
    {
		_logger.LogDebug(">>> Entering ResetInitFlag()");
        IsInit = false;
		_logger.LogDebug("<<< Exiting ResetInitFlag()");
		_logger.LogDebug("ResetInitFlag(): IsInit → {IsInit}", IsInit);
    }
}