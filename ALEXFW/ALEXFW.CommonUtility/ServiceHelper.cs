using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    /// Post请求WebApi处理
    /// </summary>
    public class ServiceHelper
    {
        //WebApi宿主地址
        public static string ServiceUrl { get; } = ConfigurationManager.AppSettings["ServiceUrl"];

        /// <summary>
        ///  获取微信Token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetServiceToken()
        {
            var result = await HttpHelper.GetHttp(ServiceUrl + "/wechat");
            return result.Substring(1, result.Length - 2);
        }
    }
}