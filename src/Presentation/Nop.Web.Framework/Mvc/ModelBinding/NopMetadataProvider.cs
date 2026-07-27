using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Nop.Core;

namespace Nop.Web.Framework.Mvc.ModelBinding;

/// <summary>
/// Represents metadata provider that adds custom attributes to the model's metadata, so it can be retrieved later
/// </summary>
public partial class NopMetadataProvider : IDisplayMetadataProvider
{
    /// <summary>
    /// Sets the values for properties of isplay metadata
    /// </summary>
    /// <param name="context">Display metadata provider context</param>
    public virtual void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
		logger.LogDebug(">>> Entering CreateDisplayMetadata(context={Context})", context);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //get all custom attributes
        var additionalValues = context.Attributes.OfType<IModelAttribute>().ToList();

		logger.LogDebug("CreateDisplayMetadata(context={Context}): additionalValues → {AdditionalValues}", context, additionalValues);
		logger.LogInformation("CreateDisplayMetadata(context)={Context}: database query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
        //and try add them as additional values of metadata
        foreach (var additionalValue in additionalValues)
        {
            if (context.DisplayMetadata.AdditionalValues.ContainsKey(additionalValue.Name))
                throw new NopException("There is already an attribute with the name '{0}' on this model", additionalValue.Name);

            context.DisplayMetadata.AdditionalValues.Add(additionalValue.Name, additionalValue);
        }
		logger.LogDebug("<<< Exiting CreateDisplayMetadata(context={Context})", context);
    }
}