using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure;

/// <summary>
/// Represents registrar of Facebook authentication service
/// </summary>
public class FacebookAuthenticationRegistrar : IExternalAuthenticationRegistrar
{
    /// <summary>
    /// Configure
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    public void Configure(AuthenticationBuilder builder)
    {
		logger.LogDebug(">>> Entering Configure(builder={Builder})", builder);
        builder.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
        {
            //set credentials
            var settings = EngineContext.Current.Resolve<FacebookExternalAuthSettings>();

			logger.LogDebug("Configure(builder={Builder}): settings → {Settings}", builder, settings);
            options.AppId = string.IsNullOrEmpty(settings?.ClientKeyIdentifier) ? nameof(options.AppId) : settings.ClientKeyIdentifier;
            options.AppSecret = string.IsNullOrEmpty(settings?.ClientSecret) ? nameof(options.AppSecret) : settings.ClientSecret;

            //store access and refresh tokens for the further usage
            options.SaveTokens = true;
			logger.LogDebug("Configure(builder={Builder}): options.SaveTokens → {Arg1}", builder, options.SaveTokens);

            //set custom events handlers
            options.Events = new OAuthEvents
			logger.LogDebug("Configure(builder={Builder}): options.Events → {Arg1}", builder, options.Events);
            {
                //in case of error, redirect the user to the specified URL
                OnRemoteFailure = context =>
                {
                    context.HandleResponse();

                    var errorUrl = context.Properties.GetString(FacebookAuthenticationDefaults.ErrorCallback);
                    context.Response.Redirect(errorUrl);

                    return Task.FromResult(0);
                }
            };
        });
		logger.LogDebug("<<< Exiting Configure(builder={Builder})", builder);
    }
}