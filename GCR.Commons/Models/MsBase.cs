

using SqlSugar;
using SqlSugar.Extensions;

namespace GCR.Commons
{
    public class MsBase
    {
        private object _obj1;//备用1
        private object _obj2;//备用2
        private string _typecode1;//分类1
        private string _typecode2;//分类2

        /// <summary>
        /// exobj数据 可以自由接. 使用GetExobj 进行获取
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public dynamic objex1 { get; set; } = new System.Dynamic.ExpandoObject();

        /// <summary>
        /// 分类2
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string typecode2
        {
            get { return _typecode2; }
            set { _typecode2 = value; }
        }

        /// <summary>
        /// 分类1
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string typecode1
        {
            get { return _typecode1; }
            set { _typecode1 = value; }
        }

        /// <summary>
        /// 备用2
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object obj2
        {
            get { return _obj2; }
            set { _obj2 = value; }
        }

        /// <summary>
        /// 备用1
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object obj1
        {
            get { return _obj1; }
            set { _obj1 = value; }
        }

        /// <summary>
        /// 转实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T parse<T>(string data)
        {
            JObject keyValuePairs = JObject.Parse(data);
            T ms = (T)keyValuePairs.ToObject(typeof(T));
            return ms;
        }

        /// <summary>
        /// 转实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T parse<T>(JObject data)
        {
            T ms = (T)data.ToObject(typeof(T));
            return ms;
        }

        /// <summary>
        /// 转成json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            if (PageContext.EnvironmentName == "Development")
            {
                return JObject.FromObject(this).ToString();
            }
            else
            {
                return JObject.FromObject(this).ToString(Formatting.None);
            }
        }

        /// <summary>
        /// 得到 objex1 参数中的值
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="exname"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public TOut? GetExobj<TOut>(string exname, Func<TOut> converter)
        {
            object value = null;
            var asg = (IDictionary<string, object>)objex1;//参数
            if (asg.ContainsKey(exname))
            {
                value = asg[exname];
            }
            return (TOut?)(value ?? converter());
        }
    }
}
