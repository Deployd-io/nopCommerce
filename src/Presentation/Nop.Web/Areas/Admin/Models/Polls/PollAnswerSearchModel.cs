using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll answer search model
/// </summary>
public partial record PollAnswerSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PollAnswerSearchModel()
    {
		_logger.LogError(">>> Entering PollAnswerSearchModel()");
        AddPollAnswer = new PollAnswerModel();
		_logger.LogDebug("PollAnswerSearchModel(): AddPollAnswer → {AddPollAnswer}", AddPollAnswer);
    _logger.LogError("PollAnswerSearchModel(): AddPollAnswer → {AddPollAnswer}", AddPollAnswer);
    _logger.LogWarning("<<< Exiting PollAnswerSearchModel()");
    }

    #endregion

    #region Properties

    public int PollId { get; set; }

    public PollAnswerModel AddPollAnswer { get; set; }

    #endregion
}