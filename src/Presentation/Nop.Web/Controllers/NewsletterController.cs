using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class NewsletterController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterModelFactory _newsletterModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public NewsletterController(CaptchaSettings captchaSettings,
        ICustomerService customerService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INewsLetterModelFactory newsletterModelFactory,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService)
    {
		_logger.LogDebug(">>> Entering NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService})", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService);
        _captchaSettings = captchaSettings;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _captchaSettings → {_captchaSettings}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _captchaSettings);
        _customerService = customerService;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _customerService → {_customerService}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _customerService);
        _languageService = languageService;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _languageService → {_languageService}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _localizationService → {_localizationService}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _localizationService);
        _newsletterModelFactory = newsletterModelFactory;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _newsletterModelFactory → {_newsletterModelFactory}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _newsletterModelFactory);
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _newsLetterSubscriptionService);
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _newsLetterSubscriptionTypeService → {_newsLetterSubscriptionTypeService}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _newsLetterSubscriptionTypeService);
        _storeContext = storeContext;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _storeContext → {_storeContext}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _workContext → {_workContext}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _workflowMessageService → {_workflowMessageService}", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService, _workflowMessageService);
		_logger.LogDebug("<<< Exiting NewsletterController(captchaSettings={CaptchaSettings},customerService={CustomerService},languageService={LanguageService},localizationService={LocalizationService},newsletterModelFactory={NewsletterModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService})", captchaSettings, customerService, languageService, localizationService, newsletterModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, storeContext, workContext, workflowMessageService);
    }

    #endregion

    #region Methods

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    [CheckLanguageSeoCode(ignore: true)]
    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> SubscribeNewsletter(string email, bool subscribe, bool captchaValid)
    {
		_logger.LogDebug(">>> Entering SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid})", email, subscribe, captchaValid);
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsletterPage && !captchaValid)
            return Json(new { Success = false, Result = await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"), });

        //validate email
        if (!CommonHelper.IsValidEmail(email))
            return Json(new { Success = false, Result = await _localizationService.GetResourceAsync("Newsletter.Email.Wrong"), });

        //get current subscriptions
        email = email.Trim();
		_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): email → {Email1}", email, subscribe, captchaValid, email);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): store → {Store}", email, subscribe, captchaValid, store);
        var currentSubscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(email, storeId: store.Id);
		_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): currentSubscriptions → {CurrentSubscriptions}", email, subscribe, captchaValid, currentSubscriptions);

        //try to subscribe a customer for all 'ticked by default' subscriptions in the current store
        if (subscribe)
        {
            var subscriptionTypes = (await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id))
                .Where(type => type.TickedByDefault)
                .ToList();

            var language = await _workContext.GetWorkingLanguageAsync();
			_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): language → {Language}", email, subscribe, captchaValid, language);

            //create new subscriptions
            var subscriptionGuid = currentSubscriptions.FirstOrDefault()?.NewsLetterSubscriptionGuid ?? Guid.NewGuid();
			_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): subscriptionGuid → {SubscriptionGuid}", email, subscribe, captchaValid, subscriptionGuid);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var newSubscriptions = await subscriptionTypes.SelectAwait(async type =>
            {
                var existingSubscription = currentSubscriptions.FirstOrDefault(subscription => subscription.TypeId == type.Id);
				_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): existingSubscription → {ExistingSubscription}", email, subscribe, captchaValid, existingSubscription);
                if (existingSubscription is not null)
                {
                    //update language if not set yet
                    if (existingSubscription.LanguageId == 0)
                    {
                        existingSubscription.LanguageId = language.Id;
						_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): existingSubscription.LanguageId → {Arg3}", email, subscribe, captchaValid, existingSubscription.LanguageId);
                        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(existingSubscription);
                    }

                    return null;
                }

                var newSubscription = new NewsLetterSubscription
                {
                    NewsLetterSubscriptionGuid = subscriptionGuid,
                    Email = email,
                    Active = false,
                    TypeId = type.Id,
                    StoreId = store.Id,
                    LanguageId = language.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(newSubscription);

                return newSubscription;
            }).Where(subscription => subscription is not null).ToListAsync();
			_logger.LogInformation("SubscribeNewsletter(email,subscribe,captchaValid)={Email},{Subscribe},{CaptchaValid}: database query executed in {Arg3} ms", email, subscribe, captchaValid, __logStart.ElapsedMilliseconds);

            //send an activation message
            if (newSubscriptions.FirstOrDefault() is NewsLetterSubscription subscription)
                await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription);
        }
        else
        {
            //send a deactivation message
            if (currentSubscriptions.FirstOrDefault(s => s.Active) is NewsLetterSubscription subscription)
                await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription);
        }

		_logger.LogDebug("SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid}): Success → {Success}", email, subscribe, captchaValid, Success);
		_logger.LogDebug("<<< Exiting SubscribeNewsletter(email={Email},subscribe={Subscribe},captchaValid={CaptchaValid})", email, subscribe, captchaValid);
        return Json(new
        {
            Success = true,
            Result = subscribe
                ? await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent")
                : await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent")
        });
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> SubscriptionActivation(Guid token, bool active)
    {
		_logger.LogDebug(">>> Entering SubscriptionActivation(token=<redacted>,active={Active})", active);
        var store = await _storeContext.GetCurrentStoreAsync();
        var subscriptions = (await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByGuidAsync(token))
            .Where(subscription => subscription.StoreId == store.Id)
            .ToList();
			_logger.LogDebug("SubscriptionActivation(token=<redacted>,active={Active}): store → {Store}", active, store);
        if (!subscriptions.Any())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        foreach (var subscription in subscriptions)
        {
            if (active)
            {
                //activate subscription if inactive
                if (!subscription.Active)
                {
                    subscription.Active = true;
					_logger.LogDebug("SubscriptionActivation(token=<redacted>,active={Active}): subscription.Active → {Arg2}", active, subscription.Active);
                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                }

                //set language by subscription language (only for guests, as registered customers have the language set)
                var customer = await _workContext.GetCurrentCustomerAsync();
				_logger.LogDebug("SubscriptionActivation(token=<redacted>,active={Active}): customer → {Customer}", active, customer);
                if (!await _customerService.IsRegisteredAsync(customer) &&
                    await _languageService.GetLanguageByIdAsync(subscription.LanguageId) is Language language)
                {
                    await _workContext.SetWorkingLanguageAsync(language);
                }
            }
            else
            {
                //delete inactive subscription
                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
            }
        }

        var model = await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);
		_logger.LogDebug("SubscriptionActivation(token=<redacted>,active={Active}): model → {Model}", active, model);

		_logger.LogDebug("<<< Exiting SubscriptionActivation(token=<redacted>,active={Active})", active);
        return View(model);
    }

    #endregion
}