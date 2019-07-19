using System;
using UnityEngine;

namespace CrossPlatformJson
{
    public class JsonObjectTest : MonoBehaviour
    {
        private string json;

        private void Awake()
        {
            json = Resources.Load<TextAsset>("JsonObject/test").text;
            Debug.Log(json);
        }

        private void Start()
        {
            //耗时测试
            var parseJson = TimeTest(() =>
            {
                for (var i = 0; i < 1000; i++) JsonObjectFactory.CreateJsonObject(json);
            });
            Debug.Log("parseJson:" + parseJson);
            var litJson = TimeTest(() =>
            {
#if !UNITY_WSA || UNITY_EDITOR
                for (var i = 0; i < 1000; i++) LitJson.JsonMapper.ToObject(json);
#endif
            });
            Debug.Log("litJson:" + litJson);
            //打印解析的值
            Debug.Log(JsonObjectFactory.CreateJsonObject(json).ToJson());
            Debug.Log(JsonObjectFactory.CreateJsonObject(json, false).ToJson());
            //将解析的出来的再转成json再解析一次
            Debug.Log(JsonObjectFactory
                .CreateJsonObject(JsonObjectFactory.CreateJsonObject(json).ToJson()).ToJson());
            Debug.Log(JsonObjectFactory
                .CreateJsonObject(JsonObjectFactory.CreateJsonObject(json, false).ToJson(), false)
                .ToJson());
            //解析单个值的情况
            Debug.Log(JsonObjectFactory.CreateJsonObject("1").ToJson());
            Debug.Log(JsonObjectFactory.CreateJsonObject("1", false).ToJson());
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