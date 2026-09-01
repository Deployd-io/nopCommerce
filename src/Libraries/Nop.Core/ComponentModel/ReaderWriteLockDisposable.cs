using Microsoft.Extensions.Logging;
namespace Nop.Core.ComponentModel;

/// <summary>
/// Provides a convenience methodology for implementing locked access to resources. 
/// </summary>
/// <remarks>
/// Intended as an infrastructure class.
/// </remarks>
public partial class ReaderWriteLockDisposable : IDisposable
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected bool _disposed;
    protected readonly ReaderWriterLockSlim _rwLock;
    protected readonly ReaderWriteLockType _readerWriteLockType;

    #endregion

    #region Ctor

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderWriteLockDisposable"/> class.
    /// </summary>
    /// <param name="rwLock">The readers–writer lock</param>
    /// <param name="readerWriteLockType">Lock type</param>
    public ReaderWriteLockDisposable(ReaderWriterLockSlim rwLock, ReaderWriteLockType readerWriteLockType = ReaderWriteLockType.Write)
    {
		_logger.LogDebug(">>> Entering ReaderWriteLockDisposable(rwLock={RwLock},readerWriteLockType={ReaderWriteLockType})", rwLock, readerWriteLockType);
        _rwLock = rwLock;
        _readerWriteLockType = readerWriteLockType;
		_logger.LogDebug("ReaderWriteLockDisposable(rwLock={RwLock},readerWriteLockType={ReaderWriteLockType}): _readerWriteLockType → {_readerWriteLockType}", rwLock, readerWriteLockType, _readerWriteLockType);
		_logger.LogDebug("ReaderWriteLockDisposable(rwLock={RwLock},readerWriteLockType={ReaderWriteLockType}): _rwLock → {_rwLock}", rwLock, readerWriteLockType, _rwLock);

        switch (_readerWriteLockType)
        {
            case ReaderWriteLockType.Read:
                _rwLock.EnterReadLock();
                break;
            case ReaderWriteLockType.Write:
                _rwLock.EnterWriteLock();
                break;
            case ReaderWriteLockType.UpgradeableRead:
                _rwLock.EnterUpgradeableReadLock();
                break;
        }
		_logger.LogDebug("<<< Exiting ReaderWriteLockDisposable(rwLock={RwLock},readerWriteLockType={ReaderWriteLockType})", rwLock, readerWriteLockType);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">Specifies whether to dispose resources</param>
    protected virtual void Dispose(bool disposing)
    {
		_logger.LogDebug(">>> Entering Dispose(disposing={Disposing})", disposing);
        if (_disposed)
            return;

        if (disposing)
            switch (_readerWriteLockType)
            {
                case ReaderWriteLockType.Read:
                    _rwLock.ExitReadLock();
                    break;
                case ReaderWriteLockType.Write:
                    _rwLock.ExitWriteLock();
                    break;
                case ReaderWriteLockType.UpgradeableRead:
                    _rwLock.ExitUpgradeableReadLock();
                    break;
            }

        _disposed = true;
		_logger.LogDebug("Dispose(disposing={Disposing}): _disposed → {_disposed}", disposing, _disposed);
		_logger.LogDebug(">>> Entering Dispose()");
		_logger.LogDebug("<<< Exiting Dispose(disposing={Disposing})", disposing);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Public implementation of Dispose pattern callable by consumers.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
