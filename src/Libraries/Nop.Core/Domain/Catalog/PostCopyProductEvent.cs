using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Post copy product event
/// </summary>
public partial class PostCopyProductEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="originalProduct">Original product</param>
    /// <param name="copyProduct">Copy product</param>
    public PostCopyProductEvent(Product originalProduct, Product copyProduct)
    {
		_logger.LogDebug(">>> Entering PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct})", originalProduct, copyProduct);
        OriginalProduct = originalProduct;
        CopyProduct = copyProduct;
		_logger.LogDebug("PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct}): CopyProduct → {CopyProduct1}", originalProduct, copyProduct, CopyProduct);
		_logger.LogDebug("PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct}): OriginalProduct → {OriginalProduct1}", originalProduct, copyProduct, OriginalProduct);
		_logger.LogDebug("<<< Exiting PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct})", originalProduct, copyProduct);
    }

    /// <summary>
    /// Original product
    /// </summary>
    public Product OriginalProduct { get; }

    /// <summary>
    /// Copy product
    /// </summary>
    public Product CopyProduct { get; }
}