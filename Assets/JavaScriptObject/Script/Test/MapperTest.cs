using CrossPlatformJson;
using UnityEngine;

public class MapperTest : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        var c = new C();
        var jsonObj = JsonMapper.ToJsonObject(c);
        Debug.LogError(jsonObj.ToJson());
        jsonObj.Remove("intValue");
        jsonObj["intValue"] = new JsonObject();
        var c2 = JsonMapper.FromJsonObject<C>(jsonObj);
        Debug.LogError(JsonMapper.ToJsonObject(c2).ToJson());
    }


    // Update is called once per frame
    private void Update()
    {
    }
}