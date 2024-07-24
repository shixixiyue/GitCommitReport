namespace GCR.Commons
{
    /// <summary>
    /// 上传Excel帮助类
    /// <para>先使用 ReadFile 得到实例</para>
    /// </summary>
    public class UplodeExcelHelper : System.IAsyncDisposable
    {
        #region 公共方法 属性 实例

        /// <summary>
        /// Excel数据
        /// </summary>
        public DataTable data { get; set; }

        /// <summary>
        /// 新的文件名
        /// </summary>
        public string newfile { get; set; }

        /// <summary>
        /// 存储的地址
        /// </summary>
        public string outpath { get; set; }

        /// <summary>
        /// 文件流
        /// </summary>
        public IFormFile excelFile { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string fileType { get; set; }

        /// <summary>
        /// 上传Excel帮助类
        /// <para>先使用 ReadFile 得到实例</para>
        /// </summary>
        public UplodeExcelHelper()
        {
        }

        /// <summary>
        /// 上传Excel帮助类
        /// <para>先使用 ReadFile 得到实例</para>
        /// <para>使用await using (var up = UplodeExcelHelper.Get(guid))方式自动释放</para>
        /// </summary>
        public UplodeExcelHelper(string _guid)
        {
            guid = _guid;
            var up = (UplodeExcelHelper)cache.Get(guid);
            if (up != null)
            {
                this.data = up.data;
                this.hubid = up.hubid;
                this.newfile = up.newfile;
                this.outpath = up.outpath;
                this.excelFile = up.excelFile;
                this.fileType = up.fileType;
            }
        }

        /// <summary>
        /// 上传Excel帮助类
        /// <para>先使用 ReadFile 得到实例</para>
        /// </summary>
        /// <param name="_guid">生成的ID 用于缓存</param>
        /// <param name="dt">导入的数据</param>
        /// <param name="hubid">HubID</param>
        /// <param name="newfile">新文件名称</param>
        /// <param name="outpath">保存的文件路径</param>
        /// <param name="excelFile">原始文件流</param>
        /// <param name="fileType">文件的后缀</param>
        public UplodeExcelHelper(out string _guid, DataTable dt, string hubid, string newfile, string outpath, IFormFile excelFile, string fileType)
        {
            this.guid = Guid.NewGuid().ToString();
            _guid = guid;
            this.data = dt;
            this.hubid = hubid;
            this.newfile = newfile;
            this.outpath = outpath;
            this.excelFile = excelFile;
            this.fileType = fileType;
            cache.Set(_guid, this);
        }

        /// <summary>
        /// 数据插入到文本框
        /// </summary>
        public Task SetMessage(string msg)
        {
            return SignalRHelper.SendHubMessageAsync("UplodeMessage", msg, hubid);
        }

        /// <summary>
        /// 可以关闭了
        /// </summary>
        public async Task Done()
        {
            StringBuilder sb = new StringBuilder();
            //可以关闭
            sb.Append($"F.ui.ExcelUpWin.setClosable(true);");
            //await Clients.Client(toUserId).SendAsync("HubMessage", message);
            //前台使用new Function执行这些函数
            await SignalRHelper.SendHubMessageAsync("UplodeFun", sb.ToString(), hubid);
            cache.Remove(guid);
        }

        /// <summary>
        /// 可以关闭了 using释放时调用
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await Done();
        }

        #endregion 公共方法 属性 实例

        #region 静态方法

        /// <summary>
        /// 检查文件 并开始解析
        /// </summary>
        /// <param name="excelFile">文件流</param>
        /// <param name="excelName">文件名称</param>
        /// <param name="hubid">hubid</param>
        /// <param name="SheetIndex">读取的页签</param>
        /// <param name="HeaderRowIndex">读取的表</param>
        /// <returns>
        /// <para>msg 消息</para>
        /// <para>check 是否读取成功</para>
        /// </returns>
        public static async Task<(string msg, bool check)> ReadFile(IFormFile excelFile, string excelName, string hubid, int SheetIndex = 0, int HeaderRowIndex = 0)
        {
            (string fileType, bool checktype) = ValidateFileType(excelFile.FileName);
            if (excelFile != null && checktype)
            {
                //存储
                var (newfile, outpath) = await SaveExcel(excelFile, excelName);
                //拿到数据
                var dt = NPOIHelper.RenderDataTableFromExcel(outpath, $".{fileType}", SheetIndex, HeaderRowIndex);
                if (dt.Rows.Count == 0)
                {
                    return ("没有需要解析的数据", false);
                }
                else
                {
                    //数据,hubid,新文件名,路径,文件流,文件后缀
                    var up = new UplodeExcelHelper(out string _guid, dt, hubid, newfile, outpath, excelFile, fileType);
                    await up.FileChange();
                    await up.SetMessage("开始解析");
                    return ("开始解析", true);
                }
            }
            else
            {
                return ("上传格式错误", false);
            }
        }

        /// <summary>
        /// 保存Excel 文件 返回保存后的路径
        /// </summary>
        /// <param name="data"></param>
        /// <param name="excelName"></param>
        /// <returns>excel文件保存的相对路径，提供前端下载</returns>
        public static async Task<string> SaveExcel(byte[] data, string excelName)
        {
            string folder = DateTime.Now.ToString("yyyyMMdd");
            //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
            var uploadPath = PageContext.WebRootPath + "/UploadFile/" + folder + "/";
            excelName = excelName.Replace(":", "_").Replace(" ", "_").Replace("\\", "_").Replace("/", "_");
            string excelFileName = excelName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            //创建目录文件夹
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            //Excel的路径及名称
            string excelPath = uploadPath + excelFileName;

            //使用FileStream文件流来写入数据（传入参数为：文件所在路径，对文件的操作方式，对文件内数据的操作）
            //var fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (FileStream fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
            {
                await fs.WriteAsync(data, 0, data.Length);
            }

            //excel文件保存的相对路径，提供前端下载
            var relativePositioning = "/UploadFile/" + folder + "/" + excelFileName;
            return relativePositioning;
        }

        /// <summary>
        /// 保存Excel 文件 返回保存后的路径
        /// </summary>
        /// <param name="ExcelFile">文件流</param>
        /// <param name="excelName">文件名称</param>
        /// <returns>
        /// <para>excelName 新的文件名</para>
        /// <para>excelFileName 文件地址</para>
        /// </returns>
        public static async Task<(string newname, string path)> SaveExcel(IFormFile ExcelFile, string excelName)
        {
            excelName = excelName.Replace(":", "_").Replace(" ", "_").Replace("\\", "_").Replace("/", "_");
            excelName = DateTime.Now.Ticks.ToString() + "_" + excelName;
            string excelFileName = PageContext.MapWebPath("~/UploadFile/" + excelName);
            using (var stream = new FileStream(excelFileName, FileMode.Create))
            {
                await ExcelFile.CopyToAsync(stream);
            }
            return (excelName, excelFileName);
        }

        #endregion 静态方法

        #region 私有方法 属性

        /// <summary>
        /// 缓存
        /// </summary>
        protected static ICache cache => PageContext.GetServerByApp<ICache>();

        /// <summary>
        /// 当前的ID
        /// </summary>
        private string guid { set; get; }

        /// <summary>
        /// 当前的hubid
        /// </summary>
        private string hubid { set; get; }

        /// <summary>
        /// 将数据记录到缓存  返回新的帮助实体
        /// </summary>
        private async Task FileChange()
        {
            StringBuilder sb = new StringBuilder();
            //解析时不能关闭
            sb.Append($"F.ui.ExcelUpWin.setClosable(false);");
            //通知数据变化了
            sb.Append($"F.ui.ExcelUpWin.trigger('fileChange',['{guid}']);");
            //重置message
            sb.Append($"F.ui.UplodeMessage.setValue('');");
            sb.Append($"F.ui.UplodeMessage.textmsg=[];");
            //sb.Append($"console.log('F.ui.ExcelUpWin>', F.ui.ExcelUpWin);");
            await SignalRHelper.SendHubMessageAsync("UplodeFun", sb.ToString(), hubid);
        }

        /// <summary>
        /// 验证后缀
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static (string fileType, bool check) ValidateFileType(string fileName)
        {
            List<string> VALID_FILE_TYPES = new List<string> { "xlsx", "xls" };
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            if (VALID_FILE_TYPES.Contains(fileType))
            {
                return (fileType, true);
            }
            else
            {
                return (fileType, false);
            }
        }

        #endregion 私有方法 属性
    }
}
