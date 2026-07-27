using FluentAssertions;
using Nop.Services.Seo;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Seo;

[TestFixture]
public class SeoExtensionsTests : ServiceTest
{
    private IUrlRecordService _urlRecordService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _urlRecordService = GetService<IUrlRecordService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _urlRecordService → {_urlRecordService}", _urlRecordService);
    }

    [Test]
    public async Task ShouldReturnLowercase()
    {
		logger.LogDebug(">>> Entering ShouldReturnLowercase()");
        var seName = await _urlRecordService.GetSeNameAsync("tEsT", false, false);
        seName.Should().Be("test");
		logger.LogDebug("<<< Exiting ShouldReturnLowercase()");
		logger.LogDebug("ShouldReturnLowercase(): seName → {SeName}", seName);
    }

    [Test]
    public async Task ShouldAllowAllLatinChars()
    {
		logger.LogDebug(">>> Entering ShouldAllowAllLatinChars()");
        var seName = await _urlRecordService.GetSeNameAsync("abcdefghijklmnopqrstuvwxyz1234567890", false, false);
        seName.Should().Be("abcdefghijklmnopqrstuvwxyz1234567890");
		logger.LogDebug("<<< Exiting ShouldAllowAllLatinChars()");
		logger.LogDebug("ShouldAllowAllLatinChars(): seName → {SeName}", seName);
    }

    [Test]
    public async Task ShouldRemoveIllegalChars()
    {
		logger.LogDebug(">>> Entering ShouldRemoveIllegalChars()");
        var seName = await _urlRecordService.GetSeNameAsync("test!@#$%^&*()+<>?/", false, false);
        seName.Should().Be("test");
		logger.LogDebug("<<< Exiting ShouldRemoveIllegalChars()");
		logger.LogDebug("ShouldRemoveIllegalChars(): seName → {SeName}", seName);
    }

    [Test]
    public async Task ShouldReplaceSpaceWithDash()
    {
		logger.LogDebug(">>> Entering ShouldReplaceSpaceWithDash()");
        var seName = await _urlRecordService.GetSeNameAsync("test test", false, false);
        seName.Should().Be("test-test");
		logger.LogDebug("ShouldReplaceSpaceWithDash(): seName → {SeName}", seName);
        seName = await _urlRecordService.GetSeNameAsync("test     test", false, false);
        seName.Should().Be("test-test");
		logger.LogDebug("<<< Exiting ShouldReplaceSpaceWithDash()");
    }

    [Test]
    public async Task CanConvertNonWesternChars()
    {
		logger.LogDebug(">>> Entering CanConvertNonWesternChars()");
        //German letters with diacritics
        var seName = await _urlRecordService.GetSeNameAsync("testäöü", true, false);
        seName.Should().Be("testaou");
		logger.LogDebug("CanConvertNonWesternChars(): seName → {SeName}", seName);
        seName = await _urlRecordService.GetSeNameAsync("testäöü", false, false);
        seName.Should().Be("test");
		logger.LogDebug("<<< Exiting CanConvertNonWesternChars()");
    }

    [Test]
    public async Task CanAllowUnicodeChars()
    {
		logger.LogDebug(">>> Entering CanAllowUnicodeChars()");
        //Russian letters
        var seName = await _urlRecordService.GetSeNameAsync("testтест", false, true);
        seName.Should().Be("testтест");
		logger.LogDebug("CanAllowUnicodeChars(): seName → {SeName}", seName);
        seName = await _urlRecordService.GetSeNameAsync("testтест", false, false);
        seName.Should().Be("test");
		logger.LogDebug("<<< Exiting CanAllowUnicodeChars()");
    }
}