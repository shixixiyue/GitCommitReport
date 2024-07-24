using Microsoft.Extensions.Hosting;

namespace GCR.Commons
{
    public class BackgroundWorks : BackgroundService
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="Factory"></param>
        public BackgroundWorks(IServiceProvider Instance, IServiceScopeFactory Factory)
        {
            if (PageContext._ServiceProvider == null)
            {
                PageContext._ServiceProvider = Instance;
            }
            /*如果拿不到 依赖可以使用 以下的一个
            *Instance.CreateScope().ServiceProvider.GetRequiredService<T>();
            *Factory.CreateScope().ServiceProvider.GetRequiredService<T>();
            **/
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //InitSysTask();
        }

        /// <summary>
        /// 定时任务
        /// </summary>
        public void InitSysTask()
        {
            //var bllsys23 = HD.Commons.PageContext.GetServerByApp<BLLSystem_23>();
            //bllsys23?.InitSysTask();
        }
    }
}
