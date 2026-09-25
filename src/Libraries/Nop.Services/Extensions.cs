using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services;

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Convert to select list
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="enumObj">Enum</param>
    /// <param name="markCurrentAsSelected">Mark current value as selected</param>
    /// <param name="valuesToExclude">Values to exclude</param>
    /// <param name="useLocalization">Localize</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the selectList
    /// </returns>
    public static async Task<SelectList> ToSelectListAsync<TEnum>(this TEnum enumObj,
        bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true) where TEnum : struct
    {
		_logger.LogError(">>> Entering ToSelectListAsync(enumObj={EnumObj},markCurrentAsSelected={MarkCurrentAsSelected},valuesToExclude={ValuesToExclude},useLocalization={UseLocalization})", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization);
			_logger.LogError("Throwing ArgumentException from ToSelectListAsync(enumObj={EnumObj},markCurrentAsSelected={MarkCurrentAsSelected},valuesToExclude={ValuesToExclude},useLocalization={UseLocalization})", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization);
        if (!typeof(TEnum).IsEnum) {
            throw new ArgumentException("An Enumeration type is required.", nameof(enumObj));
        }

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogError("ToSelectListAsync(enumObj={EnumObj},markCurrentAsSelected={MarkCurrentAsSelected},valuesToExclude={ValuesToExclude},useLocalization={UseLocalization}): localizationService → {LocalizationService}", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization, localizationService);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var values = await Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
            .SelectAwait(async enumValue => new
            {
                ID = Convert.ToInt32(enumValue),
                Name = useLocalization
                    ? await localizationService.GetLocalizedEnumAsync(enumValue)
                    : CommonHelper.SplitCamelCaseWord(enumValue.ToString())
            }).ToListAsync();
					_logger.LogDebug("ToSelectListAsync(enumObj={EnumObj},markCurrentAsSelected={MarkCurrentAsSelected},valuesToExclude={ValuesToExclude},useLocalization={UseLocalization}): Name → {Name}", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization, Name);
					_logger.LogError("ToSelectListAsync(enumObj,markCurrentAsSelected,valuesToExclude,useLocalization)={EnumObj},{MarkCurrentAsSelected},{ValuesToExclude},{UseLocalization}: database query executed in {Arg4} ms", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization, __logStart.ElapsedMilliseconds);

        object selectedValue = null;
		_logger.LogError("ToSelectListAsync(enumObj={EnumObj},markCurrentAsSelected={MarkCurrentAsSelected},valuesToExclude={ValuesToExclude},useLocalization={UseLocalization}): selectedValue → {SelectedValue}", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization, selectedValue);
        if (markCurrentAsSelected)
            selectedValue = Convert.ToInt32(enumObj);
		_logger.LogWarning("<<< Exiting ToSelectListAsync(enumObj={EnumObj},markCurrentAsSelected={MarkCurrentAsSelected},valuesToExclude={ValuesToExclude},useLocalization={UseLocalization})", enumObj, markCurrentAsSelected, valuesToExclude, useLocalization);
        return new SelectList(values, "ID", "Name", selectedValue);
    }

    /// <summary>
    /// Convert to select list
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="objList">List of objects</param>
    /// <param name="selector">Selector for name</param>
    /// <returns>SelectList</returns>
    public static SelectList ToSelectList<T>(this T objList, Func<BaseEntity, string> selector) where T : IEnumerable<BaseEntity>
    {
		_logger.LogError(">>> Entering ToSelectList(objList={ObjList},selector={Selector})", objList, selector);
		_logger.LogWarning("<<< Exiting ToSelectList(objList={ObjList},selector={Selector})", objList, selector);
        return new SelectList(objList.Select(p => new { ID = p.Id, Name = selector(p) }), "ID", "Name");
    }

    /// <summary>
    /// Convert constants defined in the passed type to select list 
    /// </summary>
    /// <param name="items">Collection add items</param>
    /// <param name="type">Type to extract constants</param>
    /// <param name="useLocalization">Localize</param>
    /// <param name="sortItems">Sort resulting items (<see cref="SelectListItem"/>)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the select list
    /// </returns>
    public static async Task<List<SelectListItem>> ConstantsToSelectListAsync(this IList<SelectListItem> items,
        Type type, bool useLocalization = true, bool sortItems = false)
    {
		_logger.LogError(">>> Entering ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems})", items, type, useLocalization, sortItems);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(type);

        var result = new List<SelectListItem>();
		_logger.LogError("ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems}): result → {Result}", items, type, useLocalization, sortItems, result);
        var fields = type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly);

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogError("ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems}): localizationService → {LocalizationService}", items, type, useLocalization, sortItems, localizationService);

        foreach (var prop in fields)
        {
            var resourceTypeName = Regex.Replace(type.FullName, "\\W+", ".");
			_logger.LogError("ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems}): resourceTypeName → {ResourceTypeName}", items, type, useLocalization, sortItems, resourceTypeName);
            var resourceConstantName = CommonHelper.SnakeCaseToPascalCase(prop.Name);
			_logger.LogError("ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems}): resourceConstantName → {ResourceConstantName}", items, type, useLocalization, sortItems, resourceConstantName);

            var resourceName = $"{NopLocalizationDefaults.LiteralLocaleStringResourcesPrefix}{resourceTypeName}.{resourceConstantName}";
			_logger.LogError("ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems}): resourceName → {ResourceName}", items, type, useLocalization, sortItems, resourceName);
            var resourceValue = useLocalization ? await localizationService.GetResourceAsync(resourceName) : resourceConstantName;
			_logger.LogError("ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems}): resourceValue → {ResourceValue}", items, type, useLocalization, sortItems, resourceValue);

            result.Add(new SelectListItem { Value = (string)prop.GetRawConstantValue(), Text = resourceValue });
        }

		_logger.LogWarning("<<< Exiting ConstantsToSelectListAsync(items={Items},type={Type},useLocalization={UseLocalization},sortItems={SortItems})", items, type, useLocalization, sortItems);
        return sortItems ? result.OrderBy(item => item.Text).ToList() : result;
    }

    /// <summary>
    /// Convert to lookup-like dictionary, for JSON serialization
    /// </summary>
    /// <typeparam name="T">Source type</typeparam>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <param name="xs">List of objects</param>
    /// <param name="keySelector">A key-selector function</param>
    /// <param name="valueSelector">A value-selector function</param>
    /// <returns>A dictionary with values grouped by key</returns>
    public static IDictionary<TKey, IList<TValue>> ToGroupedDictionary<T, TKey, TValue>(
        this IEnumerable<T> xs,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector)
    {
        var result = new Dictionary<TKey, IList<TValue>>();

        foreach (var x in xs)
        {
            var key = keySelector(x);
            var value = valueSelector(x);

            if (result.TryGetValue(key, out var list))
                list.Add(value);
            else
                result[key] = new List<TValue> { value };
        }

        return result;
    }

    /// <summary>
    /// Convert to lookup-like dictionary, for JSON serialization
    /// </summary>
    /// <typeparam name="T">Source type</typeparam>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <param name="xs">List of objects</param>
    /// <param name="keySelector">A key-selector function</param>
    /// <returns>A dictionary with values grouped by key</returns>
    public static IDictionary<TKey, IList<T>> ToGroupedDictionary<T, TKey>(
        this IEnumerable<T> xs,
        Func<T, TKey> keySelector)
    {
        return xs.ToGroupedDictionary(keySelector, x => x);
    }
}