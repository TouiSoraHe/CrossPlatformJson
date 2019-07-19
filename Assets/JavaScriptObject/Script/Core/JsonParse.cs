using System;
using System.Text;
using UnityEngine;

namespace CrossPlatformJson
{
    public class JsonParse
    {
        public enum ParseResult
        {
            /// <summary>
            ///     转换成功
            /// </summary>
            OK = 0,

            /// <summary>
            ///     空字符串
            /// </summary>
            EXPECT_VALUE,

            /// <summary>
            ///     无效的值
            /// </summary>
            INVALID_VALUE,

            /// <summary>
            ///     根节点不是单个
            /// </summary>
            ROOT_NOT_SINGULAR,

            /// <summary>
            ///     数值过大
            /// </summary>
            NUMBER_TOO_BIG,

            /// <summary>
            ///     缺少引号
            /// </summary>
            MISS_QUOTATION_MARK,

            /// <summary>
            ///     无效的字符串转义
            /// </summary>
            INVALID_STRING_ESCAPE,

            /// <summary>
            ///     非法字符
            /// </summary>
            INVALID_STRING_CHAR,

            /// <summary>
            ///     无效的十六进制Unicode编码
            /// </summary>
            INVALID_UNICODE_HEX,

            /// <summary>
            ///     无效的unicode代理
            /// </summary>
            INVALID_UNICODE_SURROGATE,

            /// <summary>
            ///     数组缺少逗号或方括号
            /// </summary>
            MISS_COMMA_OR_SQUARE_BRACKET,

            /// <summary>
            ///     object缺少key
            /// </summary>
            MISS_KEY,

            /// <summary>
            ///     缺少冒号
            /// </summary>
            MISS_COLON,

            /// <summary>
            ///     object缺少逗号或花括号
            /// </summary>
            MISS_COMMA_OR_CURLY_BRACKET
        }

        public static JsonObject Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new Exception("json字符串为空");
            JsonObject jsonObj;
            var result = Parse(json, out jsonObj);
            if (result != ParseResult.OK) Debug.LogError("json:" + json + "解析失败:" + result);
            return jsonObj;
        }

        public static ParseResult Parse(string json, out JsonObject jsonObj)
        {
            var jsonContext = new JsonContext(json);
            ParseWhitespace(jsonContext);
            var result = ParseValue(jsonContext, out jsonObj);
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

        private static void ParseWhitespace(JsonContext c)
        {
            while (c[c.index] == ' ' || c[c.index] == '\t' || c[c.index] == '\n' || c[c.index] == '\r')
                c++;
        }

        private static ParseResult ParseValue(JsonContext c, out JsonObject v)
        {
            v = null;
            switch (c[c.index])
            {
                case 'n': return ParseLiteral(c, "null", out v, new JsonObject());
                case 't': return ParseLiteral(c, "true", out v, new JsonObject(true));
                case 'f': return ParseLiteral(c, "false", out v, new JsonObject(false));
                case '\0': return ParseResult.EXPECT_VALUE;
                case '"': return ParseString(c, out v);
                case '[': return ParseArray(c, out v);
                case '{': return ParseObject(c, out v);
                default: return ParseNumber(c, out v);
            }
        }

        private static ParseResult ParseLiteral(JsonContext c, string text, out JsonObject value,
            JsonObject defaultValue)
        {
            value = null;
            for (var i = 0; i < text.Length; i++)
            {
                if (c[c.index] != text[i])
                    return ParseResult.INVALID_VALUE;
                c++;
            }

            value = defaultValue;
            return ParseResult.OK;
        }

        private static ParseResult ParseNumber(JsonContext c, out JsonObject v)
        {
            v = null;
            var index = c.index;
            if (c[index] == '-') index++;
            if (c[index] == '0')
            {
                index++;
            }
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

            try
            {
                v = new JsonObject(Convert.ToDouble(c.Substring(c.index, index - c.index)));
            }
            catch (OverflowException)
            {
                return ParseResult.NUMBER_TOO_BIG;
            }

            c.index = index;
            return ParseResult.OK;
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsDigit1To9(char ch)
        {
            return ch >= '1' && ch <= '9';
        }

        private static ParseResult ParseString(JsonContext c, out JsonObject v)
        {
            v = null;
            c++;
            var str = new StringBuilder();
            for (;;)
            {
                var ch = c[c.index++];
                switch (ch)
                {
                    case '\"':
                        v = new JsonObject(str.ToString());
                        return ParseResult.OK;
                    case '\\':
                        var a = c[c.index++];
                        switch (a)
                        {
                            case '\"':
                                str.Append('\"');
                                break;
                            case '\\':
                                str.Append('\\');
                                break;
                            case '/':
                                str.Append('/');
                                break;
                            case 'b':
                                str.Append('\b');
                                break;
                            case 'f':
                                str.Append('\f');
                                break;
                            case 'n':
                                str.Append('\n');
                                break;
                            case 'r':
                                str.Append('\r');
                                break;
                            case 't':
                                str.Append('\t');
                                break;
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

                                str.Append(char.ConvertFromUtf32((int) num));
                                break;
                            default:
                                return ParseResult.INVALID_STRING_ESCAPE;
                        }

                        break;
                    case '\0':
                        return ParseResult.MISS_QUOTATION_MARK;
                    default:
                        if (ch < 0x20) return ParseResult.INVALID_STRING_CHAR;
                        str.Append(ch);
                        break;
                }
            }
        }

        private static bool ParseHex4(JsonContext c, out uint number)
        {
            int i;
            number = 0;
            for (i = 0; i < 4; i++)
            {
                var ch = c[c.index++];
                number <<= 4;
                if (ch >= '0' && ch <= '9') number |= (uint) (ch - '0');
                else if (ch >= 'A' && ch <= 'F') number |= (uint) (ch - ('A' - 10));
                else if (ch >= 'a' && ch <= 'f') number |= (uint) (ch - ('a' - 10));
                else return false;
            }

            return true;
        }

        private static ParseResult ParseArray(JsonContext c, out JsonObject v)
        {
            v = null;
            c++;
            ParseWhitespace(c);
            if (c[c.index] == ']')
            {
                c++;
                v = new JsonObject(JsonObjectType.Array);
                return ParseResult.OK;
            }

            var ret = new JsonObject();
            while (true)
            {
                JsonObject value = null;
                var result = ParseValue(c, out value);
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

        private static ParseResult ParseObject(JsonContext c, out JsonObject v)
        {
            v = null;
            c++;
            ParseWhitespace(c);
            if (c[c.index] == '}')
            {
                c++;
                v = new JsonObject(JsonObjectType.Object);
                return ParseResult.OK;
            }

            var ret = new JsonObject();
            for (;;)
            {
                if (c[c.index] != '"')
                    return ParseResult.MISS_KEY;
                JsonObject keyObj = null;
                var keyResult = ParseString(c, out keyObj);
                if (keyResult != ParseResult.OK)
                    return keyResult;
                ParseWhitespace(c);
                if (c[c.index] != ':')
                    return ParseResult.MISS_COLON;
                c++;
                ParseWhitespace(c);
                JsonObject valueObj = null;
                var valueResult = ParseValue(c, out valueObj);
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

        private class JsonContext
        {
            private readonly string json;
            public int index;

            public JsonContext(string json)
            {
                this.json = json;
                index = 0;
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
    }
}