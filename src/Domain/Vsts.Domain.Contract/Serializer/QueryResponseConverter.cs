using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Vsts.Domain.Contract.Response;
using Vsts.Domain.Enum;

namespace Vsts.Domain.Contract.Serializer
{
    public class QueryResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(QueryResponse);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            QueryResponse result = null;

            var jObject = JObject.Load(reader);

            if (jObject != null)
            {
                var queryResultType = (QueryResultType)System.Enum.Parse(typeof(QueryResultType), jObject["queryResultType"].ToString());

                switch (queryResultType)
                {
                    case QueryResultType.WorkItem:
                        result = VstsJson<WorkItemQueryResponse>.Deserialize(jObject.ToString());
                        break;

                    default:
                        result = null;
                        break;
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (QueryResponse)value);
        }
    }
}
