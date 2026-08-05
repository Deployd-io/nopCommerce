using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskRunnerTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IScheduleTaskService _scheduleTaskService;
    private ScheduleTask _task;
    private IScheduleTaskRunner _scheduleTaskRunner;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _scheduleTaskService = GetService<IScheduleTaskService>();
        _task = new ScheduleTask
		_logger.LogDebug("SetUp(): _scheduleTaskService → {_scheduleTaskService}", _scheduleTaskService);
		_logger.LogDebug("SetUp(): _task → {_task}", _task);
        {
            Enabled = true,
            Seconds = 600,
            Name = "test schedule task",
            Type = typeof(TestScheduleTask).FullName
        };

        await _scheduleTaskService.InsertTaskAsync(_task);

        _scheduleTaskRunner = GetService<IScheduleTaskRunner>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _scheduleTaskRunner → {_scheduleTaskRunner}", _scheduleTaskRunner);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _scheduleTaskService.DeleteTaskAsync(_task);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanExecute()
    {
		_logger.LogDebug(">>> Entering CanExecute()");
        _task.Enabled = true;
        TestScheduleTask.ResetInitFlag();
		_logger.LogDebug("CanExecute(): _task.Enabled → {Arg0}", _task.Enabled);
        TestScheduleTask.IsInit.Should().BeFalse();
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanExecute()");
    }

    [Test]
    public async Task CanNotExecuteNotEnabledTask()
    {
		_logger.LogDebug(">>> Entering CanNotExecuteNotEnabledTask()");
        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        _task.Enabled = false;
		_logger.LogDebug("CanNotExecuteNotEnabledTask(): _task.Enabled → {Arg0}", _task.Enabled);
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanNotExecuteNotEnabledTask()");
    }

    [Test]
    public async Task CanExecuteNotEnabledTaskIfForceRunFlagSet()
    {
		_logger.LogDebug(">>> Entering CanExecuteNotEnabledTaskIfForceRunFlagSet()");
        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        _task.Enabled = false;
		_logger.LogDebug("CanExecuteNotEnabledTaskIfForceRunFlagSet(): _task.Enabled → {Arg0}", _task.Enabled);
        await _scheduleTaskRunner.ExecuteAsync(_task, true, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanExecuteNotEnabledTaskIfForceRunFlagSet()");
    }

    [Test]
    public void ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet()
    {
		_logger.LogDebug(">>> Entering ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet()");
        _task.Enabled = true;
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskRunner.ExecuteAsync(_task, throwException: true, ensureRunOncePerPeriod: false).Wait());
			_logger.LogDebug("ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet(): _task.Enabled → {Arg0}", _task.Enabled);
    }

    [Test]
    public async Task CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()
    {
		_logger.LogDebug(">>> Entering CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()");
        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        _task.Enabled = true;
		_logger.LogDebug("CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet(): _task.Enabled → {Arg0}", _task.Enabled);
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeTrue();

        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: true);
        TestScheduleTask.IsInit.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()");
    }
}