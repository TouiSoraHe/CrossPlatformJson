using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavaScriptObjectFactory {
    private static IJsonString2JavaScriptObjectHandle jsonString2JavaScriptObject = null;

    static JavaScriptObjectFactory()
    {
#if UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
        jsonString2JavaScriptObject = new JavaScriptObjectWithLitJson();

#elif UNITY_UWP
        jsonString2JavaScriptObject = new JavaScriptObjectWithUWP();
#else
        throw new System.Exception("该平台暂不支持");
#endif
    }

    public static JavaScriptObject CreateJavaScriptObject(string json)
    {
        return jsonString2JavaScriptObject.ToJavaScriptObject(json);
    }
}
