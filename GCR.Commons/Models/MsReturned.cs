namespace GCR.Commons
{
    /// <summary>
    /// 返回实体
    /// </summary>
    public class MsReturned
    {
        private string _strMS;//放回字符串信息
        private string _strMsId;//返回的ID集合
        private bool _MsRbool;//是否
        private object _obj1;//备用返回类型1
        private object _obj2;//备用返回类型2
        private object _obj3;//备用返回类型3
        private object _obj4;//备用返回类型4
        private List<object> _listobj1;//返回集合1
        private List<object> _listobj2;//返回集合2
        private List<object> _listdata;//返回泛型
        private int _RecordCount;//总行数
        private object _data;//返回数据

        /// <summary>
        /// 其他信息
        /// </summary>
        public dynamic dic { get; set; }

        /// <summary>
        /// exobj数据 可以自由接.
        /// </summary>
        public dynamic exobj1 { get; set; } = new System.Dynamic.ExpandoObject();

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// 总行数
        /// </summary>
        public int RecordCount
        {
            get { return _RecordCount; }
            set { _RecordCount = value; }
        }

        /// <summary>
        /// 返回泛型
        /// </summary>
        public List<object> listdata
        {
            get { return _listdata; }
            set { _listdata = value; }
        }

        /// <summary>
        /// 返回集合1
        /// </summary>
        public List<object> listobj2
        {
            get { return _listobj2; }
            set { _listobj2 = value; }
        }

        /// <summary>
        /// 返回集合1
        /// </summary>
        public List<object> listobj1
        {
            get { return _listobj1; }
            set { _listobj1 = value; }
        }

        /// <summary>
        /// 备用返回类型4
        /// </summary>
        public object obj4
        {
            get { return _obj4; }
            set { _obj4 = value; }
        }

        /// <summary>
        /// 备用返回类型3
        /// </summary>
        public object obj3
        {
            get { return _obj3; }
            set { _obj3 = value; }
        }

        /// <summary>
        /// 备用返回类型2
        /// </summary>
        public object obj2
        {
            get { return _obj2; }
            set { _obj2 = value; }
        }

        /// <summary>
        /// 备用返回类型1
        /// </summary>
        public object obj1
        {
            get { return _obj1; }
            set { _obj1 = value; }
        }

        /// <summary>
        /// 是否
        /// </summary>
        public bool MsRbool
        {
            get { return _MsRbool; }
            set { _MsRbool = value; }
        }

        /// <summary>
        /// 返回的ID集合
        /// </summary>
        public string strMsId
        {
            get { return _strMsId; }
            set { _strMsId = value; }
        }

        /// <summary>
        /// 是否是新增
        /// </summary>
        public bool isInsert { get; set; } = true;

        /// <summary>
        /// 返回字符串信息
        /// </summary>
        public string strMS
        {
            get { return _strMS; }
            set { _strMS = value; }
        }

        /// <summary>
        /// JObject.FromObject
        /// </summary>
        /// <returns></returns>
        public override string ToString()
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
    }
}
