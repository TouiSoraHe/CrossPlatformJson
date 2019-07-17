using System;
using LitJson;

namespace CrossPlatformJson
{
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
    public class JavaScriptObjectWithLitJson : IJsonString2JavaScriptObjectHandle
    {
        public JavaScriptObject ToJavaScriptObject(string json)
        {
            JavaScriptObject jsonObj = null;
            var jsonData = LitJson.JsonMapper.ToObject(json);
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
                    jsonObj = new JavaScriptObject(JavaScriptObjectType.Object);
                    ProcessObject(jsonObj, jsonData);
                    break;
                case JsonType.Array:
                    jsonObj = new JavaScriptObject(JavaScriptObjectType.Array);
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
                    throw new Exception("意外情况");
            }
        }

        private void ProcessObject(JavaScriptObject jsonObj, JsonData jsonData)
        {
            foreach (var item in jsonData.Keys)
            {
                var subJsonData = jsonData[item];
                var value = new JavaScriptObject();
                if (subJsonData != null) Process(out value, subJsonData);
                jsonObj.Add(item, value);
            }
        }

        private void ProcessArray(JavaScriptObject jsonObj, JsonData jsonData)
        {
            for (var i = 0; i < jsonData.Count; i++)
            {
                var subJsonData = jsonData[i];
                var value = new JavaScriptObject();
                if (subJsonData != null) Process(out value, subJsonData);
                jsonObj.Add(value);
            }
        }
    }
#endif
}