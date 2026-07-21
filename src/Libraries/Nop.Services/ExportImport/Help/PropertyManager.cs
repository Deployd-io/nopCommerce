using System.Drawing;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Services.ExportImport.Help;

/// <summary>
/// Class for working with PropertyByName object list
/// </summary>
/// <typeparam name="T">Object type</typeparam>
public partial class PropertyManager<T>
{
    /// <summary>
    /// Default properties
    /// </summary>
    protected readonly Dictionary<string, PropertyByName<T>> _defaultProperties;

    /// <summary>
    /// Localized properties
    /// </summary>
    protected readonly Dictionary<string, PropertyByName<T>> _localizedProperties;

    /// <summary>
    /// Catalog settings
    /// </summary>
    protected readonly CatalogSettings _catalogSettings;

    /// <summary>
    /// Languages
    /// </summary>
    protected readonly IList<Language> _languages;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="defaultProperties">Default access properties</param>
    /// <param name="catalogSettings">Catalog settings</param>
    /// <param name="localizedProperties">Localized access properties</param>
    /// <param name="languages">Languages</param>
    public PropertyManager(IEnumerable<PropertyByName<T>> defaultProperties, CatalogSettings catalogSettings, IEnumerable<PropertyByName<T>> localizedProperties = null, IList<Language> languages = null)
    {
		logger.LogDebug(">>> Entering PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages})", defaultProperties, catalogSettings, localizedProperties, languages);
        _defaultProperties = new Dictionary<string, PropertyByName<T>>();
        _catalogSettings = catalogSettings;
		logger.LogDebug("PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages}): _defaultProperties → {_defaultProperties}", defaultProperties, catalogSettings, localizedProperties, languages, _defaultProperties);
		logger.LogDebug("PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages}): _catalogSettings → {_catalogSettings}", defaultProperties, catalogSettings, localizedProperties, languages, _catalogSettings);
        _localizedProperties = new Dictionary<string, PropertyByName<T>>();
        _languages = new List<Language>();
		logger.LogDebug("PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages}): _localizedProperties → {_localizedProperties}", defaultProperties, catalogSettings, localizedProperties, languages, _localizedProperties);
		logger.LogDebug("PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages}): _languages → {_languages}", defaultProperties, catalogSettings, localizedProperties, languages, _languages);

        if (languages != null)
            _languages = languages;

        var poz = 1;
        foreach (var propertyByName in defaultProperties.Where(p => !p.Ignore))
        {
            propertyByName.PropertyOrderPosition = poz;
			logger.LogDebug("PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages}): propertyByName.PropertyOrderPosition → {Arg4}", defaultProperties, catalogSettings, localizedProperties, languages, propertyByName.PropertyOrderPosition);
            poz++;
            _defaultProperties.Add(propertyByName.PropertyName, propertyByName);
        }

        if (_languages.Count >= 2 && localizedProperties != null)
        {
            var lpoz = 1;
            foreach (var propertyByName in localizedProperties.Where(p => !p.Ignore))
            {
                propertyByName.PropertyOrderPosition = lpoz;
                lpoz++;
                _localizedProperties.Add(propertyByName.PropertyName, propertyByName);
            }
        }
		logger.LogDebug("<<< Exiting PropertyManager(defaultProperties={DefaultProperties},catalogSettings={CatalogSettings},localizedProperties={LocalizedProperties},languages={Languages})", defaultProperties, catalogSettings, localizedProperties, languages);
    }

    /// <summary>
    /// Sets the cell's value.
    /// </summary>
    /// <param name="cell">The cell to set value</param>
    /// <param name="value">The cell value</param>
    protected virtual void SetCellValue(IXLCell cell, object value)
    {
        if (value == null)
        {
            cell.SetValue(Blank.Value);

            return;
        }

        cell.Value = value switch
        {
            int intValue => intValue,
            bool boolValue => boolValue,
            DateTime dateTimeValue => dateTimeValue,
            decimal decimalValue => decimalValue,
            double doubleValue => doubleValue,
            float floatValue => floatValue,
            _ => value.ToString()
        };
		logger.LogDebug("<<< Exiting SetCellValue(cell={Cell},value={Value})", cell, value);
    }

