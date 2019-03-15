using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using ALEXFW.CommonUtility;

namespace ALEXFW.DeskTop.Controllers
{
    public class VerifyController : Controller
    {
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Index(string path, int width, int height)
        {
            var code = ValidateCodeHelper.GetValidateCode(4);
            Session["vcode_" + path] = code.ToLower();
            var image = ValidateCodeHelper.CreateValidateImage(code);
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            image.Dispose();
            stream.Position = 0;
            return File(stream, "image/jpeg", "verifyCode.jpg");
        }

        public static bool VerifyCode(Controller controller, string path, string code)
        {
            return (string) controller.Session["vcode_" + path] == code.ToLower();
        }

        public static bool VerifyCodeOnce(Controller controller, string path, string code)
        {
            var success = (string) controller.Session["vcode_" + path] == code.ToLower();
            if (success)
                controller.Session["vcode_" + path] = null;
            return success;
        }

        public static void CleanCode(Controller controller, string path)
        {
            controller.Session["vcode_" + path] = null;
        }
    }
}