namespace CrossPlatformJson
{
    public class JavaScriptObjectFactory
    {
        private static readonly IJsonString2JavaScriptObjectHandle jsonString2JavaScriptObject;
        private static IJsonString2JavaScriptObjectHandle jsonParse;

        static JavaScriptObjectFactory()
        {
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
            jsonString2JavaScriptObject = new JavaScriptObjectWithLitJson();
#elif UNITY_UWP
        jsonString2JavaScriptObject = new JavaScriptObjectWithUWP();
#else
        throw new System.Exception("该平台暂不支持");
#endif
        }

        public static JavaScriptObject CreateJavaScriptObject(string json, bool useJsonParse = true)
        {
            if (useJsonParse)
                return CreateJavaScriptObjectWithJsonParse(json);
            return jsonString2JavaScriptObject.ToJavaScriptObject(json);
        }

        public static JavaScriptObject CreateJavaScriptObjectWithJsonParse(string json)
        {
            if (jsonParse == null) jsonParse = new JavaScriptObjectWithJsonParse();
            return jsonParse.ToJavaScriptObject(json);
        }
    }
}