using Flurl.Http;
using GCR.Model;
using Newtonsoft.Json.Linq;
using System.Web;

namespace GCR.BLL
{
    public class BLLGiteeAPI
    {
        public static async Task<MsReturned> APITest(MsGCR_A1 ms)
        {
            var msr = new MsReturned() { MsRbool = false };
            var repos = await GetRepos(ms);
            msr.MsRbool = repos.Any();
            return msr;
        }

        public static async Task<List<GitRepo>> GetRepos(MsGCR_A1 ms)
        {
            if (string.IsNullOrEmpty(ms.GCR_A1_20)
                            || string.IsNullOrEmpty(ms.GCR_A1_30)
                            || string.IsNullOrEmpty(ms.GCR_A1_40)) { return new List<GitRepo>(); }
            var url = $"{ms.GCR_A1_20}{string.Format(MsGiteeAPI.ReposAPI, ms.GCR_A1_30)}";
            var response = await url.WithHeader("Authorization", $"token {ms.GCR_A1_40}").GetStringAsync();
            var repos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GitRepo>>(response);
            return repos;
        }

        /// <summary>
        /// 得到仓库提交
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static async Task<List<GitCommit>> GetCommits(MsGCR_A1 ms, string start, string end)
        {
            /*
             * 得到所有的仓库
             * 循环得到仓库分支
             * 循环得到仓库分支的提交
             */
            var repos = await GetRepos(ms);
            var branches = await GetBranches(ms, repos);
            var commits = await GetCommit(ms, branches);

            //通过时间筛选 start end 之间的提交与 commits.created 比较 注意string要转换为DateTime
            var filteredCommits = commits.Where(item => item.committer == ms.GCR_A1_30)
                  .Where(item =>
                  DateTime.Parse(item.created).Date >= DateTime.Parse(start).Date
                  && DateTime.Parse(item.created).Date <= DateTime.Parse(end).Date)
                  .ToList();
            return filteredCommits;
        }

        /// <summary>
        /// 得到仓库的分支
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="repos"></param>
        /// <returns></returns>
        public static async Task<List<Branche>> GetBranches(MsGCR_A1 ms, List<GitRepo> repos)
        {
            List<Branche> res = new List<Branche>();
            if (string.IsNullOrEmpty(ms.GCR_A1_20)
                            || string.IsNullOrEmpty(ms.GCR_A1_30)
                            || string.IsNullOrEmpty(ms.GCR_A1_40)) { return new List<Branche>(); }

            foreach (var repo in repos)
            {
                var url = $"{ms.GCR_A1_20}{string.Format(MsGiteeAPI.BranchesAPI, repo.full_name)}";
                var response = await url.WithHeader("Authorization", $"token {ms.GCR_A1_40}").GetStringAsync();
                var branches = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Branche>>(response);
                branches.ForEach(item => { item.gitRepo = repo; });
                res.AddRange(branches);
            }

            return res;
        }

        /// <summary>
        /// 得到仓库提交
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="branches"></param>
        /// <returns></returns>
        public static async Task<List<GitCommit>> GetCommit(MsGCR_A1 ms, List<Branche> branches)
        {
            List<GitCommit> gitCommits = new();
            if (string.IsNullOrEmpty(ms.GCR_A1_20)
                            || string.IsNullOrEmpty(ms.GCR_A1_30)
                            || string.IsNullOrEmpty(ms.GCR_A1_40)) { return new List<GitCommit>(); }
            foreach (var branche in branches)
            {
                var sha = HttpUtility.UrlEncode(branche.name);
                var url = $"{ms.GCR_A1_20}{string.Format(MsGiteeAPI.CommitAPI, branche.gitRepo.full_name, sha)}";
                var response = await url.WithHeader("Authorization", $"token {ms.GCR_A1_40}").GetStringAsync();
                var commitData = JToken.Parse(response);
                foreach (var item in commitData)
                {
                    try
                    {
                        var commit = new GitCommit
                        {
                            branche = branche,

                            message = item["commit"]?["message"]?.ToString(),
                            committer = item["committer"]?["login"]?.ToString(),
                            sha = item["sha"]?.ToString(),
                            created = item["commit"]?["committer"]?["date"]?.ToString()
                        };
                        gitCommits.Add(commit);
                    }
                    catch (Exception e) { }
                }
            }
            return gitCommits;
        }
    }
}
