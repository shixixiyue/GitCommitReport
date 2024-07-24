using GCR.BLL;
using GCR.Model;
using GCR.UIControl;

namespace GCR.Web.Pages
{
    public partial class SetUpModel : BaseModel
    {
        private readonly BLLGCRA1 bLLGCRA1;

        public SetUpModel(BLLGCRA1 bLLGCRA1)
        {
            this.bLLGCRA1 = bLLGCRA1;
        }

        protected async Task Page_LoadAsync(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var msa1 = await bLLGCRA1.GetUseSetUp();
                Form1.SetData(msa1);
                if (msa1.GCR_A1_50 == "1")
                {
                    GCR_A1_60.Enabled = false;
                }
                else
                {
                    GCR_A1_60.Enabled = true;
                }
            }
        }

        //选择平台
        protected async Task OnA110Change(object sender, EventArgs e)
        {
            var msa1 = await bLLGCRA1.GetMsByKey(GCR_A1_10.SelectedValue);
            Form1.SetData(msa1);
        }
        protected async Task OnSaveClickAsync(object sender, EventArgs e)
        {
            //var id = Request.Form["id"];
            var msa1 = Form1.GetData<MsGCR_A1>();
            msa1.GCR_A1_AUTOID = Convert.ToInt32(msa1.GCR_A1_10);
            msa1.GCR_A1_70 = "1";
            msa1.GCR_A1_UPTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var msr = await bLLGCRA1.Save(msa1);
            ShowNotify(msr.strMS);
        }

        protected async Task OnTestClickAsync(object sender, EventArgs e)
        {
            var msa1 = Form1.GetData<MsGCR_A1>();
            var msr = await msa1.APITest();
            ShowNotify(msr.strMS);

            //await RenderReposAsync(null, null);
        }

        protected async Task RenderReposAsync(object sender, EventArgs e)
        {
            var msa1 = Form1.GetData<MsGCR_A1>();
            var repos = await msa1.GetRepos();
            GCR_A1_60.DataSource = repos;
            GCR_A1_60.DataTextField = "name";
            GCR_A1_60.DataValueField = "id";
            GCR_A1_60.DataBind();
        }

        /// <summary>
        /// 项目变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnA150ChangeAsync(object sender, EventArgs e)
        {
            if (GCR_A1_50.SelectedValue == "1")
            {
                GCR_A1_60.Enabled = false;
            }
            else
            {
                GCR_A1_60.Enabled = true;
            }
        }
    }
}