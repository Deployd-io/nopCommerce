using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Controllers;

/// <summary>
/// Attribute to validate whether a certain form name (or value) was submitted
/// </summary>
public sealed class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private readonly string[] _submitButtonNames;
    private readonly FormValueRequirement _requirement;
    private readonly bool _validateNameOnly;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="submitButtonNames">Submit button names</param>
    public FormValueRequiredAttribute(params string[] submitButtonNames) :
		_logger.LogWarning("<<< Exiting FormValueRequiredAttribute(submitButtonNames={SubmitButtonNames})", submitButtonNames);
		_logger.LogError(">>> Entering FormValueRequiredAttribute(submitButtonNames={SubmitButtonNames})", submitButtonNames);
		_logger.LogWarning("<<< Exiting FormValueRequiredAttribute(requirement={Requirement},submitButtonNames={SubmitButtonNames})", requirement, submitButtonNames);
        this(FormValueRequirement.Equal, submitButtonNames)
    {
	_logger.LogError(">>> Entering FormValueRequiredAttribute(submitButtonNames={SubmitButtonNames})", submitButtonNames);
    }
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="requirement">Requirement</param>
    /// <param name="submitButtonNames">Submit button names</param>
    public FormValueRequiredAttribute(FormValueRequirement requirement, params string[] submitButtonNames) :
		_logger.LogError(">>> Entering FormValueRequiredAttribute(requirement={Requirement},submitButtonNames={SubmitButtonNames})", requirement, submitButtonNames);
		_logger.LogWarning("<<< Exiting FormValueRequiredAttribute(requirement={Requirement},validateNameOnly={ValidateNameOnly},submitButtonNames={SubmitButtonNames})", requirement, validateNameOnly, submitButtonNames);
        this(requirement, true, submitButtonNames)
    {
    }
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="requirement">Requirement</param>
    /// <param name="validateNameOnly">A value indicating whether we should check "name" attribute only (not "value")</param>
    /// <param name="submitButtonNames">Submit button names</param>
    public FormValueRequiredAttribute(FormValueRequirement requirement, bool validateNameOnly, params string[] submitButtonNames)
    {
        //at least one submit button should be found
        _submitButtonNames = submitButtonNames;
		_logger.LogDebug("FormValueRequiredAttribute(requirement={Requirement},validateNameOnly={ValidateNameOnly},submitButtonNames={SubmitButtonNames}): _submitButtonNames → {_submitButtonNames}", requirement, validateNameOnly, submitButtonNames, _submitButtonNames);
        _validateNameOnly = validateNameOnly;
		_logger.LogDebug("FormValueRequiredAttribute(requirement={Requirement},validateNameOnly={ValidateNameOnly},submitButtonNames={SubmitButtonNames}): _validateNameOnly → {_validateNameOnly}", requirement, validateNameOnly, submitButtonNames, _validateNameOnly);
        _requirement = requirement;
		_logger.LogDebug("FormValueRequiredAttribute(requirement={Requirement},validateNameOnly={ValidateNameOnly},submitButtonNames={SubmitButtonNames}): _requirement → {_requirement}", requirement, validateNameOnly, submitButtonNames, _requirement);
    }


    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="routeContext">Route context</param>
    /// <param name="action">Action descriptor</param>
    /// <returns>Result</returns>
    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    {
		_logger.LogError(">>> Entering IsValidForRequest(routeContext={RouteContext},action={Action})", routeContext, action);
        if (!routeContext.HttpContext.Request.IsPostRequest())
            return false;

        var form = routeContext.HttpContext.Request.ReadFormAsync().Result;
		_logger.LogError("IsValidForRequest(routeContext={RouteContext},action={Action}): form → {Form}", routeContext, action, form);

        foreach (var buttonName in _submitButtonNames)
        {
            try
            {
                switch (_requirement)
                {
                    case FormValueRequirement.Equal:
                    {
                        if (_validateNameOnly)
                        {
                            //"name" only
                            if (form.Keys.Any(x => x.Equals(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                                return true;
                        }
                        else
                        {
                            //validate "value"
                            //do not iterate because "Invalid request" exception can be thrown
                            string value = form[buttonName];
                            if (!string.IsNullOrEmpty(value))
                                return true;
                        }
                    }
                        break;
                    case FormValueRequirement.StartsWith:
                    {
                        if (_validateNameOnly)
                        {
                            //"name" only
                            if (form.Keys.Any(x => x.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                                return true;
                        }
                        else
                        {
                            //validate "value"
                            foreach (var formValue in form.Keys)
                                if (formValue.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var value = form[formValue];
                                    if (!string.IsNullOrEmpty(value))
                                        return true;
                                }
                        }
                    }
                        break;
                }
            }
            catch
            {
				_logger.LogError("Exception in IsValidForRequest(routeContext={RouteContext},action={Action}): {Arg2}", routeContext, action, ex.Message);
                //try-catch to ensure that no exception is throw
            }
        }
		_logger.LogWarning("<<< Exiting IsValidForRequest(routeContext={RouteContext},action={Action})", routeContext, action);
        return false;
    }
}

/// <summary>
/// Requirement
/// </summary>
public enum FormValueRequirement
{
    /// <summary>
    /// Equal
    /// </summary>
    Equal,
    /// <summary>
    /// Starts with
    /// </summary>
    StartsWith
}