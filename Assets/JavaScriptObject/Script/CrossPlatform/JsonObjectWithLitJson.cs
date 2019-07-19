using System;
using LitJson;

namespace CrossPlatformJson
{
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
    public class JsonObjectWithLitJson : IJsonString2JsonObjectHandle
    {
        public JsonObject ToJsonObject(string json)
        {
            JsonObject jsonObj = null;
            var jsonData = LitJson.JsonMapper.ToObject(json);
            Process(out jsonObj, jsonData);
            return jsonObj;
        }

        private void Process(out JsonObject jsonObj, JsonData jsonData)
        {
            switch (jsonData.GetJsonType())
            {
                case JsonType.None:
                    jsonObj = new JsonObject();
                    break;
                case JsonType.Object:
                    jsonObj = new JsonObject(JsonObjectType.Object);
                    ProcessObject(jsonObj, jsonData);
                    break;
                case JsonType.Array:
                    jsonObj = new JsonObject(JsonObjectType.Array);
                    ProcessArray(jsonObj, jsonData);
                    break;
                case JsonType.String:
                    jsonObj = new JsonObject(jsonData.ToString());
                    break;
                case JsonType.Int:
                case JsonType.Long:
                case JsonType.Double:
                    jsonObj = new JsonObject(double.Parse(jsonData.ToString()));
                    break;
                case JsonType.Boolean:
                    jsonObj = new JsonObject(bool.Parse(jsonData.ToString()));
                    break;
                default:
                    throw new Exception("意外情况");
            }
        }

        private void ProcessObject(JsonObject jsonObj, JsonData jsonData)
        {
            foreach (var item in jsonData.Keys)
            {
                var subJsonData = jsonData[item];
                var value = new JsonObject();
                if (subJsonData != null) Process(out value, subJsonData);
                jsonObj.Add(item, value);
            }
        }

        private void ProcessArray(JsonObject jsonObj, JsonData jsonData)
        {
            for (var i = 0; i < jsonData.Count; i++)
            {
                var subJsonData = jsonData[i];
                var value = new JsonObject();
                if (subJsonData != null) Process(out value, subJsonData);
                jsonObj.Add(value);
            }
        }
    }
#endif
}