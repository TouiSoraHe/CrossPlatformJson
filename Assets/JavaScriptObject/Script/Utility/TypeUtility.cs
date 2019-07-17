using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CrossPlatformJson
{
    public class TypeUtility
    {
        /// <summary>
        ///     判断是否为数字类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumeric(Type type)
        {
            return typeof(byte) == type
                   || typeof(sbyte) == type
                   || typeof(int) == type
                   || typeof(uint) == type
                   || typeof(long) == type
                   || typeof(ulong) == type
                   || typeof(short) == type
                   || typeof(ushort) == type
                   || typeof(decimal) == type
                   || typeof(double) == type
                   || typeof(float) == type;
        }

        /// <summary>
        ///     判断是否为string,char类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(Type type)
        {
            return typeof(char) == type
                   || typeof(string) == type;
        }

        /// <summary>
        ///     判断是否为枚举类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnum(Type type)
        {
            return type.IsEnum;
        }

        /// <summary>
        ///     判断是否为布尔值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBoolean(Type type)
        {
            return typeof(bool) == type;
        }

        /// <summary>
        ///     判断是否为基本数据类型,数字,枚举,布尔,字符,字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBasicType(Type type)
        {
            return IsNumeric(type) || IsString(type) || IsEnum(type) || IsBoolean(type);
        }

        /// <summary>
        ///     判断是否为集合类型,包括:数组,List,Set,Map
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollection(Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);
        }

        /// <summary>
        ///     判断是否为字典类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDictionay(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        /// <summary>
        ///     判断一个类型是否为数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArray(Type type)
        {
            return type.IsArray;
        }


        /// <summary>
        ///     获取集合类型的元素类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetElementType(Type type)
        {
            if (IsCollection(type))
            {
                if (IsArray(type))
                {
                    Type[] ret = {type.GetElementType()};
                    return ret;
                }

                return type.GetGenericArguments();
            }

            return new Type[0];
        }

        /// <summary>
        ///     获取指定类型中所有可序列化的字段信息(可序列化字段:public,[SerializeField],排除[NonSerialized])
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetFieldInfos(Type type)
        {
            var fieldInfos = new List<FieldInfo>();
            foreach (var item in type.GetFields(BindingFlags.NonPublic
                                                | BindingFlags.Public
                                                | BindingFlags.Instance))
                if (item.IsPublic && !Attribute.IsDefined(item, typeof(NonSerializedAttribute))
                    || Attribute.IsDefined(item, typeof(SerializeField)))
                    fieldInfos.Add(item);
            return fieldInfos;
        }

        /// <summary>
        ///     获取指定字段名的FieldInfo,字段不存在时返回null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(Type type, string name)
        {
            return type.GetField(name,
                BindingFlags.NonPublic
                | BindingFlags.Public
                | BindingFlags.Instance);
        }

        /// <summary>
        ///     获取指定字段名的Type,字段不存在时返回null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetFieldType(Type type, string name)
        {
            var fieldInfo = GetFieldInfo(type, name);
            return fieldInfo == null ? null : fieldInfo.FieldType;
        }

        /// <summary>
        ///     给指定对象的指定字段赋值,字段不存在时忽略
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void SetFieldValue(object o, string fieldName, object value)
        {
            var fieldInfo = GetFieldInfo(o.GetType(), fieldName);
            if (fieldInfo != null) fieldInfo.SetValue(o, value);
        }
    }
}