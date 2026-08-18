using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Polls;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class PollController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPollModelFactory _pollModelFactory;
    protected readonly IPollService _pollService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PollController(ICustomerService customerService,
        ILocalizationService localizationService,
        IPollModelFactory pollModelFactory,
        IPollService pollService,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext})", customerService, localizationService, pollModelFactory, pollService, workContext);
        _customerService = customerService;
		_logger.LogDebug("PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext}): _customerService → {_customerService}", customerService, localizationService, pollModelFactory, pollService, workContext, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext}): _localizationService → {_localizationService}", customerService, localizationService, pollModelFactory, pollService, workContext, _localizationService);
        _pollModelFactory = pollModelFactory;
		_logger.LogDebug("PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext}): _pollModelFactory → {_pollModelFactory}", customerService, localizationService, pollModelFactory, pollService, workContext, _pollModelFactory);
        _pollService = pollService;
		_logger.LogDebug("PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext}): _pollService → {_pollService}", customerService, localizationService, pollModelFactory, pollService, workContext, _pollService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext}): _storeMappingService → {_storeMappingService}", customerService, localizationService, pollModelFactory, pollService, workContext, _storeMappingService);
        _workContext = workContext;
		_logger.LogDebug("PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext}): _workContext → {_workContext}", customerService, localizationService, pollModelFactory, pollService, workContext, _workContext);
		_logger.LogDebug("<<< Exiting PollController(customerService={CustomerService},localizationService={LocalizationService},pollModelFactory={PollModelFactory},pollService={PollService},storeMappingService=<redacted>,workContext={WorkContext})", customerService, localizationService, pollModelFactory, pollService, workContext);
    }

    #endregion

    #region Methods

    [HttpPost]
    public virtual async Task<IActionResult> Vote(int pollAnswerId)
    {
		_logger.LogDebug(">>> Entering Vote(pollAnswerId={PollAnswerId})", pollAnswerId);
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(pollAnswerId);
        if (pollAnswer == null)
			_logger.LogDebug("Vote(pollAnswerId={PollAnswerId}): pollAnswer → {PollAnswer}", pollAnswerId, pollAnswer);
            return Json(new { error = "No poll answer found with the specified id" });

        var poll = await _pollService.GetPollByIdAsync(pollAnswer.PollId);
		_logger.LogDebug("Vote(pollAnswerId={PollAnswerId}): poll → {Poll}", pollAnswerId, poll);

        if (!poll.Published || !await _storeMappingService.AuthorizeAsync(poll))
            return Json(new { error = "Poll is not available" });

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("Vote(pollAnswerId={PollAnswerId}): customer → {Customer}", pollAnswerId, customer);
        if (await _customerService.IsGuestAsync(customer) && !poll.AllowGuestsToVote)
            return Json(new { error = await _localizationService.GetResourceAsync("Polls.OnlyRegisteredUsersVote") });

        var alreadyVoted = await _pollService.AlreadyVotedAsync(poll.Id, customer.Id);
		_logger.LogDebug("Vote(pollAnswerId={PollAnswerId}): alreadyVoted → {AlreadyVoted}", pollAnswerId, alreadyVoted);
        if (!alreadyVoted)
        {
            //vote
            await _pollService.InsertPollVotingRecordAsync(new PollVotingRecord
            {
                PollAnswerId = pollAnswer.Id,
                CustomerId = customer.Id,
                CreatedOnUtc = DateTime.UtcNow
            });

            //update totals
            pollAnswer.NumberOfVotes = (await _pollService.GetPollVotingRecordsByPollAnswerAsync(pollAnswer.Id)).Count;
            await _pollService.UpdatePollAnswerAsync(pollAnswer);
            await _pollService.UpdatePollAsync(poll);
        }

		_logger.LogDebug("<<< Exiting Vote(pollAnswerId={PollAnswerId})", pollAnswerId);
        return Json(new
        {
            html = await RenderPartialViewToStringAsync("_Poll", await _pollModelFactory.PreparePollModelAsync(poll, true)),
        });
    }

    #endregion
}