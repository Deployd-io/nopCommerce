using Microsoft.Extensions.Logging;
﻿using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class CreateContactRequest : ContactInfoDto, IBatchSupport
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CreateContactRequest(string email, bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus,
        bool sendWelcomeMessage)
    {
		_logger.LogDebug(">>> Entering CreateContactRequest(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage})", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage);
        Identifiers = [new Identifier(email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage) { Type = "email" }];
        SendWelcomeEmail = sendWelcomeMessage;
		_logger.LogDebug("<<< Exiting CreateContactRequest(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage})", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage);
		_logger.LogDebug("CreateContactRequest(email={Email},isSubscriptionActive={IsSubscriptionActive},createdOnUtc={CreatedOnUtc},inactiveStatus={InactiveStatus},sendWelcomeMessage={SendWelcomeMessage}): SendWelcomeEmail → {SendWelcomeEmail}", email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage, SendWelcomeEmail);
    }

    [JsonProperty("sendWelcomeEmail")] public bool SendWelcomeEmail { get; }
}