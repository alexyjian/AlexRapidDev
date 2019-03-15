using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using ALEXFW.Entity;

namespace ALEXFW.CommonUtility
{
    public class ValidateCodeHelper
    {
        public static string GetRandomCode(int num)
        {
            string[] source =
            {
                "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K",
                "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };
            var code = "";
            var p = int.Parse(DateTime.Now.Second + DateTime.Now.Millisecond.ToString());
            Thread.Sleep(1);
            var rd = new Random(p);
            for (var i = 0; i < num; i++)
                code += source[rd.Next(0, source.Length)];
            return code;
        }

        public static string GetValidateCode(int number)
        {
            //number :验证码的位数
            //amount :生成的验证码个数
            var amount = 200;
            int a;
            var valiCodes = new List<ValidateCode>();
            var codeArray = new string[amount];
            var rd = new Random();
            var i = 0;
            do
            {
                string[] source =
                {
                    "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "J",
                    "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
                };
                var code = "";

                for (var j = 0; j < number; j++)
                    code += source[rd.Next(0, source.Length)];

                a = 0;
                foreach (var valicode in valiCodes)
                    if (valicode.Name == code)
                    {
                        a = 1;
                        break;
                    }
                if (a != 1)
                {
                    var valicode = new ValidateCode();
                    valicode.Name = code;
                    codeArray[i] = code;
                    valiCodes.Add(valicode);
                    i++;
                }
            } while (i != amount);
            Thread.Sleep(10);
            var singleCode = codeArray[rd.Next(0, codeArray.Length)];
            return singleCode;
        }

        /// <summary>
        ///     创建验证码的图片
        /// </summary>
        /// <param name="validateCode"></param>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            var image = new Bitmap((int) Math.Ceiling(validateCode.Length*16.0), 22);
            var g = Graphics.FromImage(image);
            try
            {
                //随机转动角度
                var randAngle = 45;
                //生成随机生成器
                var random = new Random();
                //清空图片背景色
                g.Clear(Color.White);

                //验证码旋转，防止机器识别  
                var chars = validateCode.ToCharArray(); //拆散字符串成单字符数组  

                //文字距中  
                var format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                //画图片的干扰线
                for (var i = 0; i < 25; i++)
                {
                    var x1 = random.Next(image.Width);
                    var x2 = random.Next(image.Width);
                    var y1 = random.Next(image.Height);
                    var y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                var font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic);
                var brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue,
                    Color.DarkRed, 1.2f, true);

                for (var i = 0; i < chars.Length; i++)
                {
                    var dot = new Point(14, 14);
                    float angle = random.Next(-randAngle, randAngle); //转动的度数  
                    g.TranslateTransform(dot.X, dot.Y); //移动光标到指定位置  
                    g.RotateTransform(angle);
                    g.DrawString(chars[i].ToString(), font, brush, 1, 1, format);
                    g.RotateTransform(-angle); //转回去  
                    g.TranslateTransform(-2, -dot.Y); //移动光标到指定位置，每个字符紧凑显示，避免被软件识别  
                }
                //画图片的前景干扰点
                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(image.Width);
                    var y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        ///     创建验证码的图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public static Bitmap CreateValidateImage(string validateCode)
        {
            var image = new Bitmap((int) Math.Ceiling(validateCode.Length*16.0), 22);
            var g = Graphics.FromImage(image);
            try
            {
                //随机转动角度
                var randAngle = 45;

                //生成随机生成器
                var random = new Random();

                //清空图片背景色
                g.Clear(Color.White);

                //验证码旋转，防止机器识别  
                var chars = validateCode.ToCharArray(); //拆散字符串成单字符数组  

                //文字距中  
                var format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;


                //画图片的干扰线
                for (var i = 0; i < 25; i++)
                {
                    var x1 = random.Next(image.Width);
                    var x2 = random.Next(image.Width);
                    var y1 = random.Next(image.Height);
                    var y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                var font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic);
                var brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue,
                    Color.DarkRed, 1.2f, true);

                for (var i = 0; i < chars.Length; i++)
                {
                    var dot = new Point(14, 14);
                    float angle = random.Next(-randAngle, randAngle); //转动的度数  
                    g.TranslateTransform(dot.X, dot.Y); //移动光标到指定位置  
                    g.RotateTransform(angle);
                    g.DrawString(chars[i].ToString(), font, brush, 1, 1, format);
                    g.RotateTransform(-angle); //转回去  
                    g.TranslateTransform(-2, -dot.Y); //移动光标到指定位置，每个字符紧凑显示，避免被软件识别  
                }

                //画图片的前景干扰点
                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(image.Width);
                    var y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                return image;
            }
            finally
            {
                g.Dispose();
            }
        }
    }
}