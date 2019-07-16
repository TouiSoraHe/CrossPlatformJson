namespace CrossPlatformJson
{
    using LitJson;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class JavaScriptObjectTest : MonoBehaviour
    {

        string json = null;

        private void Awake()
        {
            json = Resources.Load<TextAsset>("JavaScriptObject/test").text;
            Debug.Log(json);
        }

        private void Start()
        {
            //耗时测试
            double parseJson = TimeTest(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    JavaScriptObjectFactory.CreateJavaScriptObject(json, true);
                }
            });
            Debug.Log("parseJson:"+parseJson);
            double litJson = TimeTest(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    LitJson.JsonMapper.ToObject(json);
                }
            });
            Debug.Log("litJson:" + litJson);
            //打印解析的值
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject(json, true).ToJson());
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject(json, false).ToJson());
            //将解析的出来的再转成json再解析一次
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject(JavaScriptObjectFactory.CreateJavaScriptObject(json, true).ToJson(), true).ToJson());
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject(JavaScriptObjectFactory.CreateJavaScriptObject(json, false).ToJson(), false).ToJson());
            //解析单个值的情况
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject("1", true).ToJson());
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject("1", false).ToJson());
        }

        static double TimeTest(Action action)
        {
            DateTime beforDT = System.DateTime.Now;
            action();
            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);
            return ts.TotalMilliseconds;
        }
    }

}