﻿namespace CrossPlatformJson
{
    using System;
    using System.Text;
    using UnityEngine;

    public class JsonParse
    {
        class JsonContext
        {
            private string json;
            public int index;

            public JsonContext(string json)
            {
                this.json = json;
                this.index = 0;
            }

            public char this[int index]
            {
                get
                {
                    if (index == json.Length)
                        return '\0';
                    return json[index];
                }
            }

            public string Substring(int start, int length)
            {
                return json.Substring(start, length);
            }

            public static JsonContext operator ++(JsonContext a)
            {
                a.index++;
                return a;
            }

            public static JsonContext operator --(JsonContext a)
            {
                a.index--;
                return a;
            }
        }

        public enum ParseResult
        {
            /// <summary>
            /// 转换成功
            /// </summary>
            OK = 0,

            /// <summary>
            /// 空字符串
            /// </summary>
            EXPECT_VALUE,

            /// <summary>
            /// 无效的值
            /// </summary>
            INVALID_VALUE,

            /// <summary>
            /// 根节点不是单个
            /// </summary>
            ROOT_NOT_SINGULAR,

            /// <summary>
            /// 数值过大
            /// </summary>
            NUMBER_TOO_BIG,

            /// <summary>
            /// 缺少引号
            /// </summary>
            MISS_QUOTATION_MARK,

            /// <summary>
            /// 无效的字符串转义
            /// </summary>
            INVALID_STRING_ESCAPE,

            /// <summary>
            /// 非法字符
            /// </summary>
            INVALID_STRING_CHAR,

            /// <summary>
            /// 无效的十六进制Unicode编码
            /// </summary>
            INVALID_UNICODE_HEX,

            /// <summary>
            /// 无效的unicode代理
            /// </summary>
            INVALID_UNICODE_SURROGATE,

            /// <summary>
            /// 数组缺少逗号或方括号
            /// </summary>
            MISS_COMMA_OR_SQUARE_BRACKET,

            /// <summary>
            /// object缺少key
            /// </summary>
            MISS_KEY,

            /// <summary>
            /// 缺少冒号
            /// </summary>
            MISS_COLON,

            /// <summary>
            /// object缺少逗号或花括号
            /// </summary>
            MISS_COMMA_OR_CURLY_BRACKET

        };

        public static JavaScriptObject Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new Exception("json字符串为空");
            JavaScriptObject jsonObj;
            ParseResult result = Parse(json, out jsonObj);
            if (result != ParseResult.OK) Debug.LogError("json:" + json + "解析失败:" + result.ToString());
            return jsonObj;
        }

        public static ParseResult Parse(string json, out JavaScriptObject jsonObj)
        {
            JsonContext jsonContext = new JsonContext(json);
            ParseWhitespace(jsonContext);
            ParseResult result = ParseValue(jsonContext, out jsonObj);
            if (result == ParseResult.OK)
            {
                ParseWhitespace(jsonContext);
                if (jsonContext[jsonContext.index] != '\0')
                {
                    result = ParseResult.ROOT_NOT_SINGULAR;
                    jsonObj = null;
                }
            }
            return result;
        }

        static void ParseWhitespace(JsonContext c)
        {
            while (c[c.index] == ' ' || c[c.index] == '\t' || c[c.index] == '\n' || c[c.index] == '\r')
                c++;
        }

        static ParseResult ParseValue(JsonContext c, out JavaScriptObject v)
        {
            v = null;
            switch (c[c.index])
            {
                case 'n': return ParseLiteral(c, "null", out v, new JavaScriptObject());
                case 't': return ParseLiteral(c, "true", out v, new JavaScriptObject(true));
                case 'f': return ParseLiteral(c, "false", out v, new JavaScriptObject(false));
                case '\0': return ParseResult.EXPECT_VALUE;
                case '"': return ParseString(c, out v);
                case '[': return ParseArray(c, out v);
                case '{': return ParseObject(c, out v);
                default: return ParseNumber(c, out v);
            }
        }

        static ParseResult ParseLiteral(JsonContext c, string text, out JavaScriptObject value, JavaScriptObject defaultValue)
        {
            value = null;
            for (int i = 0; i < text.Length; i++)
            {
                if (c[c.index] != text[i])
                    return ParseResult.INVALID_VALUE;
                c++;
            }
            value = defaultValue;
            return ParseResult.OK;
        }

        static ParseResult ParseNumber(JsonContext c, out JavaScriptObject v)
        {
            v = null;
            int index = c.index;
            if (c[index] == '-') index++;
            if (c[index] == '0') index++;
            else
            {
                if (!IsDigit1To9(c[index])) return ParseResult.INVALID_VALUE;
                for (index++; IsDigit(c[index]); index++) ;
            }
            if (c[index] == '.')
            {
                index++;
                if (!IsDigit(c[index])) return ParseResult.INVALID_VALUE;
                for (index++; IsDigit(c[index]); index++) ;
            }
            if (c[index] == 'e' || c[index] == 'E')
            {
                index++;
                if (c[index] == '+' || c[index] == '-') index++;
                if (!IsDigit(c[index])) return ParseResult.INVALID_VALUE;
                for (index++; IsDigit(c[index]); index++) ;
            }
            try { v = new JavaScriptObject(Convert.ToDouble(c.Substring(c.index, index - c.index))); }
            catch (OverflowException) { return ParseResult.NUMBER_TOO_BIG; }
            c.index = index;
            return ParseResult.OK;
        }

        static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        static bool IsDigit1To9(char ch)
        {
            return ch >= '1' && ch <= '9';
        }

        static ParseResult ParseString(JsonContext c, out JavaScriptObject v)
        {
            v = null;
            c++;
            StringBuilder str = new StringBuilder();
            for (; ;)
            {
                char ch = c[c.index++];
                switch (ch)
                {
                    case '\"':
                        v = new JavaScriptObject(str.ToString());
                        return ParseResult.OK;
                    case '\\':
                        char a = c[c.index++];
                        switch (a)
                        {
                            case '\"': str.Append('\"'); break;
                            case '\\': str.Append('\\'); break;
                            case '/': str.Append('/'); break;
                            case 'b': str.Append('\b'); break;
                            case 'f': str.Append('\f'); break;
                            case 'n': str.Append('\n'); break;
                            case 'r': str.Append('\r'); break;
                            case 't': str.Append('\t'); break;
                            case 'u':
                                uint num;
                                if (!ParseHex4(c, out num))
                                    return ParseResult.INVALID_UNICODE_HEX;
                                if (num >= 0xD800 && num <= 0xDBFF)
                                {
                                    if (c[c.index++] != '\\')
                                        return ParseResult.INVALID_UNICODE_SURROGATE;
                                    if (c[c.index++] != 'u')
                                        return ParseResult.INVALID_UNICODE_SURROGATE;
                                    uint num2;
                                    if (!ParseHex4(c, out num2))
                                        return ParseResult.INVALID_UNICODE_HEX;
                                    if (num2 < 0xDC00 || num2 > 0xDFFF)
                                        return ParseResult.INVALID_UNICODE_SURROGATE;
                                    num = (((num - 0xD800) << 10) | (num2 - 0xDC00)) + 0x10000;
                                }
                                str.Append(char.ConvertFromUtf32((int)num));
                                break;
                            default:
                                return ParseResult.INVALID_STRING_ESCAPE;
                        }
                        break;
                    case '\0':
                        return ParseResult.MISS_QUOTATION_MARK;
                    default:
                        if (ch < 0x20) {
                            return ParseResult.INVALID_STRING_CHAR;
                        }
                        str.Append(ch);
                        break;
                }
            }
        }

        static bool ParseHex4(JsonContext c, out uint number)
        {
            int i;
            number = 0;
            for (i = 0; i < 4; i++)
            {
                char ch = c[c.index++];
                number <<= 4;
                if (ch >= '0' && ch <= '9') number |= (uint)(ch - '0');
                else if (ch >= 'A' && ch <= 'F') number |= (uint)(ch - ('A' - 10));
                else if (ch >= 'a' && ch <= 'f') number |= (uint)(ch - ('a' - 10));
                else return false;
            }
            return true;
        }

        static ParseResult ParseArray(JsonContext c, out JavaScriptObject v)
        {
            v = null;
            c++;
            ParseWhitespace(c);
            if (c[c.index] == ']')
            {
                c++;
                v = new JavaScriptObject(JavaScriptObject.JavaScriptObjectType.Array);
                return ParseResult.OK;
            }
            JavaScriptObject ret = new JavaScriptObject();
            while(true)
            {
                JavaScriptObject value = null;
                ParseResult result = ParseValue(c, out value);
                if (result != ParseResult.OK)
                    return result;
                ret.Add(value);
                ParseWhitespace(c);
                if (c[c.index] == ',')
                {
                    c++;
                    ParseWhitespace(c);
                }
                else if (c[c.index] == ']')
                {
                    c++;
                    v = ret;
                    return ParseResult.OK;
                }
                else
                {
                    return ParseResult.MISS_COMMA_OR_SQUARE_BRACKET;
                }
            }
        }

        static ParseResult ParseObject(JsonContext c, out JavaScriptObject v)
        {
            v = null;
            c++;
            ParseWhitespace(c);
            if (c[c.index] == '}')
            {
                c++;
                v = new JavaScriptObject(JavaScriptObject.JavaScriptObjectType.Object);
                return ParseResult.OK;
            }
            JavaScriptObject ret = new JavaScriptObject();
            for (; ; )
            {
                if (c[c.index] != '"')
                    return ParseResult.MISS_KEY;
                JavaScriptObject keyObj = null;
                ParseResult keyResult = ParseString(c, out keyObj);
                if (keyResult != ParseResult.OK)
                    return keyResult;
                ParseWhitespace(c);
                if (c[c.index] != ':')
                    return ParseResult.MISS_COLON;
                c++;
                ParseWhitespace(c);
                JavaScriptObject valueObj = null;
                ParseResult valueResult = ParseValue(c, out valueObj);
                if (valueResult != ParseResult.OK)
                    return valueResult;
                ret.Add(keyObj.GetString(), valueObj);
                ParseWhitespace(c);
                if (c[c.index] == ',')
                {
                    c++;
                    ParseWhitespace(c);
                }
                else if (c[c.index] == '}')
                {
                    c++;
                    v = ret;
                    return ParseResult.OK;
                }
                else
                {
                    return ParseResult.MISS_COMMA_OR_CURLY_BRACKET;
                }
            }
        }
    }
}