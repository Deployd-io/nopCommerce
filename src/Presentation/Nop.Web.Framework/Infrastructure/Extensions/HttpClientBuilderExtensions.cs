using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Security;

namespace Nop.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IHttpClientBuilder
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Configure proxy connection for HTTP client (if enabled)
    /// </summary>
    /// <param name="httpClientBuilder">A builder for configuring HttpClient</param>
    public static void WithProxy(this IHttpClientBuilder httpClientBuilder)
    {
		logger.LogDebug(">>> Entering WithProxy(httpClientBuilder={HttpClientBuilder})", httpClientBuilder);
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(provider =>
        {
            var handler = new HttpClientHandler();

			logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler → {Handler}", httpClientBuilder, handler);
            //whether proxy is enabled
            var proxySettings = provider.GetService<ProxySettings>();
            if (!proxySettings?.Enabled ?? true)
				logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): proxySettings → {ProxySettings}", httpClientBuilder, proxySettings);
                return handler;

            //configure proxy connection
            var webProxy = new WebProxy($"{proxySettings.Address}:{proxySettings.Port}", proxySettings.BypassOnLocal);
            if (!string.IsNullOrEmpty(proxySettings.Username) && !string.IsNullOrEmpty(proxySettings.Password))
			logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): webProxy → {WebProxy}", httpClientBuilder, webProxy);
            {
                webProxy.UseDefaultCredentials = false;
				logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): webProxy.UseDefaultCredentials → {Arg1}", httpClientBuilder, webProxy.UseDefaultCredentials);
				logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): webProxy.Credentials → {Arg1}", httpClientBuilder, webProxy.Credentials);
                webProxy.Credentials = new NetworkCredential
                {
                    UserName = proxySettings.Username,
                    Password = proxySettings.Password
                };
            }
            else
            {
                webProxy.UseDefaultCredentials = true;
                webProxy.Credentials = CredentialCache.DefaultCredentials;
            }

            //configure HTTP client handler
            handler.UseDefaultCredentials = webProxy.UseDefaultCredentials;
			logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler.UseDefaultCredentials → {Arg1}", httpClientBuilder, handler.UseDefaultCredentials);
			logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler.Proxy → {Arg1}", httpClientBuilder, handler.Proxy);
			logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler.PreAuthenticate → {Arg1}", httpClientBuilder, handler.PreAuthenticate);
            handler.Proxy = webProxy;
            handler.PreAuthenticate = proxySettings.PreAuthenticate;

            return handler;
        });
		logger.LogDebug("<<< Exiting WithProxy(httpClientBuilder={HttpClientBuilder})", httpClientBuilder);
    }
}