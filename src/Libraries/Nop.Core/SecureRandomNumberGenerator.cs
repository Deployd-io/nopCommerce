using System.Security.Cryptography;

namespace Nop.Core;

/// <summary>
///  Represents the class implementation of cryptographic random number generator derive
/// </summary>
public partial class SecureRandomNumberGenerator : RandomNumberGenerator
{
    #region Field

    protected bool _disposed;
    protected readonly RandomNumberGenerator _rng;

    #endregion

    #region Ctor

    public SecureRandomNumberGenerator()
    {
		logger.LogDebug(">>> Entering SecureRandomNumberGenerator()");
        _rng = Create();
		logger.LogDebug("<<< Exiting SecureRandomNumberGenerator()");
		logger.LogDebug("SecureRandomNumberGenerator(): _rng → {_rng}", _rng);
    }

    #endregion

    #region Methods

    public int Next()
    {
		logger.LogDebug(">>> Entering Next()");
        var data = new byte[sizeof(int)];
        _rng.GetBytes(data);
		logger.LogDebug("Next(): data → {Data}", data);
		logger.LogDebug("<<< Exiting Next()");
        return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
    }

    public int Next(int maxValue)
    {
		logger.LogDebug("<<< Exiting Next(maxValue={MaxValue})", maxValue);
        return Next(0, maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minValue, maxValue);
		logger.LogDebug("<<< Exiting Next(minValue={MinValue},maxValue={MaxValue})", minValue, maxValue);
        return (int)Math.Floor(minValue + ((double)maxValue - minValue) * NextDouble());
    }

    public double NextDouble()
    {
		logger.LogDebug(">>> Entering NextDouble()");
        var data = new byte[sizeof(uint)];
        _rng.GetBytes(data);
		logger.LogDebug("NextDouble(): data → {Data}", data);
        var randUint = BitConverter.ToUInt32(data, 0);
		logger.LogDebug("NextDouble(): randUint → {RandUint}", randUint);
		logger.LogDebug("<<< Exiting NextDouble()");
        return randUint / (uint.MaxValue + 1.0);
    }

    public override void GetBytes(byte[] data)
    {
        _rng.GetBytes(data);
    }

    public override void GetNonZeroBytes(byte[] data)
    {
        _rng.GetNonZeroBytes(data);
    }

    /// <summary>
    /// Dispose secure random
    /// </summary>
    public new void Dispose()
    {
		logger.LogDebug(">>> Entering Dispose()");
        Dispose(true);
        GC.SuppressFinalize(this);
		logger.LogDebug("<<< Exiting Dispose()");
    }

    // Protected implementation of Dispose pattern.
    protected override void Dispose(bool disposing)
    {
		logger.LogDebug(">>> Entering Dispose(disposing={Disposing})", disposing);
        if (_disposed)
			logger.LogDebug("<<< Exiting Dispose(disposing={Disposing})", disposing);
            return;

        if (disposing)
        {
            _rng?.Dispose();
        }

        _disposed = true;
		logger.LogDebug("Dispose(disposing={Disposing}): _disposed → {_disposed}", disposing, _disposed);
    }

    #endregion
}