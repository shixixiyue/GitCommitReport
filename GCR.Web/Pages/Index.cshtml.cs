using GCR.BLL;
using GCR.BLL.GPT;
using GCR.Commons;
using GCR.Model;
using GCR.UIControl;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace GCR.Web.Pages
{
    public partial class IndexModel : BaseModel
    {
        private readonly BLLGCRB1 bLLGCRB1;
        private readonly BLLGCRA1 bLLGCRA1;

        public IndexModel(BLLGCRB1 bLLGCRB1, BLLGCRA1 bLLGCRA1)
        {
            this.bLLGCRB1 = bLLGCRB1;
            this.bLLGCRA1 = bLLGCRA1;
        }

        protected async Task Page_LoadAsync(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await Grid1_ReLoadAsync(null, null);
            }
        }

        protected async Task Grid1_ReLoadAsync(object sender, EventArgs e)
        {
            Grid1.DataSource = await LoadData();
            Grid1.DataBind(true);
        }

        public async Task<List<MsGCR_B1>> LoadData()
        {
            var msq = new MsQuery();
            return await bLLGCRB1.GetList(msq);
        }

        protected async Task Grid1_DeleteAsync(object sender, EventArgs e)
        {
            var id = Request.Form["id"];
            var msr = bLLGCRB1.DeleteByKey(id).Result;
            ShowNotify(msr.strMS);
        }

        protected async Task Grid2_ReLoadAsync(object sender, EventArgs e)
        {
            var id = Request.Form["id"];
            var msr = await bLLGCRB1.GetMsByKey(id.ToString());
            if (string.IsNullOrEmpty(msr.GCR_B1_40))
            {
                Grid2.DataSource = new List<GitCommit>();
            }
            else
            {
                var list = JToken.Parse(msr.GCR_B1_40)?.ToObject<List<GitCommit>>() ?? new List<GitCommit>();
                Grid2.DataSource = list;
            }
            Grid2.DataBind();
            SetText(msr.GCR_B1_50);
        }

        private void SetText(string str)
        {
            if (string.IsNullOrEmpty(str)) return;
            string js = $"F.ui.codePanel.SetText(`{str.Replace("`", "\\`")}`)";
            RegisterStartupScript(js);
        }

        protected async Task OnCommitRefreshAsync(object sender, EventArgs e)
        {
            var id = Grid1.SelectedRowID;
            var msa1 = await bLLGCRA1.GetUseSetUp();
            var msb1 = await bLLGCRB1.GetMsByKey(id);
            var updata = new List<MsGCR_B1>();
            var commits = await msa1.GetCommits(msb1.GCR_B1_20, msb1.GCR_B1_30);
            msb1.GCR_B1_40 = commits.ToJson();
            await bLLGCRB1.Update(new List<MsGCR_B1> { msb1 }, "GCR_B1_40");
            if (string.IsNullOrEmpty(msb1.GCR_B1_40))
            {
                Grid2.DataSource = new List<GitCommit>();
            }
            else
            {
                var list = JToken.Parse(msb1.GCR_B1_40)?.ToObject<List<GitCommit>>() ?? new List<GitCommit>();
                Grid2.DataSource = list;
            }

            Grid2.DataBind();
        }

        protected async Task OnCommitReportAsync(object sender, EventArgs e)
        {
            var id = Grid1.SelectedRowID;
            var msa1 = await bLLGCRA1.GetUseSetUp();
            var msb1 = await bLLGCRB1.GetMsByKey(id);
            var reprotmask = new ReportMask(msa1.GCR_A1_80);
            reprotmask.adduser(msb1.GCR_B1_40);
            var res = await reprotmask.GetResult();
            msb1.GCR_B1_50 = res;
            await bLLGCRB1.Update(new List<MsGCR_B1> { msb1 }, "GCR_B1_50");
            SetText(res);
        }
    }
}