    /// <summary>
    /// Export objects to XLSX
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="itemsToExport">The objects to export</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public virtual async Task<byte[]> ExportToXlsxAsync(IEnumerable<T> itemsToExport)
    {
		logger.LogDebug(">>> Entering ExportToXlsxAsync(itemsToExport={ItemsToExport})", itemsToExport);
        await using var stream = new MemoryStream();
        // ok, we can run the real code of the sample now
		logger.LogDebug("ExportToXlsxAsync(itemsToExport={ItemsToExport}): stream → {Stream}", itemsToExport, stream);
        using (var workbook = new XLWorkbook())
        {
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            var worksheet = workbook.Worksheets.Add(typeof(T).Name);
            var fWorksheet = workbook.Worksheets.Add("DataForFilters");
            fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

            //create Headers and format them 
            WriteDefaultCaption(worksheet);

            var lwss = new Dictionary<Language, IXLWorksheet>();

            if (_languages.Count >= 2)
            {
                foreach (var language in _languages)
                {
                    var lws = workbook.Worksheets.Add(language.UniqueSeoCode);
                    lwss.Add(language, lws);
                    WriteLocalizedCaption(lws);
                }
            }

            var row = 2;
            foreach (var items in itemsToExport)
            {
                CurrentObject = items;
                await WriteDefaultToXlsxAsync(worksheet, row, fWorksheet: fWorksheet);

                foreach (var lws in lwss)
                {
                    CurrentLanguage = lws.Key;
                    await WriteLocalizedToXlsxAsync(lws.Value, row, fWorksheet: fWorksheet);
					logger.LogDebug("<<< Exiting WriteDefaultToXlsxAsync(worksheet={Worksheet},row={Row},cellOffset={CellOffset},fWorksheet={FWorksheet})", worksheet, row, cellOffset, fWorksheet);
                }

                row++;
            }

            workbook.SaveAs(stream);
        }

        CurrentObject = default;
		logger.LogDebug("ExportToXlsxAsync(itemsToExport={ItemsToExport}): CurrentObject → {CurrentObject}", itemsToExport, CurrentObject);
		logger.LogDebug("<<< Exiting ExportToXlsxAsync(itemsToExport={ItemsToExport})", itemsToExport);
        return stream.ToArray();
    }

    /// <summary>
    /// Current object to access
    /// </summary>
    public T CurrentObject { get; set; }

    /// <summary>
    /// Current language to access
    /// </summary>
    public Language CurrentLanguage { get; set; }

    /// <summary>
    /// Return property index
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns></returns>
    public virtual int GetIndex(string propertyName)
    {
		logger.LogDebug(">>> Entering GetIndex(propertyName={PropertyName})", propertyName);
        if (!_defaultProperties.TryGetValue(propertyName, out var value))
		logger.LogDebug("<<< Exiting GetIndex(propertyName={PropertyName})", propertyName);
            return -1;

        return value.PropertyOrderPosition;
    }

    /// <summary>
    /// Remove object by property name
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public virtual void Remove(string propertyName)
    {
		logger.LogDebug(">>> Entering Remove(propertyName={PropertyName})", propertyName);
        _defaultProperties.Remove(propertyName);
		logger.LogDebug("<<< Exiting Remove(propertyName={PropertyName})", propertyName);
    }

    /// <summary>
    /// Write default object data to XLSX worksheet
    /// </summary>
    /// <param name="worksheet">Data worksheet</param>
    /// <param name="row">Row index</param>
    /// <param name="cellOffset">Cell offset</param>
    /// <param name="fWorksheet">Filters worksheet</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task WriteDefaultToXlsxAsync(IXLWorksheet worksheet, int row, int cellOffset = 0, IXLWorksheet fWorksheet = null)
    {
        if (CurrentObject == null)
            return;

        var xlRrow = worksheet.Row(row);
        xlRrow.Style.Alignment.WrapText = false;
        foreach (var prop in _defaultProperties.Values)
        {
            var cell = xlRrow.Cell(prop.PropertyOrderPosition + cellOffset);
            if (prop.IsDropDownCell && _catalogSettings.ExportImportRelatedEntitiesByName)
            {
                var dropDownElements = prop.GetDropDownElements();
                if (!dropDownElements.Any())
                {
                    cell.Value = string.Empty;
                    continue;
                }

                cell.Value = prop.GetItemText(await prop.GetProperty(CurrentObject, CurrentLanguage));

                if (!UseDropdownLists)
                    continue;

                var validator = cell.GetDataValidation();
                validator.InCellDropdown = true;

                validator.IgnoreBlanks = prop.AllowBlank;

                if (fWorksheet == null)
                    continue;

                var fRow = 1;
                foreach (var dropDownElement in dropDownElements)
                {
                    var fCell = fWorksheet.Row(fRow++).Cell(prop.PropertyOrderPosition);

                    if (!fCell.IsEmpty() && fCell.Value.ToString() == dropDownElement)
                        break;

                    fCell.Value = dropDownElement;
                }

                validator.List(fWorksheet.Range(1, prop.PropertyOrderPosition, dropDownElements.Length, prop.PropertyOrderPosition), true);
            }
            else
            {
                var value = await prop.GetProperty(CurrentObject, CurrentLanguage);
                SetCellValue(cell, value);
            }
            cell.Style.Alignment.WrapText = false;
        }
    }

