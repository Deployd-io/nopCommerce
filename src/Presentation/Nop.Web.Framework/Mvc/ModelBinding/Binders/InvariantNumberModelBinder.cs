using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders;

/// <summary>
/// Represents model binder for numeric types
/// </summary>
public partial class InvariantNumberModelBinder : IModelBinder
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected delegate bool TryParseNumber<T>(string value, NumberStyles styles, IFormatProvider provider, out T result) where T : struct;
    protected readonly IModelBinder _globalizedBinder;
    protected readonly NumberStyles _supportedStyles;

    #endregion

    #region Ctor

    public InvariantNumberModelBinder(NumberStyles supportedStyles, IModelBinder globalizedBinder)
    {
		_logger.LogDebug(">>> Entering InvariantNumberModelBinder(supportedStyles={SupportedStyles},globalizedBinder={GlobalizedBinder})", supportedStyles, globalizedBinder);
        ArgumentNullException.ThrowIfNull(globalizedBinder);

        _globalizedBinder = globalizedBinder;
		_logger.LogDebug("InvariantNumberModelBinder(supportedStyles={SupportedStyles},globalizedBinder={GlobalizedBinder}): _globalizedBinder → {_globalizedBinder}", supportedStyles, globalizedBinder, _globalizedBinder);
        _supportedStyles = supportedStyles;
		_logger.LogDebug("InvariantNumberModelBinder(supportedStyles={SupportedStyles},globalizedBinder={GlobalizedBinder}): _supportedStyles → {_supportedStyles}", supportedStyles, globalizedBinder, _supportedStyles);
		_logger.LogDebug("<<< Exiting InvariantNumberModelBinder(supportedStyles={SupportedStyles},globalizedBinder={GlobalizedBinder})", supportedStyles, globalizedBinder);
    }

    #endregion

    #region Utilities

    protected virtual T? TryParse<T>(string value, TryParseNumber<T> handler) where T : struct
    {
        if (!string.IsNullOrWhiteSpace(value) && handler(value, _supportedStyles, CultureInfo.InvariantCulture, out var parseResult))
            return parseResult;

        return null;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Attempts to bind a model
    /// </summary>
    /// <param name="bindingContext">Model binding context</param>
    public virtual Task BindModelAsync(ModelBindingContext bindingContext)
    {
		_logger.LogDebug(">>> Entering BindModelAsync(bindingContext={BindingContext})", bindingContext);
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
		_logger.LogDebug("BindModelAsync(bindingContext={BindingContext}): valueProviderResult → {ValueProviderResult}", bindingContext, valueProviderResult);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;
		_logger.LogDebug("BindModelAsync(bindingContext={BindingContext}): value → {Value}", bindingContext, value);

        if (string.IsNullOrEmpty(value))
            return Task.CompletedTask;

        value = value.Replace('/', '.'); //workaround for a kendonumeric input

        object model = bindingContext.ModelMetadata.UnderlyingOrModelType switch
        {
            Type t when t == typeof(float) => TryParse<float>(value, float.TryParse),
            Type t when t == typeof(decimal) => TryParse<decimal>(value, decimal.TryParse),
            Type t when t == typeof(double) => TryParse<double>(value, double.TryParse),
            Type t when t == typeof(int) => TryParse<int>(value, int.TryParse),
            Type t when t == typeof(long) => TryParse<long>(value, long.TryParse),
            Type t when t == typeof(short) => TryParse<short>(value, short.TryParse),
            Type t when t == typeof(sbyte) => TryParse<sbyte>(value, sbyte.TryParse),
            Type t when t == typeof(byte) => TryParse<byte>(value, byte.TryParse),
            Type t when t == typeof(ulong) => TryParse<ulong>(value, ulong.TryParse),
            Type t when t == typeof(ushort) => TryParse<ushort>(value, ushort.TryParse),
            Type t when t == typeof(uint) => TryParse<uint>(value, uint.TryParse),
            Type t when t == typeof(BigInteger) => TryParse<BigInteger>(value, BigInteger.TryParse),
            _ => null
        };

        if (model is null)
            return _globalizedBinder.BindModelAsync(bindingContext); //attempt to bind a model depending on the current culture

        bindingContext.Result = ModelBindingResult.Success(model);

		_logger.LogDebug("<<< Exiting BindModelAsync(bindingContext={BindingContext})", bindingContext);
        return Task.CompletedTask;
    }

    #endregion
}