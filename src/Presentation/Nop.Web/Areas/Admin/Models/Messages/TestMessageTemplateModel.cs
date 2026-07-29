using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

public partial record TestMessageTemplateModel : BaseNopEntityModel
{
    public TestMessageTemplateModel()
    {
		logger.LogDebug(">>> Entering TestMessageTemplateModel()");
        Tokens = new List<string>();
		logger.LogDebug("<<< Exiting TestMessageTemplateModel()");
		logger.LogDebug("TestMessageTemplateModel(): Tokens → {Tokens}", Tokens);
    }

    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.Tokens")]
    public List<string> Tokens { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.SendTo")]
    public string SendTo { get; set; }
}