    /// <summary>
    /// Write localized data to XLSX worksheet
    /// </summary>
    /// <param name="worksheet">Data worksheet</param>
    /// <param name="row">Row index</param>
    /// <param name="cellOffset">Cell offset</param>
    /// <param name="fWorksheet">Filters worksheet</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task WriteLocalizedToXlsxAsync(IXLWorksheet worksheet, int row, int cellOffset = 0, IXLWorksheet fWorksheet = null)
    {
        if (CurrentObject == null)
            return;

        var xlRrow = worksheet.Row(row);
        xlRrow.Style.Alignment.WrapText = false;
        foreach (var prop in _localizedProperties.Values)
        {
            var cell = xlRrow.Cell(prop.PropertyOrderPosition + cellOffset);
            if (prop.IsDropDownCell && _catalogSettings.ExportImportRelatedEntitiesByName)
            {
                var dropDownElements = prop.GetDropDownElements();
                if (!dropDownElements.Any())
                {
                    cell.Value = string.Empty;
                    continue;
                }

                cell.Value = prop.GetItemText(await prop.GetProperty(CurrentObject, CurrentLanguage));

                if (!UseDropdownLists)
                    continue;

                var validator = cell.GetDataValidation();
                validator.InCellDropdown = true;

                validator.IgnoreBlanks = prop.AllowBlank;

                if (fWorksheet == null)
                    continue;

                var fRow = 1;
                foreach (var dropDownElement in dropDownElements)
                {
                    var fCell = fWorksheet.Row(fRow++).Cell(prop.PropertyOrderPosition);

                    if (!fCell.IsEmpty() && fCell.Value.ToString() == dropDownElement)
                        break;

                    fCell.Value = dropDownElement;
                }

                validator.List(fWorksheet.Range(1, prop.PropertyOrderPosition, dropDownElements.Length, prop.PropertyOrderPosition), true);
            }
            else
            {
                var value = await prop.GetProperty(CurrentObject, CurrentLanguage);
                SetCellValue(cell, value);
            }
            cell.Style.Alignment.WrapText = false;
        }
    }

    /// <summary>
    /// Read object data from default XLSX worksheet
    /// </summary>
    /// <param name="worksheet">worksheet</param>
    /// <param name="row">Row index</param>
    /// /// <param name="cellOffset">Cell offset</param>
    public virtual void ReadDefaultFromXlsx(IXLWorksheet worksheet, int row, int cellOffset = 0)
    {
        if (worksheet?.Cells() == null)
            return;

        foreach (var prop in _defaultProperties.Values)
        {
            prop.PropertyValue = worksheet.Row(row).Cell(prop.PropertyOrderPosition + cellOffset).Value;
        }
		logger.LogDebug("<<< Exiting ReadDefaultFromXlsx(worksheet={Worksheet},row={Row},cellOffset={CellOffset})", worksheet, row, cellOffset);
    }

    /// <summary>
    /// Read object data from localized XLSX worksheet
    /// </summary>
    /// <param name="worksheet">worksheet</param>
    /// <param name="row">Row index</param>
    /// /// <param name="cellOffset">Cell offset</param>
    public virtual void ReadLocalizedFromXlsx(IXLWorksheet worksheet, int row, int cellOffset = 0)
    {
        if (worksheet?.Cells() == null)
            return;

        foreach (var prop in _localizedProperties.Values)
        {
            prop.PropertyValue = worksheet.Row(row).Cell(prop.PropertyOrderPosition + cellOffset).Value;
        }
		logger.LogDebug("<<< Exiting ReadLocalizedFromXlsx(worksheet={Worksheet},row={Row},cellOffset={CellOffset})", worksheet, row, cellOffset);
    }

