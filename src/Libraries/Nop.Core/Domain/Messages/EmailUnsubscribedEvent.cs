using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Messages;

/// <summary>
/// Email unsubscribed event
/// </summary>
public partial class EmailUnsubscribedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="subscription">Subscription</param>
    public EmailUnsubscribedEvent(NewsLetterSubscription subscription)
    {
		_logger.LogDebug(">>> Entering EmailUnsubscribedEvent(subscription={Subscription})", subscription);
        Subscription = subscription;
		_logger.LogDebug("EmailUnsubscribedEvent(subscription={Subscription}): Subscription → {Subscription1}", subscription, Subscription);
		_logger.LogDebug("<<< Exiting EmailUnsubscribedEvent(subscription={Subscription})", subscription);
    }

    /// <summary>
    /// Subscription
    /// </summary>
    public NewsLetterSubscription Subscription { get; }

    /// <summary>
    /// Equals
    /// </summary>
    /// <param name="other">Other event</param>
    /// <returns>Result</returns>
    public bool Equals(EmailUnsubscribedEvent other)
    {
		_logger.LogDebug(">>> Entering Equals(other={Other})", other);
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

		_logger.LogDebug("<<< Exiting Equals(obj={Obj})", obj);
        return Equals(other.Subscription, Subscription);
    }

    /// <summary>
    /// Equals
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Result</returns>
    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != typeof(EmailUnsubscribedEvent))
            return false;

        return Equals((EmailUnsubscribedEvent)obj);
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
		_logger.LogDebug(">>> Entering GetHashCode()");
		_logger.LogDebug("<<< Exiting GetHashCode()");
        return Subscription != null ? Subscription.GetHashCode() : 0;
    }
}