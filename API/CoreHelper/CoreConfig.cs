using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreHelper
{
    [Serializable]
    internal class CoreConfig
    {
        #region 属性
        private DateTime lastUpdateTime = DateTime.Now;
        /// <summary>
        /// 上次更新时间
        /// </summary>
        public DateTime LastUpdateTime
        {
            get
            {
                return lastUpdateTime;
            }
            set
            {
                lastUpdateTime = value;
            }
        }
        /// <summary>
        /// 日志消息ID
        /// </summary>
        public long LogMsgId
        {
            get;
            set;
        }
        #endregion
        const string confgiFile = "/Config/CoreConfig.config";
        /// <summary>
        /// Encrypt密钥
        /// </summary>
        public const string EncryptKey = "2qeecf73";//S8S7FLDL
        private static CoreConfig instance;
        /// <summary>
        /// 实例
        /// </summary>
        public static CoreConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = FromFile();
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        public static CoreConfig FromFile()
        {
            string file = System.Web.Hosting.HostingEnvironment.MapPath(confgiFile);
            CoreConfig cache = null;
            if (System.IO.File.Exists(file))
            {
                try
                {
                    cache = CoreHelper.SerializeHelper.BinaryDeserialize<CoreConfig>(file);
                    CoreHelper.EventLog.Log("读取CoreConfig");
                }
                catch { }
            }
            if (cache == null)
                cache = new CoreConfig();
            return cache;
        }
        public void Save()
        {
            string file = System.Web.Hosting.HostingEnvironment.MapPath(confgiFile);
            CoreHelper.SerializeHelper.BinarySerialize(this, file);
            //CoreHelper.EventLog.Log("保存CoreConfig");
        }
    }
}
