namespace CrossPlatformJson
{

    public class JavaScriptObjectWithJsonParse : IJsonString2JavaScriptObjectHandle
    {
        public JavaScriptObject ToJavaScriptObject(string json)
        {
            return JsonParse.Parse(json);
        }
    }
}