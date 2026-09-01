using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Message template service
/// </summary>
public partial class MessageTemplateService : IMessageTemplateService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IRepository<MessageTemplate> _messageTemplateRepository;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public MessageTemplateService(
        IStaticCacheManager staticCacheManager,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IRepository<MessageTemplate> messageTemplateRepository,
        IStoreMappingService storeMappingService)
    {
		_logger.LogDebug(">>> Entering MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>)", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>): _staticCacheManager → {_staticCacheManager}", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository, _staticCacheManager);
        _languageService = languageService;
		_logger.LogDebug("MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>): _languageService → {_languageService}", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>): _localizationService → {_localizationService}", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>): _localizedEntityService → {_localizedEntityService}", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository, _localizedEntityService);
        _messageTemplateRepository = messageTemplateRepository;
		_logger.LogDebug("MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>): _messageTemplateRepository → {_messageTemplateRepository}", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository, _messageTemplateRepository);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("MessageTemplateService(staticCacheManager={StaticCacheManager},languageService={LanguageService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateRepository={MessageTemplateRepository},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", staticCacheManager, languageService, localizationService, localizedEntityService, messageTemplateRepository, _storeMappingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete a message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteMessageTemplateAsync(MessageTemplate messageTemplate)
    {
		_logger.LogDebug(">>> Entering DeleteMessageTemplateAsync(messageTemplate={MessageTemplate})", messageTemplate);
        await _messageTemplateRepository.DeleteAsync(messageTemplate);
    }

    /// <summary>
    /// Inserts a message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertMessageTemplateAsync(MessageTemplate messageTemplate)
    {
		_logger.LogDebug(">>> Entering InsertMessageTemplateAsync(messageTemplate={MessageTemplate})", messageTemplate);
        await _messageTemplateRepository.InsertAsync(messageTemplate);
    }

    /// <summary>
    /// Updates a message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMessageTemplateAsync(MessageTemplate messageTemplate)
    {
		_logger.LogDebug(">>> Entering UpdateMessageTemplateAsync(messageTemplate={MessageTemplate})", messageTemplate);
        await _messageTemplateRepository.UpdateAsync(messageTemplate);
    }

    /// <summary>
    /// Gets a message template
    /// </summary>
    /// <param name="messageTemplateId">Message template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template
    /// </returns>
    public virtual async Task<MessageTemplate> GetMessageTemplateByIdAsync(int messageTemplateId)
    {
		_logger.LogDebug(">>> Entering GetMessageTemplateByIdAsync(messageTemplateId={MessageTemplateId})", messageTemplateId);
		_logger.LogInformation("GetMessageTemplateByIdAsync(messageTemplateId)={MessageTemplateId}: find query executing", messageTemplateId);
        return await _messageTemplateRepository.GetByIdAsync(messageTemplateId, cache => default);
    }

    /// <summary>
    /// Gets message templates by the name
    /// </summary>
    /// <param name="messageTemplateName">Message template name</param>
    /// <param name="storeId">Store identifier; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of message templates
    /// </returns>
    public virtual async Task<IList<MessageTemplate>> GetMessageTemplatesByNameAsync(string messageTemplateName, int? storeId = null)
    {
		_logger.LogDebug(">>> Entering GetMessageTemplatesByNameAsync(messageTemplateName={MessageTemplateName},storeId={StoreId})", messageTemplateName, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(messageTemplateName))
			_logger.LogWarning("Throwing ArgumentException from GetMessageTemplatesByNameAsync(messageTemplateName={MessageTemplateName},storeId={StoreId})", messageTemplateName, storeId);
            throw new ArgumentException(nameof(messageTemplateName));

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopMessageDefaults.MessageTemplatesByNameCacheKey, messageTemplateName, storeId);
		_logger.LogWarning("GetMessageTemplatesByNameAsync(messageTemplateName={MessageTemplateName},storeId={StoreId}): key → {Key}", messageTemplateName, storeId, key);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            //get message templates with the passed name
            var templatesQuery = _messageTemplateRepository.Table
                .Where(messageTemplate => messageTemplate.Name.Equals(messageTemplateName));

            if (storeId.HasValue && storeId.Value > 0)
            {
                templatesQuery = await _storeMappingService.ApplyStoreMapping(templatesQuery, storeId.Value);
            }

			_logger.LogInformation("GetMessageTemplatesByNameAsync(messageTemplateName,storeId)={MessageTemplateName},{StoreId}: external service call took {Arg2} ms", messageTemplateName, storeId, __logStart.ElapsedMilliseconds);
            return await templatesQuery.OrderBy(messageTemplate => messageTemplate.Id)
            .ToListAsync();
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetMessageTemplatesByNameAsync(messageTemplateName,storeId)={MessageTemplateName},{StoreId}: database query executed in {Arg2} ms", messageTemplateName, storeId, __logStartDb.ElapsedMilliseconds);
        });
		_logger.LogDebug("<<< Exiting GetMessageTemplatesByNameAsync(messageTemplateName={MessageTemplateName},storeId={StoreId})", messageTemplateName, storeId);
    }

    /// <summary>
    /// Gets all message templates
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="keywords">Keywords to search by name, body, or subject</param>
    /// <param name="isActive">A value indicating whether to get active records; "null" to load all records; "false" to load only inactive records; "true" to load only active records</param>
    /// <param name="emailAccountId">Email account identifier; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template list
    /// </returns>
    public virtual async Task<IList<MessageTemplate>> GetAllMessageTemplatesAsync(int storeId, string keywords = null, bool? isActive = null, int emailAccountId = 0)
    {
		_logger.LogDebug(">>> Entering GetAllMessageTemplatesAsync(storeId={StoreId},keywords={Keywords},isActive={IsActive},emailAccountId={EmailAccountId})", storeId, keywords, isActive, emailAccountId);
        var messageTemplates = await _messageTemplateRepository.GetAllAsync(async query =>
        {
			_logger.LogDebug("GetAllMessageTemplatesAsync(storeId={StoreId},keywords={Keywords},isActive={IsActive},emailAccountId={EmailAccountId}): query → {Query}", storeId, keywords, isActive, emailAccountId, query);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);
			_logger.LogDebug("GetAllMessageTemplatesAsync(storeId={StoreId},keywords={Keywords},isActive={IsActive},emailAccountId={EmailAccountId}): messageTemplates → {MessageTemplates}", storeId, keywords, isActive, emailAccountId, messageTemplates);
			_logger.LogInformation("GetAllMessageTemplatesAsync(storeId,keywords,isActive,emailAccountId)={StoreId},{Keywords},{IsActive},{EmailAccountId}: find query executed in {Arg4} ms", storeId, keywords, isActive, emailAccountId, __logStart.ElapsedMilliseconds);

            if (isActive.HasValue)
                query = query.Where(mt => mt.IsActive == isActive);

            return query.OrderBy(t => t.Name);
        }, cache => cache.PrepareKeyForDefaultCache(NopMessageDefaults.MessageTemplatesAllCacheKey, storeId, isActive));

        if (emailAccountId > 0)
            messageTemplates = messageTemplates.Where(mt => mt.EmailAccountId == emailAccountId).ToList();

        if (!string.IsNullOrWhiteSpace(keywords))
            messageTemplates = messageTemplates.Where(x => (x.Subject?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (x.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (x.Name?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false)).ToList();

		_logger.LogDebug("<<< Exiting GetAllMessageTemplatesAsync(storeId={StoreId},keywords={Keywords},isActive={IsActive},emailAccountId={EmailAccountId})", storeId, keywords, isActive, emailAccountId);
        return messageTemplates;
    }

    /// <summary>
    /// Create a copy of message template with all depended data
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template copy
    /// </returns>
    public virtual async Task<MessageTemplate> CopyMessageTemplateAsync(MessageTemplate messageTemplate)
    {
		_logger.LogDebug(">>> Entering CopyMessageTemplateAsync(messageTemplate={MessageTemplate})", messageTemplate);
        ArgumentNullException.ThrowIfNull(messageTemplate);

        var mtCopy = new MessageTemplate
        {
            Name = messageTemplate.Name,
            BccEmailAddresses = messageTemplate.BccEmailAddresses,
            Subject = messageTemplate.Subject,
            Body = messageTemplate.Body,
            IsActive = messageTemplate.IsActive,
            AttachedDownloadId = messageTemplate.AttachedDownloadId,
            EmailAccountId = messageTemplate.EmailAccountId,
            LimitedToStores = messageTemplate.LimitedToStores,
            DelayBeforeSend = messageTemplate.DelayBeforeSend,
            DelayPeriod = messageTemplate.DelayPeriod
        };

        await InsertMessageTemplateAsync(mtCopy);

        var languages = await _languageService.GetAllLanguagesAsync(true);
		_logger.LogDebug("CopyMessageTemplateAsync(messageTemplate={MessageTemplate}): languages → {Languages}", messageTemplate, languages);

        //localization
        foreach (var lang in languages)
        {
            var bccEmailAddresses = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.BccEmailAddresses, lang.Id, false, false);
			_logger.LogDebug("CopyMessageTemplateAsync(messageTemplate={MessageTemplate}): bccEmailAddresses → {BccEmailAddresses}", messageTemplate, bccEmailAddresses);
            if (!string.IsNullOrEmpty(bccEmailAddresses))
                await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.BccEmailAddresses, bccEmailAddresses, lang.Id);

            var subject = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.Subject, lang.Id, false, false);
			_logger.LogDebug("CopyMessageTemplateAsync(messageTemplate={MessageTemplate}): subject → {Subject}", messageTemplate, subject);
            if (!string.IsNullOrEmpty(subject))
                await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.Subject, subject, lang.Id);

            var body = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.Body, lang.Id, false, false);
			_logger.LogDebug("CopyMessageTemplateAsync(messageTemplate={MessageTemplate}): body → {Body}", messageTemplate, body);
            if (!string.IsNullOrEmpty(body))
                await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.Body, body, lang.Id);

            var emailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.EmailAccountId, lang.Id, false, false);
			_logger.LogDebug("CopyMessageTemplateAsync(messageTemplate={MessageTemplate}): emailAccountId → {EmailAccountId}", messageTemplate, emailAccountId);
            if (emailAccountId > 0)
                await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.EmailAccountId, emailAccountId, lang.Id);
        }

        //store mapping
        var selectedStoreIds = await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate);
		_logger.LogDebug("CopyMessageTemplateAsync(messageTemplate={MessageTemplate}): selectedStoreIds → {SelectedStoreIds}", messageTemplate, selectedStoreIds);
        foreach (var id in selectedStoreIds)
            await _storeMappingService.InsertStoreMappingAsync(mtCopy, id);

		_logger.LogDebug("<<< Exiting CopyMessageTemplateAsync(messageTemplate={MessageTemplate})", messageTemplate);
        return mtCopy;
    }

    #endregion
}