using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IHttpClientBuilder
/// </summary>
public static class HttpClientBuilderExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Configure proxy connection for HTTP client (if enabled)
    /// </summary>
    /// <param name="httpClientBuilder">A builder for configuring HttpClient</param>
    public static void WithProxy(this IHttpClientBuilder httpClientBuilder)
    {
		_logger.LogError(">>> Entering WithProxy(httpClientBuilder={HttpClientBuilder})", httpClientBuilder);
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(provider =>
        {
            var handler = new HttpClientHandler();
			_logger.LogError("WithProxy(httpClientBuilder={HttpClientBuilder}): handler → {Handler}", httpClientBuilder, handler);

            //whether proxy is enabled
            var proxySettings = provider.GetService<ProxySettings>();
			_logger.LogError("WithProxy(httpClientBuilder={HttpClientBuilder}): proxySettings → {ProxySettings}", httpClientBuilder, proxySettings);
            if (!proxySettings?.Enabled ?? true)
                return handler;

            //configure proxy connection
            var webProxy = new WebProxy($"{proxySettings.Address}:{proxySettings.Port}", proxySettings.BypassOnLocal);
			_logger.LogError("WithProxy(httpClientBuilder={HttpClientBuilder}): webProxy → {WebProxy}", httpClientBuilder, webProxy);
            if (!string.IsNullOrEmpty(proxySettings.Username) && !string.IsNullOrEmpty(proxySettings.Password))
            {
                webProxy.UseDefaultCredentials = false;
				_logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): webProxy.UseDefaultCredentials → {Arg1}", httpClientBuilder, webProxy.UseDefaultCredentials);
                webProxy.Credentials = new NetworkCredential
                {
					_logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): webProxy.Credentials → {Arg1}", httpClientBuilder, webProxy.Credentials);
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
			_logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler.UseDefaultCredentials → {Arg1}", httpClientBuilder, handler.UseDefaultCredentials);
			_logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler.Proxy → {Arg1}", httpClientBuilder, handler.Proxy);
			_logger.LogDebug("WithProxy(httpClientBuilder={HttpClientBuilder}): handler.PreAuthenticate → {Arg1}", httpClientBuilder, handler.PreAuthenticate);
            handler.Proxy = webProxy;
            handler.PreAuthenticate = proxySettings.PreAuthenticate;

		_logger.LogWarning("<<< Exiting WithProxy(httpClientBuilder={HttpClientBuilder})", httpClientBuilder);
            return handler;
        });
    }
}