using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace CrossPlatformJson
{
    public class JsonMapper
    {
        private static readonly StringBuilder repeatWhitespace = new StringBuilder("");

        public static string RepeatWhitespace
        {
            get
            {
                var ret = repeatWhitespace.ToString();
                repeatWhitespace.Append(" ");
                return ret;
            }
        }

        /*--------------------------------object转JsonObject------------------------------------------*/

        public static JsonObject ToJsonObject(object o)
        {
            if (o == null) return new JsonObject();
            var type = o.GetType();
            JsonObject ret;
            //处理基本数据类型
            if (TypeUtility.IsBasicType(type))
                return BasicTypeToJsonObject(o);
            //处理集合类型

            if (TypeUtility.IsCollection(type))
                return CollectionToJsonObject(o);
            //处理对象类型

            ret = new JsonObject(JsonObjectType.Object);
            foreach (var item in TypeUtility.GetFieldInfos(type))
                ret.Add(item.Name, ToJsonObject(item.GetValue(o)));
            return ret;
        }

        /// <summary>
        ///     基本数据类型转JsonObject
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static JsonObject BasicTypeToJsonObject(object o)
        {
            var type = o.GetType();
            JsonObject ret;
            if (TypeUtility.IsNumeric(type))
                ret = new JsonObject(Convert.ToDouble(o));
            else if (TypeUtility.IsString(type))
                ret = new JsonObject(Convert.ToString(o));
            else if (TypeUtility.IsEnum(type))
                ret = new JsonObject(Convert.ToInt32(o));
            else
                ret = new JsonObject((bool) o);
            return ret;
        }

        /// <summary>
        ///     集合类型转JsonObject
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static JsonObject CollectionToJsonObject(object o)
        {
            var type = o.GetType();
            JsonObject ret;
            //处理字典
            if (TypeUtility.IsDictionay(type))
            {
                ret = new JsonObject(JsonObjectType.Object);
                var collection = (IDictionary) o;
                foreach (DictionaryEntry item in collection)
                    //基本数据类型直接ToString
                    if (TypeUtility.IsBasicType(item.Key.GetType()))
                        ret.Add(item.Key.ToString(), ToJsonObject(item.Value));
                    //其他类型key统一转为json
                    else
                        //为了防止key重复,这里需要在json后面补上递增的空格
                        ret.Add(ToJsonObject(item.Key).ToJson() + RepeatWhitespace,
                            ToJsonObject(item.Value));
            }
            //处理数组,list类型
            else
            {
                ret = new JsonObject(JsonObjectType.Array);
                var collection = (ICollection) o;
                foreach (var item in collection) ret.Add(ToJsonObject(item));
            }

            return ret;
        }

        /*--------------------------------JsonObject转object------------------------------------------*/

        public static T FromJsonObject<T>(JsonObject jsonObj)
        {
            var o = FromJsonObject(jsonObj, typeof(T));
            return o == null ? default(T) : (T) o;
        }

        public static object FromJsonObject(JsonObject jsonObj, Type type)
        {
            if (jsonObj == null) throw new ArgumentNullException("jsonObj", "参数jsonObj为空");
            if (type == null) throw new ArgumentNullException("type", "参数type为空");
            if (jsonObj.Type == JsonObjectType.Null) return null;
            //处理基本数据类型
            if (TypeUtility.IsBasicType(type))
                return JsonObjectToBasicType(jsonObj, type);
            //处理集合
            if (TypeUtility.IsCollection(type))
                return JsonObjectToCollection(jsonObj, type);
            //处理对象
            return JsonObjectToObject(jsonObj, type);
        }

        private static object JsonObjectToBasicType(JsonObject jsonObj, Type type)
        {
            if (TypeUtility.IsNumeric(type))
            {
                ThrowCastErrorException(jsonObj, JsonObjectType.Number, type);
                return Convert.ChangeType(jsonObj.GetNumber(), type);
            }

            if (TypeUtility.IsString(type) || TypeUtility.IsEnum(type))
            {
                if (TypeUtility.IsString(type))
                {
                    ThrowCastErrorException(jsonObj, JsonObjectType.String, type);
                    if (typeof(char) == type) return Convert.ToChar(jsonObj.GetString());
                    return jsonObj.GetString();
                }
                //处理枚举

                if (jsonObj.Type != JsonObjectType.String
                    && jsonObj.Type != JsonObjectType.Number)
                    ThrowCastErrorException(jsonObj, type);
                if (jsonObj.Type == JsonObjectType.String)
                    return Enum.Parse(type, jsonObj.GetString());
                return Enum.ToObject(type, (int) jsonObj.GetNumber());
            }

            ThrowCastErrorException(jsonObj, JsonObjectType.Boolean, type);
            return jsonObj.GetBoolean();
        }

        private static object JsonObjectToCollection(JsonObject jsonObj, Type type)
        {
            //处理字典
            if (TypeUtility.IsDictionay(type))
                return JsonObjectToDictionary(jsonObj, type);
            //处理数组,list类型
            return JsonObjectToList(jsonObj, type);
        }

        private static object JsonObjectToDictionary(JsonObject jsonObj, Type type)
        {
            ThrowCastErrorException(jsonObj, JsonObjectType.Object, type);
            var collection = (IDictionary) Activator.CreateInstance(type);
            var types = TypeUtility.GetElementType(type);
            var keyType = types[0];
            var valueType = types[1];
            foreach (var item in jsonObj)
            {
                object key = null;
                //基本数据类型直接转回来
                if (TypeUtility.IsBasicType(keyType))
                {
                    if (TypeUtility.IsEnum(keyType))
                        key = Enum.Parse(keyType, item.Key.GetString());
                    else
                        key = Convert.ChangeType(item.Key.GetString(), keyType);
                }
                //其他类型由json反序列化回来
                else
                {
                    var valueObj = JsonObjectFactory.CreateJsonObject(item.Key.GetString());
                    key = FromJsonObject(valueObj, keyType);
                }

                var value = FromJsonObject(item.Value, valueType);
                collection[key] = value;
            }

            return collection;
        }

        private static object JsonObjectToList(JsonObject jsonObj, Type type)
        {
            ThrowCastErrorException(jsonObj, JsonObjectType.Array, type);
            var elementType = TypeUtility.GetElementType(type)[0];
            var array = Array.CreateInstance(elementType, jsonObj.Count);
            foreach (var item in jsonObj)
            {
                var index = (int) item.Key.GetNumber();
                var value = FromJsonObject(item.Value, elementType);
                array.SetValue(value, index);
            }

            //处理数组
            if (TypeUtility.IsArray(type))
                return array;
            return Activator.CreateInstance(type, array);
        }

        private static object JsonObjectToObject(JsonObject jsonObj, Type type)
        {
            ThrowCastErrorException(jsonObj, JsonObjectType.Object, type);
            var o = Activator.CreateInstance(type);
            foreach (var item in jsonObj)
            {
                Debug.Log(item.Key.GetString() + ":" + item.Value.ToJson());
                var fieldName = item.Key.GetString();
                var valueType = TypeUtility.GetFieldType(type, fieldName);
                if (valueType != null)
                    TypeUtility.SetFieldValue(o, fieldName, FromJsonObject(item.Value, valueType));
            }

            return o;
        }

        private static void ThrowCastErrorException(JsonObject jsonObj,
            JsonObjectType expectType, Type type)
        {
            if (jsonObj.Type != expectType)
                throw new Exception("类型转换错误,无法从" + jsonObj.Type + "转换为" + type.Name + ",\njson:\n" + jsonObj.ToJson());
        }

        private static void ThrowCastErrorException(JsonObject jsonObj, Type type)
        {
            throw new Exception("类型转换错误,无法从" + jsonObj.Type + "转换为" + type.Name + ",\njson:\n" + jsonObj.ToJson());
        }
    }
}