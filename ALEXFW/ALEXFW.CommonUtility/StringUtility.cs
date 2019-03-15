using System.Text.RegularExpressions;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    ///     一些扩展的简单的字符串操作方法
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        ///     剔除字符串中 Html 标识符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length">如果为零，则返回全部，否则返回指定的长度</param>
        /// <returns></returns>
        public static string HtmlToText(string source, int length = 0)
        {
            var strNohtml = Regex.Replace(source, "<[^>]+>", "");
            strNohtml = Regex.Replace(strNohtml, "&[^;]+;", "");

            if (length > 0)
                if (strNohtml.Length > length - 2)
                    strNohtml = strNohtml.Substring(0, length - 2) + "...";
            return strNohtml;
        }

        /// <summary>
        ///     过滤换行符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length">如果为零，则返回全部，否则返回指定的长度</param>
        /// <returns></returns>
        public static string HtmlToFilterTabSymbols(string source, int length = 0)
        {
            var strNohtml = Regex.Replace(source, "\r?\n", "");
            strNohtml = Regex.Replace(strNohtml, " +<", "<");
            if (length > 0)
                if (strNohtml.Length > length - 2)
                    strNohtml = strNohtml.Substring(0, length - 2) + "...";
            return strNohtml;
        }
    }
}