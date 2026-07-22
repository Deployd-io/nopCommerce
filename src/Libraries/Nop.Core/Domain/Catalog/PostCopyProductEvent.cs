namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Post copy product event
/// </summary>
public partial class PostCopyProductEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="originalProduct">Original product</param>
    /// <param name="copyProduct">Copy product</param>
    public PostCopyProductEvent(Product originalProduct, Product copyProduct)
    {
		logger.LogDebug(">>> Entering PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct})", originalProduct, copyProduct);
        OriginalProduct = originalProduct;
        CopyProduct = copyProduct;
		logger.LogDebug("<<< Exiting PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct})", originalProduct, copyProduct);
		logger.LogDebug("PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct}): CopyProduct → {CopyProduct1}", originalProduct, copyProduct, CopyProduct);
		logger.LogDebug("PostCopyProductEvent(originalProduct={OriginalProduct},copyProduct={CopyProduct}): OriginalProduct → {OriginalProduct1}", originalProduct, copyProduct, OriginalProduct);
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
