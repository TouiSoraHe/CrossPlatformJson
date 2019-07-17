namespace CrossPlatformJson
{
    public interface IJsonString2JavaScriptObjectHandle
    {
        JavaScriptObject ToJavaScriptObject(string json);
    }
}