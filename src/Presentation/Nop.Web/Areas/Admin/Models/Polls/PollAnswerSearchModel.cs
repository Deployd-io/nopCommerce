using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll answer search model
/// </summary>
public partial record PollAnswerSearchModel : BaseSearchModel
{
    #region Ctor

    public PollAnswerSearchModel()
    {
		logger.LogDebug(">>> Entering PollAnswerSearchModel()");
        AddPollAnswer = new PollAnswerModel();
		logger.LogDebug("<<< Exiting PollAnswerSearchModel()");
		logger.LogDebug("PollAnswerSearchModel(): AddPollAnswer → {AddPollAnswer}", AddPollAnswer);
    }

    #endregion

    #region Properties

    public int PollId { get; set; }

    public PollAnswerModel AddPollAnswer { get; set; }

    #endregion
}