    /// <summary>
    /// Write caption (first row) to default XLSX worksheet
    /// </summary>
    /// <param name="worksheet">worksheet</param>
    /// <param name="row">Row number</param>
    /// <param name="cellOffset">Cell offset</param>
    public virtual void WriteDefaultCaption(IXLWorksheet worksheet, int row = 1, int cellOffset = 0)
    {
        foreach (var caption in _defaultProperties.Values)
        {
            var cell = worksheet.Row(row).Cell(caption.PropertyOrderPosition + cellOffset);
            cell.Value = caption.ToString();

            SetCaptionStyle(cell);
        }
    }

    /// <summary>
    /// Write caption (first row) to localized XLSX worksheet
    /// </summary>
    /// <param name="worksheet">worksheet</param>
    /// <param name="row">Row number</param>
    /// <param name="cellOffset">Cell offset</param>
    public virtual void WriteLocalizedCaption(IXLWorksheet worksheet, int row = 1, int cellOffset = 0)
    {
        foreach (var caption in _localizedProperties.Values)
        {
            var cell = worksheet.Row(row).Cell(caption.PropertyOrderPosition + cellOffset);
            cell.Value = caption.ToString();

            SetCaptionStyle(cell);
        }
    }

    /// <summary>
    /// Set caption style to excel cell
    /// </summary>
    /// <param name="cell">Excel cell</param>
    public virtual void SetCaptionStyle(IXLCell cell)
    {
        cell.Style.Fill.PatternType = XLFillPatternValues.Solid;
        cell.Style.Fill.BackgroundColor = XLColor.FromColor(Color.FromArgb(184, 204, 228));
        cell.Style.Font.Bold = true;
    }

    /// <summary>
    /// Count of default properties
    /// </summary>
    public int Count => _defaultProperties.Count;

    /// <summary>
    /// Get default property by name
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public virtual PropertyByName<T> GetDefaultProperty(string propertyName)
    {
		logger.LogDebug(">>> Entering GetDefaultProperty(propertyName={PropertyName})", propertyName);
		logger.LogDebug("<<< Exiting GetDefaultProperty(propertyName={PropertyName})", propertyName);
        return _defaultProperties.TryGetValue(propertyName, out var value) ? value : null;
    }

    /// <summary>
    /// Get localized property by name
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public virtual PropertyByName<T> GetLocalizedProperty(string propertyName)
    {
		logger.LogDebug(">>> Entering GetLocalizedProperty(propertyName={PropertyName})", propertyName);
		logger.LogDebug("<<< Exiting GetLocalizedProperty(propertyName={PropertyName})", propertyName);
        return _localizedProperties.TryGetValue(propertyName, out var value) ? value : null;
    }

    /// <summary>
    /// Get default property array
    /// </summary>
    public virtual PropertyByName<T>[] GetDefaultProperties => _defaultProperties.Values.ToArray();

    /// <summary>
    /// Get localized property array
    /// </summary>
    public virtual PropertyByName<T>[] GetLocalizedProperties => _localizedProperties.Values.ToArray();

    /// <summary>
    /// Set SelectList
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="list">SelectList</param>
    public virtual void SetSelectList(string propertyName, SelectList list)
    {
		logger.LogDebug(">>> Entering SetSelectList(propertyName={PropertyName},list={List})", propertyName, list);
        var tempProperty = GetDefaultProperty(propertyName);
        if (tempProperty != null)
			logger.LogDebug("SetSelectList(propertyName={PropertyName},list={List}): tempProperty → {TempProperty}", propertyName, list, tempProperty);
			logger.LogDebug("SetSelectList(propertyName={PropertyName},list={List}): tempProperty.DropDownElements → {Arg2}", propertyName, list, tempProperty.DropDownElements);
            tempProperty.DropDownElements = list;
			logger.LogDebug("<<< Exiting SetSelectList(propertyName={PropertyName},list={List})", propertyName, list);
    }

    /// <summary>
    /// Is caption
    /// </summary>
    public bool IsCaption => _defaultProperties.Values.All(p => p.IsCaption);

    /// <summary>
    /// Gets a value indicating whether need create dropdown list for export
    /// </summary>
    public bool UseDropdownLists => _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities && _catalogSettings.ExportImportRelatedEntitiesByName;
	logger.LogDebug("<<< Exiting WriteLocalizedToXlsxAsync(worksheet={Worksheet},row={Row},cellOffset={CellOffset},fWorksheet={FWorksheet})", worksheet, row, cellOffset, fWorksheet);
}
