using System.ComponentModel;
using PdfRpt.Core.Contracts;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the shipment document
/// </summary>
public partial class ShipmentDocument : PdfDocument<ProductItem>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    private PdfGrid CreateDefaultHeader()
    {
		_logger.LogError(">>> Entering CreateDefaultHeader()");
        var headerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

		_logger.LogError("CreateDefaultHeader(): headerTable → {HeaderTable}", headerTable);
        headerTable.AddCell(BuildPdfPCell<ShipmentDocument>(source => OrderNumberText, OrderNumberText));
        headerTable.AddCell(BuildPdfPCell<ShipmentDocument>(source => ShipmentNumberText, ShipmentNumberText));

		_logger.LogWarning("<<< Exiting CreateDefaultHeader()");
        return headerTable;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate shipment
    /// </summary>
    /// <param name="pdfStreamOutput">Stream for PDF output</param>
    public override void Generate(Stream pdfStreamOutput)
    {
		_logger.LogError(">>> Entering Generate(pdfStreamOutput={PdfStreamOutput})", pdfStreamOutput);
        Document
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .PagesHeader(header =>
            {
                header.InlineHeader(inlineHeader =>
                {
                    inlineHeader.AddPageHeader(data => CreateDefaultHeader());
                });
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(Products);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Name, width: 10));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Sku, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Quantity, width: 3));

            })
            .MainTableEvents(events => events.MainTableCreated(events =>
            {
                events.PdfDoc.Add(BuildAddressTable<ShipmentDocument>(p => p.Address, Address));
            }))
            .Generate(builder => builder.AsPdfStream(pdfStreamOutput, closeStream: false));
			_logger.LogWarning("<<< Exiting Generate(pdfStreamOutput={PdfStreamOutput})", pdfStreamOutput);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the shipping address
    /// </summary>
    [DisplayName("Pdf.ShippingInformation")]
    public required AddressItem Address { get; init; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Pdf.Order")]
    public required string OrderNumberText { get; init; }

    /// <summary>
    /// Gets or sets the shipment number
    /// </summary>
    [DisplayName("Pdf.Shipment")]
    public required string ShipmentNumberText { get; init; }

    #endregion
}