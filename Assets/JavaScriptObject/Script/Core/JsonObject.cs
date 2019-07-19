using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CrossPlatformJson
{
    public class JsonObject : IEnumerable<KeyValuePair<JsonObject, JsonObject>>
    {
        private static readonly char[] hexDigits =
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        /// <summary>
        ///     用来存放数组
        /// </summary>
        private List<JsonObject> _arrayValue;

        /// <summary>
        ///     用来存放基本数据类型
        /// </summary>
        private object _baseValue;

        /// <summary>
        ///     用来存放对象
        /// </summary>
        private Dictionary<string, JsonObject> _objectValue;

        /// <summary>
        ///     类型
        /// </summary>
        private JsonObjectType _type;

        public JsonObject(double value)
        {
            Type = JsonObjectType.Number;
            _baseValue = value;
        }

        public JsonObject(string value)
        {
            Type = JsonObjectType.String;
            _baseValue = value;
        }

        public JsonObject(char value)
        {
            Type = JsonObjectType.String;
            _baseValue = new string(value, 1);
        }

        public JsonObject(bool value)
        {
            Type = JsonObjectType.Boolean;
            _baseValue = value;
        }

        public JsonObject(JsonObjectType type = JsonObjectType.Null)
        {
            Type = type;
            if (Type == JsonObjectType.Object)
                _objectValue = new Dictionary<string, JsonObject>();
            else if (Type == JsonObjectType.Array) _arrayValue = new List<JsonObject>();
        }

        public JsonObject this[int index]
        {
            get
            {
                if (Type != JsonObjectType.Array) throw new Exception("该对象不是一个数组类型");
                return _arrayValue[index];
            }

            set
            {
                if (Type != JsonObjectType.Array) throw new Exception("该对象不是一个数组类型");
                _arrayValue[index] = value;
            }
        }

        public JsonObject this[string key]
        {
            get
            {
                if (Type != JsonObjectType.Object) throw new Exception("该对象不是一个对象类型");
                return _objectValue[key];
            }

            set
            {
                if (Type != JsonObjectType.Object) throw new Exception("该对象不是一个对象类型");
                _objectValue[key] = value;
            }
        }

        /// <summary>
        ///     获取该对象/数组的子对象/子元素数量,对于Null永远返回0,对于基本数据类型永远返回1
        /// </summary>
        public int Count
        {
            get
            {
                switch (Type)
                {
                    case JsonObjectType.Null:
                        return 0;
                    case JsonObjectType.Boolean:
                        return 1;
                    case JsonObjectType.Number:
                        return 1;
                    case JsonObjectType.String:
                        return 1;
                    case JsonObjectType.Array:
                        return _arrayValue.Count;
                    case JsonObjectType.Object:
                        return _objectValue.Count;
                    default:
                        return -1;
                }
            }
        }

        public JsonObjectType Type
        {
            get { return _type; }

            private set { _type = value; }
        }

        public IEnumerator<KeyValuePair<JsonObject, JsonObject>> GetEnumerator()
        {
            if (_type == JsonObjectType.Object)
                foreach (var item in _objectValue)
                    yield return new KeyValuePair<JsonObject, JsonObject>(new JsonObject(item.Key),
                        item.Value);
            else if (_type == JsonObjectType.Array)
                for (var i = 0; i < _arrayValue.Count; i++)
                    yield return new KeyValuePair<JsonObject, JsonObject>(new JsonObject(i),
                        _arrayValue[i]);
            else
                throw new NotImplementedException("迭代器只对数组或对象类型有效");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public double GetNumber()
        {
            if (Type == JsonObjectType.Number)
                return (double) _baseValue;
            throw new Exception("该对象不是一个数值类型");
        }

        public string GetString()
        {
            if (Type == JsonObjectType.String)
                return (string) _baseValue;
            throw new Exception("该对象不是一个字符串类型");
        }

        public bool GetBoolean()
        {
            if (Type == JsonObjectType.Boolean)
                return (bool) _baseValue;
            throw new Exception("该对象不是一个布尔值类型");
        }

        public void Add(JsonObject value)
        {
            if (Type == JsonObjectType.Null)
            {
                Type = JsonObjectType.Array;
                _arrayValue = new List<JsonObject>();
            }

            if (Type != JsonObjectType.Array) throw new Exception("该对象不是一个数组类型");
            _arrayValue.Add(value);
        }

        public void Add(double value)
        {
            Add(new JsonObject(value));
        }

        public void Add(string value)
        {
            Add(new JsonObject(value));
        }

        public void Add(char value)
        {
            Add(new JsonObject(value));
        }

        public void Add(bool value)
        {
            Add(new JsonObject(value));
        }

        public void Add(string key, JsonObject value)
        {
            if (Type == JsonObjectType.Null)
            {
                Type = JsonObjectType.Object;
                _objectValue = new Dictionary<string, JsonObject>();
            }

            if (Type != JsonObjectType.Object) throw new Exception("该对象不是一个对象类型");
            _objectValue.Add(key, value);
        }

        public void Add(string key, double value)
        {
            Add(key, new JsonObject(value));
        }

        public void Add(string key, string value)
        {
            Add(key, new JsonObject(value));
        }

        public void Add(string key, char value)
        {
            Add(key, new JsonObject(value));
        }

        public void Add(string key, bool value)
        {
            Add(key, new JsonObject(value));
        }

        public void Remove(int index)
        {
            if (Type != JsonObjectType.Array) throw new Exception("该对象不是一个数组类型");
            _arrayValue.RemoveAt(index);
        }

        /// <summary>
        ///     从对象中移除一个字段,该类型必须为Object
        /// </summary>
        /// <param name="key">需要移除的字段名</param>
        public void Remove(string key)
        {
            if (Type != JsonObjectType.Object) throw new Exception("该对象不是一个对象类型");
            _objectValue.Remove(key);
        }

        /// <summary>
        ///     判断指定的字段名是否存在该对象中,类型必须为Object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (Type != JsonObjectType.Object) throw new Exception("该对象不是一个对象类型");
            return _objectValue.ContainsKey(key);
        }

        /// <summary>
        ///     清空对象/数组的所有子对象/子元素
        /// </summary>
        public void Clear()
        {
            if (_objectValue != null) _objectValue.Clear();
            if (_arrayValue != null) _arrayValue.Clear();
        }

        public void SetNumber(double value)
        {
            if (Type == JsonObjectType.Null) Type = JsonObjectType.Number;
            if (Type == JsonObjectType.Number)
                _baseValue = value;
            else
                throw new Exception("该对象不是一个数值类型");
        }

        public void SetString(string value)
        {
            if (Type == JsonObjectType.Null) Type = JsonObjectType.String;
            if (Type == JsonObjectType.String)
                _baseValue = value;
            else
                throw new Exception("该对象不是一个字符串类型");
        }

        public void SetString(char value)
        {
            SetString(new string(value, 1));
        }

        public void SetBoolean(bool value)
        {
            if (Type == JsonObjectType.Null) Type = JsonObjectType.Boolean;
            if (Type == JsonObjectType.Boolean)
                _baseValue = value;
            else
                throw new Exception("该对象不是一个布尔值类型");
        }

        /// <summary>
        /// 将JsonObject转换为json
        /// </summary>
        /// <param name="digit">设定转换数字的时候保留几位小数,默认为null</param>
        /// <returns></returns>
        public string ToJson(uint? digit = null)
        {
            switch (Type)
            {
                case JsonObjectType.Null:
                    return "null";
                case JsonObjectType.Boolean:
                    return GetBoolean() ? "true" : "false";
                case JsonObjectType.Number:
                    if (digit != null)
                    {
                        ulong multiple = (ulong)Math.Pow(10, digit.Value);
                        return (Math.Floor(GetNumber() * multiple) / multiple).ToString();
                    }
                    return GetNumber().ToString();
                case JsonObjectType.String:
                    return StringifyString(GetString());
                case JsonObjectType.Array:
                {
                    var ret = new StringBuilder();
                    ret.Append("[");
                    for (var i = 0; i < _arrayValue.Count; i++)
                    {
                        if (i != 0) ret.Append(",");
                        ret.Append(_arrayValue[i].ToJson());
                    }

                    ret.Append("]");
                    return ret.ToString();
                }

                case JsonObjectType.Object:
                {
                    var ret = new StringBuilder();
                    ret.Append("{");
                    var index = 0;
                    foreach (var item in _objectValue)
                    {
                        if (index != 0) ret.Append(",");
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

        private static string StringifyString(string value)
        {
            if (value == null) return "null";
            var str = new StringBuilder();
            str.Append("\"");
            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];
                switch (ch)
                {
                    case '\"':
                        str.Append('\\');
                        str.Append('\"');
                        break;
                    case '\\':
                        str.Append('\\');
                        str.Append('\\');
                        break;
                    case '\b':
                        str.Append('\\');
                        str.Append('b');
                        break;
                    case '\f':
                        str.Append('\\');
                        str.Append('f');
                        break;
                    case '\n':
                        str.Append('\\');
                        str.Append('n');
                        break;
                    case '\r':
                        str.Append('\\');
                        str.Append('r');
                        break;
                    case '\t':
                        str.Append('\\');
                        str.Append('t');
                        break;
                    default:
                        if (ch < 0x20)
                        {
                            str.Append('\\');
                            str.Append('u');
                            str.Append('0');
                            str.Append('0');
                            str.Append(hexDigits[ch >> 4]);
                            str.Append(hexDigits[ch & 15]);
                        }
                        else
                        {
                            str.Append(ch);
                        }

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

        /// <summary>
        /// 比较两个JsonObject是否一样,对于小数,该方法将忽略指定精度后的值,默认精度为6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj, 6);
        }

        public bool Equals(object obj, int digit)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((JsonObject)obj, digit);
        }

        protected bool Equals(JsonObject other, int digit)
        {
            return
                _type == other._type
                && (_type == JsonObjectType.Number
                    ? Math.Abs(GetNumber() - other.GetNumber()) < Math.Pow(10, digit * -1) ||
                      GetNumber().ToString().Equals(other.GetNumber().ToString())
                    : Equals(_baseValue, other._baseValue))
                && CollectionUtility.CompareX(_arrayValue, other._arrayValue)
                && CollectionUtility.CompareX(_objectValue, other._objectValue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _arrayValue != null ? _arrayValue.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (_baseValue != null ? _baseValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_objectValue != null ? _objectValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)_type;
                return hashCode;
            }
        }
    }

    /// <summary>
    ///     JsonObject的类型
    /// </summary>
    public enum JsonObjectType
    {
        Null = 0,
        Boolean = 1,
        Number = 2,
        String = 3,
        Array = 4,
        Object = 5
    }
}