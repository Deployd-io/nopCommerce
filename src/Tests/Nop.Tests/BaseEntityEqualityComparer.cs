using Nop.Core;

namespace Nop.Tests;

public class BaseEntityEqualityComparer<T> : IEqualityComparer<T> where T : BaseEntity
{
    public bool Equals(T x, T y)
    {
		logger.LogDebug(">>> Entering Equals(x={X},y={Y})", x, y);
		logger.LogDebug("<<< Exiting Equals(x={X},y={Y})", x, y);
        return x?.Id == y?.Id;
    }

    public int GetHashCode(T obj)
    {
		logger.LogDebug(">>> Entering GetHashCode(obj={Obj})", obj);
		logger.LogDebug("<<< Exiting GetHashCode(obj={Obj})", obj);
        return obj.Id;
    }
}
