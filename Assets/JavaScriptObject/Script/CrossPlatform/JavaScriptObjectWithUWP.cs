namespace CrossPlatformJson
{
#if UNITY_UWP

using Windows.Data.Json;

public class JavaScriptObjectWithUWP : IJsonString2JavaScriptObjectHandle
{
    public JavaScriptObject ToJavaScriptObject(string json)
    {
        JavaScriptObject jsonObj = new JavaScriptObject();
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
        Process(jsonObj, jsonData);
        return jsonObj;
    }

    private void Process(JavaScriptObject jsonObj, IJsonValue jsonData)
    {
        switch (jsonData.ValueType)
        {
            case JsonValueType.Null:
                break;
            case JsonValueType.Boolean:
                jsonObj.SetBoolean(jsonData.GetBoolean());
                break;
            case JsonValueType.Number:
                jsonObj.SetNumber(jsonData.GetNumber());
                break;
            case JsonValueType.String:
                jsonObj.SetString(jsonData.GetString());
                break;
            case JsonValueType.Array:
                ProcessArray(jsonObj,jsonData.GetArray());
                break;
            case JsonValueType.Object:
                ProcessObject(jsonObj,jsonData.GetObject());
                break;
            default:
                break;
        }
    }

    private void ProcessObject(JavaScriptObject jsonObj, JsonObject jsonData)
    {
        foreach (var item in jsonData)
        {
            JavaScriptObject value = new JavaScriptObject();
            jsonObj.Add(item.Key, value);
            Process(value, item.Value);
        }
    }

    private void ProcessArray(JavaScriptObject jsonObj, JsonArray jsonData)
    {
        foreach (var item in jsonData)
        {
            JavaScriptObject value = new JavaScriptObject();
            jsonObj.Add(value);
            Process(value, item);
        }
    }
}

#endif
}