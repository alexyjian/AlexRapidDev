using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace ALEXFW.CommonUtility
{
    public static class UploadHelper
    {
        /// <summary>
        ///     上传多图，非自动控制器时 可用此方法更新图片
        ///     图片保存到 WebUploadFile/T 目录下
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="entity"></param>
        /// <param name="propertyname">属性名</param>
        /// <returns></returns>
        public static string UpMultiImage<T>(this Controller controller, T entity, string propertyname = null) where T : class
        {
            var boType = typeof(T);
            var propertyName = propertyname ??
                boType.GetProperties()
                    .FirstOrDefault(
                        x =>
                            x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false) != null &&
                            ((CustomDataTypeAttribute)x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false))
                                .Custom == "MultiImage")
                    ?.Name;

            var basePath = controller.Server.MapPath("~/WebUploadFile/");
            var oldImg = controller.Request.Form[propertyName + "_oldImg"];
            var imageArray = new List<string>();
            foreach (var file in controller.Request.Files.GetMultiple(propertyName + "_newImg"))
            {
                if (file.ContentLength <= 0) continue;

                var bitmap = new Bitmap(file.InputStream);
                var height = bitmap.Height;
                var width = bitmap.Width;
                var extension = Path.GetExtension(file.FileName);
                string fileName;

                // 压缩大于2MB的图片
                using (var stream = new MemoryStream())
                {
                    if (file.ContentLength > 1024 * 1024 * 2)
                    {
                        fileName = CompressImage<T>(bitmap, basePath, extension, height, width, 50);
                    }
                    else
                    {
                        bitmap.Save(stream, _getImageFormat(extension));
                        stream.Position = 0;
                        fileName = ProcessUpload<T>(stream, extension, basePath);
                    }
                }

                bitmap.Dispose();

                imageArray.Add("/WebUploadFile" + fileName);
            }

            if (oldImg == null)
                oldImg = "";
            var sourceImg = _getSourceImg(entity, "MultiImage") ?? "";
            var sourceImgArray = sourceImg.Split(',');
            var oldImgArray = oldImg.Split(',');
            var deleteImgArray = sourceImgArray.Except(oldImgArray).ToArray();
            foreach (var img in deleteImgArray)
            {
                try
                {
                    if (File.Exists(controller.Server.MapPath(img)))
                        File.Delete(controller.Server.MapPath(img));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (!string.IsNullOrEmpty(oldImg))
                oldImg += "," + string.Join(",", imageArray);
            else
                oldImg = string.Join(",", imageArray);

            return oldImg;
        }

        /// <summary>
        ///     上传单图，非自动控制器时 可用此方法更新图片
        ///     图片保存到 WebUploadFile/T 目录下
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string UpSingleImage<T>(this Controller controller, T entity) where T : class
        {
            var boType = typeof(T);
            var propertyName =
                boType.GetProperties()
                    .FirstOrDefault(
                        x =>
                            x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false) != null &&
                            ((CustomDataTypeAttribute)x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false))
                                .Custom == "SingleImage")
                    ?.Name;

            var oldImg = _getSourceImg(entity, "SingleImage");
            var file = controller.Request.Files[propertyName];
            if (file == null || file.ContentLength <= 0)
            {
                return oldImg;
            }

            var bitmap = new Bitmap(file.InputStream);
            var height = bitmap.Height;
            var width = bitmap.Width;
            var extension = Path.GetExtension(file.FileName);
            string fileName;
            var basePath = controller.Server.MapPath("~/WebUploadFile/");
            // 压缩大于2MB的图片
            using (var stream = new MemoryStream())
            {
                if (file.ContentLength > 1024 * 1024 * 2)
                {
                    fileName = CompressImage<T>(bitmap, basePath, extension, height, width, 50);
                }
                else
                {
                    bitmap.Save(stream, _getImageFormat(extension));
                    stream.Position = 0;
                    fileName = ProcessUpload<T>(stream, extension, basePath);
                }
            }
            bitmap.Dispose();

            try
            {
                if (File.Exists(controller.Server.MapPath(oldImg)))
                    File.Delete(controller.Server.MapPath(oldImg));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "/WebUploadFile" + fileName;
        }

        /// <summary>
        ///     上传单文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string UpSingleFile<T>(this Controller controller, T entity)
        {
            var boType = typeof(T);
            var propertyName =
                boType.GetProperties()
                    .FirstOrDefault(
                        x =>
                            x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false) != null &&
                            ((CustomDataTypeAttribute)x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false))
                                .Custom == "SingleFile")
                    ?.Name;
            var oldFile = _getSourceImg(entity, "SingleFile");
            var file = controller.Request.Files[propertyName];
            if (file == null || file.ContentLength <= 0)
            {
                return oldFile;
            }

            var extension = Path.GetExtension(file.FileName);
            string fileName;
            var basePath = controller.Server.MapPath("~/WebUploadFile/");
            using (var stream = new MemoryStream())
            {
                stream.Position = 0;
                fileName = ProcessUpload<T>(stream, extension, basePath);
                file.SaveAs(basePath + fileName);
            }

            try
            {
                if (File.Exists(controller.Server.MapPath(oldFile)))
                    File.Delete(controller.Server.MapPath(oldFile));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "/WebUploadFile" + fileName;
        }

        /// <summary>
        ///     根据扩展名，获取正确的保存的格式
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ImageFormat _getImageFormat(string extension)
        {
            ImageFormat result = null;
            var s = extension.ToLower();
            switch (s)
            {
                case ".jpeg":
                    result = ImageFormat.Jpeg;
                    break;

                case ".gif":
                    result = ImageFormat.Gif;
                    break;

                case ".png":
                    result = ImageFormat.Png;
                    break;

                default:
                    result = ImageFormat.Jpeg;
                    break;
            }
            return result;
        }

        private static string _getSourceImg<T>(T entity, string custom)
        {
            var boType = typeof(T);
            var firstOrDefault =
                boType.GetProperties()
                    .FirstOrDefault(
                        x =>
                            x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false) != null &&
                            ((CustomDataTypeAttribute)x.GetCustomAttribute(typeof(CustomDataTypeAttribute), false))
                                .Custom == custom);
            if (firstOrDefault != null)
            {
                var propertiesValue = (string)firstOrDefault.GetValue(entity);
                return propertiesValue;
            }
            return "";
        }

        private static string ProcessUpload<T>(Stream inputStream, string fileExtension, string basePath)
        {
            var filename = NewFilePath<T>(fileExtension, basePath);
            using (
                Stream stream = File.Open(basePath + filename, FileMode.CreateNew,
                    FileAccess.Write, FileShare.Read))
            {
                inputStream.CopyTo(stream);
                stream.Close();
                stream.Dispose();
            }
            return filename.Replace('\\', '/');
        }

        private static string NewFilePath<T>(string fileExtension, string basePath)
        {
            var path = "\\" + typeof(T).Name;// + "\\";
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            var now = DateTime.Now;
            path += "\\" + now.Year + now.Month.ToString().PadLeft(2, '0');
            if (!Directory.Exists(basePath + path))
                Directory.CreateDirectory(basePath + path);
            path += "\\" + now.Day.ToString().PadLeft(2, '0');
            if (!Directory.Exists(basePath + path))
                Directory.CreateDirectory(basePath + path);
            path += "\\" + Guid.NewGuid() + fileExtension;
            return path;
        }

        /// <summary>
        ///     无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth">宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>
        private static string CompressImage<T>(Bitmap sFile, string dFile, string extension, int dHeight, int dWidth,
            int flag)
        {
            var iSource = sFile;
            int sW = 0, sH = 0;
            //按比例缩放
            var tem_size = new Size(iSource.Width, iSource.Height);

            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if (tem_size.Width * dHeight > tem_size.Height * dWidth)
                {
                    sW = dWidth;
                    sH = dWidth * tem_size.Height / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = tem_size.Width * dHeight / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }
            var ob = new Bitmap(dWidth, dHeight);
            var g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width,
                iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = flag; //设置压缩的比例1-100
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                var fileName = "";
                using (var stream = new MemoryStream())
                {
                    ImageCodecInfo codecInfo = GetEncoder(_getImageFormat(extension));
                    ob.Save(stream, codecInfo, ep);
                    stream.Position = 0;
                    fileName = ProcessUpload<T>(stream, extension, dFile);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}