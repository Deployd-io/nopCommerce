using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using static Nop.Services.Common.NopLinksDefaults;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
internal class NopLinksDefaultsTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected static async Task TestUrlsAsync(IList<PropertyInfo> properties)
    {
		_logger.LogDebug(">>> Entering TestUrlsAsync(properties={Properties})", properties);
        //skip external URL validation in automated test environments
        //instead, just validate that the URLs are well-formed
        foreach (var propertyInfo in properties)
        {
            var url = propertyInfo.GetValue(null)?.ToString();

			_logger.LogDebug("TestUrlsAsync(properties={Properties}): url → {Url}", properties, url);
            if (string.IsNullOrEmpty(url))
                continue;

            //validate that the URL is well-formed
            Uri.IsWellFormedUriString(url, UriKind.Absolute)
                .Should().BeTrue($"URL '{url}' from property '{propertyInfo.Name}' should be well-formed");

            //validate that it's an HTTPS URL for security
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                uri.Scheme.Should().Be("https", $"URL '{url}' should use HTTPS for security");
            }
        }
        await Task.CompletedTask;
		_logger.LogDebug("<<< Exiting TestUrlsAsync(properties={Properties})", properties);
    }

    [Test]
    public async Task TestOfficialSiteLinksAsync()
    {
		_logger.LogDebug(">>> Entering TestOfficialSiteLinksAsync()");
        var prop = typeof(OfficialSite).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
            .Where(p => p.PropertyType == typeof(string)).ToList();

        prop.Should().NotBeEmpty();

        await TestUrlsAsync(prop);
		_logger.LogDebug("<<< Exiting TestOfficialSiteLinksAsync()");
    }

    [Test]
    public async Task TestDocsLinksAsync()
    {
		_logger.LogDebug(">>> Entering TestDocsLinksAsync()");
        var prop = typeof(Docs).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
            .Where(p => p.PropertyType == typeof(string)).ToList();

        prop.Should().NotBeEmpty();

        await TestUrlsAsync(prop);
		_logger.LogDebug("<<< Exiting TestDocsLinksAsync()");
    }
}