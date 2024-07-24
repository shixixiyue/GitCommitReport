using GCR.BLL.GPT;
using GCR.Model;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace GCR.BLL
{
    public class BLLGCRB1 : SqlSugarHelp<MsGCR_B1>, ISqlQueryed<MsGCR_B1>
    {
        private readonly Lazy<BLLGCRA1> bLLGCRA1;

        public BLLGCRB1(Lazy<BLLGCRA1> bLLGCRA1)
        {
            this.bLLGCRA1 = bLLGCRA1;
        }

        public async Task<MsGCR_B1> GetNew(string today = "")
        {
            var ms = new MsGCR_B1();
            if (string.IsNullOrEmpty(today)) { today = DateTime.Now.ToString("yyyy-MM-dd"); }

            var key = "GetNew" + today;
            var cache = PageContext.GetServerByApp<ICache>();
            if (cache.Exists(key)) { return (MsGCR_B1)cache.Get(key); }
            else
            {
                var mask = new GPTB1Mask();
                mask.adduser(today);
                string res = await mask.GetResult();
                try
                {
                    ms = MsGCR_B1.parse(JObject.Parse(res));
                }
                catch
                {
                    ms = new MsGCR_B1();
                }
                cache.Set(key, ms, true);
                return ms;
            }
        }

        public async Task<MsReturned> Handle(SqlSugarQueryed_Event<MsGCR_B1> request, CancellationToken cancellationToken)
        {
            var type = request.type;
            var msr = request.msr;
            if (type != SqlSugarQueryedType.Query)
            {
                SignalRHelper.SendHubMessage("SaveGCR_B1", msr.ToJson());
            }
            if (type == SqlSugarQueryedType.Update)
            {
                //var upmsr = request.msr as MsSqlSugarUpdate;
                //var data = upmsr.GetSaveData(() => new MsGCR_B1());
                //foreach (var item in data)
                //{
                //    var msa1 = await bLLGCRA1.Value.GetUseSetUp();
                //    await msa1.GetCommits(item.GCR_B1_20, item.GCR_B1_30);
                //}
            }
            if (type == SqlSugarQueryedType.Insert)
            {
                var addmsr = request.msr as MsSqlSugarInsert;
                var data = addmsr.GetSaveData(() => new MsGCR_B1()).ToList();
                var ids = addmsr.ids;
                for (int i = 0; i < ids.Length; i++)
                {
                    data[i].GCR_B1_AUTOID = Convert.ToInt32(ids[i]);
                }
                var msa1 = await bLLGCRA1.Value.GetUseSetUp();
                var updata = new List<MsGCR_B1>();
                foreach (var item in data)
                {
                    var commits = await msa1.GetCommits(item.GCR_B1_20, item.GCR_B1_30);
                    //updata.Add()
                    item.GCR_B1_40 = commits.ToJson();
                }
                await Update(data, "GCR_B1_40");
            }
            return new MsReturned();
        }
    }
}
