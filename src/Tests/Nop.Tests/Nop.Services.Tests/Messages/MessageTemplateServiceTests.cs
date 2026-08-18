using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class MessageTemplateServiceTests : ServiceTest<MessageTemplate>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IMessageTemplateService _messageTemplateService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
		_logger.LogDebug(">>> Entering OneTimeSetUp()");
        _messageTemplateService = GetService<IMessageTemplateService>();
		_logger.LogDebug("OneTimeSetUp(): _messageTemplateService → {_messageTemplateService}", _messageTemplateService);
		_logger.LogDebug("<<< Exiting OneTimeSetUp()");
    }
    
    [Test]
    public async Task CanGetAllMessageTemplates()
    {
		_logger.LogDebug(">>> Entering CanGetAllMessageTemplates()");
        var templates = await _messageTemplateService.GetAllMessageTemplatesAsync(1);
        templates.Count.Should().BeGreaterThan(0);
		_logger.LogDebug("CanGetAllMessageTemplates(): templates → {Templates}", templates);
        templates = await _messageTemplateService.GetAllMessageTemplatesAsync(0, "NOT_EXISTS_ONE");
        templates.Count.Should().Be(0);
        templates = await _messageTemplateService.GetAllMessageTemplatesAsync(0, "Email validation");
        templates.Count.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanGetAllMessageTemplates()");
    }

    [Test]
    public async Task CanCopyMessageTemplate()
    {
		_logger.LogDebug(">>> Entering CanCopyMessageTemplate()");
        var initMessage = new MessageTemplate { Body = "Test body", Name = "Test template" };

		_logger.LogDebug("CanCopyMessageTemplate(): initMessage → {InitMessage}", initMessage);
        var message = await _messageTemplateService.CopyMessageTemplateAsync(initMessage);
		_logger.LogDebug("CanCopyMessageTemplate(): message → {Message}", message);
        message.Should().NotBeNull();
        message.Should().NotBeEquivalentTo(initMessage);
        message.Body.Should().BeEquivalentTo(initMessage.Body);
        message.Name.Should().BeEquivalentTo(initMessage.Name);
		_logger.LogDebug("<<< Exiting CanCopyMessageTemplate()");
    }

    protected override CrudData<MessageTemplate> CrudData
    {
        get
        {
            var basTemplate = new MessageTemplate { Body = "Test body", Name = "Test template" };
            var updatedTemplate = new MessageTemplate { Body = "Test body", Name = "Test template", Subject = "Test subject" };

            return new CrudData<MessageTemplate>
            {
                BaseEntity = basTemplate,
                UpdatedEntity = updatedTemplate,
                Insert = _messageTemplateService.InsertMessageTemplateAsync,
                Update = _messageTemplateService.UpdateMessageTemplateAsync,
                Delete = _messageTemplateService.DeleteMessageTemplateAsync,
                GetById = _messageTemplateService.GetMessageTemplateByIdAsync,
                IsEqual = (first, second) => first.Body.Equals(second.Body) && first.Name.Equals(second.Name) && first.Subject.Equals(second.Subject)
            };
        }
    }
}