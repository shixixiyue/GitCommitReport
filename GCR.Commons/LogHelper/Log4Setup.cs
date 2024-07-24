

namespace GCR.Commons
{
    /// <summary>
    /// 日志配置
    /// </summary>
    public static class Log4Setup
    {
        /// <summary>
        /// log4net 仓储库
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        /// <summary>
        /// 缓存帮助类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="fun"></param>
        public static void AddLog4Setup(this IServiceCollection services, IConfiguration config, Func<ILoggerRepository, ILoggerRepository> fun)
        {
            repository = fun(repository);
            services.AddSingleton<ILoggerHelper, LogHelper>();
        }

        /// <summary>
        ///
        /// </summary>
        public static ILoggerHelper? _log => PageContext.GetServerByStr<ILoggerHelper>();
    }
}
