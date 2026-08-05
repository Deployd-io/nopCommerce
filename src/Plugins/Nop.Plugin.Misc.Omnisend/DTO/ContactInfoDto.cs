using Microsoft.Extensions.Logging;
﻿using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class ContactInfoDto : BaseContactInfoDto
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [JsonProperty("identifiers")] public List<Identifier> Identifiers { get; set; }

    #region Nested class

    public class Identifier
    {
        public Identifier(string email, bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus, bool sendWelcomeMessage = false)
        {
			_logger.LogDebug(">>> Entering Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage})", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage);
            Id = email;
            SendWelcomeMessage = sendWelcomeMessage;
			_logger.LogDebug("Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): Id → {Id}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, Id);
			_logger.LogDebug("Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): SendWelcomeMessage → {SendWelcomeMessage1}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, SendWelcomeMessage);
			_logger.LogDebug(">>> Entering Identifier()");
            Channels = new(isSubscriptionActive, createdOnUtc, inactiveStatus);
			_logger.LogDebug("<<< Exiting Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage})", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage);
			_logger.LogDebug("Identifier(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): Channels → {Channels}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, Channels);
        }

        public Identifier()
        {
		_logger.LogDebug("<<< Exiting Identifier()");
        }

        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("sendWelcomeMessage")] public bool SendWelcomeMessage { get; set; }
        [JsonProperty("channels")] public EmailChannel Channels { get; }

        #region Nested class

        public class EmailChannel
        {
            public EmailChannel(bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus)
            {
				_logger.LogDebug(">>> Entering EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
                Email = new Channel(isSubscriptionActive, createdOnUtc, inactiveStatus);
				_logger.LogDebug("<<< Exiting EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
				_logger.LogDebug("EmailChannel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): Email → {Email}", isSubscriptionActive, createdOnUtc, inactiveStatus, Email);
				_logger.LogDebug(">>> Entering EmailChannel()");
            }

            public EmailChannel()
            {
			_logger.LogDebug("<<< Exiting EmailChannel()");
            }

            [JsonProperty("email")] public Channel Email { get; set; }

            #region Nested class

            public class Channel
            {
                public Channel(bool isSubscriptionActive, DateTime createdOnUtc,  string inactiveStatus)
                {
					_logger.LogDebug(">>> Entering Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
                    Status = isSubscriptionActive ? "subscribed" : inactiveStatus;
                    StatusDate = createdOnUtc.ToDtoString();
					_logger.LogDebug("<<< Exiting Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus})", isSubscriptionActive, createdOnUtc, inactiveStatus);
					_logger.LogDebug("Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): Status → {Status}", isSubscriptionActive, createdOnUtc, inactiveStatus, Status);
					_logger.LogDebug("Channel(isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus}): StatusDate → {StatusDate}", isSubscriptionActive, createdOnUtc, inactiveStatus, StatusDate);
					_logger.LogDebug(">>> Entering Channel()");
                }

                public Channel()
                {
				_logger.LogDebug("<<< Exiting Channel()");
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