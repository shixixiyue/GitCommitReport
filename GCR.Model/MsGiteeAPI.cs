namespace GCR.Model
{
    /// <summary>
    /// Gitee https://gitee.com/api/v5/swagger
    /// </summary>
    public class MsGiteeAPI
    {
        /// <summary>
        /// 所有仓库的API <br/>
        /// {0} 用户
        /// </summary>
        public static string ReposAPI = "/api/v5/users/{0}/subscriptions";

        /// <summary>
        /// 所有仓库的分支 API <br/>
        /// {0} 仓库路径 full_name
        /// </summary>
        public static string BranchesAPI = "/api/v5/repos/{0}/branches";

        /// <summary>
        /// 所有提交的API<br/>
        /// {0} owner 仓库路径 full_name <br/>
        /// {1} repo 分支
        /// </summary>
        public static string CommitAPI = "/api/v5/repos/{0}/commits?sha={1}";

        /// <summary>
        /// 用户信息
        /// </summary>
        public static string UserAPI = "/api/v5/user";
    }
}
