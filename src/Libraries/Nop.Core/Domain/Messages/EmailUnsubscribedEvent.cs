namespace Nop.Core.Domain.Messages;

/// <summary>
/// Email unsubscribed event
/// </summary>
public partial class EmailUnsubscribedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="subscription">Subscription</param>
    public EmailUnsubscribedEvent(NewsLetterSubscription subscription)
    {
		logger.LogDebug(">>> Entering EmailUnsubscribedEvent(subscription={Subscription})", subscription);
        Subscription = subscription;
		logger.LogDebug("<<< Exiting EmailUnsubscribedEvent(subscription={Subscription})", subscription);
		logger.LogDebug("EmailUnsubscribedEvent(subscription={Subscription}): Subscription → {Subscription1}", subscription, Subscription);
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
		logger.LogDebug(">>> Entering Equals(other={Other})", other);
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
		logger.LogDebug("<<< Exiting Equals(other={Other})", other);
            return true;

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
		logger.LogDebug("<<< Exiting Equals(obj={Obj})", obj);
            return false;

        return Equals((EmailUnsubscribedEvent)obj);
    }

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
		logger.LogDebug(">>> Entering GetHashCode()");
		logger.LogDebug("<<< Exiting GetHashCode()");
        return Subscription != null ? Subscription.GetHashCode() : 0;
    }
}
