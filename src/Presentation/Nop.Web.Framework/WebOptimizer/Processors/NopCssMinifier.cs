using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using NUglify;
using NUglify.Css;
using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer.Processors;

/// <summary>
/// Represents a class of processor that handle style assets
/// </summary>
/// <remarks>Implementation has taken from WebOptimizer to add logging</remarks>
public partial class NopCssMinifier : Processor
{
    #region Methods

    /// <summary>
    /// Executes the processor on the specified configuration.
    /// </summary>
    /// <param name="context">The context used to perform processing to WebOptimizer.IAsset instances</param>
    public override async Task ExecuteAsync(IAssetContext context)
    {
		logger.LogDebug(">>> Entering ExecuteAsync(context={Context})", context);
        var content = new Dictionary<string, byte[]>();

		logger.LogDebug("ExecuteAsync(context={Context}): content → {Content}", context, content);
        foreach (var key in context.Content.Keys)
        {
            if (key.EndsWith(".min.css", StringComparison.InvariantCultureIgnoreCase))
            {
                content[key] = context.Content[key];
                continue;
            }

            var input = context.Content[key].AsString();
            var result = Uglify.Css(input, new CssSettings());
			logger.LogDebug("ExecuteAsync(context={Context}): input → {Input}", context, input);
			logger.LogDebug("ExecuteAsync(context={Context}): result → {Result}", context, result);

            var minified = result.Code;

            if (result.HasErrors)
            {
                await EngineContext.Current.Resolve<ILogger>()
                    .WarningAsync($"Stylesheet minification: {key}", new(string.Join(Environment.NewLine, result.Errors)));
            }

            content[key] = minified.AsByteArray();
        }

        context.Content = content;
		logger.LogDebug("ExecuteAsync(context={Context}): context.Content → {Arg1}", context, context.Content);

		logger.LogDebug("<<< Exiting ExecuteAsync(context={Context})", context);
        return;
    }

    #endregion

}
