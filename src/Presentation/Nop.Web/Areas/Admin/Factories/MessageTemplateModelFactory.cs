using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the message template model factory implementation
/// </summary>
public partial class MessageTemplateModelFactory : IMessageTemplateModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IMessageTokenProvider _messageTokenProvider;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public MessageTemplateModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IMessageTemplateService messageTemplateService,
        IMessageTokenProvider messageTokenProvider,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IStoreService storeService)
    {
		_logger.LogDebug(">>> Entering MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService})", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _catalogSettings → {_catalogSettings}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _catalogSettings);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _baseAdminModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _localizationService → {_localizationService}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _localizedModelFactory → {_localizedModelFactory}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _localizedModelFactory);
        _messageTemplateService = messageTemplateService;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _messageTemplateService → {_messageTemplateService}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _messageTemplateService);
        _messageTokenProvider = messageTokenProvider;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _messageTokenProvider → {_messageTokenProvider}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _messageTokenProvider);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _storeMappingSupportedModelFactory);
        _storeService = storeService;
		_logger.LogDebug("MessageTemplateModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,storeMappingSupportedModelFactory=<redacted>,storeService={StoreService}): _storeService → {_storeService}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, messageTemplateService, storeService, _storeService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare message template search model
    /// </summary>
    /// <param name="searchModel">Message template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template search model
    /// </returns>
    public virtual async Task<MessageTemplateSearchModel> PrepareMessageTemplateSearchModelAsync(MessageTemplateSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareMessageTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.List.IsActive.All")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.List.IsActive.ActiveOnly")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.List.IsActive.InactiveOnly")
        });

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare available email accounts
        await _baseAdminModelFactory.PrepareEmailAccountsAsync(searchModel.AvailableEmailAccounts,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.List.SearchEmailAccount.All"));
        searchModel.HideEmailAccount = searchModel.AvailableEmailAccounts.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareMessageTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged message template list model
    /// </summary>
    /// <param name="searchModel">Message template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template list model
    /// </returns>
    public virtual async Task<MessageTemplateListModel> PrepareMessageTemplateListModelAsync(MessageTemplateSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareMessageTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);
		_logger.LogDebug("PrepareMessageTemplateListModelAsync(searchModel={SearchModel}): isActive → {IsActive}", searchModel, isActive);

        //get message templates
        var messageTemplates = (await _messageTemplateService
            .GetAllMessageTemplatesAsync(searchModel.SearchStoreId, searchModel.SearchKeywords, isActive, searchModel.EmailAccountId)).ToPagedList(searchModel);

        //prepare store names (to avoid loading for each message template)
        var stores = (await _storeService.GetAllStoresAsync()).Select(store => new { store.Id, store.Name }).ToList();
		_logger.LogDebug("PrepareMessageTemplateListModelAsync(searchModel={SearchModel}): stores → {Stores}", searchModel, stores);

        //prepare list model
        var model = await new MessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
        {
            return messageTemplates.SelectAwait(async messageTemplate =>
            {
                //fill in model values from the entity
                var messageTemplateModel = messageTemplate.ToModel<MessageTemplateModel>();
				_logger.LogDebug("PrepareMessageTemplateListModelAsync(searchModel={SearchModel}): messageTemplateModel → {MessageTemplateModel}", searchModel, messageTemplateModel);

                //fill in additional values (not existing in the entity)
                if (messageTemplate.LimitedToStores)
                {
                    await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(messageTemplateModel, messageTemplate, false);
                    var storeNames = stores
                        .Where(store => messageTemplateModel.SelectedStoreIds.Contains(store.Id)).Select(store => store.Name);
                    messageTemplateModel.ListOfStores = string.Join(", ", storeNames);
                }
                else
                {
                    var allstores = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
					_logger.LogDebug("PrepareMessageTemplateListModelAsync(searchModel={SearchModel}): allstores → {Allstores}", searchModel, allstores);
					_logger.LogDebug("PrepareMessageTemplateListModelAsync(searchModel={SearchModel}): messageTemplateModel.ListOfStores → {Arg1}", searchModel, messageTemplateModel.ListOfStores);
                    messageTemplateModel.ListOfStores = allstores;
                }

                return messageTemplateModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareMessageTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare message template model
    /// </summary>
    /// <param name="model">Message template model</param>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template model
    /// </returns>
    public virtual async Task<MessageTemplateModel> PrepareMessageTemplateModelAsync(MessageTemplateModel model,
        MessageTemplate messageTemplate, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties})", model, messageTemplate, excludeProperties);
        Func<MessageTemplateLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (messageTemplate != null)
        {
            //fill in model values from the entity
            model ??= messageTemplate.ToModel<MessageTemplateModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.BccEmailAddresses = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.BccEmailAddresses, languageId, false, false);
                locale.Subject = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId, false, false);
                locale.Body = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId, false, false);
                locale.EmailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.EmailAccountId, languageId, false, false);

                //prepare available email accounts
                await _baseAdminModelFactory.PrepareEmailAccountsAsync(locale.AvailableEmailAccounts,
                    defaultItemText: await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard"));

                //PrepareEmailAccounts only gets available accounts, we need to set the item as selected manually
                if (locale.AvailableEmailAccounts?.FirstOrDefault(x => x.Value == locale.EmailAccountId.ToString()) is SelectListItem emailAccountListItem)
                {
                    emailAccountListItem.Selected = true;
					_logger.LogDebug("PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties}): emailAccountListItem.Selected → {Arg3}", model, messageTemplate, excludeProperties, emailAccountListItem.Selected);
                }

            };
        }

        model.SendImmediately = !model.DelayBeforeSend.HasValue;
		_logger.LogDebug("PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties}): model.SendImmediately → {Arg3}", model, messageTemplate, excludeProperties, model.SendImmediately);
		_logger.LogDebug("PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties}): model.HasAttachedDownload → {Arg3}", model, messageTemplate, excludeProperties, model.HasAttachedDownload);
        model.HasAttachedDownload = model.AttachedDownloadId > 0;

        var allowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokensAsync(_messageTokenProvider.GetTokenGroups(messageTemplate).ToList()));
		_logger.LogDebug("PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties}): allowedTokens → {AllowedTokens}", model, messageTemplate, excludeProperties, allowedTokens);
        model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}" +
                              $"{await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";
							_logger.LogDebug("PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties}): model.AllowedTokens → {Arg3}", model, messageTemplate, excludeProperties, model.AllowedTokens);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available email accounts
        await _baseAdminModelFactory.PrepareEmailAccountsAsync(model.AvailableEmailAccounts);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, messageTemplate, excludeProperties);

		_logger.LogDebug("<<< Exiting PrepareMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},excludeProperties={ExcludeProperties})", model, messageTemplate, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare test message template model
    /// </summary>
    /// <param name="model">Test message template model</param>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the st message template model
    /// </returns>
    public virtual async Task<TestMessageTemplateModel> PrepareTestMessageTemplateModelAsync(TestMessageTemplateModel model,
        MessageTemplate messageTemplate, int languageId)
    {
		_logger.LogDebug(">>> Entering PrepareTestMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},languageId={LanguageId})", model, messageTemplate, languageId);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(messageTemplate);

        model.Id = messageTemplate.Id;
		_logger.LogDebug("PrepareTestMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},languageId={LanguageId}): model.Id → {Arg3}", model, messageTemplate, languageId, model.Id);
		_logger.LogDebug("PrepareTestMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},languageId={LanguageId}): model.LanguageId → {Arg3}", model, messageTemplate, languageId, model.LanguageId);
        model.LanguageId = languageId;

        //filter tokens to the current template
        var subject = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId);
		_logger.LogDebug("PrepareTestMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},languageId={LanguageId}): subject → {Subject}", model, messageTemplate, languageId, subject);
        var body = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId);
		_logger.LogDebug("PrepareTestMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},languageId={LanguageId}): body → {Body}", model, messageTemplate, languageId, body);
        model.Tokens = (await _messageTokenProvider.GetListOfAllowedTokensAsync())
            .Where(token => subject.Contains(token) || body.Contains(token)).ToList();

		_logger.LogDebug("<<< Exiting PrepareTestMessageTemplateModelAsync(model={Model},messageTemplate={MessageTemplate},languageId={LanguageId})", model, messageTemplate, languageId);
        return model;
    }

    #endregion
}