namespace CrossPlatformJson
{
    public class JsonObjectFactory
    {
        private static readonly IJsonString2JsonObjectHandle JsonString2JsonObject;
        private static IJsonString2JsonObjectHandle jsonParse;

        static JsonObjectFactory()
        {
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
            JsonString2JsonObject = new JsonObjectWithLitJson();
#elif UNITY_UWP
        JsonString2JsonObject = new JsonObjectWithUWP();
#else
        throw new System.Exception("该平台暂不支持");
#endif
        }

        public static JsonObject CreateJsonObject(string json, bool useJsonParse = true)
        {
            if (useJsonParse)
                return CreateJsonObjectWithJsonParse(json);
            return JsonString2JsonObject.ToJsonObject(json);
        }

        public static JsonObject CreateJsonObjectWithJsonParse(string json)
        {
            if (jsonParse == null) jsonParse = new JsonObjectWithJsonParse();
            return jsonParse.ToJsonObject(json);
        }
    }
}