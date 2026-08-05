using Microsoft.Extensions.Logging;
﻿using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Validators;

public partial class ValidatorUtilities
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static bool PageSizeOptionsValidator(string value)
    {
		_logger.LogDebug(">>> Entering PageSizeOptionsValidator(value={Value})", value);
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }
        var notValid = value.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
		_logger.LogDebug("PageSizeOptionsValidator(value={Value}): notValid → {NotValid}", value, notValid);
		_logger.LogDebug("<<< Exiting PageSizeOptionsValidator(value={Value})", value);
        return !notValid;
    }

    public static bool PageSizeOptionsInAdvancedSettingsValidator(SettingModel model, string value)
    {
		_logger.LogDebug(">>> Entering PageSizeOptionsInAdvancedSettingsValidator(model={Model},value={Value})", model, value);
        if (model.Name.ToLowerInvariant().Contains("pagesizeoptions"))
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var notValid = value.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
            return !notValid;
        }
		_logger.LogDebug("<<< Exiting PageSizeOptionsInAdvancedSettingsValidator(model={Model},value={Value})", model, value);
        return true;
    }
}