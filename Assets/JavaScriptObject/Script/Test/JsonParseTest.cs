﻿namespace CrossPlatformJson
{
    using UnityEngine;
    using UnityEngine.Assertions;

    public class JsonParseTest:MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("劲".Length);
            main();
        }

        public static void main()
        {
            TestParseNull();
            TestParseTrue();
            TestParseFalse();
            TestParseExpectValue();
            TestParseInvalidValue();
            TestParseRootNotSingular();
            TestParseNumber();
            TestParseNumberTooBig();
            TestParseString();
            TestParseMissingQuotationMark();
            TestParseInvalidStringEscape();
            TestParseInvalidStringChar();
            TestParseInvalidUnicodeHex();
            TestParseInvalidUnicodeSurrogate();
            TestParseArray();
            TestParseMissCommaOrSquareBracket();
            TestParseObject();
            TestParseMissKey();
            TestParseMissColon();
            TestParseMissCommaOrCurlyBracket();
        }

        static JavaScriptObject BaseTest(string json,JsonParse.ParseResult expectResult)
        {
            JavaScriptObject jsonObj;
            JsonParse.ParseResult result = JsonParse.Parse(json, out jsonObj);
            Assert.IsTrue(result == expectResult,"期望:"+ expectResult+",实际:"+result);
            return jsonObj;
        }

        static void TestParseNull()
        {
            Assert.IsTrue(BaseTest("null", JsonParse.ParseResult.OK).Type == JavaScriptObject.JavaScriptObjectType.Null);
            Assert.IsTrue(BaseTest(" \nnull\n ", JsonParse.ParseResult.OK).Type == JavaScriptObject.JavaScriptObjectType.Null);
        }

        static void TestParseTrue()
        {
            Assert.IsTrue(BaseTest("true", JsonParse.ParseResult.OK).GetBoolean() == true);
            Assert.IsTrue(BaseTest("  \t\rtrue\t\t", JsonParse.ParseResult.OK).GetBoolean() == true);
        }

        static void TestParseFalse()
        {
            Assert.IsTrue(BaseTest("false", JsonParse.ParseResult.OK).GetBoolean() == false);
            Assert.IsTrue(BaseTest("  \t\rfalse\t\t", JsonParse.ParseResult.OK).GetBoolean() == false);
        }

        static void TestParseExpectValue()
        {
            Assert.IsTrue(BaseTest("", JsonParse.ParseResult.EXPECT_VALUE) == null);
            Assert.IsTrue(BaseTest(" ", JsonParse.ParseResult.EXPECT_VALUE) == null);
        }

        static void TestParseInvalidValue()
        {
            Assert.IsTrue(BaseTest("nul", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("?", JsonParse.ParseResult.INVALID_VALUE) == null);

            Assert.IsTrue(BaseTest("+0", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("+1", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest(".123", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("1.", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("INF", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("inf", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("NAN", JsonParse.ParseResult.INVALID_VALUE) == null);
            Assert.IsTrue(BaseTest("nan", JsonParse.ParseResult.INVALID_VALUE) == null);
        }

        static void TestParseRootNotSingular()
        {
            Assert.IsTrue(BaseTest("null x", JsonParse.ParseResult.ROOT_NOT_SINGULAR) == null);

            // 0后面必须是 '.' , 'E' , 'e'或 空
            Assert.IsTrue(BaseTest("0123", JsonParse.ParseResult.ROOT_NOT_SINGULAR) == null);
            Assert.IsTrue(BaseTest("0x0", JsonParse.ParseResult.ROOT_NOT_SINGULAR) == null);
            Assert.IsTrue(BaseTest("0x123", JsonParse.ParseResult.ROOT_NOT_SINGULAR) == null);
        }

        static void TestNumber(double number,string json)
        {
            Assert.IsTrue(BaseTest(json, JsonParse.ParseResult.OK).GetNumber() == number);
        }

        static void TestParseNumber()
        {
            TestNumber(0.0, "0");
            TestNumber(0.0, "-0");
            TestNumber(0.0, "-0.0");
            TestNumber(1.0, "1");
            TestNumber(-1.0, "-1");
            TestNumber(1.5, "1.5");
            TestNumber(-1.5, "-1.5");
            TestNumber(3.1416, "3.1416");
            TestNumber(1E10, "1E10");
            TestNumber(1e10, "1e10");
            TestNumber(1E+10, "1E+10");
            TestNumber(1E-10, "1E-10");
            TestNumber(-1E10, "-1E10");
            TestNumber(-1e10, "-1e10");
            TestNumber(-1E+10, "-1E+10");
            TestNumber(-1E-10, "-1E-10");
            TestNumber(1.234E+10, "1.234E+10");
            TestNumber(1.234E-10, "1.234E-10");
            TestNumber(0.0, "1e-10000");

            TestNumber(1.0000000000000002, "1.0000000000000002"); 
            TestNumber(4.9406564584124654e-324, "4.9406564584124654e-324"); 
            TestNumber(-4.9406564584124654e-324, "-4.9406564584124654e-324");
            TestNumber(2.2250738585072009e-308, "2.2250738585072009e-308");  
            TestNumber(-2.2250738585072009e-308, "-2.2250738585072009e-308");
            TestNumber(2.2250738585072014e-308, "2.2250738585072014e-308");  
            TestNumber(-2.2250738585072014e-308, "-2.2250738585072014e-308");
            TestNumber(1.7976931348623157e+308, "1.7976931348623157e+308");  
            TestNumber(-1.7976931348623157e+308, "-1.7976931348623157e+308");
        }

        static void TestParseNumberTooBig()
        {
            Assert.IsTrue(BaseTest("1e309", JsonParse.ParseResult.NUMBER_TOO_BIG) == null);
            Assert.IsTrue(BaseTest("-1e309", JsonParse.ParseResult.NUMBER_TOO_BIG) == null);
        }

        static void TestString(string value, string json)
        {
            Assert.IsTrue(BaseTest(json, JsonParse.ParseResult.OK).GetString() == value);
        }

        static void TestParseString()
        {
            TestString("", "\"\"");
            TestString("Hello", "\"Hello\"");
            TestString("Hello\nWorld", "\"Hello\\nWorld\"");
            TestString("\" \\ / \b \f \n \r \t", "\"\\\" \\\\ \\/ \\b \\f \\n \\r \\t\"");
            TestString("你好", "\"你好\"");
            TestString("\u52B2", "\"\\u52B2\"");
            TestString("劲", "\"\\u52B2\"");
            TestString("Hello\0World", "\"Hello\\u0000World\"");
            TestString("\u0024", "\"\\u0024\"");         
            TestString("\u00a2", "\"\\u00A2\"");     
            TestString("\u20AC", "\"\\u20AC\""); 
            TestString("\uD834\uDD1E", "\"\\uD834\\uDD1E\"");
            TestString("\ud834\udd1e", "\"\\ud834\\udd1e\"");
        }

        static void TestParseMissingQuotationMark()
        {
            Assert.IsTrue(BaseTest("\"", JsonParse.ParseResult.MISS_QUOTATION_MARK) == null);
            Assert.IsTrue(BaseTest("\"abc", JsonParse.ParseResult.MISS_QUOTATION_MARK) == null);
        }

        static void TestParseInvalidStringEscape()
        {
            Assert.IsTrue(BaseTest("\"", JsonParse.ParseResult.MISS_QUOTATION_MARK) == null);

            Assert.IsTrue(BaseTest("\"\\v\"", JsonParse.ParseResult.INVALID_STRING_ESCAPE) == null);
            Assert.IsTrue(BaseTest("\"\\'\"", JsonParse.ParseResult.INVALID_STRING_ESCAPE) == null);
            Assert.IsTrue(BaseTest("\"\\v\"", JsonParse.ParseResult.INVALID_STRING_ESCAPE) == null);
            Assert.IsTrue(BaseTest("\"\\0\"", JsonParse.ParseResult.INVALID_STRING_ESCAPE) == null);
            Assert.IsTrue(BaseTest("\"\\x12\"", JsonParse.ParseResult.INVALID_STRING_ESCAPE) == null);
        }

        static void TestParseInvalidStringChar()
        {
            Assert.IsTrue(BaseTest("\"\x01\"", JsonParse.ParseResult.INVALID_STRING_CHAR) == null);
            Assert.IsTrue(BaseTest("\"\x1F\"", JsonParse.ParseResult.INVALID_STRING_CHAR) == null);
        }

        static void TestParseInvalidUnicodeHex()
        {
            Assert.IsTrue(BaseTest("\"\\u\""    , JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u0\""   , JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u01\""  , JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u012\"" , JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u/000\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\uG000\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u0/00\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u0G00\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u00/0\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u00G0\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u000/\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
            Assert.IsTrue(BaseTest("\"\\u000G\"", JsonParse.ParseResult.INVALID_UNICODE_HEX) == null);
        }

        static void TestParseInvalidUnicodeSurrogate()
        {
            Assert.IsTrue(BaseTest("\"\\uD800\""       , JsonParse.ParseResult.INVALID_UNICODE_SURROGATE) == null);
            Assert.IsTrue(BaseTest("\"\\uDBFF\""       , JsonParse.ParseResult.INVALID_UNICODE_SURROGATE) == null);
            Assert.IsTrue(BaseTest("\"\\uD800\\\\\""   , JsonParse.ParseResult.INVALID_UNICODE_SURROGATE) == null);
            Assert.IsTrue(BaseTest("\"\\uD800\\uDBFF\"", JsonParse.ParseResult.INVALID_UNICODE_SURROGATE) == null);
            Assert.IsTrue(BaseTest("\"\\uD800\\uE000\"", JsonParse.ParseResult.INVALID_UNICODE_SURROGATE) == null);
        }

        static void TestParseArray()
        {
            Assert.IsTrue(BaseTest("[ ]", JsonParse.ParseResult.OK).Count == 0);
            Assert.IsTrue(BaseTest("[ ]", JsonParse.ParseResult.OK).Type == JavaScriptObject.JavaScriptObjectType.Array);

            JavaScriptObject jsonObj = BaseTest("[ null , false , true , 123 , \"abc\" ]", JsonParse.ParseResult.OK);
            Assert.IsTrue(jsonObj[0].Type == JavaScriptObject.JavaScriptObjectType.Null);
            Assert.IsTrue(jsonObj[1].GetBoolean() == false);
            Assert.IsTrue(jsonObj[2].GetBoolean() == true);
            Assert.IsTrue(jsonObj[3].GetNumber() == 123);
            Assert.IsTrue(jsonObj[4].GetString() == "abc");

            jsonObj = BaseTest("[ [ ] , [ 0 ] , [ 0 , 1 ] , [ 0 , 1 , 2 ] ]", JsonParse.ParseResult.OK);
            foreach (var item in jsonObj)
            {
                foreach (var v in item.Value)
                {
                    Assert.IsTrue(v.Key.GetNumber() == v.Value.GetNumber());
                }
            }
        }

        static void TestParseMissCommaOrSquareBracket()
        {
            Assert.IsTrue(BaseTest("[1"  , JsonParse.ParseResult.MISS_COMMA_OR_SQUARE_BRACKET) == null);
            Assert.IsTrue(BaseTest("[1}" , JsonParse.ParseResult.MISS_COMMA_OR_SQUARE_BRACKET) == null);
            Assert.IsTrue(BaseTest("[1 2", JsonParse.ParseResult.MISS_COMMA_OR_SQUARE_BRACKET) == null);
            Assert.IsTrue(BaseTest("[[]", JsonParse.ParseResult.MISS_COMMA_OR_SQUARE_BRACKET) == null);
        }

        static void TestParseObject()
        {
            Assert.IsTrue(BaseTest(" { } ", JsonParse.ParseResult.OK).Count == 0);
            Assert.IsTrue(BaseTest(" { } ", JsonParse.ParseResult.OK).Type == JavaScriptObject.JavaScriptObjectType.Object);

            JavaScriptObject jsonObj = BaseTest(
                " { " +

                "\"n\" : null , " +

                "\"f\" : false , " +

                "\"t\" : true , " +

                "\"i\" : 123 , " +

                "\"s\" : \"abc\", " +

                "\"a\" : [ 1, 2, 3 ]," +

                "\"o\" : { \"1\" : 1, \"2\" : 2, \"3\" : 3 }" +

                " } "
                , JsonParse.ParseResult.OK);
            Assert.IsTrue(jsonObj["n"].Type == JavaScriptObject.JavaScriptObjectType.Null);
            Assert.IsTrue(jsonObj["f"].GetBoolean() == false);
            Assert.IsTrue(jsonObj["t"].GetBoolean() == true);
            Assert.IsTrue(jsonObj["i"].GetNumber() == 123);
            Assert.IsTrue(jsonObj["s"].GetString() == "abc");
            foreach (var item in jsonObj["o"])
            {
                Assert.IsTrue(item.Key.GetString().Equals(item.Value.GetNumber().ToString()));
            }
        }

        static void TestParseMissKey()
        {
            Assert.IsTrue(BaseTest("{:1,"      , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{1:1,"     , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{true:1,"  , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{false:1," , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{null:1,"  , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{[]:1,"    , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{{}:1,"    , JsonParse.ParseResult.MISS_KEY) == null);
            Assert.IsTrue(BaseTest("{\"a\":1,", JsonParse.ParseResult.MISS_KEY) == null);
        }

        static void TestParseMissColon()
        {
            Assert.IsTrue(BaseTest("{\"a\"}", JsonParse.ParseResult.MISS_COLON) == null);
            Assert.IsTrue(BaseTest("{\"a\",\"b\"}", JsonParse.ParseResult.MISS_COLON) == null);
        }

        static void TestParseMissCommaOrCurlyBracket()
        {
            Assert.IsTrue(BaseTest("{\"a\":1"      , JsonParse.ParseResult.MISS_COMMA_OR_CURLY_BRACKET) == null);
            Assert.IsTrue(BaseTest("{\"a\":1]"     , JsonParse.ParseResult.MISS_COMMA_OR_CURLY_BRACKET) == null);
            Assert.IsTrue(BaseTest("{\"a\":1 \"b\"", JsonParse.ParseResult.MISS_COMMA_OR_CURLY_BRACKET) == null);
            Assert.IsTrue(BaseTest("{\"a\":{}", JsonParse.ParseResult.MISS_COMMA_OR_CURLY_BRACKET) == null);
        }

    }
}