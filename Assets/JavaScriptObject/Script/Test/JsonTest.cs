using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        JavaScriptObject jsonObj = JavaScriptObjectFactory.CreateJavaScriptObject(Resources.Load<TextAsset>("test").text);
        Debug.Log(jsonObj.ToString());
        Debug.Log(jsonObj.ContainsKey("code"));
        Debug.Log(jsonObj.ContainsKey("fsdf"));
        Debug.Log(jsonObj["code"]);
        foreach (var item in jsonObj)
        {
            Debug.Log(item.Key + ":" + item.Value);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
