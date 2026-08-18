using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class NotificationServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private INotificationService _notificationService;
    private ITempDataDictionaryFactory _tempDataDictionaryFactory;
    private HttpContext _httpContext;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _notificationService = GetService<INotificationService>();
        _tempDataDictionaryFactory = GetService<ITempDataDictionaryFactory>();
		_logger.LogDebug("SetUp(): _tempDataDictionaryFactory → {_tempDataDictionaryFactory}", _tempDataDictionaryFactory);
		_logger.LogDebug("SetUp(): _notificationService → {_notificationService}", _notificationService);
        _httpContext = GetService<IHttpContextAccessor>().HttpContext;
		_logger.LogDebug("SetUp(): _httpContext → {_httpContext}", _httpContext);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    private IList<NotifyData> DeserializedDataDictionary =>
        JsonConvert.DeserializeObject<IList<NotifyData>>(_tempDataDictionaryFactory.GetTempData(_httpContext)[NopMessageDefaults.NotificationListKey].ToString());

    [Test]
    public void CanAddNotification()
    {
		_logger.LogDebug(">>> Entering CanAddNotification()");
        _tempDataDictionaryFactory.GetTempData(_httpContext).Clear();
        _notificationService.SuccessNotification("success");
        _notificationService.WarningNotification("warning");
        _notificationService.ErrorNotification("error");
        var messageList = DeserializedDataDictionary;

        messageList.Count.Should().Be(3);
        var successMsg = messageList
            .First(m => m.Type == NotifyType.Success);
        successMsg.Message.Should().Be("success");

        var warningMsg = messageList
            .First(m => m.Type == NotifyType.Warning);
        warningMsg.Message.Should().Be("warning");

        var errorMsg = messageList
            .First(m => m.Type == NotifyType.Error);
        errorMsg.Message.Should().Be("error");
		_logger.LogDebug("<<< Exiting CanAddNotification()");
    }

    [Test]
    public async Task CanAddNotificationFromException()
    {
		_logger.LogDebug(">>> Entering CanAddNotificationFromException()");
        _tempDataDictionaryFactory.GetTempData(_httpContext).Clear();
        await _notificationService.ErrorNotificationAsync(new Exception("error"));
        var msg = DeserializedDataDictionary.First();
		_logger.LogDebug("CanAddNotificationFromException(): msg → {Msg}", msg);
        msg.Message.Should().Be("error");
		_logger.LogDebug("<<< Exiting CanAddNotificationFromException()");
    }
}