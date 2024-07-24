using GCR.BLL;
using GCR.Model;
using GCR.UIControl;

namespace GCR.Web.Pages
{
    public partial class EditNewModel : BaseModel
    {
        private readonly BLLGCRB1 bLLGCRB1;

        public EditNewModel(BLLGCRB1 bLLGCRB1)
        {
            this.bLLGCRB1 = bLLGCRB1;
        }

        protected async Task Page_LoadAsync(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                labtoday.Text = DateTime.Now.ToString("yyyy-MM-dd");
                var ms = await bLLGCRB1.GetNew(labtoday.Text);
                FormNew.SetData(ms);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected async Task OnSaveClickAsync(object sender, EventArgs e)
        {
            var msb1 = FormNew.GetData<MsGCR_B1>();
            var msr = await bLLGCRB1.Save(msb1);
            ShowNotify(msr.strMS);
            HideWindow();
        }
    }
}