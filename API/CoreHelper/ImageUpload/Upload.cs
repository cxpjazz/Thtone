using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CoreHelper.ImageUpload
{
    /// <summary>
    /// 图片上传
    /// </summary>
    public class Upload
    {
        #region 属性
        /// <summary>
        /// 上传路径
        /// </summary>
        public static string BaseFolder
        {
            get
            {
                string fileuploadpath = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"].ToString();
                string path = System.Web.HttpContext.Current.Server.MapPath("/") + fileuploadpath;
                //try
                //{
                //    path = CoreHelper.CustomSetting.GetConfigKey("CORE_UploadImagePath");
                //    path = System.Web.HttpContext.Current.Server.MapPath("/") + path;
                //}
                //catch { }
                return path;
            }
        }
        
        #endregion
        
        public static void CreateFolder(string path)
        {
            string folder = "";
            string[] arry = path.Split('\\');
            for (int i = 0; i < arry.Length; i++)
            {
                folder += arry[i] + "\\";
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
            }
        }




        #region 上传文件
        /// <summary>
        /// 通过BYTE数据保存
        /// </summary>
        /// <param name="data">图片数据，参看方面下面的调用示例</param>
        /// <param name="uploadFolder">上传文件夹名</param>
        /// <param name="message">返回消息</param>
        /// <param name="saveFile">上传成功后文件名</param>
        /// <returns></returns>
        public static bool SaveFile(byte[] data, string uploadFolder, out string message, out string saveFile)
        {
            message = "";
            //以当前日期作为文件夹名称
            string folder = DateTime.Now.ToString(@"yyyyMMdd\\");
            if (!string.IsNullOrEmpty(uploadFolder))
            {
                folder = uploadFolder + "\\" + folder;
            }
            folder = folder.Replace(@"\\", @"\");
            //如果文件夹不存在，创建
            CreateFolder(BaseFolder + "\\" + folder);

            string guid = System.Guid.NewGuid().ToString();
            guid = guid.Substring(0, 6) + DateTime.Now.ToString("ssffff");//guid前6位加当前时间秒与毫秒，避免文件名重复
            saveFile = folder + guid + ".jpg";

            FileStream fs = new FileStream(BaseFolder + "\\" + saveFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(data, 0, data.Length);
            fs.Close();
            fs = null;
            saveFile = saveFile.Replace("\\", "/");
            return true;
        }
        //调用示例
        ///将获取的文件转为byte[]
        //byte[] imgData = null;
        //if (Request.Files.Count > 0)
        //{
        //    var file = Request.Files[0];
        //    if (file.ContentLength > 0)
        //    {
        //        var stream = file.InputStream;
        //        imgData = new byte[stream.Length];
        //        stream.Read(imgData, 0, imgData.Length);
        //    }
        //}
        //string message = string.Empty;
        //string saveFile = string.Empty;
        //CoreHelper.ImageUpload.Upload.SaveFile(imgData, "headpic", 0, out message, out saveFile);
        #endregion

        static string ReMoveHost(string path)
        {
            if (path.ToLower().IndexOf("http://") == -1)
                return path;
            Uri u = new Uri(path);
            return u.AbsolutePath;
        }

        #region 生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="thumbnailMode">2:生成150*150一张，3:生成150*150和350*350两张</param>
        /// <returns></returns>
        public static bool MakeThumbImage(string fileName, params int[] thumbnailMode)
        {
            fileName = ReMoveHost(fileName);
            fileName = fileName.Replace("/", "\\");
            string imageFileExt = "|.gif|.jpg|.bmp|.png|";
            string exName = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
            if (imageFileExt.IndexOf(exName) == -1)
            {
                return false;
            }
            string file = BaseFolder + "\\" + fileName;
            bool a = false;
            foreach (var m in thumbnailMode)
            {
                a = ImageHelper.MakeThumbImage(file, m);
            }
            return a;
        } 
        #endregion



        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DeleteFile(string file)
        {
            file = ReMoveHost(file);
            CoreHelper.EventLog.Info(string.Format("删除图片 {0}", file));
            file = file.Replace("/", "\\");
            string path = BaseFolder + "\\" + file;
            path = path.Substring(0, path.LastIndexOf('\\'));
            string fileName = file.Substring(file.LastIndexOf('\\') + 1);
            //fileName = fileName.Substring(0, fileName.IndexOf('.'));
            string[] files = System.IO.Directory.GetFiles(path, fileName + "*");
            foreach (string s in files)
            {
                System.IO.File.Delete(s);
            }
            return true;
        }
    }
}
