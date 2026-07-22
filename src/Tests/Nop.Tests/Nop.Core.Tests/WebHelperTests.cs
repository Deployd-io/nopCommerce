using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class WebHelperTests : BaseNopTest
{
    private HttpContext _httpContext;
    private IWebHelper _webHelper;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _webHelper = GetService<IWebHelper>();
        _httpContext = GetService<IHttpContextAccessor>().HttpContext;
		logger.LogDebug("SetUp(): _webHelper → {_webHelper}", _webHelper);
		logger.LogDebug("SetUp(): _httpContext → {_httpContext}", _httpContext);

        var queryString = new QueryString(string.Empty);
        queryString = queryString.Add("Key1", "Value1");
		logger.LogDebug("SetUp(): queryString → {QueryString}", queryString);
        queryString = queryString.Add("Key2", "Value2");
        _httpContext.Request.QueryString = queryString;
		logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public void TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        var queryString = new QueryString(string.Empty);
        _httpContext.Request.QueryString = queryString;
		logger.LogDebug("<<< Exiting TearDown()");
		logger.LogDebug("TearDown(): queryString → {QueryString}", queryString);
    }

    [Test]
    public void CanGetStoreHostWithoutSsl()
    {
		logger.LogDebug(">>> Entering CanGetStoreHostWithoutSsl()");
        _webHelper.GetStoreHost(false).Should().Be($"http://{NopTestsDefaults.HostIpAddress}/");
		logger.LogDebug("<<< Exiting CanGetStoreHostWithoutSsl()");
    }

    [Test]
    public void CanGetStoreHostWithSsl()
    {
		logger.LogDebug(">>> Entering CanGetStoreHostWithSsl()");
        _webHelper.GetStoreHost(true).Should().Be($"https://{NopTestsDefaults.HostIpAddress}/");
		logger.LogDebug("<<< Exiting CanGetStoreHostWithSsl()");
    }

    [Test]
    public void CanGetStoreLocationWithoutSsl()
    {
		logger.LogDebug(">>> Entering CanGetStoreLocationWithoutSsl()");
        _webHelper.GetStoreLocation(false).Should().Be($"http://{NopTestsDefaults.HostIpAddress}/");
		logger.LogDebug("<<< Exiting CanGetStoreLocationWithoutSsl()");
    }

    [Test]
    public void CanGetStoreLocationWithSsl()
    {
		logger.LogDebug(">>> Entering CanGetStoreLocationWithSsl()");
        _webHelper.GetStoreLocation(true).Should().Be($"https://{NopTestsDefaults.HostIpAddress}/");
		logger.LogDebug("<<< Exiting CanGetStoreLocationWithSsl()");
    }

    [Test]
    public void CanGetStoreLocationInVirtualDirectory()
    {
		logger.LogDebug(">>> Entering CanGetStoreLocationInVirtualDirectory()");
        _httpContext.Request.PathBase = "/nopCommercepath";
        _webHelper.GetStoreLocation(false).Should().Be($"http://{NopTestsDefaults.HostIpAddress}/nopCommercepath/");
        _httpContext.Request.PathBase = string.Empty;
		logger.LogDebug("<<< Exiting CanGetStoreLocationInVirtualDirectory()");
    }

    [Test]
    public void CanGetQueryString()
    {
		logger.LogDebug(">>> Entering CanGetQueryString()");
        _webHelper.QueryString<string>("Key1").Should().Be("Value1");
        _webHelper.QueryString<string>("Key2").Should().Be("Value2");
        _webHelper.QueryString<string>("Key3").Should().Be(null);
		logger.LogDebug("<<< Exiting CanGetQueryString()");
    }

    [Test]
    public void CanRemoveQueryString()
    {
		logger.LogDebug(">>> Entering CanRemoveQueryString()");
        //empty URL
        _webHelper.RemoveQueryString(null, null).Should().Be(string.Empty);
        //empty key
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/", null).Should().Be($"http://{NopTestsDefaults.HostIpAddress}/");
        //non-existing param with fragment
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/#fragment", "param").Should().Be($"http://{NopTestsDefaults.HostIpAddress}/#fragment");
        //first param (?)
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param1")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param2=value1");
        //second param (&)
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param2")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1");
        //non-existing param
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param3")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1");
        //with fragment
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1#fragment", "param1")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param2=value1#fragment");
        //specific value
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param1=value2&param2=value1", "param1", "value1")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value2&param2=value1");
        //all values
        _webHelper.RemoveQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param1=value2&param2=value1", "param1")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param2=value1");
			logger.LogDebug("<<< Exiting CanRemoveQueryString()");
    }

    [Test]
    public void CanModifyQueryString()
    {
		logger.LogDebug(">>> Entering CanModifyQueryString()");
        //empty URL
        _webHelper.ModifyQueryString(null, null).Should().Be(string.Empty);
        //empty key
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/", null).Should().Be($"http://{NopTestsDefaults.HostIpAddress}/");
        //empty value
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/", "param").Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param=");
        //first param (?)
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "Param1", "value2")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value2&param2=value1");
        //second param (&)
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param2", "value2")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value2");
        //non-existing param
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param3", "value1")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1&param3=value1");
        //multiple values
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param1", "value1", "value2", "value3")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1,value2,value3&param2=value1");
        //with fragment
        _webHelper.ModifyQueryString($"http://{NopTestsDefaults.HostIpAddress}/?param1=value1&param2=value1#fragment", "param1", "value2")
            .Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?param1=value2&param2=value1#fragment");
			logger.LogDebug("<<< Exiting CanModifyQueryString()");
    }

    [Test]
    public void CanModifyQueryStringInVirtualDirectory()
    {
		logger.LogDebug(">>> Entering CanModifyQueryStringInVirtualDirectory()");
        _httpContext.Request.PathBase = "/nopCommercepath";
        _webHelper.ModifyQueryString("/nopCommercepath/Controller/Action", "param1", "value1").Should().Be("/nopCommercepath/Controller/Action?param1=value1");
        _httpContext.Request.PathBase = string.Empty;
		logger.LogDebug("<<< Exiting CanModifyQueryStringInVirtualDirectory()");
    }
}
