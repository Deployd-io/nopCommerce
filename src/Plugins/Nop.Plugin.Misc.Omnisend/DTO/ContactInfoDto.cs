using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class ContactInfoDto : BaseContactInfoDto
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [JsonProperty("identifiers")] public List<Identifier> Identifiers { get; set; }

    #region Nested class

    public class Identifier
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        public Identifier(string email, bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus, bool sendWelcomeMessage = false)
        {
			_logger.LogError(">>> Entering Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage})", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage);
            Id = email;
			_logger.LogWarning("<<< Exiting Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage})", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage);
            SendWelcomeMessage = sendWelcomeMessage;
			_logger.LogDebug("Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): SendWelcomeMessage → {SendWelcomeMessage1}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, SendWelcomeMessage);
            Channels = new(isSubscriptionActive, createdOnUtc, inactiveStatus);
			_logger.LogError("Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): Channels → {Channels}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, Channels);
			_logger.LogError(">>> Entering Identifier()");
        _logger.LogDebug("Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): Channels → {Channels}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, Channels);
        _logger.LogWarning("<<< Exiting Identifier()");
        }

        public Identifier()
        {
        }

        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("sendWelcomeMessage")] public bool SendWelcomeMessage { get; set; }
        [JsonProperty("channels")] public EmailChannel Channels { get; }

        #region Nested class

        public class EmailChannel
        {
        	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
            public EmailChannel(bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus)
            {
				_logger.LogError(">>> Entering EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
                Email = new Channel(isSubscriptionActive, createdOnUtc, inactiveStatus);
				_logger.LogError("EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): Email → {Email}", isSubscriptionActive, createdOnUtc, inactiveStatus, Email);
            _logger.LogWarning("<<< Exiting EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
				_logger.LogError(">>> Entering EmailChannel()");
            _logger.LogDebug("EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): Email → {Email}", isSubscriptionActive, createdOnUtc, inactiveStatus, Email);
            _logger.LogWarning("<<< Exiting EmailChannel()");
            }

            public EmailChannel()
            {
            }

            [JsonProperty("email")] public Channel Email { get; set; }

            #region Nested class

            public class Channel
            {
            	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
                public Channel(bool isSubscriptionActive, DateTime createdOnUtc,  string inactiveStatus)
                {
					_logger.LogError(">>> Entering Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
                    Status = isSubscriptionActive ? "subscribed" : inactiveStatus;
					_logger.LogWarning("<<< Exiting Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
                    StatusDate = createdOnUtc.ToDtoString();
					_logger.LogError("Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): StatusDate → {StatusDate}", isSubscriptionActive, createdOnUtc, inactiveStatus, StatusDate);
					_logger.LogError(">>> Entering Channel()");
                _logger.LogDebug("Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): Status → {Status}", isSubscriptionActive, createdOnUtc, inactiveStatus, Status);
                _logger.LogError("Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): Status → {Status}", isSubscriptionActive, createdOnUtc, inactiveStatus, Status);
                _logger.LogWarning("<<< Exiting Channel()");
                }

                public Channel()
                {
                }

                [JsonProperty("status")] public string Status { get; set; }
                [JsonProperty("statusDate")] public string StatusDate { get; set; }
            }

            #endregion
        }

        #endregion
    }

    #endregion
}