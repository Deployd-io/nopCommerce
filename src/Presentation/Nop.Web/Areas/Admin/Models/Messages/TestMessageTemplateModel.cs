using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

public partial record TestMessageTemplateModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TestMessageTemplateModel()
    {
		_logger.LogError(">>> Entering TestMessageTemplateModel()");
        Tokens = new List<string>();
		_logger.LogDebug("TestMessageTemplateModel(): Tokens → {Tokens}", Tokens);
    _logger.LogError("TestMessageTemplateModel(): Tokens → {Tokens}", Tokens);
    _logger.LogWarning("<<< Exiting TestMessageTemplateModel()");
    }

    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.Tokens")]
    public List<string> Tokens { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.SendTo")]
    public string SendTo { get; set; }
}