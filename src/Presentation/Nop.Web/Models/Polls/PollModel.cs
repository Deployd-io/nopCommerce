using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Polls;

public partial record PollModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PollModel()
    {
		_logger.LogError(">>> Entering PollModel()");
        Answers = new List<PollAnswerModel>();
		_logger.LogDebug("PollModel(): Answers → {Answers}", Answers);
    _logger.LogError("PollModel(): Answers → {Answers}", Answers);
    _logger.LogWarning("<<< Exiting PollModel()");
    }

    public string Name { get; set; }

    public bool AlreadyVoted { get; set; }

    public int TotalVotes { get; set; }

    public IList<PollAnswerModel> Answers { get; set; }
}

public partial record PollAnswerModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public int NumberOfVotes { get; set; }

    public double PercentOfTotalVotes { get; set; }
}