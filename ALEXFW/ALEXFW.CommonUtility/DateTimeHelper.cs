using System;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    ///     日期信息方法
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        ///     获取指定日期的月份第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeMonthFirstDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        ///     获取指定月份最后一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeMonthLastDay(this DateTime dateTime)
        {
            var day = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, day);
        }

        public static long GetTimeLikeJs(this DateTime dt)
        {
            var lLeft = 621355968000000000;
            var sticks = (dt.Ticks - lLeft)/10000;
            return sticks;
        }

        public static long GetJavascriptTimestamp(this DateTime input)
        {
            var span = new TimeSpan(DateTime.Parse("1/1/1970").Ticks);
            var time = input.Subtract(span);
            return time.Ticks/10000;
        }
    }
}