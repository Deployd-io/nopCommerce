using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Models;

public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
{
    public ConfigurationModel()
    {
		_logger.LogDebug(">>> Entering ConfigurationModel()");
        Locales = new List<ConfigurationLocalizedModel>();
		_logger.LogDebug("ConfigurationModel(): Locales → {Locales}", Locales);
		_logger.LogDebug("<<< Exiting ConfigurationModel()");
    }

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.DescriptionText")]
    public string DescriptionText { get; set; }
    public bool DescriptionText_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.AdditionalFee")]
    public decimal AdditionalFee { get; set; }
    public bool AdditionalFee_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.AdditionalFeePercentage")]
    public bool AdditionalFeePercentage { get; set; }
    public bool AdditionalFeePercentage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.ShippableProductRequired")]
    public bool ShippableProductRequired { get; set; }
    public bool ShippableProductRequired_OverrideForStore { get; set; }

    public IList<ConfigurationLocalizedModel> Locales { get; set; }

    #region Nested class

    public class ConfigurationLocalizedModel : ILocalizedLocaleModel
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.DescriptionText")]
        public string DescriptionText { get; set; }
    }

    #endregion

}