using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class TokenizerTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ITokenizer _tokenizer;
    private MessageTemplatesSettings _messageTemplatesSettings;
    private bool _defaultCaseInvariantReplacement;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _messageTemplatesSettings = GetService<MessageTemplatesSettings>();
        _defaultCaseInvariantReplacement = _messageTemplatesSettings.CaseInvariantReplacement;
		_logger.LogDebug("SetUp(): _messageTemplatesSettings → {_messageTemplatesSettings}", _messageTemplatesSettings);
		_logger.LogDebug("SetUp(): _defaultCaseInvariantReplacement → {_defaultCaseInvariantReplacement}", _defaultCaseInvariantReplacement);
		_logger.LogDebug("SetUp(): _messageTemplatesSettings.CaseInvariantReplacement → {Arg0}", _messageTemplatesSettings.CaseInvariantReplacement);
        _messageTemplatesSettings.CaseInvariantReplacement = false;
        await GetService<ISettingService>().SaveSettingAsync(_messageTemplatesSettings);

        _tokenizer = GetService<ITokenizer>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _tokenizer → {_tokenizer}", _tokenizer);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _messageTemplatesSettings.CaseInvariantReplacement = _defaultCaseInvariantReplacement;
        await GetService<ISettingService>().SaveSettingAsync(_messageTemplatesSettings);
		_logger.LogDebug("<<< Exiting TearDown()");
		_logger.LogDebug("TearDown(): _messageTemplatesSettings.CaseInvariantReplacement → {Arg0}", _messageTemplatesSettings.CaseInvariantReplacement);
    }

    [Test]
    public void CanReplaceTokensCaseSensitive()
    {
		_logger.LogDebug(">>> Entering CanReplaceTokensCaseSensitive()");
        var tokens = new List<Token>
        {
            new("Token1", "Value1")
        };
        //correct case
        _tokenizer
            .Replace("Some text %Token1%", tokens, false)
            .Should().Be("Some text Value1");
        //wrong case
        _tokenizer
            .Replace("Some text %TOKeN1%", tokens, false)
            .Should().Be("Some text %TOKeN1%");
			_logger.LogDebug("<<< Exiting CanReplaceTokensCaseSensitive()");
    }

    [Test]
    public async Task CanReplaceTokensCaseInvariant()
    {
		_logger.LogDebug(">>> Entering CanReplaceTokensCaseInvariant()");
        _messageTemplatesSettings.CaseInvariantReplacement = true;
        await GetService<ISettingService>().SaveSettingAsync(_messageTemplatesSettings);
		_logger.LogDebug("CanReplaceTokensCaseInvariant(): _messageTemplatesSettings.CaseInvariantReplacement → {Arg0}", _messageTemplatesSettings.CaseInvariantReplacement);

        var tokenizer = GetService<ITokenizer>();
		_logger.LogDebug("CanReplaceTokensCaseInvariant(): tokenizer → {Tokenizer}", tokenizer);

        var tokens = new List<Token>
        {
            new("Token1", "Value1")
        };
        tokenizer
            .Replace("Some text %TOKEn1%", tokens, false)
            .Should().Be("Some text Value1");
			_logger.LogDebug("<<< Exiting CanReplaceTokensCaseInvariant()");
    }

    [Test]
    public void CanHtmlEncode()
    {
		_logger.LogDebug(">>> Entering CanHtmlEncode()");
        var tokens = new List<Token>
        {
            new("Token1", "<Value1>")
        };

        _tokenizer
            .Replace("Some text %Token1%", tokens, true)
            .Should().Be("Some text &lt;Value1&gt;");
			_logger.LogDebug("<<< Exiting CanHtmlEncode()");
    }

    [Test]
    public void ShouldNotHtmlEncodeIfTokenDoesNotAllowIt()
    {
		_logger.LogDebug(">>> Entering ShouldNotHtmlEncodeIfTokenDoesNotAllowIt()");
        var tokens = new List<Token>
        {
            new("Token1", "<Value1>", true)
        };

        _tokenizer
            .Replace("Some text %Token1%", tokens, true)
            .Should().Be("Some text <Value1>");
			_logger.LogDebug("<<< Exiting ShouldNotHtmlEncodeIfTokenDoesNotAllowIt()");
    }

    [Test]
    public void CanReplaceTokensWithConditionalStatements()
    {
		_logger.LogDebug(">>> Entering CanReplaceTokensWithConditionalStatements()");
        var tokens = new List<Token>
        {
            new("ConditionToken", true),
            new("ConditionToken2", 2),
            new("ThenToken", "value"),
            new("ThenToken2", "value2"),
            new("SomeValueToken", 10),
        };

        //simple condition
        _tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken% endif% %SomeValueToken%", tokens, true)
            .Should().Be("Some text value  10");

        //broken token in condition
        _tokenizer.Replace(@"Some text %if (ConditionToken%) %ThenToken% endif% %SomeValueToken%", tokens, true)
            .Should().Be("Some text  10");

        //multiple conditions
        _tokenizer.Replace(@"Some text %if (%ConditionToken% && %ConditionToken2% > 1) %ThenToken% endif% %SomeValueToken%", tokens, true)
            .Should().Be("Some text value  10");

        //nested conditional statements
        _tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken% %if (%ConditionToken2% > 1) %ThenToken2% endif% %if (!%ConditionToken%) %ThenToken% endif% %ThenToken% endif% %SomeValueToken%", tokens, true)
            .Should().Be("Some text value value2   value  10");

        //wrong condition
        _tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken%", tokens, true)
            .Should().Be("Some text %if (True) value");

        //complex condition
        _tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken% endif% %SomeValueToken% %if (%ConditionToken2% > 1) %ThenToken2% %if (!%ConditionToken%) %ThenToken% endif% endif% %SomeValueToken%", tokens, true)
            .Should().Be("Some text value  10 value2   10");
			_logger.LogDebug("<<< Exiting CanReplaceTokensWithConditionalStatements()");
    }

    [Test]
    public void CanReplaceTokensWithNonStringValues()
    {
		_logger.LogDebug(">>> Entering CanReplaceTokensWithNonStringValues()");
        var tokens = new List<Token>
        {
            new("Token1", true),
            new("Token2", 1),
            new("Token3", "value")
        };

        _tokenizer.Replace("Some text %Token1%, %Token2%, %Token3%", tokens, true)
            .Should().Be("Some text True, 1, value");
			_logger.LogDebug("<<< Exiting CanReplaceTokensWithNonStringValues()");
    }
}