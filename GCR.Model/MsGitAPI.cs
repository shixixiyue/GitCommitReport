namespace GCR.Model
{
    /// <summary>
    /// 仓库
    /// </summary>
    public class GitRepo
    {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 仓库名
        /// </summary>
        public string full_name { get; set; }
    }

    /// <summary>
    /// 分支
    /// </summary>
    public class Branche
    {
        /// <summary>
        /// 分支
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        public GitRepo gitRepo { get; set; }
    }

    /// <summary>
    /// 提交记录
    /// </summary>
    public class GitCommit
    {
        /// <summary>
        /// 分支
        /// </summary>
        public Branche branche { get; set; }

        /// <summary>
        /// 提交信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        public string committer { get; set; }

        /// <summary>
        /// SHA
        /// </summary>
        public string sha { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string created { get; set; }
    }
}
