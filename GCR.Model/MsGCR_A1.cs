
using GCR.Commons;
using SqlSugar;

namespace GCR.Model
{
    /// <summary>
    /// 配置表
    /// </summary>
    [SugarTable("GCR_A1", TableDescription = "配置表")]
    public partial class MsGCR_A1 : MsBase
    {
        /// <summary>
        /// AutoID
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int? GCR_A1_AUTOID { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public string GCR_A1_ISDEL { get; set; }

        /// <summary>
        /// 新增时间
        /// </summary>
        public string GCR_A1_MAKETIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string GCR_A1_UPTIME { get; set; }

        /// <summary>
        /// 新增人
        /// </summary>
        public string GCR_A1_MAKEUSER { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int? GCR_A1_PARENTID { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string GCR_A1_MAKEFROM { get; set; }

        /// <summary>
        /// 平台类型
        /// </summary>
        public string GCR_A1_10 { get; set; }

        /// <summary>
        /// 平台地址
        /// </summary>
        public string GCR_A1_20 { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string GCR_A1_30 { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string GCR_A1_40 { get; set; }

        /// <summary>
        /// 是否全部项目
        /// </summary>
        public string GCR_A1_50 { get; set; }

        /// <summary>
        /// 项目 逗号分割
        /// </summary>
        public string GCR_A1_60 { get; set; }

        /// <summary>
        /// 是否有效 1 有效 0 无效
        /// </summary>
        public string GCR_A1_70 { get; set; }

        /// <summary>
        /// GPT角色
        /// </summary>
        public string GCR_A1_80 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_A1_90 { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string GCR_A1_100 { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<MsGCR_A1> Child { get; set; }

        /// <summary>
        /// Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MsGCR_A1 parse(Newtonsoft.Json.Linq.JObject data)
        {
            MsGCR_A1 ms = (MsGCR_A1)data.ToObject(typeof(MsGCR_A1));
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