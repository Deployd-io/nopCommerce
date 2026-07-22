using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Html;

namespace Nop.Services.Vendors;

/// <summary>
/// Vendor service
/// </summary>
public partial class VendorService : IVendorService
{
    #region Fields

    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<Vendor> _vendorRepository;
    protected readonly IRepository<VendorNote> _vendorNoteRepository;

    #endregion

    #region Ctor

    public VendorService(IHtmlFormatter htmlFormatter,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IRepository<Vendor> vendorRepository,
        IRepository<VendorNote> vendorNoteRepository)
    {
        _htmlFormatter = htmlFormatter;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _vendorRepository = vendorRepository;
        _vendorNoteRepository = vendorNoteRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a vendor by vendor identifier
    /// </summary>
    /// <param name="vendorId">Vendor identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor
    /// </returns>
    public virtual async Task<Vendor> GetVendorByIdAsync(int vendorId)
    {
		logger.LogDebug(">>> Entering GetVendorByIdAsync(vendorId={VendorId})", vendorId);
		logger.LogDebug("<<< Exiting GetVendorByIdAsync(vendorId={VendorId})", vendorId);
        return await _vendorRepository.GetByIdAsync(vendorId, cache => default);
    }

    /// <summary>
    /// Gets a vendor by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor
    /// </returns>
    public virtual async Task<Vendor> GetVendorByProductIdAsync(int productId)
    {
		logger.LogDebug(">>> Entering GetVendorByProductIdAsync(productId={ProductId})", productId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (productId == 0)
		logger.LogDebug("<<< Exiting GetVendorByProductIdAsync(productId={ProductId})", productId);
            return null;

        return await (from v in _vendorRepository.Table
            join p in _productRepository.Table on v.Id equals p.VendorId
            where p.Id == productId
            select v).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets vendors by product identifiers
    /// </summary>
    /// <param name="productIds">Array of product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendors
    /// </returns>
    public virtual async Task<IList<Vendor>> GetVendorsByProductIdsAsync(int[] productIds)
    {
		logger.LogDebug(">>> Entering GetVendorsByProductIdsAsync(productIds={ProductIds})", productIds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(productIds);

		logger.LogDebug("<<< Exiting GetVendorsByProductIdsAsync(productIds={ProductIds})", productIds);
        return await (from v in _vendorRepository.Table
            join p in _productRepository.Table on v.Id equals p.VendorId
            where productIds.Contains(p.Id) && !v.Deleted && v.Active
            select v).Distinct().ToListAsync();
    }

    /// <summary>
    /// Gets a vendors by customers identifiers
    /// </summary>
    /// <param name="customerIds">Array of customer identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendors
    /// </returns>
    public virtual async Task<IList<Vendor>> GetVendorsByCustomerIdsAsync(int[] customerIds)
    {
		logger.LogDebug(">>> Entering GetVendorsByCustomerIdsAsync(customerIds={CustomerIds})", customerIds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customerIds);

		logger.LogDebug("<<< Exiting GetVendorsByCustomerIdsAsync(customerIds={CustomerIds})", customerIds);
        return await (from v in _vendorRepository.Table
            join c in _customerRepository.Table on v.Id equals c.VendorId
            where customerIds.Contains(c.Id) && !v.Deleted && v.Active
            select v).Distinct().ToListAsync();
    }

    /// <summary>
    /// Delete a vendor
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteVendorAsync(Vendor vendor)
    {
		logger.LogDebug(">>> Entering DeleteVendorAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _vendorRepository.DeleteAsync(vendor);
		logger.LogDebug("<<< Exiting DeleteVendorAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteVendorAsync(vendor)={Vendor}: external service call took {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteVendorAsync(vendor)={Vendor}: delete query executed in {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteVendorAsync(vendor)={Vendor}: external service call took {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteVendorAsync(vendor)={Vendor}: delete query executed in {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all vendors
    /// </summary>
    /// <param name="name">Vendor name</param>
    /// <param name="email">Vendor email</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendors
    /// </returns>
    public virtual async Task<IPagedList<Vendor>> GetAllVendorsAsync(string name = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
		logger.LogDebug(">>> Entering GetAllVendorsAsync(name={Name},email={Email},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", name, email, pageIndex, pageSize, showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var vendors = await _vendorRepository.GetAllPagedAsync(query =>
        {
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));

			logger.LogDebug("GetAllVendorsAsync(name={Name},email={Email},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): query → {Query}", name, email, pageIndex, pageSize, showHidden, query);
			logger.LogInformation("GetAllVendorsAsync(name,email,pageIndex,pageSize,showHidden)={Name},{Email},{PageIndex},{PageSize},{ShowHidden}: find query executed in {Arg5} ms", name, email, pageIndex, pageSize, showHidden, __logStart.ElapsedMilliseconds);
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(v => v.Email.Contains(email));

            if (!showHidden)
                query = query.Where(v => v.Active);

            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name).ThenBy(v => v.Email);

            return query;
        }, pageIndex, pageSize);

		logger.LogDebug("<<< Exiting GetAllVendorsAsync(name={Name},email={Email},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", name, email, pageIndex, pageSize, showHidden);
        return vendors;
    }

    /// <summary>
    /// Inserts a vendor
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertVendorAsync(Vendor vendor)
    {
		logger.LogDebug(">>> Entering InsertVendorAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _vendorRepository.InsertAsync(vendor);
		logger.LogDebug("<<< Exiting InsertVendorAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertVendorAsync(vendor)={Vendor}: save query executed in {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertVendorAsync(vendor)={Vendor}: save query executed in {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the vendor
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateVendorAsync(Vendor vendor)
    {
		logger.LogDebug(">>> Entering UpdateVendorAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _vendorRepository.UpdateAsync(vendor);
		logger.LogDebug("<<< Exiting UpdateVendorAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateVendorAsync(vendor)={Vendor}: update query executed in {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateVendorAsync(vendor)={Vendor}: update query executed in {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a vendor note
    /// </summary>
    /// <param name="vendorNoteId">The vendor note identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor note
    /// </returns>
    public virtual async Task<VendorNote> GetVendorNoteByIdAsync(int vendorNoteId)
    {
		logger.LogDebug(">>> Entering GetVendorNoteByIdAsync(vendorNoteId={VendorNoteId})", vendorNoteId);
		logger.LogDebug("<<< Exiting GetVendorNoteByIdAsync(vendorNoteId={VendorNoteId})", vendorNoteId);
        return await _vendorNoteRepository.GetByIdAsync(vendorNoteId, cache => default);
    }

    /// <summary>
    /// Gets all vendor notes
    /// </summary>
    /// <param name="vendorId">Vendor identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor notes
    /// </returns>
    public virtual async Task<IPagedList<VendorNote>> GetVendorNotesByVendorAsync(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		logger.LogDebug(">>> Entering GetVendorNotesByVendorAsync(vendorId={VendorId},pageIndex={PageIndex},pageSize={PageSize})", vendorId, pageIndex, pageSize);
        var query = _vendorNoteRepository.Table.Where(vn => vn.VendorId == vendorId);

		logger.LogDebug("GetVendorNotesByVendorAsync(vendorId={VendorId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", vendorId, pageIndex, pageSize, query);
        query = query.OrderBy(v => v.CreatedOnUtc).ThenBy(v => v.Id);

		logger.LogDebug("<<< Exiting GetVendorNotesByVendorAsync(vendorId={VendorId},pageIndex={PageIndex},pageSize={PageSize})", vendorId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Deletes a vendor note
    /// </summary>
    /// <param name="vendorNote">The vendor note</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteVendorNoteAsync(VendorNote vendorNote)
    {
		logger.LogDebug(">>> Entering DeleteVendorNoteAsync(vendorNote={VendorNote})", vendorNote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _vendorNoteRepository.DeleteAsync(vendorNote);
		logger.LogDebug("<<< Exiting DeleteVendorNoteAsync(vendorNote={VendorNote})", vendorNote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteVendorNoteAsync(vendorNote)={VendorNote}: external service call took {Arg1} ms", vendorNote, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteVendorNoteAsync(vendorNote)={VendorNote}: delete query executed in {Arg1} ms", vendorNote, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteVendorNoteAsync(vendorNote)={VendorNote}: external service call took {Arg1} ms", vendorNote, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteVendorNoteAsync(vendorNote)={VendorNote}: delete query executed in {Arg1} ms", vendorNote, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Inserts a vendor note
    /// </summary>
    /// <param name="vendorNote">Vendor note</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertVendorNoteAsync(VendorNote vendorNote)
    {
		logger.LogDebug(">>> Entering InsertVendorNoteAsync(vendorNote={VendorNote})", vendorNote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _vendorNoteRepository.InsertAsync(vendorNote);
		logger.LogDebug("<<< Exiting InsertVendorNoteAsync(vendorNote={VendorNote})", vendorNote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertVendorNoteAsync(vendorNote)={VendorNote}: save query executed in {Arg1} ms", vendorNote, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertVendorNoteAsync(vendorNote)={VendorNote}: save query executed in {Arg1} ms", vendorNote, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Formats the vendor note text
    /// </summary>
    /// <param name="vendorNote">Vendor note</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatVendorNoteText(VendorNote vendorNote)
    {
		logger.LogDebug(">>> Entering FormatVendorNoteText(vendorNote={VendorNote})", vendorNote);
        ArgumentNullException.ThrowIfNull(vendorNote);

        var text = vendorNote.Note;

		logger.LogDebug("FormatVendorNoteText(vendorNote={VendorNote}): text → {Text}", vendorNote, text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);

		logger.LogDebug("<<< Exiting FormatVendorNoteText(vendorNote={VendorNote})", vendorNote);
        return text;
    }

    #endregion
}
