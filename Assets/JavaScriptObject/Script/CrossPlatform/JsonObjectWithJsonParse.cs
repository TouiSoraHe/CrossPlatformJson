namespace CrossPlatformJson
{
    public class JsonObjectWithJsonParse : IJsonString2JsonObjectHandle
    {
        public JsonObject ToJsonObject(string json)
        {
            return JsonParse.Parse(json);
        }
    }
}