﻿namespace Microshaoft
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class StringHelper
    {
        // 控制字符
        //private static readonly Regex _controlCharRegex = new Regex(@"[\p{C}]", RegexOptions.Compiled);

        /// <summary>
        /// 移除控制字符
        /// </summary>
        /// https://www.cnblogs.com/deali/p/17085516.html
        public static string RemoveControlChars(this string @this)
                    => (@this == null ? null : string.Concat(@this.Where(c => !char.IsControl(c))))!;
        public static string RemoveStarts(this string @this, string remove)
        {
            var r = @this;
            if (@this.StartsWith(remove, StringComparison.OrdinalIgnoreCase))
            {
                var l = @this.Length;
                var ll = remove.Length;
                r = @this.Substring(l - ll);
            }
            return
                r;
        }
        public static IEnumerable<string> SplitToCharacters(this string @this)
        {
            //原来有一个"零宽度连接符"(Zero - width joiner / ZWJ)的概念，
            //值为0x200D。如果发现char为该值，则说明它是一个零宽度连接符，
            //此时后面的emoji应该与前面的emoji连接。可以使用如下代码分析"👨‍👩‍👧‍👦"这个emoji
            /*
             * https://www.cnblogs.com/sdflysha/archive/2019/10/28/20191026-split-string-to-character-list.html
             */
            var l = @this.Length;
            for (var i = 0; i < l; ++i)
            {
                if (char.IsHighSurrogate(@this[i]))
                {
                    int length = 0;
                    while (true)
                    {
                        length += 2;
                        if 
                            (
                                i + length < @this.Length
                                &&
                                @this[i + length] == 0x200D
                            )
                        {
                            length += 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    yield
                        return
                            @this
                                .Substring
                                    (
                                        i
                                        , length
                                    );
                    i += length - 1;
                }
                else
                {
                    yield
                        return
                            @this[i]
                                .ToString();
                }
            }
        }


        /// <summary>
        /// Returns first non whitespace character
        /// </summary>
        /// <param name="@this">Text to search in</param>
        /// <returns>Non whitespace or default char</returns>
        public static char FirstNonWhitespaceCharacter(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return '\0';
            }
            for (int i = 0; i < @this.Length; i++)
            {
                if (!char.IsWhiteSpace(@this[i]))
                {
                    return @this[i];
                }
            }
            return '\0';
        }
        public static int FindIndex
                                (
                                    this string @this
                                    , string search
                                    , int hits
                                    , bool ignoreCase = true
                                )
        {
            int p = 0;
            var s = @this;
            var ss = search;
            if (ignoreCase)
            {
                s = @this.ToUpper();
                ss = search.ToUpper();
            }
            var i = 0;
            var l = @this.Length;
            var ll = search.Length;
            do
            {
                p = s.IndexOf(ss, p);
                if (p > 0)
                {
                    i++;
                    p += ll;
                }
            }
            while 
                (
                    p <= l
                    &&
                    p >= 0
                    &&
                    i < hits
                );
            return p;
        }
        public static bool IsNullOrEmptyOrWhiteSpace(this string @this)
        {
            return 
                    string.IsNullOrEmpty(@this)
                    ||
                    string.IsNullOrWhiteSpace(@this);
        }
        public static bool IsNullOrEmptyOrWhiteSpaceOrZeroLength(this string @this)
        {
            return
                    IsNullOrEmptyOrWhiteSpace(@this)
                    ||
                    @this.Length == 0;
        }
        public static bool IsValidString(string text)
        {
            return 
                (
                    text != string.Empty
                    && text != null
                    && text.Trim().Length > 0
                );
        }
        public static string PadLeftInBytes
                            (
                                this string text
                                , int totalWidth
                                , char paddingChar = ' '
                                , Encoding? encoding = null
                            )
        {
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }
            totalWidth -=
                            (
                                encoding.GetByteCount(text)
                                - text.Length
                            );
            return
                text
                    .PadLeft
                        (
                            totalWidth
                            , paddingChar
                        );
        }
        public static string PadRightInBytes
                            (
                                this string text
                                , int totalWidth
                                , char paddingChar = ' '
                                , Encoding? encoding = null
                            )
        {
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }
            totalWidth -=
                            (
                                encoding.GetByteCount(text)
                                - text.Length
                            );
            return
                text
                    .PadRight
                        (
                            totalWidth
                            , paddingChar
                        );
        }
    }
    public static class StringsHelper
    {
        public static bool StringsCompareWithWild
                                (
                                    Tuple<string, string>[] a
                                    , string wild = "*"
                                )
        {
            var r = true;
            foreach (var xx in a)
            {
                r = r &&
                    (
                        xx.Item1 == wild ?
                        true
                        :
                        (string.Compare(xx.Item1, xx.Item2, true) == 0)
                    );
                if (!r)
                {
                    break;
                }
            }
            return r;
        }
        public static string ToNormalizedVersionString
                            (
                                this string @this
                            )
        {
            return @this
                        .Split
                            (
                                '.'
                            )
                        .Aggregate
                            (
                                (x, y) =>
                                {
                                    if (!x.StartsWith("0."))
                                    {
                                        x = x.TrimStart('0');
                                    }
                                    y = y.TrimStart('0');
                                    if (x.Length == 0)
                                    {
                                        x = "0";
                                    }
                                    if (y.Length == 0)
                                    {
                                        y = "0";
                                    }
                                    return $"{x}.{y}";
                                }
                            );
        }
    }
}
namespace Microshaoft
{
    using System.Collections.Specialized;
    using System.Text;

    public static class HtmlHelper
    {
        public static string GenerateFormHTML
                                (
                                    NameValueCollection httpRequestFields
                                    , string formName
                                    , string httpMethod
                                    , string actionUrl
                                )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\"><br>", formName, httpMethod, actionUrl));
            foreach (string? var in httpRequestFields.AllKeys)
            {
                string s = string.Format
                                    (
                                        "{0} <input type=\"text\" name=\"{0}\" value=\"{1}\" /><br>"
                                        , var
                                        , httpRequestFields[var]
                                    );
                if (StringHelper.IsValidString(s))
                {
                    sb.Append(s);
                }
            }
            sb.Append("<input type=\"submit\" />");
            sb.Append("</form>");
            //sb.Append(string.Format("<script type=\"text/javascript\">alert('asda');{0}.submit()</script>", formName));
            return sb.ToString();
        }
    }
}