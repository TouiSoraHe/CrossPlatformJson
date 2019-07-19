
namespace CrossPlatformJson
{
#if UNITY_UWP
    using System;
    using Windows.Data.Json;

    public class JsonObjectWithUWP : IJsonString2JsonObjectHandle
    {
        public JsonObject ToJsonObject(string json)
        {
            JsonObject jsonObj = null;
            IJsonValue jsonData = null;
            Windows.Data.Json.JsonObject jsonObject;
            JsonValue jsonValue;
            JsonArray jsonArray;
            if (Windows.Data.Json.JsonObject.TryParse(json, out jsonObject))
                jsonData = jsonObject;
            else if (JsonArray.TryParse(json, out jsonArray))
                jsonData = jsonArray;
            else if (JsonValue.TryParse(json, out jsonValue))
                jsonData = jsonValue;
            else
                throw new Exception("json:" + json + ",解析失败!");
            Process(out jsonObj, jsonData);
            return jsonObj;
        }

        private void Process(out JsonObject jsonObj, IJsonValue jsonData)
        {
            switch (jsonData.ValueType)
            {
                case JsonValueType.Null:
                    jsonObj = new JsonObject();
                    break;
                case JsonValueType.Boolean:
                    jsonObj = new JsonObject(jsonData.GetBoolean());
                    break;
                case JsonValueType.Number:
                    jsonObj = new JsonObject(jsonData.GetNumber());
                    break;
                case JsonValueType.String:
                    jsonObj = new JsonObject(jsonData.GetString());
                    break;
                case JsonValueType.Array:
                    jsonObj = new JsonObject(JsonObjectType.Array);
                    ProcessArray(jsonObj, jsonData.GetArray());
                    break;
                case JsonValueType.Object:
                    jsonObj = new JsonObject(JsonObjectType.Object);
                    ProcessObject(jsonObj, jsonData.GetObject());
                    break;
                default:
                    throw new Exception("意外情况");
            }
        }

        private void ProcessObject(JsonObject jsonObj, Windows.Data.Json.JsonObject jsonData)
        {
            foreach (var item in jsonData)
            {
                JsonObject value = null;
                Process(out value, item.Value);
                jsonObj.Add(item.Key, value);
            }
        }

        private void ProcessArray(JsonObject jsonObj, JsonArray jsonData)
        {
            foreach (var item in jsonData)
            {
                JsonObject value = null;
                Process(out value, item);
                jsonObj.Add(value);
            }
        }
    }

#endif
}