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
		_logger.LogDebug(">>> Entering DateRangeService(deliveryDateRepository={DeliveryDateRepository},productAvailabilityRangeRepository={ProductAvailabilityRangeRepository})", deliveryDateRepository, productAvailabilityRangeRepository);
        _deliveryDateRepository = deliveryDateRepository;
        _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
		_logger.LogDebug("<<< Exiting DateRangeService(deliveryDateRepository={DeliveryDateRepository},productAvailabilityRangeRepository={ProductAvailabilityRangeRepository})", deliveryDateRepository, productAvailabilityRangeRepository);
		_logger.LogDebug("DateRangeService(deliveryDateRepository={DeliveryDateRepository},productAvailabilityRangeRepository={ProductAvailabilityRangeRepository}): _productAvailabilityRangeRepository → {_productAvailabilityRangeRepository}", deliveryDateRepository, productAvailabilityRangeRepository, _productAvailabilityRangeRepository);
		_logger.LogDebug("DateRangeService(deliveryDateRepository={DeliveryDateRepository},productAvailabilityRangeRepository={ProductAvailabilityRangeRepository}): _deliveryDateRepository → {_deliveryDateRepository}", deliveryDateRepository, productAvailabilityRangeRepository, _deliveryDateRepository);
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
		_logger.LogDebug(">>> Entering GetDeliveryDateByIdAsync(deliveryDateId={DeliveryDateId})", deliveryDateId);
		_logger.LogDebug("<<< Exiting GetDeliveryDateByIdAsync(deliveryDateId={DeliveryDateId})", deliveryDateId);
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
		_logger.LogDebug(">>> Entering GetAllDeliveryDatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var deliveryDates = await _deliveryDateRepository.GetAllAsync(query =>
        {
            return from dd in query
                orderby dd.DisplayOrder, dd.Id
                select dd;
				_logger.LogInformation("GetAllDeliveryDatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		_logger.LogDebug("<<< Exiting GetAllDeliveryDatesAsync()");
        return deliveryDates;
    }

    /// <summary>
    /// Insert a delivery date
    /// </summary>
    /// <param name="deliveryDate">Delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		_logger.LogDebug(">>> Entering InsertDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
        await _deliveryDateRepository.InsertAsync(deliveryDate);
		_logger.LogDebug("<<< Exiting InsertDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
    }

    /// <summary>
    /// Update the delivery date
    /// </summary>
    /// <param name="deliveryDate">Delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		_logger.LogDebug(">>> Entering UpdateDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
        await _deliveryDateRepository.UpdateAsync(deliveryDate);
		_logger.LogDebug("<<< Exiting UpdateDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
    }

    /// <summary>
    /// Delete a delivery date
    /// </summary>
    /// <param name="deliveryDate">The delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		_logger.LogDebug(">>> Entering DeleteDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _deliveryDateRepository.DeleteAsync(deliveryDate);
		_logger.LogDebug("<<< Exiting DeleteDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: external service call took {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: external service call took {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
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
		_logger.LogDebug(">>> Entering GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId={ProductAvailabilityRangeId})", productAvailabilityRangeId);
		_logger.LogDebug("<<< Exiting GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId={ProductAvailabilityRangeId})", productAvailabilityRangeId);
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
		_logger.LogDebug(">>> Entering GetAllProductAvailabilityRangesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _productAvailabilityRangeRepository.GetAllAsync(query =>
        {
            return from par in query
                orderby par.DisplayOrder, par.Id
                select par;
				_logger.LogInformation("GetAllProductAvailabilityRangesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);
		_logger.LogDebug("<<< Exiting GetAllProductAvailabilityRangesAsync()");
    }

    /// <summary>
    /// Insert the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		_logger.LogDebug(">>> Entering InsertProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
        await _productAvailabilityRangeRepository.InsertAsync(productAvailabilityRange);
		_logger.LogDebug("<<< Exiting InsertProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
    }

    /// <summary>
    /// Update the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		_logger.LogDebug(">>> Entering UpdateProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
        await _productAvailabilityRangeRepository.UpdateAsync(productAvailabilityRange);
		_logger.LogDebug("<<< Exiting UpdateProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
    }

    /// <summary>
    /// Delete the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		_logger.LogDebug(">>> Entering DeleteProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAvailabilityRangeRepository.DeleteAsync(productAvailabilityRange);
		_logger.LogDebug("<<< Exiting DeleteProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: external service call took {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: external service call took {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    #endregion

    #endregion
}