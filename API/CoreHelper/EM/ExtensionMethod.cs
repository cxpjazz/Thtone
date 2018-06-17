using System.Collections.Generic;
using System;
using System.Reflection;
namespace CoreHelper
{
    public  static partial class ExtensionMethod
    {
        public static string ToText(this bool b, string trueText, string falseText)
        {
            return b ? trueText : falseText;
        }
        public static string ToSex(this bool b)
        {
            return b ? "男" : "女";
        }
        public static string ToShow(this bool b)
        {
            return b ? "显示" : "不显示";
        }
        public static string ToPublic(this bool b)
        {
            return b ? "公开" : "保密";
        }
        public static string ToEnable(this bool b)
        {
            return b ? "启用" : "禁用";
        }
        public static string ToLock(this bool b)
        {
            return b ? "锁定" : "正常";
        }
        public static string ToCheck(this bool b)
        {
            return b ? "审核过" : "未审核";
        }
		private static Dictionary<Enum, string> dictDiscs = new Dictionary<Enum, string>();

		public static string Discription(this Enum myEnum)
		{

			string strDisc = string.Empty;



			if (dictDiscs.ContainsKey(myEnum))
			{

				strDisc = dictDiscs[myEnum];

			}

			else
			{

				strDisc = GetDiscription(myEnum);

				dictDiscs.Add(myEnum, strDisc);

			}

			return strDisc;

		}



		private static string GetDiscription(Enum myEnum)
		{

			FieldInfo fieldInfo = myEnum.GetType().GetField(myEnum.ToString());

			object[] attrs = fieldInfo.GetCustomAttributes(typeof(ItemDiscAttribute), true);

			if (attrs != null && attrs.Length > 0)
			{

				ItemDiscAttribute desc = attrs[0] as ItemDiscAttribute;

				if (desc != null)
				{

					return desc.Description;

				}
			}
			return myEnum.ToString();

		}
    }
}