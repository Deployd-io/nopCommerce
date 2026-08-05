using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a meta tags generator model
/// </summary>
public partial record MetaTagsGeneratorModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MetaTagsGeneratorModel()
    {
	_logger.LogDebug(">>> Entering MetaTagsGeneratorModel()");
	_logger.LogDebug("<<< Exiting MetaTagsGeneratorModel()");
    }

    public MetaTagsGeneratorModel(IMetaTagsSupportedModel model, string entityTypeName, int languagePosition = 0, int languageId = 0)
    {
		_logger.LogDebug(">>> Entering MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId})", model, entityTypeName, languagePosition, languageId);
        LanguageId = languageId;
        EntityType = entityTypeName;
		_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): EntityType → {EntityType}", model, entityTypeName, languagePosition, languageId, EntityType);
		_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): LanguageId → {LanguageId1}", model, entityTypeName, languagePosition, languageId, LanguageId);

        if (model is BaseNopEntityModel entityModel)
			_logger.LogDebug("<<< Exiting MetaTagsGeneratorModel()");
            EntityId = entityModel.Id;
			_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): EntityId → {EntityId}", model, entityTypeName, languagePosition, languageId, EntityId);

        var prefix = languageId > 0 ? $"Locales_{languagePosition}__" : string.Empty;

		_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): prefix → {Prefix}", model, entityTypeName, languagePosition, languageId, prefix);
        MetaTitleElementId = $"{prefix}{nameof(model.MetaTitle)}";
		_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): MetaTitleElementId → {MetaTitleElementId}", model, entityTypeName, languagePosition, languageId, MetaTitleElementId);
        MetaKeywordsElementId = $"{prefix}{nameof(model.MetaKeywords)}";
		_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): MetaKeywordsElementId → {MetaKeywordsElementId}", model, entityTypeName, languagePosition, languageId, MetaKeywordsElementId);
        MetaDescriptionElementId = $"{prefix}{nameof(model.MetaDescription)}";
    }

	_logger.LogDebug("MetaTagsGeneratorModel(model={Model},entityTypeName={EntityTypeName},languagePosition={LanguagePosition},languageId={LanguageId}): MetaDescriptionElementId → {MetaDescriptionElementId}", model, entityTypeName, languagePosition, languageId, MetaDescriptionElementId);
    #endregion

    #region Properties

    public string TitleFieldId { get; set; }
    public string TextFieldId { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public int LanguageId { get; set; }

    public string MetaKeywordsElementId { get; set; }
    public string MetaDescriptionElementId { get; set; }
    public string MetaTitleElementId { get; set; }

    #endregion
}