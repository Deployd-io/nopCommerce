using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class TaskSchedulerTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private TestTaskScheduler _taskScheduler;
    private IRepository<ScheduleTask> _scheduleTaskRepository;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _taskScheduler = GetService<ITaskScheduler>() as TestTaskScheduler;
        _scheduleTaskRepository = GetService<IRepository<ScheduleTask>>();
		_logger.LogDebug("SetUp(): _scheduleTaskRepository → {_scheduleTaskRepository}", _scheduleTaskRepository);
		_logger.LogDebug("SetUp(): _taskScheduler → {_taskScheduler}", _taskScheduler);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var item = await _scheduleTaskRepository.GetByIdAsync(1);
        item.LastStartUtc = DateTime.UtcNow.AddSeconds(-(item.Seconds + 1));
		_logger.LogInformation("SetUp(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug("SetUp(): item → {Item}", item);
		_logger.LogInformation("SetUp(): update query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        await _scheduleTaskRepository.UpdateAsync(item);

        item = await _scheduleTaskRepository.GetByIdAsync(2);
        item.LastStartUtc = DateTime.UtcNow.AddSeconds(item.Seconds - 1);
        await _scheduleTaskRepository.UpdateAsync(item);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _taskScheduler.StopSchedulerAsync();
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanInitialize()
    {
		_logger.LogDebug(">>> Entering CanInitialize()");
        _taskScheduler.IsInit.Should().BeFalse();
        await _taskScheduler.InitializeAsync();
        _taskScheduler.IsInit.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanInitialize()");
    }

    [Test]
    public async Task CanStartStopScheduler()
    {
		_logger.LogDebug(">>> Entering CanStartStopScheduler()");
        await _taskScheduler.InitializeAsync();
        _taskScheduler.IsRun.Should().BeFalse();
        await _taskScheduler.StartSchedulerAsync();
        _taskScheduler.IsRun.Should().BeTrue();
        await _taskScheduler.StopSchedulerAsync();
        _taskScheduler.IsRun.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanStartStopScheduler()");
    }
}