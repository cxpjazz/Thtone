using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace CoreHelper
{
    public class QrCodeHelper
    {
        #region 创建二维码
        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="SavePath">二维码图片存储路径,默认存储在网站/Upload/QRCode/日期目录下</param>
        /// <param name="LogoImage">二维码中间的Logo</param>
        /// <returns></returns>
        public static string CreateQrCode(string data, string SavePath = "",string LogoImage = "")
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Image image = qrCodeEncoder.Encode(data);
            //如果没有指定存储路径，则使用默认路径 
            if (string.IsNullOrEmpty(SavePath))
            {
                SavePath = "/Upload/QRCode/" + DateTime.Now.ToString("yyyy-MM-dd");
            }
            string DiskPath = HttpContext.Current.Server.MapPath(SavePath);
            if (!Directory.Exists(DiskPath))
            {
                Directory.CreateDirectory(DiskPath);
            }
            string FileName = string.Format("{0}{1}{2}", System.DateTime.Now.ToString("yyyyMMddHHmmssyyyy"), Guid.NewGuid(), ".jpg");
            string FilePath = DiskPath + "/" + FileName;
            //如果指定了二维码中心的Logo
            if (!string.IsNullOrEmpty(LogoImage))
            {
                image = CombinImage(image, LogoImage);//添加二维码中间的Log图片  
            }
            image.Save(FilePath);//写入图片文件中  
            //HttpContext.Current.Response.Write(WebPath + "/" + FileName);  
            //HttpContext.Current.Response.ContentType = "image/png";  
            return SavePath + "/" + FileName;
        }

        /// <summary>  
        /// 调用此函数后使此两种图片合并，类似相册，有个背景图，中间贴自己的目标图片    
        /// </summary>  
        /// <param name="imgBack">源图片(作为背景的大图)</param>  
        /// <param name="LogoImage">目标图片(中心小图)</param>  
        /// <returns>图片</returns>  
        public static Image CombinImage(Image imgBack, string LogoImage)
        {
            Bitmap bLogo = GetImg(LogoImage);
            Graphics g = Graphics.FromImage(imgBack);
            int X = imgBack.Width;
            int Y = imgBack.Height;
            Point point = new Point(X / 2 - bLogo.Width / 2, Y / 2 - bLogo.Height / 2);//logo图片绘制到二维码上，这里将简单计算一下logo所在的坐标 

            g.DrawImage(imgBack, 0, 0, X, Y);
            //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);
            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框  
            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
            g.DrawImage(bLogo, point);
            g.Dispose();
            GC.Collect();
            return imgBack;
        }

        /// <summary>  
        /// Resize图片        
        /// </summary>  
        /// <param name="bmp">原始Bitmap </param>  
        /// <param name="newW">新的宽度</param>  
        /// <param name="newH">新的高度</param>  
        /// <param name="Mode">保留着，暂时未用</param>  
        /// <returns>处理以后的图片</returns>  
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据图片的url路径获得Bitmap对象
        /// </summary>
        /// <param name="ImageUrl">图片Url</param>
        /// <returns>Bitmap对象</returns>
        public static Bitmap GetImg(string ImageUrl)
        {
            Bitmap img = null;
            try
            {
                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    System.Uri httpUrl = new System.Uri(ImageUrl);
                    WebRequest webRequest = (HttpWebRequest)(WebRequest.Create(httpUrl));
                    webRequest.Timeout = 10000; //设置超时值10秒
                    HttpWebRequest request = webRequest as HttpWebRequest;
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                    img = new Bitmap(image,30,30);
                    stream.Close();
                }
            }
            catch
            {
                return null;
            }
            return img;
        }
        #endregion
    }
}
