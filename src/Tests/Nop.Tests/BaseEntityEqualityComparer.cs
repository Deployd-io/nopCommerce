using Microsoft.Extensions.Logging;
﻿using Nop.Core;

namespace Nop.Tests;

public class BaseEntityEqualityComparer<T> : IEqualityComparer<T> where T : BaseEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public bool Equals(T x, T y)
    {
		_logger.LogDebug(">>> Entering Equals(x={X},y={Y})", x, y);
		_logger.LogDebug("<<< Exiting Equals(x={X},y={Y})", x, y);
        return x?.Id == y?.Id;
    }

    public int GetHashCode(T obj)
    {
		_logger.LogDebug(">>> Entering GetHashCode(obj={Obj})", obj);
		_logger.LogDebug("<<< Exiting GetHashCode(obj={Obj})", obj);
        return obj.Id;
    }
}