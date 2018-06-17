using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace CoreHelper
{
    public class EnumHelper
    {
        /// <summary>
        /// 存储枚举的键值对对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        public class KVP<T, S>
        {
            public int Key { get; set; }
            public string Value { get; set; }
            public KVP(int k, string v)
            {
                this.Key = k;
                this.Value = v;
            }
        }


        //将枚举转换为Hashtable;T为枚举名称
        public Hashtable EnumToHashtable<T>()
        {
            Hashtable has = new Hashtable();
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                has.Add(i.ToString(), Enum.GetName(typeof(T), i));
            }
            return has;
        }

        //将枚举转换为泛型类;T为枚举名称
        public static List<KVP<int, string>> EnumToList<T>()
        {
            List<KVP<int, string>> list = new List<KVP<int, string>>();
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                KVP<int, string> tmp = new KVP<int, string>(i, Enum.GetName(typeof(T), i));
                list.Add(tmp);
            }
            return list;
        }

        //通过枚举的值获取对应的枚举变量
        public static string GetEnumVariable<T>(int key)
        {
            Hashtable hs = new Hashtable();
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                hs.Add(i, Enum.GetName(typeof(T), i));
            }
            return hs[key].ToString();
        }

        ////通过枚举的值获取对应的枚举变量
        //public static IEnumerable GetEnumVariable<T>(int key)
        //{
        //    foreach (int i in Enum.GetValues(typeof(T)))
        //    {
        //        if (i == key)
        //        {
        //            yield return Enum.GetName(typeof(T), i);
        //            break;
        //        }
        //    }
        //}
    }
}