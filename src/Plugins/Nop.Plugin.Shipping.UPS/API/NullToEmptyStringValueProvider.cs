using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Shipping.UPS.API;

public class NullToEmptyStringValueProvider : IValueProvider
{
    private readonly PropertyInfo _memberInfo;

    public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
    {
		logger.LogError(">>> Entering NullToEmptyStringValueProvider(memberInfo={MemberInfo})", memberInfo);
        _memberInfo = memberInfo;
		logger.LogDebug("NullToEmptyStringValueProvider(memberInfo={MemberInfo}): _memberInfo → {_memberInfo}", memberInfo, _memberInfo);
    logger.LogWarning("<<< Exiting NullToEmptyStringValueProvider(memberInfo={MemberInfo})", memberInfo);
    }

    public object GetValue(object target)
    {
		logger.LogError(">>> Entering GetValue(target={Target})", target);
        var result = _memberInfo.GetValue(target);

		logger.LogError("GetValue(target={Target}): result → {Result}", target, result);
        if (_memberInfo.PropertyType != typeof(string))
            return result;

        var attributes = _memberInfo
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)).FirstOrDefault() as System.ComponentModel.DataAnnotations.RequiredAttribute;
            
        if ((attributes?.AllowEmptyStrings ?? false) && result == null)
            result = "";

		logger.LogWarning("<<< Exiting GetValue(target={Target})", target);
        return result;
    }

    public void SetValue(object target, object value)
    {
		logger.LogError(">>> Entering SetValue(target={Target},value={Value})", target, value);
        _memberInfo.SetValue(target, value);
		logger.LogWarning("<<< Exiting SetValue(target={Target},value={Value})", target, value);
    }
}