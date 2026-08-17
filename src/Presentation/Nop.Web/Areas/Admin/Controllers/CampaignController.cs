using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CampaignController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly ICampaignModelFactory _campaignModelFactory;
    protected readonly ICampaignService _campaignService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CampaignController(EmailAccountSettings emailAccountSettings,
        ICampaignModelFactory campaignModelFactory,
        ICampaignService campaignService,
        ICustomerActivityService customerActivityService,
        IDateTimeHelper dateTimeHelper,
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext})", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext);
        _emailAccountSettings = emailAccountSettings;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _emailAccountSettings → {_emailAccountSettings}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _emailAccountSettings);
        _campaignModelFactory = campaignModelFactory;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _campaignModelFactory → {_campaignModelFactory}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _campaignModelFactory);
        _campaignService = campaignService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _campaignService → {_campaignService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _campaignService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _customerActivityService → {_customerActivityService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _customerActivityService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _dateTimeHelper);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _emailAccountService → {_emailAccountService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _emailAccountService);
        _localizationService = localizationService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _localizationService → {_localizationService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _notificationService → {_notificationService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _notificationService);
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _newsLetterSubscriptionService);
        _permissionService = permissionService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _permissionService → {_permissionService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _permissionService);
        _storeContext = storeContext;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _storeContext → {_storeContext}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _storeContext);
        _storeService = storeService;
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _storeService → {_storeService}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _storeService);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext})", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext);
		_logger.LogDebug("CampaignController(emailAccountSettings={EmailAccountSettings},campaignModelFactory={CampaignModelFactory},campaignService={CampaignService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},emailAccountService={EmailAccountService},localizationService={LocalizationService},notificationService={NotificationService},newsLetterSubscriptionService={NewsLetterSubscriptionService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext}): _workContext → {_workContext}", emailAccountSettings, campaignModelFactory, campaignService, customerActivityService, dateTimeHelper, emailAccountService, localizationService, notificationService, newsLetterSubscriptionService, permissionService, storeContext, storeService, workContext, _workContext);
    }

    #endregion

    #region Utilities

    protected virtual async Task<EmailAccount> GetEmailAccountAsync(int emailAccountId)
    {
		_logger.LogDebug(">>> Entering GetEmailAccountAsync(emailAccountId={EmailAccountId})", emailAccountId);
		_logger.LogDebug("<<< Exiting GetEmailAccountAsync(emailAccountId={EmailAccountId})", emailAccountId);
        return await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId)
               ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
               ?? throw new NopException("Email account could not be loaded");
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignSearchModelAsync(new CampaignSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_VIEW)]
    public virtual async Task<IActionResult> List(CampaignSearchModel searchModel)
    {
        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignModelAsync(new CampaignModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> Create(CampaignModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var campaign = model.ToEntity<Campaign>();

            campaign.CreatedOnUtc = DateTime.UtcNow;
            campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

            await _campaignService.InsertCampaignAsync(campaign);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCampaign",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): campaign → {Campaign}", id, campaign);
        if (campaign == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignModelAsync(null, campaign);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> Edit(CampaignModel model, bool continueEditing)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
        if (campaign == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            campaign = model.ToEntity(campaign);

            campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

            await _campaignService.UpdateCampaignAsync(campaign);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCampaign",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("send-test-email")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS)]
    public virtual async Task<IActionResult> SendTestEmail(CampaignModel model)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
        if (campaign == null)
            return RedirectToAction("List");

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign);
		_logger.LogDebug("SendTestEmail(model={Model}): model → {Model1}", model, model);

        //ensure that the entered email is valid
        if (!CommonHelper.IsValidEmail(model.TestEmail))
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
            return View(model);
        }

        try
        {
            var emailAccount = await GetEmailAccountAsync(model.EmailAccountId);
			_logger.LogDebug("SendTestEmail(model={Model}): emailAccount → {EmailAccount}", model, emailAccount);
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("SendTestEmail(model={Model}): store → {Store}", model, store);
            var subscription = (await _newsLetterSubscriptionService
                .GetNewsLetterSubscriptionsByEmailAsync(model.TestEmail, storeId: store.Id, subscriptionTypeId: model.NewsLetterSubscriptionTypeId))
                .FirstOrDefault();
            if (subscription != null)
            {
                //there's a subscription. let's use it
                await _campaignService.SendCampaignAsync(campaign, emailAccount, new List<NewsLetterSubscription> { subscription });
            }
            else
            {
                var workingLanguage = await _workContext.GetWorkingLanguageAsync();
				_logger.LogDebug("SendTestEmail(model={Model}): workingLanguage → {WorkingLanguage}", model, workingLanguage);

                //no subscription found
                await _campaignService.SendCampaignAsync(campaign, emailAccount, model.TestEmail, workingLanguage.Id);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.TestEmailSentToCustomers"));

            return RedirectToAction("Edit", new { id = campaign.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in SendTestEmail(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting SendTestEmail(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("send-mass-email")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS)]
    public virtual async Task<IActionResult> SendMassEmail(CampaignModel model)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
        if (campaign == null)
            return RedirectToAction("List");

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);
		_logger.LogDebug("SendMassEmail(model={Model}): model → {Model1}", model, model);

        try
        {
            var emailAccount = await GetEmailAccountAsync(model.EmailAccountId);
			_logger.LogDebug("SendMassEmail(model={Model}): emailAccount → {EmailAccount}", model, emailAccount);

            //subscribers of certain store?
            var storeId = (await _storeService.GetStoreByIdAsync(campaign.StoreId))?.Id ?? 0;
			_logger.LogDebug("SendMassEmail(model={Model}): storeId → {StoreId}", model, storeId);
            var subscriptions = (await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(storeId: storeId,
                customerRoleId: model.CustomerRoleId,
                subscriptionTypeId: model.NewsLetterSubscriptionTypeId,
                isActive: true)).DistinctBy(x => x.Email);
            var totalEmailsSent = await _campaignService.SendCampaignAsync(campaign, emailAccount, subscriptions);
			_logger.LogDebug("SendMassEmail(model={Model}): totalEmailsSent → {TotalEmailsSent}", model, totalEmailsSent);

            _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.MassEmailSentToCustomers"), totalEmailsSent));

            return RedirectToAction("Edit", new { id = campaign.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in SendMassEmail(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting SendMassEmail(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): campaign → {Campaign}", id, campaign);
        if (campaign == null)
            return RedirectToAction("List");

        await _campaignService.DeleteCampaignAsync(campaign);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCampaign",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCampaign"), campaign.Id), campaign);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> CopyCampaign(CampaignModel model)
    {
        var copyModel = model.CopyCampaignModel;
        if (copyModel is null)
            return RedirectToAction("List");

        try
        {
            var originalCampaign = await _campaignService.GetCampaignByIdAsync(copyModel.OriginalCampaignId);

            if (originalCampaign is null)
                return RedirectToAction("List");

            var newCampaign = await _campaignService.CopyCampaignAsync(originalCampaign, copyModel.Name);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Copied"));

            return RedirectToAction("Edit", new { id = newCampaign.Id });
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
            return RedirectToAction("Edit", new { id = copyModel.OriginalCampaignId });
        }
    }

    #endregion
}