using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Vsts.Domain.Contract.Serializer
{
    public static class VstsJson<T> where T : class
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter()
            }
        };

        public static string Serialize(T data)
        {
            return JsonConvert.SerializeObject(data, settings);
        }

        public static T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}