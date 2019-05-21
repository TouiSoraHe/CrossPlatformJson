using CrossPlatformJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        JavaScriptObject jsonObj = JavaScriptObjectFactory.CreateJavaScriptObject(Resources.Load<TextAsset>("test").text);
        Debug.Log("转换为json:"+jsonObj.ToString());
        Debug.Log("判断code字段是否存在:"+jsonObj.ContainsKey("code"));
        Debug.Log("判断test字段是否存在:"+jsonObj.ContainsKey("test"));
        Debug.Log("获取子对象data:"+jsonObj["data"]);
        Debug.Log("获取子对象data的name字段:" + jsonObj["data"]["name"]);
        Debug.Log("修改data的值");
        jsonObj["data"] = new JavaScriptObject(1);
        Debug.Log("获取子对象data:" + jsonObj["data"]);
        //添加一个新的字段
        jsonObj.Add("test add num", 1);
        jsonObj.Add("test add boolean", false);
        jsonObj.Add("test add string", "new string");
        JavaScriptObject array = new JavaScriptObject();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        jsonObj.Add("test add array", array);
        JavaScriptObject subObj = new JavaScriptObject();
        subObj.Add("sub obj num", 1);
        subObj.Add("sub obj boolean", false);
        jsonObj.Add("test add subObject", subObj);
        Debug.Log("转换为json:" + jsonObj.ToJson());
        Debug.Log("-----------遍历----------");
        foreach (var item in jsonObj)
        {
            Debug.Log(item.Key + ":" + item.Value);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
