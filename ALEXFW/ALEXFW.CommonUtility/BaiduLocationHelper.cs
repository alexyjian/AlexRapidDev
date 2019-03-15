using System;

namespace ALEXFW.CommonUtility
{
    public class BaiduLocationHelper
    {
        /// <summary>
        ///     百度地图 坐标定位
        /// </summary>
        /// <param name="address"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        public static void GetLocation(string address, ref double lng, ref double lat)
        {
            var ak = "emKzTIR8orgFMUjXFLjgU1IT";
            var url = "http://api.map.baidu.com/geocoder/v2/?output=json&ak=" + ak + "&address=" + address;

            var postReturn = JsonPost.doPostRequest(url, new byte[1]);
            var newObj = new JsonObject(postReturn);

            if (newObj["status"].Value == "0")
            {
                var s = newObj["result"].Items[0]["location"].Items;
                lng = Convert.ToDouble(s[0]["Lng"].Value);
                lat = Convert.ToDouble(s[0]["Lat"].Value);
            }
        }

        /// <summary>
        ///     坐标转换
        ///     参考 ：http://developer.baidu.com/map/index.php?title=webapi/guide/changeposition#.E6.9C.8D.E5.8A.A1.E5.9C.B0.E5.9D.80
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static void GeoConvert(double lng, double lat, ref double x, ref double y)
        {
            var ak = "emKzTIR8orgFMUjXFLjgU1IT";
            var url = "http://api.map.baidu.com/geoconv/v1/?coords=" + lng + "," + lat + "&from=1&to=5&ak=" + ak;

            var postReturn = JsonPost.doPostRequest(url, new byte[1]);
            var newObj = new JsonObject(postReturn);
            if (newObj["status"].Value == "0")
            {
                var s = newObj["result"].Items[0];
                x = Convert.ToDouble(s["x"].Value);
                y = Convert.ToDouble(s["y"].Value);
            }
        }
    }
}