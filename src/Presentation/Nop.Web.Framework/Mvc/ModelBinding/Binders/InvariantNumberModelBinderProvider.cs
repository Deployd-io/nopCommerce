using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders;

/// <summary>
/// Represents a model binder provider for binding numeric types
/// </summary>
public partial class InvariantNumberModelBinderProvider : IModelBinderProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static readonly HashSet<Type> _integerTypes =
    [
        typeof(int), typeof(long), typeof(short), typeof(sbyte),
    typeof(byte), typeof(ulong), typeof(ushort), typeof(uint), typeof(BigInteger)
    ];

    protected static readonly HashSet<Type> _floatingPointTypes =
    [
        typeof(double), typeof(decimal), typeof(float)
    ];

    #endregion

    /// <summary>
    /// Creates a model binder
    /// </summary>
    /// <param name="context">Context object</param>
    /// <returns>Instance of model binder for floating-point types</returns>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
		_logger.LogError(">>> Entering GetBinder(context={Context})", context);
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.UnderlyingOrModelType;

        if (modelType is null)
            return null;

        if (_floatingPointTypes.Contains(modelType))
            return new InvariantNumberModelBinder(NumberStyles.Float, new FloatingPointTypeModelBinderProvider().GetBinder(context));

		_logger.LogWarning("<<< Exiting GetBinder(context={Context})", context);
        if (_integerTypes.Contains(modelType)) {
            return new InvariantNumberModelBinder(NumberStyles.Integer, new SimpleTypeModelBinderProvider().GetBinder(context));
        }

        return null;
    }
}