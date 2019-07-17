using System;
using UnityEngine;

namespace CrossPlatformJson
{
    public class JavaScriptObjectTest : MonoBehaviour
    {
        private string json;

        private void Awake()
        {
            json = Resources.Load<TextAsset>("JavaScriptObject/test").text;
            Debug.Log(json);
        }

        private void Start()
        {
            //耗时测试
            var parseJson = TimeTest(() =>
            {
                for (var i = 0; i < 1000; i++) JavaScriptObjectFactory.CreateJavaScriptObject(json);
            });
            Debug.Log("parseJson:" + parseJson);
            var litJson = TimeTest(() =>
            {
                for (var i = 0; i < 1000; i++) LitJson.JsonMapper.ToObject(json);
            });
            Debug.Log("litJson:" + litJson);
            //打印解析的值
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject(json).ToJson());
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject(json, false).ToJson());
            //将解析的出来的再转成json再解析一次
            Debug.Log(JavaScriptObjectFactory
                .CreateJavaScriptObject(JavaScriptObjectFactory.CreateJavaScriptObject(json).ToJson()).ToJson());
            Debug.Log(JavaScriptObjectFactory
                .CreateJavaScriptObject(JavaScriptObjectFactory.CreateJavaScriptObject(json, false).ToJson(), false)
                .ToJson());
            //解析单个值的情况
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject("1").ToJson());
            Debug.Log(JavaScriptObjectFactory.CreateJavaScriptObject("1", false).ToJson());
        }

        private static double TimeTest(Action action)
        {
            var beforDT = DateTime.Now;
            action();
            var afterDT = DateTime.Now;
            var ts = afterDT.Subtract(beforDT);
            return ts.TotalMilliseconds;
        }
    }
}