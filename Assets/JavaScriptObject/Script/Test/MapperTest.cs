using CrossPlatformJson;
using UnityEngine;

public class MapperTest : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        var c = new C();
        var jsonObj = JsonMapper.ToJavaScriptObject(c);
        Debug.LogError(jsonObj.ToJson());
        jsonObj.Remove("intValue");
        jsonObj["intValue"] = new JavaScriptObject();
        var c2 = JsonMapper.FromJavaScriptObject<C>(jsonObj);
        Debug.LogError(JsonMapper.ToJavaScriptObject(c2).ToJson());
    }


    // Update is called once per frame
    private void Update()
    {
    }
}