using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Vsts.Domain.Contract.Serializer
{
    public class VstsJson<T> where T : class
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        static VstsJson()
        {
            _settings.Converters.Add(new StringEnumConverter());
        }

        public static string Serialize(T t)
        {
            return JsonConvert.SerializeObject(t, _settings);
        }

        public static T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
