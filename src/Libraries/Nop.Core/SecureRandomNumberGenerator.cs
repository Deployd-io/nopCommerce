using Microsoft.Extensions.Logging;
﻿using System.Security.Cryptography;

namespace Nop.Core;

/// <summary>
///  Represents the class implementation of cryptographic random number generator derive
/// </summary>
public partial class SecureRandomNumberGenerator : RandomNumberGenerator
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Field

    protected bool _disposed;
    protected readonly RandomNumberGenerator _rng;

    #endregion

    #region Ctor

    public SecureRandomNumberGenerator()
    {
		_logger.LogDebug(">>> Entering SecureRandomNumberGenerator()");
        _rng = Create();
		_logger.LogDebug("SecureRandomNumberGenerator(): _rng → {_rng}", _rng);
		_logger.LogDebug("<<< Exiting SecureRandomNumberGenerator()");
    }

    #endregion

    #region Methods

    public int Next()
    {
		_logger.LogDebug(">>> Entering Next()");
        var data = new byte[sizeof(int)];
        _rng.GetBytes(data);
		_logger.LogDebug("Next(): data → {Data}", data);
		_logger.LogDebug("<<< Exiting Next()");
		_logger.LogDebug("<<< Exiting Next(maxValue={MaxValue})", maxValue);
		_logger.LogDebug("<<< Exiting Next(minValue={MinValue},maxValue={MaxValue})", minValue, maxValue);
        return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
    }

    public int Next(int maxValue)
    {
        return Next(0, maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minValue, maxValue);
        return (int)Math.Floor(minValue + ((double)maxValue - minValue) * NextDouble());
    }

    public double NextDouble()
    {
		_logger.LogDebug(">>> Entering NextDouble()");
        var data = new byte[sizeof(uint)];
        _rng.GetBytes(data);
		_logger.LogDebug("NextDouble(): data → {Data}", data);
        var randUint = BitConverter.ToUInt32(data, 0);
		_logger.LogDebug("NextDouble(): randUint → {RandUint}", randUint);
		_logger.LogDebug("<<< Exiting NextDouble()");
        return randUint / (uint.MaxValue + 1.0);
    }

    public override void GetBytes(byte[] data)
    {
		_logger.LogDebug(">>> Entering GetBytes(data={Data})", data);
        _rng.GetBytes(data);
		_logger.LogDebug("<<< Exiting GetBytes(data={Data})", data);
    }

    public override void GetNonZeroBytes(byte[] data)
    {
		_logger.LogDebug(">>> Entering GetNonZeroBytes(data={Data})", data);
        _rng.GetNonZeroBytes(data);
		_logger.LogDebug("<<< Exiting GetNonZeroBytes(data={Data})", data);
    }

    /// <summary>
    /// Dispose secure random
    /// </summary>
    public new void Dispose()
    {
		_logger.LogDebug(">>> Entering Dispose()");
        Dispose(true);
        GC.SuppressFinalize(this);
		_logger.LogDebug("<<< Exiting Dispose()");
    }

    // Protected implementation of Dispose pattern.
    protected override void Dispose(bool disposing)
    {
		_logger.LogDebug(">>> Entering Dispose(disposing={Disposing})", disposing);
        if (_disposed)
			_logger.LogDebug("<<< Exiting Dispose(disposing={Disposing})", disposing);
            return;

        if (disposing)
        {
            _rng?.Dispose();
        }

        _disposed = true;
		_logger.LogDebug("Dispose(disposing={Disposing}): _disposed → {_disposed}", disposing, _disposed);
    }

    #endregion
}