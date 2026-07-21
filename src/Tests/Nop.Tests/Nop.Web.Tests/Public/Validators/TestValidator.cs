using FluentValidation;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

public class TestValidator : InlineValidator<Person>
{
    public TestValidator()
    {
	logger.LogDebug(">>> Entering TestValidator()");
	logger.LogDebug("<<< Exiting TestValidator()");
    }

    public TestValidator(params Action<TestValidator>[] actions)
    {
		logger.LogDebug(">>> Entering TestValidator(actions={Actions})", actions);
        foreach (var action in actions)
            action(this);
			logger.LogDebug("<<< Exiting TestValidator(actions={Actions})", actions);
    }
}
