using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the catalog document
/// </summary>
public partial class CatalogDocument : PdfDocument<CatalogItem>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    protected virtual PdfPCell BuildProductImages(IList<CellData> rowData)
    {
		_logger.LogError(">>> Entering BuildProductImages(rowData={RowData})", rowData);
        var table = PdfDocumentHelper.BuildPdfGrid(2, DocumentRunDirection);

		_logger.LogError("BuildProductImages(rowData={RowData}): table → {Table}", rowData, table);
		_logger.LogDebug("BuildProductImages(rowData={RowData}): table.HorizontalAlignment → {Arg1}", rowData, table.HorizontalAlignment);
        table.HorizontalAlignment = Element.ALIGN_CENTER;

        var picPaths = (HashSet<string>)rowData.GetValueOf<CatalogItem>(x => x.PicturePaths) ?? new();
		_logger.LogError("BuildProductImages(rowData={RowData}): picPaths → {PicPaths}", rowData, picPaths);

        if (!picPaths.Any())
            return PdfDocumentHelper.BuildPdfPCell(table, DocumentRunDirection, horizontalAlign: Element.ALIGN_CENTER);

        foreach (var path in picPaths)
        {
            var image = PdfImageHelper.GetITextSharpImageFromByteArray(GetImageAsync(path).Result ?? Array.Empty<byte>());
			_logger.LogError("BuildProductImages(rowData={RowData}): image → {Image}", rowData, image);
            image.ScaleToFit(ImageTargetSize, ImageTargetSize);

            var imageCell = new PdfPCell(image, fit: false)
            {
                Border = 0,
                Padding = 10,
                VerticalAlignment = Element.ALIGN_CENTER,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            if (picPaths.Count % 2 != 0 && picPaths.Last() == path)
                imageCell.Colspan = 2;
				_logger.LogDebug("BuildProductImages(rowData={RowData}): imageCell.Colspan → {Arg1}", rowData, imageCell.Colspan);

            table.AddCell(imageCell);
        }

        return PdfDocumentHelper.BuildPdfPCell(table, DocumentRunDirection, horizontalAlign: Element.ALIGN_CENTER);
    }

    protected virtual PdfPCell BuildProductProperties(IList<CellData> rowData)
    {
		_logger.LogError(">>> Entering BuildProductProperties(rowData={RowData})", rowData);
        var table = PdfDocumentHelper.BuildPdfGrid(1, DocumentRunDirection);

		_logger.LogError("BuildProductProperties(rowData={RowData}): table → {Table}", rowData, table);
        var price = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Price);
		_logger.LogError("BuildProductProperties(rowData={RowData}): price → {Price}", rowData, price);
        if (!string.IsNullOrEmpty(price))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Price, price));

        var sku = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Sku);
		_logger.LogError("BuildProductProperties(rowData={RowData}): sku → {Sku}", rowData, sku);
        if (!string.IsNullOrEmpty(sku))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Sku, sku));

        var weight = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Weight);
		_logger.LogError("BuildProductProperties(rowData={RowData}): weight → {Weight}", rowData, weight);
        if (!string.IsNullOrEmpty(weight))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Weight, weight));

        var stock = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Stock);
		_logger.LogError("BuildProductProperties(rowData={RowData}): stock → {Stock}", rowData, stock);
        if (!string.IsNullOrEmpty(stock))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Stock, stock));

		_logger.LogWarning("<<< Exiting BuildProductProperties(rowData={RowData})", rowData);
        return PdfDocumentHelper.BuildPdfPCell(table, DocumentRunDirection);
    }

    protected virtual PdfPCell ConfigureCatalogItemTemplate(InlineFieldData data)
    {
		_logger.LogError(">>> Entering ConfigureCatalogItemTemplate(data={Data})", data);
        var font16Bold = PdfDocumentHelper.GetFont(Font, Font.Size * 1.6f, DocumentFontStyle.Bold);

		_logger.LogError("ConfigureCatalogItemTemplate(data={Data}): font16Bold → {Font16Bold}", data, font16Bold);
        var table = new PdfGrid(numColumns: 1)
        {
            WidthPercentage = 100,
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT
        };
        var rowData = data.Attributes.RowData.TableRowData;
        var name = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Name);
		_logger.LogError("ConfigureCatalogItemTemplate(data={Data}): name → {Name}", data, name);
        table.AddCell(new PdfPCell(new Phrase(name, font16Bold))
        {
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT,
            VerticalAlignment = Element.ALIGN_TOP,
            PaddingBottom = 14,
            Border = 0
        });
		_logger.LogDebug("ConfigureCatalogItemTemplate(data={Data}): Border → {Border}", data, Border);

        var description = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Description);
		_logger.LogError("ConfigureCatalogItemTemplate(data={Data}): description → {Description}", data, description);

        var cell = new PdfPCell(new Paragraph(description, Font))
        {
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT,
            VerticalAlignment = Element.ALIGN_TOP,
            Border = 0,
            PaddingBottom = 10
        };
        cell.SetLeading(0f, 1.3f);
        table.AddCell(cell);

        table.AddCell(BuildProductProperties(rowData));
        table.AddCell(BuildProductImages(rowData));

        var tableContainer = new PdfGrid(numColumns: 1)
        {
            WidthPercentage = 100,
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT
        };

        tableContainer.AddCell(new PdfPCell(table)
        {
            VerticalAlignment = Element.ALIGN_TOP,
            HorizontalAlignment = Element.ALIGN_CENTER,
            Border = 0,
        });

        return new PdfPCell(tableContainer)
        {
            RunDirection = DocumentRunDirection,
            Border = 0,
            MinimumHeight = 25,
            VerticalAlignment = Element.ALIGN_TOP
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate the catalog
    /// </summary>
    /// <param name="pdfStreamOutput">Stream for PDF output</param>
    public override void Generate(Stream pdfStreamOutput)
    {
		_logger.LogError(">>> Entering Generate(pdfStreamOutput={PdfStreamOutput})", pdfStreamOutput);
        Document
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.NumberOfDataRowsPerPage(1);
                table.ShowHeaderRow(false);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(Products);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    ConfigureProductColumn(column, p => p.Name);
                    column.ColumnItemsTemplate(itemsTemplate =>
                    {
                        itemsTemplate.InlineField(inlineField => inlineField.RenderCell(ConfigureCatalogItemTemplate));
                    });
                });

            })
            .Generate(builder => builder.AsPdfStream(pdfStreamOutput, closeStream: false));
			_logger.LogWarning("<<< Exiting Generate(pdfStreamOutput={PdfStreamOutput})", pdfStreamOutput);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a function to get an image binary data by the path
    /// </summary>
    public required Func<string, Task<byte[]>> GetImageAsync { get; init; }

    #endregion
}