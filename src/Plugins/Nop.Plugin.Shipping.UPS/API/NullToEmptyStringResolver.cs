using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Shipping.UPS.API;

public class NullToEmptyStringResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
		logger.LogDebug(">>> Entering CreateProperties(type={Type},memberSerialization={MemberSerialization})", type, memberSerialization);
        return type.GetProperties()
            .Select(p =>
            {
                var jp = base.CreateProperty(p, memberSerialization);
                jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                return jp;
            }).ToList();
			logger.LogDebug("<<< Exiting CreateProperties(type={Type},memberSerialization={MemberSerialization})", type, memberSerialization);
    }
}
