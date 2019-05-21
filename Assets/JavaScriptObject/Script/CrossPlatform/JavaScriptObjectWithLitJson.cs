namespace CrossPlatformJson
{
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
    using LitJson;

    public class JavaScriptObjectWithLitJson : IJsonString2JavaScriptObjectHandle
    {
        public JavaScriptObject ToJavaScriptObject(string json)
        {
            JavaScriptObject jsonObj = new JavaScriptObject();
            JsonData jsonData = JsonMapper.ToObject(json);
            Process(jsonObj, jsonData);
            return jsonObj;
        }

        private void Process(JavaScriptObject jsonObj, JsonData jsonData)
        {
            switch (jsonData.GetJsonType())
            {
                case JsonType.None:
                    break;
                case JsonType.Object:
                    ProcessObject(jsonObj, jsonData);
                    break;
                case JsonType.Array:
                    ProcessArray(jsonObj, jsonData);
                    break;
                case JsonType.String:
                    jsonObj.SetString(jsonData.ToString());
                    break;
                case JsonType.Int:
                case JsonType.Long:
                case JsonType.Double:
                    jsonObj.SetNumber(double.Parse(jsonData.ToString()));
                    break;
                case JsonType.Boolean:
                    jsonObj.SetBoolean(bool.Parse(jsonData.ToString()));
                    break;
                default:
                    throw new System.Exception("意外情况");
            }
        }

        private void ProcessObject(JavaScriptObject jsonObj, JsonData jsonData)
        {
            foreach (var item in jsonData.Keys)
            {
                JavaScriptObject value = new JavaScriptObject();
                jsonObj.Add(item, value);
                JsonData subJsonData = jsonData[item];
                if (subJsonData != null)
                {
                    Process(value, subJsonData);
                }
            }
        }

        private void ProcessArray(JavaScriptObject jsonObj, JsonData jsonData)
        {
            for (int i = 0; i < jsonData.Count; i++)
            {
                JavaScriptObject value = new JavaScriptObject();
                jsonObj.Add(value);
                JsonData subJsonData = jsonData[i];
                if (subJsonData != null)
                {
                    Process(value, subJsonData);
                }
            }
        }
    }
#endif
}