namespace GCR.Model
{
    /// <summary>
    /// GitHub https://docs.github.com/en/rest/activity/watching?apiVersion=2022-11-28
    /// </summary>
    public class MsGitHubAPI
    {
        /// <summary>
        /// 所有仓库的API <br/>
        /// {0} 用户
        /// </summary>
        public static string ReposAPI = "/user/subscriptions";

        /// <summary>
        /// 所有仓库的分支 API <br/>
        /// {0} 仓库路径 full_name
        /// </summary>
        public static string BranchesAPI = "/repos/{0}/branches";

        /// <summary>
        /// 所有提交的API<br/>
        /// {0} owner 仓库路径 full_name <br/>
        /// {1} repo 分支
        /// </summary>
        public static string CommitAPI = "/repos/{0}/commits?sha={1}";

        /// <summary>
        /// 用户信息
        /// </summary>
        public static string UserAPI = "/user";
    }
}
