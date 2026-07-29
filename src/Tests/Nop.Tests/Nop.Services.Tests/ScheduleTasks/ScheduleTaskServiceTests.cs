using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskServiceTests : ServiceTest
{
    private IScheduleTaskService _scheduleTaskService;
    private ScheduleTask _task;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _scheduleTaskService = GetService<IScheduleTaskService>();
        _task = new ScheduleTask
		logger.LogDebug("SetUp(): _scheduleTaskService → {_scheduleTaskService}", _scheduleTaskService);
		logger.LogDebug("SetUp(): _task → {_task}", _task);
        {
            Enabled = false,
            Seconds = 1,
            Name = "test schedule task",
            Type = typeof(TestScheduleTask).FullName
        };

        await _scheduleTaskService.InsertTaskAsync(_task);
		logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _scheduleTaskService.DeleteTaskAsync(_task);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    #region CRUD tests

    [Test]
    public async Task CanInsertAndGetTask()
    {
		logger.LogDebug(">>> Entering CanInsertAndGetTask()");
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
		logger.LogDebug("CanInsertAndGetTask(): _task.Id → {Arg0}", _task.Id);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        await _scheduleTaskService.DeleteTaskAsync(_task);
		logger.LogDebug("CanInsertAndGetTask(): task → {Task}", task);

        _task.Id.Should().NotBe(0);
        task.Id.Should().Be(_task.Id);
        task.Name.Should().Be(_task.Name);
		logger.LogDebug("<<< Exiting CanInsertAndGetTask()");
    }

    [Test]
    public void InsertTaskShouldRaiseExceptionIfTaskIsNull()
    {
		logger.LogDebug(">>> Entering InsertTaskShouldRaiseExceptionIfTaskIsNull()");
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.InsertTaskAsync(null).Wait());
			logger.LogDebug("<<< Exiting InsertTaskShouldRaiseExceptionIfTaskIsNull()");
    }

    [Test]
    public async Task GetTaskByIdShouldReturnNullIfTaskIdIsZero()
    {
		logger.LogDebug(">>> Entering GetTaskByIdShouldReturnNullIfTaskIdIsZero()");
        var task = await _scheduleTaskService.GetTaskByIdAsync(0);
        task.Should().BeNull();
		logger.LogDebug("<<< Exiting GetTaskByIdShouldReturnNullIfTaskIdIsZero()");
		logger.LogDebug("GetTaskByIdShouldReturnNullIfTaskIdIsZero(): task → {Task}", task);
    }

    [Test]
    public async Task GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()
    {
		logger.LogDebug(">>> Entering GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()");
        var task = await _scheduleTaskService.GetTaskByIdAsync(int.MaxValue);
        task.Should().BeNull();
		logger.LogDebug("<<< Exiting GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()");
		logger.LogDebug("GetTaskByIdShouldReturnNullIfTaskIdIsNotExists(): task → {Task}", task);
    }

    [Test]
    public async Task CanUpdateTask()
    {
		logger.LogDebug(">>> Entering CanUpdateTask()");
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
		logger.LogDebug("CanUpdateTask(): _task.Id → {Arg0}", _task.Id);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        task.Name = "new test name";
		logger.LogDebug("CanUpdateTask(): task → {Task}", task);
		logger.LogDebug("CanUpdateTask(): task.Name → {Arg0}", task.Name);
        await _scheduleTaskService.UpdateTaskAsync(task);
        var task2 = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        await _scheduleTaskService.DeleteTaskAsync(_task);
		logger.LogDebug("CanUpdateTask(): task2 → {Task2}", task2);

        task.Id.Should().Be(task2.Id);
        task2.Name.Should().NotBe(_task.Name);
		logger.LogDebug("<<< Exiting CanUpdateTask()");
    }

    [Test]
    public void UpdateTaskShouldRaiseExceptionIfTaskIsNull()
    {
		logger.LogDebug(">>> Entering UpdateTaskShouldRaiseExceptionIfTaskIsNull()");
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.UpdateTaskAsync(null).Wait());
			logger.LogDebug("<<< Exiting UpdateTaskShouldRaiseExceptionIfTaskIsNull()");
    }

    public async Task CanDeleteTask()
    {
		logger.LogDebug(">>> Entering CanDeleteTask()");
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
		logger.LogDebug("CanDeleteTask(): _task.Id → {Arg0}", _task.Id);
        await _scheduleTaskService.DeleteTaskAsync(_task);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        task.Should().BeNull();
		logger.LogDebug("<<< Exiting CanDeleteTask()");
		logger.LogDebug("CanDeleteTask(): task → {Task}", task);
    }

    [Test]
    public void DeleteTaskShouldRaiseExceptionIfTaskIsNull()
    {
		logger.LogDebug(">>> Entering DeleteTaskShouldRaiseExceptionIfTaskIsNull()");
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.DeleteTaskAsync(null).Wait());
			logger.LogDebug("<<< Exiting DeleteTaskShouldRaiseExceptionIfTaskIsNull()");
    }

    #endregion

    [Test]
    public async Task CanGetTaskByType()
    {
		logger.LogDebug(">>> Entering CanGetTaskByType()");
        var task = await _scheduleTaskService.GetTaskByTypeAsync(typeof(TestScheduleTask).FullName);

		logger.LogDebug("CanGetTaskByType(): task → {Task}", task);
        task.Id.Should().Be(_task.Id);
        task.Name.Should().Be(_task.Name);
		logger.LogDebug("<<< Exiting CanGetTaskByType()");
    }

    [Test]
    public async Task GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()
    {
		logger.LogDebug(">>> Entering GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()");
        var task = await _scheduleTaskService.GetTaskByTypeAsync(null);
        task.Should().BeNull();
		logger.LogDebug("GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists(): task → {Task}", task);

        task = await _scheduleTaskService.GetTaskByTypeAsync(string.Empty);
        task.Should().BeNull();

        task = await _scheduleTaskService.GetTaskByTypeAsync("not exists task type");
        task.Should().BeNull();
		logger.LogDebug("<<< Exiting GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()");
    }

    [Test]
    public async Task CanGetAllTasksAsync()
    {
		logger.LogDebug(">>> Entering CanGetAllTasksAsync()");
        var tasks = await _scheduleTaskService.GetAllTasksAsync();

		logger.LogDebug("CanGetAllTasksAsync(): tasks → {Tasks}", tasks);
        tasks.Count.Should().Be(5);
        tasks.Any(p => p.Enabled == false).Should().BeFalse();
        tasks.Any(p => p.Id == _task.Id).Should().BeFalse();

        tasks = await _scheduleTaskService.GetAllTasksAsync(true);

        tasks.Count.Should().Be(9);
        tasks.Any(p => p.Enabled).Should().BeTrue();
        tasks.Any(p => p.Id == _task.Id).Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetAllTasksAsync()");
    }
}