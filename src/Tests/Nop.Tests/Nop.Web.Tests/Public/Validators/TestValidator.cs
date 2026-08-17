using Microsoft.Extensions.Logging;
﻿using FluentValidation;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

public class TestValidator : InlineValidator<Person>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TestValidator()
    {
	_logger.LogDebug(">>> Entering TestValidator()");
	_logger.LogDebug("<<< Exiting TestValidator()");
    }

    public TestValidator(params Action<TestValidator>[] actions)
    {
		_logger.LogDebug(">>> Entering TestValidator(actions={Actions})", actions);
        foreach (var action in actions)
            action(this);
			_logger.LogDebug("<<< Exiting TestValidator(actions={Actions})", actions);
    }
}