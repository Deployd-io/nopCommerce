using Nop.Core.Domain.Shipping;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping.Date;

/// <summary>
/// Represents the date range service
/// </summary>
public partial class DateRangeService : IDateRangeService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<DeliveryDate> _deliveryDateRepository;
    protected readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;

    #endregion

    #region Ctor

    public DateRangeService(IRepository<DeliveryDate> deliveryDateRepository,
        IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository)
    {
		_logger.LogError(">>> Entering DateRangeService(deliveryDateRepository={DeliveryDateRepository},productAvailabilityRangeRepository={ProductAvailabilityRangeRepository})", deliveryDateRepository, productAvailabilityRangeRepository);
        _deliveryDateRepository = deliveryDateRepository;
        _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
		_logger.LogDebug("DateRangeService(deliveryDateRepository={DeliveryDateRepository},productAvailabilityRangeRepository={ProductAvailabilityRangeRepository}): _productAvailabilityRangeRepository → {_productAvailabilityRangeRepository}", deliveryDateRepository, productAvailabilityRangeRepository, _productAvailabilityRangeRepository);
    }

    #endregion

    #region Methods

    #region Delivery dates

    /// <summary>
    /// Get a delivery date
    /// </summary>
    /// <param name="deliveryDateId">The delivery date identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the delivery date
    /// </returns>
    public virtual async Task<DeliveryDate> GetDeliveryDateByIdAsync(int deliveryDateId)
    {
		_logger.LogError(">>> Entering GetDeliveryDateByIdAsync(deliveryDateId={DeliveryDateId})", deliveryDateId);
		_logger.LogError("GetDeliveryDateByIdAsync(deliveryDateId)={DeliveryDateId}: find query executing", deliveryDateId);
    _logger.LogWarning("<<< Exiting GetDeliveryDateByIdAsync(deliveryDateId={DeliveryDateId})", deliveryDateId);
        return await _deliveryDateRepository.GetByIdAsync(deliveryDateId, cache => default);
    }

    /// <summary>
    /// Get all delivery dates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the delivery dates
    /// </returns>
    public virtual async Task<IList<DeliveryDate>> GetAllDeliveryDatesAsync()
    {
		_logger.LogError(">>> Entering GetAllDeliveryDatesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var deliveryDates = await _deliveryDateRepository.GetAllAsync(query =>
        {
            return from dd in query
                orderby dd.DisplayOrder, dd.Id
                select dd;
        }, cache => default);
		_logger.LogError("GetAllDeliveryDatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllDeliveryDatesAsync()");
        return deliveryDates;
    }

    /// <summary>
    /// Insert a delivery date
    /// </summary>
    /// <param name="deliveryDate">Delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		_logger.LogError(">>> Entering InsertDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
        await _deliveryDateRepository.InsertAsync(deliveryDate);
		_logger.LogWarning("<<< Exiting InsertDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
    }

    /// <summary>
    /// Update the delivery date
    /// </summary>
    /// <param name="deliveryDate">Delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		_logger.LogError(">>> Entering UpdateDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
        await _deliveryDateRepository.UpdateAsync(deliveryDate);
		_logger.LogWarning("<<< Exiting UpdateDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
    }

    /// <summary>
    /// Delete a delivery date
    /// </summary>
    /// <param name="deliveryDate">The delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		_logger.LogError(">>> Entering DeleteDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _deliveryDateRepository.DeleteAsync(deliveryDate);
		_logger.LogError("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: external service call took {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
    }

    #endregion

    #region Product availability ranges

    /// <summary>
    /// Get a product availability range
    /// </summary>
    /// <param name="productAvailabilityRangeId">The product availability range identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product availability range
    /// </returns>
    public virtual async Task<ProductAvailabilityRange> GetProductAvailabilityRangeByIdAsync(int productAvailabilityRangeId)
    {
		_logger.LogError(">>> Entering GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId={ProductAvailabilityRangeId})", productAvailabilityRangeId);
		_logger.LogError("GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId)={ProductAvailabilityRangeId}: find query executing", productAvailabilityRangeId);
    _logger.LogWarning("<<< Exiting GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId={ProductAvailabilityRangeId})", productAvailabilityRangeId);
        return productAvailabilityRangeId != 0 ? await _productAvailabilityRangeRepository.GetByIdAsync(productAvailabilityRangeId, cache => default) : null;
    }

    /// <summary>
    /// Get all product availability ranges
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product availability ranges
    /// </returns>
    public virtual async Task<IList<ProductAvailabilityRange>> GetAllProductAvailabilityRangesAsync()
    {
		_logger.LogError(">>> Entering GetAllProductAvailabilityRangesAsync()");
        return await _productAvailabilityRangeRepository.GetAllAsync(query =>
        {
            return from par in query
                orderby par.DisplayOrder, par.Id
                select par;
				_logger.LogError("GetAllProductAvailabilityRangesAsync(): find query executing");
        }, cache => default);
		_logger.LogWarning("<<< Exiting GetAllProductAvailabilityRangesAsync()");
    }

    /// <summary>
    /// Insert the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		_logger.LogError(">>> Entering InsertProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
        await _productAvailabilityRangeRepository.InsertAsync(productAvailabilityRange);
		_logger.LogWarning("<<< Exiting InsertProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
    }

    /// <summary>
    /// Update the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		_logger.LogError(">>> Entering UpdateProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
        await _productAvailabilityRangeRepository.UpdateAsync(productAvailabilityRange);
		_logger.LogWarning("<<< Exiting UpdateProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
    }

    /// <summary>
    /// Delete the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		_logger.LogError(">>> Entering DeleteProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAvailabilityRangeRepository.DeleteAsync(productAvailabilityRange);
		_logger.LogError("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: external service call took {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
    }

    #endregion

    #endregion
}