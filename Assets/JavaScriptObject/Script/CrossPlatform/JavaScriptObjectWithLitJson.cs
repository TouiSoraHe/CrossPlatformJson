namespace CrossPlatformJson
{
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
    using LitJson;

    public class JavaScriptObjectWithLitJson : IJsonString2JavaScriptObjectHandle
    {
        public JavaScriptObject ToJavaScriptObject(string json)
        {
            JavaScriptObject jsonObj = null;
            JsonData jsonData = JsonMapper.ToObject(json);
            Process(out jsonObj, jsonData);
            return jsonObj;
        }

        private void Process(out JavaScriptObject jsonObj, JsonData jsonData)
        {
            switch (jsonData.GetJsonType())
            {
                case JsonType.None:
                    jsonObj = new JavaScriptObject();
                    break;
                case JsonType.Object:
                    jsonObj = new JavaScriptObject(JavaScriptObject.JavaScriptObjectType.Object);
                    ProcessObject(jsonObj, jsonData);
                    break;
                case JsonType.Array:
                    jsonObj = new JavaScriptObject(JavaScriptObject.JavaScriptObjectType.Array);
                    ProcessArray(jsonObj, jsonData);
                    break;
                case JsonType.String:
                    jsonObj = new JavaScriptObject(jsonData.ToString());
                    break;
                case JsonType.Int:
                case JsonType.Long:
                case JsonType.Double:
                    jsonObj = new JavaScriptObject(double.Parse(jsonData.ToString()));
                    break;
                case JsonType.Boolean:
                    jsonObj = new JavaScriptObject(bool.Parse(jsonData.ToString()));
                    break;
                default:
                    throw new System.Exception("意外情况");
            }
        }

        private void ProcessObject(JavaScriptObject jsonObj, JsonData jsonData)
        {
            foreach (var item in jsonData.Keys)
            {
                JsonData subJsonData = jsonData[item];
                JavaScriptObject value = new JavaScriptObject();
                if (subJsonData != null)
                {
                    Process(out value, subJsonData);
                }
                jsonObj.Add(item, value);
            }
        }

        private void ProcessArray(JavaScriptObject jsonObj, JsonData jsonData)
        {
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData subJsonData = jsonData[i];
                JavaScriptObject value = new JavaScriptObject();
                if (subJsonData != null)
                {
                    Process(out value, subJsonData);
                }
                jsonObj.Add(value);
            }
        }
    }
#endif
}