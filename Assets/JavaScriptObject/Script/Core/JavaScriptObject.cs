namespace CrossPlatformJson
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class JavaScriptObject : IEnumerable<KeyValuePair<JavaScriptObject, JavaScriptObject>>
    {
        /// <summary>
        /// JavaScriptObject的类型
        /// </summary>
        public enum JavaScriptObjectType
        {
            Null = 0,
            Boolean = 1,
            Number = 2,
            String = 3,
            Array = 4,
            Object = 5
        }

        /// <summary>
        /// 类型
        /// </summary>
        private JavaScriptObjectType _type;

        /// <summary>
        /// 用来存放基本数据类型
        /// </summary>
        private object _baseValue;

        /// <summary>
        /// 用来存放数组
        /// </summary>
        private List<JavaScriptObject> _arrayValue;

        /// <summary>
        /// 用来存放对象
        /// </summary>
        private Dictionary<string, JavaScriptObject> _objectValue;

        public JavaScriptObject(double value)
        {
            Type = JavaScriptObjectType.Number;
            _baseValue = value;
        }

        public JavaScriptObject(string value)
        {
            Type = JavaScriptObjectType.String;
            _baseValue = value;
        }

        public JavaScriptObject(char value)
        {
            Type = JavaScriptObjectType.String;
            _baseValue = new string(value,1);
        }

        public JavaScriptObject(bool value)
        {
            Type = JavaScriptObjectType.Boolean;
            _baseValue = value;
        }

        public JavaScriptObject(JavaScriptObjectType type = JavaScriptObjectType.Null)
        {
            Type = type;
            if (Type == JavaScriptObjectType.Object)
            {
                _objectValue = new Dictionary<string, JavaScriptObject>();
            }
            else if (Type == JavaScriptObjectType.Array)
            {
                _arrayValue = new List<JavaScriptObject>();
            }
        }

        public double GetNumber()
        {
            if (Type == JavaScriptObjectType.Number)
            {
                return (double)_baseValue;
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
                return (string)_baseValue;
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
                return (bool)_baseValue;
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

        public void Add(char value)
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

        public void Add(string key, char value)
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

        /// <summary>
        /// 从对象中移除一个字段,该类型必须为Object
        /// </summary>
        /// <param name="key">需要移除的字段名</param>
        public void Remove(string key)
        {
            if (Type != JavaScriptObjectType.Object)
            {
                throw new Exception("该对象不是一个对象类型");
            }
            _objectValue.Remove(key);
        }

        /// <summary>
        /// 判断指定的字段名是否存在该对象中,类型必须为Object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (Type != JavaScriptObjectType.Object)
            {
                throw new Exception("该对象不是一个对象类型");
            }
            return _objectValue.ContainsKey(key);
        }

        /// <summary>
        /// 清空对象/数组的所有子对象/子元素
        /// </summary>
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

        /// <summary>
        /// 获取该对象/数组的子对象/子元素数量,对于Null永远返回0,对于基本数据类型永远返回1
        /// </summary>
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
                _baseValue = value;
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
                _baseValue = value;
            }
            else
            {
                throw new Exception("该对象不是一个字符串类型");
            }
        }

        public void SetString(char value)
        {
            SetString(new string(value, 1));
        }

        public void SetBoolean(bool value)
        {
            if (Type == JavaScriptObjectType.Null)
            {
                Type = JavaScriptObjectType.Boolean;
            }
            if (Type == JavaScriptObjectType.Boolean)
            {
                _baseValue = value;
            }
            else
            {
                throw new Exception("该对象不是一个布尔值类型");
            }
        }

        public string ToJson()
        {
            switch (Type)
            {
                case JavaScriptObjectType.Null:
                    return "null";
                case JavaScriptObjectType.Boolean:
                    return GetBoolean() ? "true" : "false";
                case JavaScriptObjectType.Number:
                    return GetNumber().ToString();
                case JavaScriptObjectType.String:
                    return StringifyString(GetString());
                case JavaScriptObjectType.Array:
                    {
                        StringBuilder ret = new StringBuilder();
                        ret.Append("[");
                        for (int i = 0; i < _arrayValue.Count; i++)
                        {
                            if (i != 0)
                            {
                                ret.Append(",");
                            }
                            ret.Append(_arrayValue[i].ToJson());
                        }
                        ret.Append("]");
                        return ret.ToString();
                    }
                case JavaScriptObjectType.Object:
                    {
                        StringBuilder ret = new StringBuilder();
                        ret.Append("{");
                        int index = 0;
                        foreach (var item in _objectValue)
                        {
                            if (index != 0)
                            {
                                ret.Append(",");
                            }
                            ret.Append(StringifyString(item.Key));
                            ret.Append(":");
                            ret.Append(item.Value.ToJson());
                            index++;
                        }
                        ret.Append("}");
                        return ret.ToString();
                    }
                default:
                    throw new Exception("意外的情况");
            }
        }

        private static readonly char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private static string StringifyString(string value)
        {
            if (value == null) return "null";
            StringBuilder str = new StringBuilder();
            str.Append("\"");
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                switch (ch)
                {
                    case '\"': str.Append( '\\'); str.Append('\"'); break;
                    case '\\': str.Append('\\'); str.Append('\\'); break;
                    case '\b': str.Append('\\'); str.Append('b'); break;
                    case '\f': str.Append('\\'); str.Append('f'); break;
                    case '\n': str.Append('\\'); str.Append('n'); break;
                    case '\r': str.Append('\\'); str.Append('r'); break;
                    case '\t': str.Append('\\'); str.Append('t'); break;
                    default:
                        if (ch < 0x20)
                        {
                            str.Append('\\'); str.Append('u'); str.Append('0'); str.Append('0');
                            str.Append(hexDigits[ch >> 4]);
                            str.Append(hexDigits[ch & 15]);
                        }
                        else
                            str.Append(ch);
                        break;
                }
            }
            str.Append("\"");
            return str.ToString();
        }

        public override string ToString()
        {
            return ToJson();
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