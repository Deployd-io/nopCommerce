using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Validators;

public partial class ValidatorUtilities
{
    public static bool PageSizeOptionsValidator(string value)
    {
		logger.LogDebug(">>> Entering PageSizeOptionsValidator(value={Value})", value);
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }
        var notValid = value.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
		logger.LogDebug("PageSizeOptionsValidator(value={Value}): notValid → {NotValid}", value, notValid);
		logger.LogDebug("<<< Exiting PageSizeOptionsValidator(value={Value})", value);
        return !notValid;
    }

    public static bool PageSizeOptionsInAdvancedSettingsValidator(SettingModel model, string value)
    {
		logger.LogDebug(">>> Entering PageSizeOptionsInAdvancedSettingsValidator(model={Model},value={Value})", model, value);
        if (model.Name.ToLowerInvariant().Contains("pagesizeoptions"))
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var notValid = value.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
            return !notValid;
        }
		logger.LogDebug("<<< Exiting PageSizeOptionsInAdvancedSettingsValidator(model={Model},value={Value})", model, value);
        return true;
    }
}
