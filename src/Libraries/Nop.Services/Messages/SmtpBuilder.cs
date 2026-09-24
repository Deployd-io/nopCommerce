using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// SMTP Builder
/// </summary>
public partial class SmtpBuilder : ISmtpBuilder
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    public SmtpBuilder(EmailAccountSettings emailAccountSettings,
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider)
    {
		_logger.LogError(">>> Entering SmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider})", emailAccountSettings, emailAccountService, localizationService, fileProvider);
        _emailAccountSettings = emailAccountSettings;
        _emailAccountService = emailAccountService;
		_logger.LogDebug("SmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider}): _emailAccountService → {_emailAccountService}", emailAccountSettings, emailAccountService, localizationService, fileProvider, _emailAccountService);
		_logger.LogDebug("SmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider}): _emailAccountSettings → {_emailAccountSettings}", emailAccountSettings, emailAccountService, localizationService, fileProvider, _emailAccountSettings);
    _logger.LogWarning("<<< Exiting SmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider})", emailAccountSettings, emailAccountService, localizationService, fileProvider);
        _localizationService = localizationService;
		_logger.LogDebug("SmtpBuilder(emailAccountSettings={EmailAccountSettings},emailAccountService={EmailAccountService},localizationService={LocalizationService},fileProvider={FileProvider}): _localizationService → {_localizationService}", emailAccountSettings, emailAccountService, localizationService, fileProvider, _localizationService);
        _fileProvider = fileProvider;
    }

    #endregion

    #region Utilities

    protected virtual async Task<SaslMechanism> GetGmailCredentialsAsync(EmailAccount emailAccount)
    {
		_logger.LogError(">>> Entering GetGmailCredentialsAsync(emailAccount={EmailAccount})", emailAccount);
        ArgumentNullException.ThrowIfNull(emailAccount);

        if (string.IsNullOrEmpty(emailAccount.ClientId))
			_logger.LogError("Throwing NopException from GetGmailCredentialsAsync(emailAccount={EmailAccount})", emailAccount);
            throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientId.Required"));

        if (string.IsNullOrEmpty(emailAccount.ClientSecret))
            throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientSecret.Required"));

        var tokenFilePath = _fileProvider.MapPath(NopMessageDefaults.GmailAuthStorePath);
		_logger.LogError("GetGmailCredentialsAsync(emailAccount={EmailAccount}): tokenFilePath → {TokenFilePath}", emailAccount, tokenFilePath);
        var credentialRoot = _fileProvider.Combine(tokenFilePath, emailAccount.Email);
		_logger.LogError("GetGmailCredentialsAsync(emailAccount={EmailAccount}): credentialRoot → {CredentialRoot}", emailAccount, credentialRoot);

        var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = emailAccount.ClientId,
                ClientSecret = emailAccount.ClientSecret
            },
            Scopes = NopMessageDefaults.GmailScopes,
            DataStore = new FileDataStore(credentialRoot, true)
        });
			_logger.LogDebug("GetGmailCredentialsAsync(emailAccount={EmailAccount}): DataStore → {DataStore}", emailAccount, DataStore);

        var authCode = new AuthorizationCodeWebApp(codeFlow, null, null);
		_logger.LogError("GetGmailCredentialsAsync(emailAccount={EmailAccount}): authCode → {AuthCode}", emailAccount, authCode);

        var authResult = await authCode.AuthorizeAsync(emailAccount.Email, CancellationToken.None);
		_logger.LogError("GetGmailCredentialsAsync(emailAccount={EmailAccount}): authResult → {AuthResult}", emailAccount, authResult);

        if (authResult.Credential is null)
            throw new NopException("Failed to obtain user credentials for the authorization server. Check the client secrets and allow the application to perform required operations.");

        if (authResult.Credential.Token?.IsStale == true)
            await authResult.Credential.RefreshTokenAsync(CancellationToken.None);

		_logger.LogWarning("<<< Exiting GetGmailCredentialsAsync(emailAccount={EmailAccount})", emailAccount);
        return new SaslMechanismOAuth2(authResult.Credential.UserId, authResult.Credential.Token.AccessToken);
    }

    protected virtual async Task<SaslMechanism> GetExchangeCredentialsAsync(EmailAccount emailAccount)
    {
		_logger.LogError(">>> Entering GetExchangeCredentialsAsync(emailAccount={EmailAccount})", emailAccount);
        ArgumentNullException.ThrowIfNull(emailAccount);

        if (string.IsNullOrEmpty(emailAccount.ClientId))
			_logger.LogError("Throwing NopException from GetExchangeCredentialsAsync(emailAccount={EmailAccount})", emailAccount);
            throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientId.Required"));

        if (string.IsNullOrEmpty(emailAccount.ClientSecret))
            throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientSecret.Required"));

        if (string.IsNullOrEmpty(emailAccount.TenantId))
            throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.TenantId.Required"));

        var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(emailAccount.ClientId)
            .WithAuthority(string.Format(NopMessageDefaults.MSALTenantPattern, emailAccount.TenantId))
            .WithClientSecret(emailAccount.ClientSecret)
            .Build();

        var authToken = await confidentialClientApplication.AcquireTokenForClient(NopMessageDefaults.MSALScopes).ExecuteAsync();
		_logger.LogError("GetExchangeCredentialsAsync(emailAccount={EmailAccount}): authToken → {AuthToken}", emailAccount, authToken);

		_logger.LogWarning("<<< Exiting GetExchangeCredentialsAsync(emailAccount={EmailAccount})", emailAccount);
        return new SaslMechanismOAuth2(emailAccount.Email, authToken.AccessToken);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a new SMTP client for a specific email account
    /// </summary>
    /// <param name="emailAccount">Email account to use. If null, then would be used EmailAccount by default</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the an SMTP client that can be used to send email messages
    /// </returns>
    public virtual async Task<SmtpClient> BuildAsync(EmailAccount emailAccount = null)
    {
		_logger.LogError(">>> Entering BuildAsync(emailAccount={EmailAccount})", emailAccount);
        emailAccount ??= await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
                         ?? throw new NopException("Email account could not be loaded");

        var client = new SmtpClient
        {
            ServerCertificateValidationCallback = ValidateServerCertificate
        };

        try
        {
            await client.ConnectAsync(
                emailAccount.Host,
                emailAccount.Port,
                emailAccount.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable);

            switch (emailAccount.EmailAuthenticationMethod)
            {
                case EmailAuthenticationMethod.Login:
                    await client.AuthenticateAsync(new SaslMechanismLogin(emailAccount.Username, emailAccount.Password));
                    break;
                case EmailAuthenticationMethod.GmailOAuth2:
                    await client.AuthenticateAsync(await GetGmailCredentialsAsync(emailAccount));
                    break;
                case EmailAuthenticationMethod.MicrosoftOAuth2:
                    await client.AuthenticateAsync(await GetExchangeCredentialsAsync(emailAccount));
                    break;
                case EmailAuthenticationMethod.Ntlm:
                    await client.AuthenticateAsync(new SaslMechanismNtlm());
                    break;
            }

            return client;
        }
        catch (Exception ex)
        {
            client.Dispose();
            throw new NopException(ex.Message, ex);
        }
    }

    /// <summary>
    /// Validates the remote Secure Sockets Layer (SSL) certificate used for authentication.
    /// </summary>
    /// <param name="sender">An object that contains state information for this validation.</param>
    /// <param name="certificate">The certificate used to authenticate the remote party.</param>
    /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
    /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
    /// <returns>A System.Boolean value that determines whether the specified certificate is accepted for authentication</returns>
    public virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
		_logger.LogError(">>> Entering ValidateServerCertificate(sender={Sender},certificate={Certificate},chain={Chain},sslPolicyErrors={SslPolicyErrors})", sender, certificate, chain, sslPolicyErrors);
        //By default, server certificate verification is disabled.
		_logger.LogWarning("<<< Exiting ValidateServerCertificate(sender={Sender},certificate={Certificate},chain={Chain},sslPolicyErrors={SslPolicyErrors})", sender, certificate, chain, sslPolicyErrors);
        return true;
    }

    #endregion
}