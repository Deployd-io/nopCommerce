using System.ComponentModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the invoice document
/// </summary>
public partial class InvoiceDocument : PdfDocument<ProductItem>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    protected virtual PdfGrid CreateAdressesInfo()
    {
		_logger.LogError(">>> Entering CreateAdressesInfo()");
        var hasShipping = !string.IsNullOrEmpty(ShippingAddress.ShippingMethod);
        var addressesTable = PdfDocumentHelper.BuildPdfGrid(numColumns: hasShipping ? 2 : 1, DocumentRunDirection);
		_logger.LogError("CreateAdressesInfo(): addressesTable → {AddressesTable}", addressesTable);
		_logger.LogError("CreateAdressesInfo(): hasShipping → {HasShipping}", hasShipping);

        var billingInfo = PdfDocumentHelper.BuildPdfPCell(BuildAddressTable<InvoiceDocument>(source => BillingAddress, BillingAddress), DocumentRunDirection);
		_logger.LogError("CreateAdressesInfo(): billingInfo → {BillingInfo}", billingInfo);
        addressesTable.AddCell(billingInfo);

        if (hasShipping)
        {
            var shippingInfo = PdfDocumentHelper.BuildPdfPCell(BuildAddressTable<InvoiceDocument>(source => ShippingAddress, ShippingAddress), DocumentRunDirection);
			_logger.LogError("CreateAdressesInfo(): shippingInfo → {ShippingInfo}", shippingInfo);
            addressesTable.AddCell(shippingInfo);
        }

		_logger.LogWarning("<<< Exiting CreateAdressesInfo()");
        return addressesTable;
    }

    protected virtual PdfGrid CreateInvoiceHeader()
    {
		_logger.LogError(">>> Entering CreateInvoiceHeader()");
        var headerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

		_logger.LogError("CreateInvoiceHeader(): headerTable → {HeaderTable}", headerTable);
        var info = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
		_logger.LogError("CreateInvoiceHeader(): info → {Info}", info);
        info.SpacingAfter = 15;
		_logger.LogDebug("CreateInvoiceHeader(): info.SpacingAfter → {Arg0}", info.SpacingAfter);

        info.AddCell(BuildPdfPCell<InvoiceDocument>(source => OrderNumberText, OrderNumberText));
        info.AddCell(BuildHyperLinkCell<InvoiceDocument>(source => StoreUrl, StoreUrl));
        info.AddCell(BuildTextCell<InvoiceDocument>(source => OrderDateUser, OrderDateUser));

        headerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(info, DocumentRunDirection, horizontalAlign: Element.ALIGN_LEFT));

        if (LogoData is not null)
        {
            var logo = PdfImageHelper.GetITextSharpImageFromByteArray(LogoData);
			_logger.LogError("CreateInvoiceHeader(): logo → {Logo}", logo);
            headerTable.AddCell(new PdfPCell(logo, fit: true)
            {
                Border = 0,
                FixedHeight = 65,
                RunDirection = DocumentRunDirection,
                VerticalAlignment = Element.ALIGN_CENTER,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
			_logger.LogDebug("CreateInvoiceHeader(): HorizontalAlignment → {HorizontalAlignment}", HorizontalAlignment);
        }
        else
        {
            headerTable.AddCell(new PdfPCell(new Phrase())
            {
                Border = 0,
                Padding = 0
            });
			_logger.LogDebug("CreateInvoiceHeader(): Padding → {Padding}", Padding);
        }

        headerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(CreateAdressesInfo(), DocumentRunDirection, collSpan: 2));

		_logger.LogWarning("<<< Exiting CreateInvoiceHeader()");
        return headerTable;
    }

    protected virtual PdfGrid CreateFooter(FooterData footerData)
    {
		_logger.LogError(">>> Entering CreateFooter(footerData={FooterData})", footerData);
        var footerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

		_logger.LogError("CreateFooter(footerData={FooterData}): footerTable → {FooterTable}", footerData, footerTable);
        if (!FooterTextColumn1.Any() && !FooterTextColumn2.Any())
            return footerTable;

        var footer1Table = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
		_logger.LogError("CreateFooter(footerData={FooterData}): footer1Table → {Footer1Table}", footerData, footer1Table);
        foreach (var line in FooterTextColumn1)
            footer1Table.AddCell(BuildPdfPCell(line));

        var footer2Table = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
		_logger.LogError("CreateFooter(footerData={FooterData}): footer2Table → {Footer2Table}", footerData, footer2Table);
        foreach (var line in FooterTextColumn2)
            footer2Table.AddCell(BuildPdfPCell(line));

        footerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(footer1Table, DocumentRunDirection));
        footerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(footer2Table, DocumentRunDirection));

        footerTable.AddCell(BuildPdfPCell($"- {footerData.CurrentPageNumber} -", collSpan: 2, horizontalAlign: Element.ALIGN_CENTER));

		_logger.LogWarning("<<< Exiting CreateFooter(footerData={FooterData})", footerData);
        return footerTable;
    }

    protected virtual PdfGrid CreateSummary()
    {
		_logger.LogError(">>> Entering CreateSummary()");
        var summaryData = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

		_logger.LogError("CreateSummary(): summaryData → {SummaryData}", summaryData);
        if (!string.IsNullOrEmpty(Totals.SubTotal))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.SubTotal, Totals.SubTotal));
        if (!string.IsNullOrEmpty(Totals.Discount))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.Discount, Totals.Discount));
        if (!string.IsNullOrEmpty(Totals.Shipping))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.Shipping, Totals.Shipping));
        if (!string.IsNullOrEmpty(Totals.PaymentMethodAdditionalFee))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.PaymentMethodAdditionalFee, Totals.PaymentMethodAdditionalFee));
        if (!string.IsNullOrEmpty(Totals.Tax))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.Tax, Totals.Tax));

        foreach (var rate in Totals.TaxRates)
            summaryData.AddCell(BuildPdfPCell(rate));

        foreach (var card in Totals.GiftCards)
            summaryData.AddCell(BuildPdfPCell(card));

        if (!string.IsNullOrEmpty(Totals.RewardPoints))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.RewardPoints, Totals.RewardPoints));
        if (!string.IsNullOrEmpty(Totals.OrderTotal))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.OrderTotal, Totals.OrderTotal));

		_logger.LogWarning("<<< Exiting CreateSummary()");
        return summaryData;
    }

    protected virtual PdfGrid CreateCheckoutAttributes()
    {
		_logger.LogError(">>> Entering CreateCheckoutAttributes()");
        var attributesData = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

		_logger.LogError("CreateCheckoutAttributes(): attributesData → {AttributesData}", attributesData);
        attributesData.AddCell(BuildPdfPCell(CheckoutAttributes));

		_logger.LogWarning("<<< Exiting CreateCheckoutAttributes()");
        return attributesData;
    }

    protected virtual PdfGrid CreateOrderNotes()
    {
		_logger.LogError(">>> Entering CreateOrderNotes()");
        var notesTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

		_logger.LogError("CreateOrderNotes(): notesTable → {NotesTable}", notesTable);
        if (OrderNotes?.Any() != true)
            return notesTable;

        notesTable.SetWidths([2, 5]);

        var fontBold = PdfDocumentHelper.GetFont(Font, Font.Size, DocumentFontStyle.Bold);
		_logger.LogError("CreateOrderNotes(): fontBold → {FontBold}", fontBold);
        var label = LabelField<InvoiceDocument, List<(string, string)>>(invoice => invoice.OrderNotes, fontBold, Language);
		_logger.LogError("CreateOrderNotes(): label → {Label}", label);

        notesTable.AddCell(
            new PdfPCell(new Phrase(label))
            {
                Border = 0,
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                PaddingBottom = 5,
                RunDirection = DocumentRunDirection,
            });

        foreach (var (date, note) in OrderNotes)
        {
            notesTable.AddCell(BuildPdfPCell(Language.Rtl ? date.FixWeakCharacters() : date));
            notesTable.AddCell(BuildPdfPCell(note));
        }

		_logger.LogWarning("<<< Exiting CreateOrderNotes()");
        return notesTable;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate the invoice
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
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(Products);
            })
            .PagesFooter(footer =>
            {
                footer.InlineFooter(inlineFooter =>
                {
                    inlineFooter.FooterProperties(new FooterBasicProperties
                    {
                        PdfFont = footer.PdfFont,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        RunDirection = Language.Rtl ? PdfRunDirection.RightToLeft : PdfRunDirection.LeftToRight
                    });
                    inlineFooter.AddPageFooter(data => CreateFooter(data));
                });
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Name, width: 10, printProductAttributes: true));
                if (ShowSkuInProductList)
                    columns.AddColumn(column => ConfigureProductColumn(column, p => p.Sku, width: 3));
                if (ShowVendorInProductList)
                    columns.AddColumn(column => ConfigureProductColumn(column, p => p.VendorName, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Price, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Quantity, width: 2));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Total, width: 3));
            })
            .MainTableEvents(events =>
            {
                events.MainTableCreated(events =>
                {
                    //add to body, since adding hyperlinks to document header is not allowed
                    events.PdfDoc.Add(CreateInvoiceHeader());
                });
                events.MainTableAdded(events =>
                {
                    var summaryTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 3, DocumentRunDirection);
					_logger.LogError("Generate(pdfStreamOutput={PdfStreamOutput}): summaryTable → {SummaryTable}", pdfStreamOutput, summaryTable);
                    summaryTable.AddCell(PdfDocumentHelper.BuildPdfPCell(CreateCheckoutAttributes(), DocumentRunDirection, 3, horizontalAlign: Element.ALIGN_LEFT));

                    summaryTable.AddCell(new PdfPCell() { Colspan = 2, Border = 0 });
                    summaryTable.AddCell(PdfDocumentHelper.BuildPdfPCell(CreateSummary(), DocumentRunDirection));

                    events.PdfDoc.Add(summaryTable);
                    events.PdfDoc.Add(CreateOrderNotes());
                });
            })
            .Generate(builder => builder.AsPdfStream(pdfStreamOutput, closeStream: false));
			_logger.LogWarning("<<< Exiting Generate(pdfStreamOutput={PdfStreamOutput})", pdfStreamOutput);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the logo binary
    /// </summary>
    public byte[] LogoData { get; set; }

    /// <summary>
    /// Gets or sets the date and time of order creation
    /// </summary>
    [DisplayName("Pdf.OrderDate")]
    public required string OrderDateUser { get; init; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Pdf.Order")]
    public required string OrderNumberText { get; init; }

    /// <summary>
    /// Gets or sets store location
    /// </summary>
    public string StoreUrl { get; init; }

    /// <summary>
    /// Gets or sets the billing address
    /// </summary>
    [DisplayName("Pdf.BillingInformation")]
    public required AddressItem BillingAddress { get; init; }

    /// <summary>
    /// Gets or sets the shipping address
    /// </summary>
    [DisplayName("Pdf.ShippingInformation")]
    public AddressItem ShippingAddress { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to display product SKU in the invoice document
    /// </summary>
    public bool ShowSkuInProductList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display vendor name in the invoice document
    /// </summary>
    public bool ShowVendorInProductList { get; set; }

    /// <summary>
    /// Gets or sets the checkout attribute description
    /// </summary>
    public string CheckoutAttributes { get; set; }

    /// <summary>
    /// Gets or sets order totals
    /// </summary>
    public InvoiceTotals Totals { get; set; } = new();

    /// <summary>
    /// Gets or sets order notes
    /// </summary>
    [DisplayName("Pdf.OrderNotes")]
    public List<(string, string)> OrderNotes { get; set; }

    /// <summary>
    /// Gets or sets the text that will appear at the bottom of invoice (column 1)
    /// </summary>
    public List<string> FooterTextColumn1 { get; set; } = new();

    /// <summary>
    /// Gets or sets the text that will appear at the bottom of invoice (column 2)
    /// </summary>
    public List<string> FooterTextColumn2 { get; set; } = new();

    #endregion
}