using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ALEXFW.CommonUtility
{
    public static class ImageHelper
    {
        public static string ImageWebsite { get; } = ConfigurationManager.AppSettings["ImageWebsite"];

        public static async Task<string> UploadImage(Stream stream, string filename, int? width, int? height,
            int? stretch)
        {
            var url = ImageWebsite + "?filename=" + filename;
            if (width.HasValue && height.HasValue)
                url += "&width=" + width.Value + "&height=" + height.Value;
            if (stretch.HasValue)
                url += "&stretch=" + stretch.Value;
            var request = WebRequest.CreateHttp(url);
            request.Method = "POST";
            var requestStream = await request.GetRequestStreamAsync();
            await stream.CopyToAsync(requestStream);
            requestStream.Close();
            try
            {
                var response = (HttpWebResponse) await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                var path = await reader.ReadToEndAsync();
                reader.Close();
                response.Close();
                return path;
            }
            catch
            {
                return null;
            }
        }

        public static Task<string> UploadImage(Stream stream, string filename)
        {
            return UploadImage(stream, filename, null, null, null);
        }

        /// <summary>
        ///     用户端上传多图
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="name">前端 input 标签的名字。</param>
        /// <returns></returns>
        public static async Task<string> UpMultiImage(this Controller controller, string name)
        {
            var imageArray = new List<string>();
            foreach (var img in controller.Request.Files.GetMultiple(name))
                if (img.InputStream.Length > 1024*1024*0.5)
                {
                    var extension = Path.GetExtension(img.FileName);
                    if (extension.Equals(".gif"))
                        imageArray.Add(await gif(img.InputStream));
                    else
                        imageArray.Add(
                            await
                                UploadImage(Compress(new Bitmap(img.InputStream), extension, 50),
                                    _GetNewFileName(img.FileName)));
                }
                else
                {
                    imageArray.Add(await UploadImage(img.InputStream, _GetNewFileName(img.FileName)));
                }
            return string.Join(",", imageArray);
        }

        private static string _GetNewFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return Guid.NewGuid() + extension;
        }

        /// <summary>
        ///     用户端上传单图
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="name">前端 input 标签的名字。</param>
        /// <returns></returns>
        public static async Task<string> UpSingleImage(this Controller controller, string name)
        {
            var img = controller.Request.Files[name];
            if (img != null)
                return await UploadImage(img.InputStream, img.FileName);
            return "";
        }

        public static async Task<bool> DeleteImage(string path)
        {
            var request = WebRequest.CreateHttp(ImageWebsite + path);
            request.Method = "DELETE";
            try
            {
                var response = (HttpWebResponse) await request.GetResponseAsync();
                var code = response.StatusCode;
                response.Close();
                return code == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> gif(Stream srcImage)
        {
            //原图路径
            //string imgPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\0.gif";
            var filename = Guid.NewGuid() + ".gif";
            var path = "";
            //原图
            var img = Image.FromStream(srcImage);
            //不够100*100的不缩放
            if ((img.Width > 100) && (img.Height > 100))
            {
                //新图第一帧
                Image new_img = new Bitmap(100, 100);
                //新图其他帧
                Image new_imgs = new Bitmap(100, 100);
                //新图第一帧GDI+绘图对象
                var g_new_img = Graphics.FromImage(new_img);
                //新图其他帧GDI+绘图对象
                var g_new_imgs = Graphics.FromImage(new_imgs);
                //配置新图第一帧GDI+绘图对象
                g_new_img.CompositingMode = CompositingMode.SourceCopy;
                g_new_img.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g_new_img.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g_new_img.SmoothingMode = SmoothingMode.HighQuality;
                g_new_img.Clear(Color.FromKnownColor(KnownColor.Transparent));
                //配置其他帧GDI+绘图对象
                g_new_imgs.CompositingMode = CompositingMode.SourceCopy;
                g_new_imgs.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g_new_imgs.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g_new_imgs.SmoothingMode = SmoothingMode.HighQuality;
                g_new_imgs.Clear(Color.FromKnownColor(KnownColor.Transparent));
                //遍历维数
                foreach (var gid in img.FrameDimensionsList)
                {
                    //因为是缩小GIF文件所以这里要设置为Time
                    //如果是TIFF这里要设置为PAGE
                    var f = FrameDimension.Time;
                    //获取总帧数
                    var count = img.GetFrameCount(f);
                    //保存标示参数
                    var encoder = Encoder.SaveFlag;
                    //
                    EncoderParameters ep = null;
                    //图片编码、解码器
                    ImageCodecInfo ici = null;
                    //图片编码、解码器集合
                    var icis = ImageCodecInfo.GetImageDecoders();
                    //为 图片编码、解码器 对象 赋值
                    foreach (var ic in icis)
                        if (ic.FormatID == ImageFormat.Gif.Guid)
                        {
                            ici = ic;
                            break;
                        }
                    //每一帧
                    for (var c = 0; c < count; c++)
                    {
                        //选择由维度和索引指定的帧
                        img.SelectActiveFrame(f, c);
                        //第一帧
                        if (c == 0)
                        {
                            //将原图第一帧画给新图第一帧
                            g_new_img.DrawImage(img, new Rectangle(0, 0, 100, 100),
                                new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                            //把振频和透明背景调色板等设置复制给新图第一帧
                            for (var i = 0; i < img.PropertyItems.Length; i++)
                                new_img.SetPropertyItem(img.PropertyItems[i]);
                            ep = new EncoderParameters(1);
                            //第一帧需要设置为MultiFrame
                            ep.Param[0] = new EncoderParameter(encoder, (long) EncoderValue.MultiFrame);
                            //保存第一帧
                            new_img.Save(@"D:\" + filename, ici, ep);
                        }
                        //其他帧
                        else
                        {
                            //把原图的其他帧画给新图的其他帧
                            g_new_imgs.DrawImage(img, new Rectangle(0, 0, 100, 100),
                                new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                            //把振频和透明背景调色板等设置复制给新图第一帧
                            for (var i = 0; i < img.PropertyItems.Length; i++)
                                new_imgs.SetPropertyItem(img.PropertyItems[i]);
                            ep = new EncoderParameters(1);
                            //如果是GIF这里设置为FrameDimensionTime
                            //如果为TIFF则设置为FrameDimensionPage
                            ep.Param[0] = new EncoderParameter(encoder, (long) EncoderValue.FrameDimensionTime);
                            //向新图添加一帧
                            new_img.SaveAdd(new_imgs, ep);
                        }
                    }
                    ep = new EncoderParameters(1);
                    //关闭多帧文件流
                    ep.Param[0] = new EncoderParameter(encoder, (long) EncoderValue.Flush);
                    new_img.SaveAdd(ep);
                }

                //上传文件
                var stream = new MemoryStream();
                var gifimg = Image.FromFile(@"D:\" + filename);
                gifimg.Save(stream, ImageFormat.Gif);
                stream.Position = 0;
                path = await UploadImage(stream, filename);
                gifimg.Dispose();

                File.Delete(@"D:\" + filename);
                //释放文件
                img.Dispose();
                new_img.Dispose();
                new_imgs.Dispose();
                g_new_img.Dispose();
                g_new_imgs.Dispose();

                return path;
            }
            return await UploadImage(srcImage, filename);
        }

        /// <summary>
        ///     图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="extension"></param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static Stream Compress(Bitmap srcBitmap, string extension, long level)
        {
            var myImageCodecInfo = UploadHelper.GetEncoder(UploadHelper._getImageFormat(extension));
            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = level; //设置压缩的比例1-100
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;

            var destStream = new MemoryStream();
            srcBitmap.Save(destStream, myImageCodecInfo, ep);
            destStream.Position = 0;
            return destStream;
        }
    }
}