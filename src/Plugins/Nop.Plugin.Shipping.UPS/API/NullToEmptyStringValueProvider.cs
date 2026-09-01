using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Shipping.UPS.API;

public class NullToEmptyStringValueProvider : IValueProvider
{
    private readonly PropertyInfo _memberInfo;

    public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
    {
		logger.LogDebug(">>> Entering NullToEmptyStringValueProvider(memberInfo={MemberInfo})", memberInfo);
        _memberInfo = memberInfo;
		logger.LogDebug("NullToEmptyStringValueProvider(memberInfo={MemberInfo}): _memberInfo → {_memberInfo}", memberInfo, _memberInfo);
    }

    public object GetValue(object target)
    {
		logger.LogDebug(">>> Entering GetValue(target={Target})", target);
        var result = _memberInfo.GetValue(target);

		logger.LogDebug("GetValue(target={Target}): result → {Result}", target, result);
        if (_memberInfo.PropertyType != typeof(string))
            return result;

        var attributes = _memberInfo
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)).FirstOrDefault() as System.ComponentModel.DataAnnotations.RequiredAttribute;
            
        if ((attributes?.AllowEmptyStrings ?? false) && result == null)
            result = "";

		logger.LogDebug("<<< Exiting GetValue(target={Target})", target);
        return result;
    }

    public void SetValue(object target, object value)
    {
		logger.LogDebug(">>> Entering SetValue(target={Target},value={Value})", target, value);
        _memberInfo.SetValue(target, value);
		logger.LogDebug("<<< Exiting SetValue(target={Target},value={Value})", target, value);
    }
}