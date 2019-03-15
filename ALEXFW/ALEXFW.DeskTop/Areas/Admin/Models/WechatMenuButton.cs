using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALEXFW.DeskTop.Areas.Admin.Models
{
    public class WechatMenuButton
    {
        public string name { get; set; }

        public string type { get; set; }

        public string key { get; set; }

        public string url { get; set; }

        public string media_id { get; set; }

        public List<WechatMenuButton> sub_button { get; set; }
    }
}