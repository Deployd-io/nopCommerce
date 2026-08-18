using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IScheduleTaskService _scheduleTaskService;
    private ScheduleTask _task;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _scheduleTaskService = GetService<IScheduleTaskService>();
        _task = new ScheduleTask
		_logger.LogDebug("SetUp(): _scheduleTaskService → {_scheduleTaskService}", _scheduleTaskService);
		_logger.LogDebug("SetUp(): _task → {_task}", _task);
        {
            Enabled = false,
            Seconds = 1,
            Name = "test schedule task",
            Type = typeof(TestScheduleTask).FullName
        };

        await _scheduleTaskService.InsertTaskAsync(_task);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _scheduleTaskService.DeleteTaskAsync(_task);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    #region CRUD tests

    [Test]
    public async Task CanInsertAndGetTask()
    {
		_logger.LogDebug(">>> Entering CanInsertAndGetTask()");
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
		_logger.LogDebug("CanInsertAndGetTask(): _task.Id → {Arg0}", _task.Id);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
		_logger.LogDebug("CanInsertAndGetTask(): task → {Task}", task);
        await _scheduleTaskService.DeleteTaskAsync(_task);

        _task.Id.Should().NotBe(0);
        task.Id.Should().Be(_task.Id);
        task.Name.Should().Be(_task.Name);
		_logger.LogDebug("<<< Exiting CanInsertAndGetTask()");
    }

    [Test]
    public void InsertTaskShouldRaiseExceptionIfTaskIsNull()
    {
		_logger.LogDebug(">>> Entering InsertTaskShouldRaiseExceptionIfTaskIsNull()");
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.InsertTaskAsync(null).Wait());
			_logger.LogDebug("<<< Exiting InsertTaskShouldRaiseExceptionIfTaskIsNull()");
    }

    [Test]
    public async Task GetTaskByIdShouldReturnNullIfTaskIdIsZero()
    {
		_logger.LogDebug(">>> Entering GetTaskByIdShouldReturnNullIfTaskIdIsZero()");
        var task = await _scheduleTaskService.GetTaskByIdAsync(0);
        task.Should().BeNull();
		_logger.LogDebug("GetTaskByIdShouldReturnNullIfTaskIdIsZero(): task → {Task}", task);
		_logger.LogDebug("<<< Exiting GetTaskByIdShouldReturnNullIfTaskIdIsZero()");
    }

    [Test]
    public async Task GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()
    {
		_logger.LogDebug(">>> Entering GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()");
        var task = await _scheduleTaskService.GetTaskByIdAsync(int.MaxValue);
        task.Should().BeNull();
		_logger.LogDebug("GetTaskByIdShouldReturnNullIfTaskIdIsNotExists(): task → {Task}", task);
		_logger.LogDebug("<<< Exiting GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()");
    }

    [Test]
    public async Task CanUpdateTask()
    {
		_logger.LogDebug(">>> Entering CanUpdateTask()");
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
		_logger.LogDebug("CanUpdateTask(): _task.Id → {Arg0}", _task.Id);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
		_logger.LogDebug("CanUpdateTask(): task → {Task}", task);
		_logger.LogDebug("CanUpdateTask(): task.Name → {Arg0}", task.Name);
        task.Name = "new test name";
        await _scheduleTaskService.UpdateTaskAsync(task);
        var task2 = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
		_logger.LogDebug("CanUpdateTask(): task2 → {Task2}", task2);
        await _scheduleTaskService.DeleteTaskAsync(_task);

        task.Id.Should().Be(task2.Id);
        task2.Name.Should().NotBe(_task.Name);
		_logger.LogDebug("<<< Exiting CanUpdateTask()");
    }

    [Test]
    public void UpdateTaskShouldRaiseExceptionIfTaskIsNull()
    {
		_logger.LogDebug(">>> Entering UpdateTaskShouldRaiseExceptionIfTaskIsNull()");
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.UpdateTaskAsync(null).Wait());
			_logger.LogDebug("<<< Exiting UpdateTaskShouldRaiseExceptionIfTaskIsNull()");
    }

    public async Task CanDeleteTask()
    {
		_logger.LogDebug(">>> Entering CanDeleteTask()");
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
		_logger.LogDebug("CanDeleteTask(): _task.Id → {Arg0}", _task.Id);
        await _scheduleTaskService.DeleteTaskAsync(_task);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
		_logger.LogDebug("CanDeleteTask(): task → {Task}", task);
        task.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanDeleteTask()");
    }

    [Test]
    public void DeleteTaskShouldRaiseExceptionIfTaskIsNull()
    {
		_logger.LogDebug(">>> Entering DeleteTaskShouldRaiseExceptionIfTaskIsNull()");
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.DeleteTaskAsync(null).Wait());
			_logger.LogDebug("<<< Exiting DeleteTaskShouldRaiseExceptionIfTaskIsNull()");
    }

    #endregion

    [Test]
    public async Task CanGetTaskByType()
    {
		_logger.LogDebug(">>> Entering CanGetTaskByType()");
        var task = await _scheduleTaskService.GetTaskByTypeAsync(typeof(TestScheduleTask).FullName);

		_logger.LogDebug("CanGetTaskByType(): task → {Task}", task);
        task.Id.Should().Be(_task.Id);
        task.Name.Should().Be(_task.Name);
		_logger.LogDebug("<<< Exiting CanGetTaskByType()");
    }

    [Test]
    public async Task GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()
    {
		_logger.LogDebug(">>> Entering GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()");
        var task = await _scheduleTaskService.GetTaskByTypeAsync(null);
        task.Should().BeNull();
		_logger.LogDebug("GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists(): task → {Task}", task);

        task = await _scheduleTaskService.GetTaskByTypeAsync(string.Empty);
        task.Should().BeNull();

        task = await _scheduleTaskService.GetTaskByTypeAsync("not exists task type");
        task.Should().BeNull();
		_logger.LogDebug("<<< Exiting GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()");
    }

    [Test]
    public async Task CanGetAllTasksAsync()
    {
		_logger.LogDebug(">>> Entering CanGetAllTasksAsync()");
        var tasks = await _scheduleTaskService.GetAllTasksAsync();

		_logger.LogDebug("CanGetAllTasksAsync(): tasks → {Tasks}", tasks);
        tasks.Count.Should().Be(5);
        tasks.Any(p => p.Enabled == false).Should().BeFalse();
        tasks.Any(p => p.Id == _task.Id).Should().BeFalse();

        tasks = await _scheduleTaskService.GetAllTasksAsync(true);

        tasks.Count.Should().Be(9);
        tasks.Any(p => p.Enabled).Should().BeTrue();
        tasks.Any(p => p.Id == _task.Id).Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetAllTasksAsync()");
    }
}