namespace CrossPlatformJson
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class JavaScriptObject : IEnumerable<KeyValuePair<JavaScriptObject, JavaScriptObject>>
    {
        public enum JavaScriptObjectType
        {
            Null = 0,
            Boolean = 1,
            Number = 2,
            String = 3,
            Array = 4,
            Object = 5
        }

        private JavaScriptObjectType _type;
        private double _numberValue;
        private string _stringValue;
        private bool _boolValue;
        private List<JavaScriptObject> _arrayValue;
        private Dictionary<string, JavaScriptObject> _objectValue;

        public JavaScriptObject(double value)
        {
            Type = JavaScriptObjectType.Number;
            _numberValue = value;
        }

        public JavaScriptObject(string value)
        {
            Type = JavaScriptObjectType.String;
            _stringValue = value;
        }

        public JavaScriptObject(bool value)
        {
            Type = JavaScriptObjectType.Boolean;
            _boolValue = value;
        }

        public JavaScriptObject()
        {
            Type = JavaScriptObjectType.Null;
        }

        public double GetNumber()
        {
            if (Type == JavaScriptObjectType.Number)
            {
                return _numberValue;
            }
            else
            {
                throw new Exception("该对象不是一个数值类型");
            }
        }

        public string GetString()
        {
            if (Type == JavaScriptObjectType.String)
            {
                return _stringValue;
            }
            else
            {
                throw new Exception("该对象不是一个字符串类型");
            }
        }

        public bool GetBoolean()
        {
            if (Type == JavaScriptObjectType.Boolean)
            {
                return _boolValue;
            }
            else
            {
                throw new Exception("该对象不是一个布尔值类型");
            }
        }

        public JavaScriptObject this[int index]
        {
            get
            {
                if (Type != JavaScriptObjectType.Array)
                {
                    throw new Exception("该对象不是一个数组类型");
                }
                return _arrayValue[index];
            }

            set
            {
                if (Type != JavaScriptObjectType.Array)
                {
                    throw new Exception("该对象不是一个数组类型");
                }
                _arrayValue[index] = value;
            }
        }

        public JavaScriptObject this[string key]
        {
            get
            {
                if (Type != JavaScriptObjectType.Object)
                {
                    throw new Exception("该对象不是一个对象类型");
                }
                return _objectValue[key];
            }

            set
            {
                if (Type != JavaScriptObjectType.Object)
                {
                    throw new Exception("该对象不是一个对象类型");
                }
                _objectValue[key] = value;
            }
        }

        public void Add(JavaScriptObject value)
        {
            if (Type == JavaScriptObjectType.Null)
            {
                Type = JavaScriptObjectType.Array;
                _arrayValue = new List<JavaScriptObject>();
            }
            if (Type != JavaScriptObjectType.Array)
            {
                throw new Exception("该对象不是一个数组类型");
            }
            _arrayValue.Add(value);
        }

        public void Add(double value)
        {
            Add(new JavaScriptObject(value));
        }

        public void Add(string value)
        {
            Add(new JavaScriptObject(value));
        }

        public void Add(bool value)
        {
            Add(new JavaScriptObject(value));
        }

        public void Add(string key, JavaScriptObject value)
        {
            if (Type == JavaScriptObjectType.Null)
            {
                Type = JavaScriptObjectType.Object;
                _objectValue = new Dictionary<string, JavaScriptObject>();
            }
            if (Type != JavaScriptObjectType.Object)
            {
                throw new Exception("该对象不是一个对象类型");
            }
            _objectValue.Add(key, value);
        }

        public void Add(string key, double value)
        {
            Add(key, new JavaScriptObject(value));
        }

        public void Add(string key, string value)
        {
            Add(key, new JavaScriptObject(value));
        }

        public void Add(string key, bool value)
        {
            Add(key, new JavaScriptObject(value));
        }

        public void Remove(int index)
        {
            if (Type != JavaScriptObjectType.Array)
            {
                throw new Exception("该对象不是一个数组类型");
            }
            _arrayValue.RemoveAt(index);
        }

        public void Remove(string key)
        {
            if (Type != JavaScriptObjectType.Object)
            {
                throw new Exception("该对象不是一个对象类型");
            }
            _objectValue.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            if (Type != JavaScriptObjectType.Object)
            {
                throw new Exception("该对象不是一个对象类型");
            }
            return _objectValue.ContainsKey(key);
        }

        public void Clear()
        {
            if (_objectValue != null)
            {
                _objectValue.Clear();
            }
            if (_arrayValue != null)
            {
                _arrayValue.Clear();
            }
        }

        public int Count
        {
            get
            {
                switch (Type)
                {
                    case JavaScriptObjectType.Null:
                        return 0;
                    case JavaScriptObjectType.Boolean:
                        return 1;
                    case JavaScriptObjectType.Number:
                        return 1;
                    case JavaScriptObjectType.String:
                        return 1;
                    case JavaScriptObjectType.Array:
                        return _arrayValue.Count;
                    case JavaScriptObjectType.Object:
                        return _objectValue.Count;
                    default:
                        return -1;
                }
            }
        }

        public JavaScriptObjectType Type
        {
            get
            {
                return _type;
            }

            private set
            {
                _type = value;
            }
        }

        public void SetNumber(double value)
        {
            if (Type == JavaScriptObjectType.Null)
            {
                Type = JavaScriptObjectType.Number;
            }
            if (Type == JavaScriptObjectType.Number)
            {
                _numberValue = value;
            }
            else
            {
                throw new Exception("该对象不是一个数值类型");
            }
        }

        public void SetString(string value)
        {
            if (Type == JavaScriptObjectType.Null)
            {
                Type = JavaScriptObjectType.String;
            }
            if (Type == JavaScriptObjectType.String)
            {
                _stringValue = value;
            }
            else
            {
                throw new Exception("该对象不是一个字符串类型");
            }
        }

        public void SetBoolean(bool value)
        {
            if (Type == JavaScriptObjectType.Null)
            {
                Type = JavaScriptObjectType.Boolean;
            }
            if (Type == JavaScriptObjectType.Boolean)
            {
                _boolValue = value;
            }
            else
            {
                throw new Exception("该对象不是一个布尔值类型");
            }
        }

        public string ToJson()
        {
            return ToString();
        }

        public override string ToString()
        {
            switch (Type)
            {
                case JavaScriptObjectType.Null:
                    return "null";
                case JavaScriptObjectType.Boolean:
                    return _boolValue ? "true" : "false";
                case JavaScriptObjectType.Number:
                    return _numberValue.ToString();
                case JavaScriptObjectType.String:
                    return "\"" + _stringValue + "\"";
                case JavaScriptObjectType.Array:
                    {
                        string ret = "[";
                        for (int i = 0; i < _arrayValue.Count; i++)
                        {
                            if (i != 0)
                            {
                                ret += ",";
                            }
                            ret += _arrayValue[i].ToString();
                        }
                        ret += "]";
                        return ret;
                    }
                case JavaScriptObjectType.Object:
                    {
                        string ret = "{";
                        int index = 0;
                        foreach (var item in _objectValue)
                        {
                            if (index != 0)
                            {
                                ret += ",";
                            }
                            ret += "\"" + item.Key + "\":" + item.Value.ToString();
                            index++;
                        }
                        ret += "}";
                        return ret;
                    }
                default:
                    throw new Exception("意外的情况");
            }
        }

        public IEnumerator<KeyValuePair<JavaScriptObject, JavaScriptObject>> GetEnumerator()
        {
            if (_type == JavaScriptObjectType.Object)
            {
                foreach (var item in _objectValue)
                {
                    yield return new KeyValuePair<JavaScriptObject, JavaScriptObject>(new JavaScriptObject(item.Key), item.Value);
                }
            }
            else if (_type == JavaScriptObjectType.Array)
            {
                for (int i = 0; i < _arrayValue.Count; i++)
                {
                    yield return new KeyValuePair<JavaScriptObject, JavaScriptObject>(new JavaScriptObject(i), _arrayValue[i]);
                }
            }
            else
            {
                throw new NotImplementedException("迭代器只对数组或对象类型有效");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}