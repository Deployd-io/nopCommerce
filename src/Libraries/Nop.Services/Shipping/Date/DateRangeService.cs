using Nop.Core.Domain.Shipping;
using Nop.Data;

namespace Nop.Services.Shipping.Date;

/// <summary>
/// Represents the date range service
/// </summary>
public partial class DateRangeService : IDateRangeService
{
    #region Fields

    protected readonly IRepository<DeliveryDate> _deliveryDateRepository;
    protected readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;

    #endregion

    #region Ctor

    public DateRangeService(IRepository<DeliveryDate> deliveryDateRepository,
        IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository)
    {
        _deliveryDateRepository = deliveryDateRepository;
        _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
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
		logger.LogDebug(">>> Entering GetDeliveryDateByIdAsync(deliveryDateId={DeliveryDateId})", deliveryDateId);
		logger.LogDebug("<<< Exiting GetDeliveryDateByIdAsync(deliveryDateId={DeliveryDateId})", deliveryDateId);
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
		logger.LogDebug(">>> Entering GetAllDeliveryDatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var deliveryDates = await _deliveryDateRepository.GetAllAsync(query =>
        {
            return from dd in query
                orderby dd.DisplayOrder, dd.Id
                select dd;
				logger.LogInformation("GetAllDeliveryDatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllDeliveryDatesAsync()");
        return deliveryDates;
    }

    /// <summary>
    /// Insert a delivery date
    /// </summary>
    /// <param name="deliveryDate">Delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		logger.LogDebug(">>> Entering InsertDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _deliveryDateRepository.InsertAsync(deliveryDate);
		logger.LogDebug("<<< Exiting InsertDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertDeliveryDateAsync(deliveryDate)={DeliveryDate}: save query executed in {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertDeliveryDateAsync(deliveryDate)={DeliveryDate}: save query executed in {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Update the delivery date
    /// </summary>
    /// <param name="deliveryDate">Delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		logger.LogDebug(">>> Entering UpdateDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _deliveryDateRepository.UpdateAsync(deliveryDate);
		logger.LogDebug("<<< Exiting UpdateDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateDeliveryDateAsync(deliveryDate)={DeliveryDate}: update query executed in {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateDeliveryDateAsync(deliveryDate)={DeliveryDate}: update query executed in {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete a delivery date
    /// </summary>
    /// <param name="deliveryDate">The delivery date</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate)
    {
		logger.LogDebug(">>> Entering DeleteDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _deliveryDateRepository.DeleteAsync(deliveryDate);
		logger.LogDebug("<<< Exiting DeleteDeliveryDateAsync(deliveryDate={DeliveryDate})", deliveryDate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: external service call took {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: delete query executed in {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: external service call took {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteDeliveryDateAsync(deliveryDate)={DeliveryDate}: delete query executed in {Arg1} ms", deliveryDate, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId={ProductAvailabilityRangeId})", productAvailabilityRangeId);
		logger.LogDebug("<<< Exiting GetProductAvailabilityRangeByIdAsync(productAvailabilityRangeId={ProductAvailabilityRangeId})", productAvailabilityRangeId);
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
		logger.LogDebug(">>> Entering GetAllProductAvailabilityRangesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _productAvailabilityRangeRepository.GetAllAsync(query =>
        {
            return from par in query
                orderby par.DisplayOrder, par.Id
                select par;
				logger.LogInformation("GetAllProductAvailabilityRangesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);
		logger.LogDebug("<<< Exiting GetAllProductAvailabilityRangesAsync()");
    }

    /// <summary>
    /// Insert the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		logger.LogDebug(">>> Entering InsertProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAvailabilityRangeRepository.InsertAsync(productAvailabilityRange);
		logger.LogDebug("<<< Exiting InsertProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: save query executed in {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: save query executed in {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Update the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		logger.LogDebug(">>> Entering UpdateProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAvailabilityRangeRepository.UpdateAsync(productAvailabilityRange);
		logger.LogDebug("<<< Exiting UpdateProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: update query executed in {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: update query executed in {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete the product availability range
    /// </summary>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAvailabilityRangeAsync(ProductAvailabilityRange productAvailabilityRange)
    {
		logger.LogDebug(">>> Entering DeleteProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAvailabilityRangeRepository.DeleteAsync(productAvailabilityRange);
		logger.LogDebug("<<< Exiting DeleteProductAvailabilityRangeAsync(productAvailabilityRange={ProductAvailabilityRange})", productAvailabilityRange);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: external service call took {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: delete query executed in {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: external service call took {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteProductAvailabilityRangeAsync(productAvailabilityRange)={ProductAvailabilityRange}: delete query executed in {Arg1} ms", productAvailabilityRange, __logStart.ElapsedMilliseconds);
    #endregion

    #endregion
}
