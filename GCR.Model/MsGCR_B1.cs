
using GCR.Commons;
using SqlSugar;

namespace GCR.Model
{
    /// <summary>
    /// 日志
    /// </summary>
    [SugarTable("GCR_B1", TableDescription = "日志")]
    public partial class MsGCR_B1 : MsBase
    {
        /// <summary>
        /// AutoID
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int? GCR_B1_AUTOID { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public string GCR_B1_ISDEL { get; set; }

        /// <summary>
        /// 新增时间
        /// </summary>
        public string GCR_B1_MAKETIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string GCR_B1_UPTIME { get; set; }

        /// <summary>
        /// 新增人
        /// </summary>
        public string GCR_B1_MAKEUSER { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int? GCR_B1_PARENTID { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string GCR_B1_MAKEFROM { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string GCR_B1_10 { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string GCR_B1_20 { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string GCR_B1_30 { get; set; }

        /// <summary>
        /// 提交的数据
        /// </summary>
        public string GCR_B1_40 { get; set; }

        /// <summary>
        /// 生成的数据
        /// </summary>
        public string GCR_B1_50 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_B1_60 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_B1_70 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_B1_80 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_B1_90 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_B1_100 { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<MsGCR_B1> Child { get; set; }

        /// <summary>
        /// Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MsGCR_B1 parse(Newtonsoft.Json.Linq.JObject data)
        {
            MsGCR_B1 ms = (MsGCR_B1)data.ToObject(typeof(MsGCR_B1));
            return ms;
        }

        /// <summary>
        /// Json
        /// </summary>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JObject ToJObject()
        {
            return Newtonsoft.Json.Linq.JObject.FromObject(this);
        }
    }
}