using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Nop.Core;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.ModelBinding;

/// <summary>
/// Represents metadata provider that adds custom attributes to the model's metadata, so it can be retrieved later
/// </summary>
public partial class NopMetadataProvider : IDisplayMetadataProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Sets the values for properties of isplay metadata
    /// </summary>
    /// <param name="context">Display metadata provider context</param>
    public virtual void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
		_logger.LogError(">>> Entering CreateDisplayMetadata(context={Context})", context);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //get all custom attributes
        var additionalValues = context.Attributes.OfType<IModelAttribute>().ToList();

		_logger.LogError("CreateDisplayMetadata(context)={Context}: database query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
		_logger.LogError("CreateDisplayMetadata(context={Context}): additionalValues → {AdditionalValues}", context, additionalValues);
        //and try add them as additional values of metadata
        foreach (var additionalValue in additionalValues)
        {
            if (context.DisplayMetadata.AdditionalValues.ContainsKey(additionalValue.Name))
				_logger.LogError("Throwing NopException from CreateDisplayMetadata(context={Context})", context);
                throw new NopException("There is already an attribute with the name '{0}' on this model", additionalValue.Name);

            context.DisplayMetadata.AdditionalValues.Add(additionalValue.Name, additionalValue);
        }
		_logger.LogWarning("<<< Exiting CreateDisplayMetadata(context={Context})", context);
    }
}