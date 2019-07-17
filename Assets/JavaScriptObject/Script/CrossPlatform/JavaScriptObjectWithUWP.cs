namespace CrossPlatformJson
{
#if UNITY_UWP
    using Windows.Data.Json;

    public class JavaScriptObjectWithUWP : IJsonString2JavaScriptObjectHandle
    {
        public JavaScriptObject ToJavaScriptObject(string json)
        {
            JavaScriptObject jsonObj = null;
            IJsonValue jsonData = null;
            JsonObject jsonObject;
            JsonValue jsonValue;
            JsonArray jsonArray;
            if (JsonObject.TryParse(json, out jsonObject))
            {
                jsonData = jsonObject;
            }
            else if (JsonArray.TryParse(json, out jsonArray))
            {
                jsonData = jsonArray;
            }
            else if (JsonValue.TryParse(json, out jsonValue))
            {
                jsonData = jsonValue;
            }
            else
            {
                throw new System.Exception("json:" + json + ",解析失败!");
            }
            Process(out jsonObj, jsonData);
            return jsonObj;
        }

        private void Process(out JavaScriptObject jsonObj, IJsonValue jsonData)
        {
            switch (jsonData.ValueType)
            {
                case JsonValueType.Null:
                    jsonObj = new JavaScriptObject();
                    break;
                case JsonValueType.Boolean:
                    jsonObj = new JavaScriptObject(jsonData.GetBoolean());
                    break;
                case JsonValueType.Number:
                    jsonObj = new JavaScriptObject(jsonData.GetNumber());
                    break;
                case JsonValueType.String:
                    jsonObj = new JavaScriptObject(jsonData.GetString());
                    break;
                case JsonValueType.Array:
                    jsonObj = new JavaScriptObject(JavaScriptObject.JavaScriptObjectType.Array);
                    ProcessArray(jsonObj, jsonData.GetArray());
                    break;
                case JsonValueType.Object:
                    jsonObj = new JavaScriptObject(JavaScriptObject.JavaScriptObjectType.Object);
                    ProcessObject(jsonObj, jsonData.GetObject());
                    break;
                default:
                    throw new System.Exception("意外情况");
            }
        }

        private void ProcessObject(JavaScriptObject jsonObj, JsonObject jsonData)
        {
            foreach (var item in jsonData)
            {
                JavaScriptObject value = null;
                Process(out value, item.Value);
                jsonObj.Add(item.Key, value);
            }
        }

        private void ProcessArray(JavaScriptObject jsonObj, JsonArray jsonData)
        {
            foreach (var item in jsonData)
            {
                JavaScriptObject value = null;
                Process(out value, item);
                jsonObj.Add(value);
            }
        }
    }

#endif
}