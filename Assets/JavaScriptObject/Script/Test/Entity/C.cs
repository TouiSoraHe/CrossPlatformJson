using System;
using System.Collections.Generic;
using CrossPlatformJson;
using UnityEngine;

public class C : A
{
    public int[] array = {3, 2, 1};

    public Dictionary<B[], int> arrayDic =
        new Dictionary<B[], int>
        {
            {new[] {new B()}, 1},
            {new[] {new B()}, 1},
            {new B[] {null}, 1}
        };

    public D<string> d = new D<string>("这是D");
    public JavaScriptObjectType enumVlaue = JavaScriptObjectType.Number;
    public Dictionary<int, int> intDic = new Dictionary<int, int> {{1, 1}, {2, 2}};
    public int intValue = 10;

    public List<int> list = new List<int> {1, 2, 3};
    public int[][] multiArray;
    public string name = "C";
    [NonSerialized] public string nonSerializedAttribute = "NonSerializedAttribute";
    public Dictionary<B, int> objDic = new Dictionary<B, int> {{new B(), 1}, {new B(), 2}};

    private string privateField = "privateField";
    [SerializeField] private string serializeField = "SerializeField";
    public Dictionary<string, int> stringDic = new Dictionary<string, int> {{"key1", 1}, {"key2", 2}};

    public C()
    {
        multiArray = new int[2][];
        multiArray[0] = new[] {1, 2, 3, 4};
        multiArray[1] = new[] {1, 2, 3, 4, 5, 6};
    }

    public int autoProperties { get; private set; }


    public override string ToString()
    {
        return string.Format("Array: {0}, ArrayDic: {1}, D: {2}, EnumVlaue: {3}, IntDic: {4}, IntValue: {5}, List: {6}, MultiArray: {7}, Name: {8}, NonSerializedAttribute: {9}, ObjDic: {10}, PrivateField: {11}, SerializeField: {12}, StringDic: {13}, autoProperties: {14}", array, arrayDic, d, enumVlaue, intDic, intValue, list, multiArray, name, nonSerializedAttribute, objDic, privateField, serializeField, stringDic, autoProperties);
    }
}