using System.Web.Optimization;

namespace ALEXFW.DeskTop
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.form.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.complexify.js",
                "~/Scripts/jquery.placeholder.min.js",
                "~/Scripts/modernizr-{version}.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/respond.js",
                "~/Scripts/classie.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/jquery.pnotify.css",
                "~/Content/bootstrap.min.css",
                "~/Content/bootstrap-datetimepicker.min.css",
                "~/Content/bootstrap-theme.min.css",
                "~/Content/site.css"));
        }
    }
}