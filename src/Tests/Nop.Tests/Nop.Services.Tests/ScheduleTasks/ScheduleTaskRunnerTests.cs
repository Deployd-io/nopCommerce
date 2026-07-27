using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskRunnerTests : ServiceTest
{
    private IScheduleTaskService _scheduleTaskService;
    private ScheduleTask _task;
    private IScheduleTaskRunner _scheduleTaskRunner;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _scheduleTaskService = GetService<IScheduleTaskService>();
        _task = new ScheduleTask
		logger.LogDebug("SetUp(): _scheduleTaskService → {_scheduleTaskService}", _scheduleTaskService);
		logger.LogDebug("SetUp(): _task → {_task}", _task);
        {
            Enabled = true,
            Seconds = 600,
            Name = "test schedule task",
            Type = typeof(TestScheduleTask).FullName
        };

        await _scheduleTaskService.InsertTaskAsync(_task);

        _scheduleTaskRunner = GetService<IScheduleTaskRunner>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _scheduleTaskRunner → {_scheduleTaskRunner}", _scheduleTaskRunner);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _scheduleTaskService.DeleteTaskAsync(_task);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanExecute()
    {
		logger.LogDebug(">>> Entering CanExecute()");
        _task.Enabled = true;
        TestScheduleTask.ResetInitFlag();
		logger.LogDebug("CanExecute(): _task.Enabled → {Arg0}", _task.Enabled);
        TestScheduleTask.IsInit.Should().BeFalse();
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanExecute()");
    }

    [Test]
    public async Task CanNotExecuteNotEnabledTask()
    {
		logger.LogDebug(">>> Entering CanNotExecuteNotEnabledTask()");
        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        _task.Enabled = false;
		logger.LogDebug("CanNotExecuteNotEnabledTask(): _task.Enabled → {Arg0}", _task.Enabled);
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeFalse();
		logger.LogDebug("<<< Exiting CanNotExecuteNotEnabledTask()");
    }

    [Test]
    public async Task CanExecuteNotEnabledTaskIfForceRunFlagSet()
    {
		logger.LogDebug(">>> Entering CanExecuteNotEnabledTaskIfForceRunFlagSet()");
        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        _task.Enabled = false;
		logger.LogDebug("CanExecuteNotEnabledTaskIfForceRunFlagSet(): _task.Enabled → {Arg0}", _task.Enabled);
        await _scheduleTaskRunner.ExecuteAsync(_task, true, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanExecuteNotEnabledTaskIfForceRunFlagSet()");
    }

    [Test]
    public void ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet()
    {
		logger.LogDebug(">>> Entering ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet()");
        _task.Enabled = true;
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskRunner.ExecuteAsync(_task, throwException: true, ensureRunOncePerPeriod: false).Wait());
			logger.LogDebug("ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet(): _task.Enabled → {Arg0}", _task.Enabled);
    }

    [Test]
    public async Task CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()
    {
		logger.LogDebug(">>> Entering CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()");
        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        _task.Enabled = true;
		logger.LogDebug("CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet(): _task.Enabled → {Arg0}", _task.Enabled);
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
        TestScheduleTask.IsInit.Should().BeTrue();

        TestScheduleTask.ResetInitFlag();
        TestScheduleTask.IsInit.Should().BeFalse();
        await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: true);
        TestScheduleTask.IsInit.Should().BeFalse();
		logger.LogDebug("<<< Exiting CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()");
    }
}