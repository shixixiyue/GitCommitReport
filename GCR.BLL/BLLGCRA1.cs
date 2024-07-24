using GCR.Model;

namespace GCR.BLL
{
    /// <summary>
    /// 配置
    /// </summary>
    public class BLLGCRA1 : SqlSugarHelp<MsGCR_A1>
    {
        /// <summary>
        /// 得到有效的配置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MsGCR_A1> GetUseSetUp()
        {
            var msq = new MsQuery();
            //msq.AddQuermodel(nameof(MsGCR_A1.GCR_A1_70), "1");
            msq.strorder = " GCR_A1_UPTIME desc";
            var ms = await GetMs(msq);
            return ms;
        }
    }

    public static class BLLGCRA1Ex
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static async Task<MsReturned> APITest(this MsGCR_A1 ms)
        {
            MsReturned b = ms.GCR_A1_10 switch
            {
                "1" => b = await BLLGiteaAPI.APITest(ms),
                "2" => b = await BLLGiteeAPI.APITest(ms),
                "3" => b = await BLLGitHubAPI.APITest(ms),
                _ => new MsReturned(),
            };
            if (b.MsRbool) { b.strMS = "测试成功"; }
            return b;
        }

        /// <summary>
        /// 得到仓库
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static async Task<List<GitRepo>> GetRepos(this MsGCR_A1 ms)
        {
            List<GitRepo> res = ms.GCR_A1_10 switch
            {
                "1" => res = await BLLGiteaAPI.GetRepos(ms),
                "2" => res = await BLLGiteeAPI.GetRepos(ms),
                "3" => res = await BLLGitHubAPI.GetRepos(ms),
                _ => new List<GitRepo>(),
            };
            return res;
        }

        /// <summary>
        /// 得到仓库提交
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static async Task<List<GitCommit>> GetCommits(this MsGCR_A1 ms, string start, string end)
        {
            List<GitCommit> res = ms.GCR_A1_10 switch
            {
                "1" => res = await BLLGiteaAPI.GetCommits(ms, start, end),
                "2" => res = await BLLGiteeAPI.GetCommits(ms, start, end),
                "3" => res = await BLLGitHubAPI.GetCommits(ms, start, end),
                _ => new List<GitCommit>(),
            };
            return res;
        }
    }
}
