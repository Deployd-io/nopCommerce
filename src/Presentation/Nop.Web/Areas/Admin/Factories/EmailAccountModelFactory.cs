using Nop.Core.Domain.Messages;
using Nop.Services;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the email account model factory implementation
/// </summary>
public partial class EmailAccountModelFactory : IEmailAccountModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly IEmailAccountService _emailAccountService;

    #endregion

    #region Ctor

    public EmailAccountModelFactory(EmailAccountSettings emailAccountSettings,
        IEmailAccountService emailAccountService)
    {
		_logger.LogDebug(">>> Entering EmailAccountModelFactory(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService})", emailAccountSettings, emailAccountService);
        _emailAccountSettings = emailAccountSettings;
		_logger.LogDebug("EmailAccountModelFactory(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService}): _emailAccountSettings → {_emailAccountSettings}", emailAccountSettings, emailAccountService, _emailAccountSettings);
        _emailAccountService = emailAccountService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare email account search model
    /// </summary>
    /// <param name="searchModel">Email account search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email account search model
    /// </returns>
    public virtual Task<EmailAccountSearchModel> PrepareEmailAccountSearchModelAsync(EmailAccountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareEmailAccountSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareEmailAccountSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged email account list model
    /// </summary>
    /// <param name="searchModel">Email account search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email account list model
    /// </returns>
    public virtual async Task<EmailAccountListModel> PrepareEmailAccountListModelAsync(EmailAccountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareEmailAccountListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get email accounts
        var emailAccounts = (await _emailAccountService.GetAllEmailAccountsAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareEmailAccountListModelAsync(searchModel={SearchModel}): emailAccounts → {EmailAccounts}", searchModel, emailAccounts);

        //prepare grid model
        var model = new EmailAccountListModel().PrepareToGrid(searchModel, emailAccounts, () =>
        {
            return emailAccounts.Select(emailAccount =>
            {
                //fill in model values from the entity
                var emailAccountModel = emailAccount.ToModel<EmailAccountModel>();
				_logger.LogDebug("PrepareEmailAccountListModelAsync(searchModel={SearchModel}): emailAccountModel → {EmailAccountModel}", searchModel, emailAccountModel);

                //fill in additional values (not existing in the entity)
                emailAccountModel.IsDefaultEmailAccount = emailAccount.Id == _emailAccountSettings.DefaultEmailAccountId;

                return emailAccountModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareEmailAccountListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare email account model
    /// </summary>
    /// <param name="model">Email account model</param>
    /// <param name="emailAccount">Email account</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email account model
    /// </returns>
    public virtual async Task<EmailAccountModel> PrepareEmailAccountModelAsync(EmailAccountModel model,
        EmailAccount emailAccount, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareEmailAccountModelAsync(model={Model},emailAccount={EmailAccount},excludeProperties={ExcludeProperties})", model, emailAccount, excludeProperties);
        //fill in model values from the entity
        if (emailAccount != null)
            model ??= emailAccount.ToModel<EmailAccountModel>();

        //set default values for the new model
        if (emailAccount == null)
        {
            model.Port = 25;
			_logger.LogDebug("PrepareEmailAccountModelAsync(model={Model},emailAccount={EmailAccount},excludeProperties={ExcludeProperties}): model.Port → {Arg3}", model, emailAccount, excludeProperties, model.Port);
			_logger.LogDebug("PrepareEmailAccountModelAsync(model={Model},emailAccount={EmailAccount},excludeProperties={ExcludeProperties}): model.MaxNumberOfEmails → {Arg3}", model, emailAccount, excludeProperties, model.MaxNumberOfEmails);
            model.MaxNumberOfEmails = 50;
        }

        model.AvailableEmailAuthenticationMethods.AddRange(await EmailAuthenticationMethod.None.ToSelectListAsync(false));

		_logger.LogDebug("<<< Exiting PrepareEmailAccountModelAsync(model={Model},emailAccount={EmailAccount},excludeProperties={ExcludeProperties})", model, emailAccount, excludeProperties);
        return model;
    }

    #endregion
}