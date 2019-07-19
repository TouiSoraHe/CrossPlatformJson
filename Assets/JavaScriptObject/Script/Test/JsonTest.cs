using System;
using CrossPlatformJson;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        JsonObjectTest();
    }

    private void JsonObjectTest()
    {
        //创建一个JsonObject对象
        var jsonObj = JsonObjectFactory.CreateJsonObject(Resources.Load<TextAsset>("test").text);
        Debug.Log("----------------转换为JSON---------------");
        Debug.Log(jsonObj.ToJson());
        Debug.Log("----------------判断code属性是否存在---------------");
        Debug.Log(jsonObj.ContainsKey("code"));
        Debug.Log("----------------获取子对象data---------------");
        Debug.Log("获取子对象data:" + jsonObj["data"].ToJson());
        Debug.Log("----------------获取子对象data的name属性---------------");
        Debug.Log("获取子对象data的name字段:" + jsonObj["data"]["name"].GetString());
        Debug.Log("----------------修改data的值---------------");
        jsonObj["data"] = new JsonObject(1);
        Debug.Log("----------------修改后data的值---------------");
        Debug.Log(jsonObj["data"].GetNumber());
        Debug.Log("----------------添加一个新的Number属性test add num---------------");
        jsonObj.Add("test add num", 1);
        Debug.Log(jsonObj["test add num"].GetNumber());
        Debug.Log("----------------添加一个新的boolean属性test add boolean---------------");
        jsonObj.Add("test add boolean", false);
        Debug.Log(jsonObj["test add boolean"].GetBoolean());
        Debug.Log("----------------添加一个新的string属性---------------");
        jsonObj.Add("test add string", "new string");
        Debug.Log(jsonObj["test add string"].GetString());
        Debug.Log("----------------添加一个数组属性test add array---------------");
        var array = new JsonObject();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        jsonObj.Add("test add array", array);
        Debug.Log(jsonObj["test add array"].ToJson());
        Debug.Log("----------------添加一个object属性test add subObject---------------");
        var subObj = new JsonObject();
        subObj.Add("sub obj num", 1);
        subObj.Add("sub obj boolean", false);
        jsonObj.Add("test add subObject", subObj);
        Debug.Log(jsonObj["test add subObject"].ToJson());
        Debug.Log("----------------修改后的json---------------");
        Debug.Log(jsonObj.ToJson());
        Debug.Log("----------------遍历所有根属性---------------");
        foreach (var item in jsonObj) Debug.Log(item.Key + ":" + item.Value);

        var helloWorldJsonObj = JsonObjectFactory.CreateJsonObject("{}");
        JsonUtility.FromJson<HelloWorld>(helloWorldJsonObj.ToJson());

        Debug.Log((char) 0x1f);
    }
}

[Serializable]
internal class HelloWorld
{
    public int id;
    public bool isSuccess;
    public string message;

    public HelloWorld()
    {
    }

    public HelloWorld(string message, int id, bool isSuccess)
    {
        this.message = message;
        this.id = id;
        this.isSuccess = isSuccess;
    }

    public override string ToString()
    {
        return "message:" + message + ";id:" + id + ";isSuccess:" + isSuccess;
    }
}