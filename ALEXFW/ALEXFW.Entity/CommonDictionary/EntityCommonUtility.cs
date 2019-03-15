using System;
using System.Globalization;
using System.Linq;

namespace ALEXFW.Entity.CommonDictionary
{
    public class EntityCommonUtility
    {
        /// <summary>
        ///     提取根据系统时间生成 带实体名的SortCode 所需要的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetSortCode<T>()
        {
            var result = "Default";
            var timeStampString = "";

            var nowTime = DateTime.Now;
            timeStampString = nowTime.ToString("yyyy-MM-dd-hh-mm-ss-ffff", DateTimeFormatInfo.InvariantInfo);

            var entityName = typeof(T).Name;
            result = entityName + "_" + timeStampString;
            return result;
        }

        /// <summary>
        ///     提取根据系统时间生成 SortCode 所需要的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetSortCode()
        {
            var timeStampString = "";
            var nowTime = DateTime.Now;
            timeStampString = nowTime.ToString("yyyyMMddHHmmssffff", DateTimeFormatInfo.InvariantInfo);
            return timeStampString;
        }

        /// <summary>
        ///     提取指定的业务对象属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperties().FirstOrDefault(pn => pn.Name == propertyName);
            if (property != null)
            {
                var propertyValue = property.GetValue(obj);
                return propertyValue;
            }
            return null;
        }
    }
}