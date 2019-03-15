using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace ALEXFW.CommonUtility
{
    public class ZXingHelper
    {
        /// <summary>
        /// 生成条码或二维码
        /// </summary>
        /// <param name="qrcontent">二维码包涵的内容，生成条码必须是纯数字</param>
        /// <param name="barcontent">条码包涵的内容，生成条码必须是纯数字</param>
        /// <param name="qrpath">二统合码保存的路径</param>
        /// <param name="barpath">条码保存的路径</param>
        public static void GetCode(string qrcontent, string barcontent, string qrpath, string barpath)
        {
            var qrCodeWriter = new QRCodeWriter();
            var barcodeWriter = new BarcodeWriter();

            //生成二维码
            barcodeWriter.Encoder = qrCodeWriter;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new EncodingOptions
            {
                Width = 256,
                Height = 256,
                Margin = 0,
                PureBarcode = true
            };
            //生成二维码图片
            var Qrbitmap = barcodeWriter.Write(qrcontent);
            var stream1 = new MemoryStream();
            Qrbitmap.Save(stream1, ImageFormat.Jpeg);
            Qrbitmap.Dispose();
            stream1.Position = 0;
            var sw1 = new StreamWriter(qrpath);
            stream1.CopyTo(sw1.BaseStream);
            sw1.Flush();
            sw1.Close();

            //生成条码
            barcodeWriter.Encoder = null;
            barcodeWriter.Format = BarcodeFormat.ITF;
            barcodeWriter.Options = new EncodingOptions
            {
                Width = 256,
                Height = 80,
                PureBarcode = true
            };
            //生成条码图片
            var Barbitmap = barcodeWriter.Write(barcontent);
            var stream2 = new MemoryStream();
            Barbitmap.Save(stream2, ImageFormat.Jpeg);
            Barbitmap.Dispose();
            stream2.Position = 0;
            var sw2 = new StreamWriter(barpath);
            stream2.CopyTo(sw2.BaseStream);
            sw2.Flush();
            sw2.Close();
        }
    }
}