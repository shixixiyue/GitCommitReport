using log4net;
using log4net.Config;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GCR.BLL
{
    public static class BLLHelp
    {
        /// <summary>
		/// 注册数据库帮助类
		/// </summary>
		/// <param name="services"></param>
		public static void AddSystemBP(this IServiceCollection services, IConfiguration Configuration)
        {
            //MediatR
            var Assemblylist = new List<Assembly>() {
                Assembly.GetExecutingAssembly(),
                typeof(SignalRHub).Assembly
            };
            services.AddSignalR();
            services.AddMediatR(Assemblylist.ToArray());
            services.AddLog4Setup(Configuration, (repository) =>
            {
                repository = LogManager.CreateRepository("");//需要获取日志的仓库名，也就是你的当然项目名
                XmlConfigurator.Configure(repository, new FileInfo("Log4net.config"));//指定配置文件，
                return repository;
            });
            services.AddHostedService<BackgroundWorks>();
            services.AddCacheSetup(Configuration);//缓存帮助类
            services.AddSqlsugarSetup(Configuration);//数据库帮助类

            var allAssemblies = Assembly.GetExecutingAssembly().GetTypes();
            var blls = allAssemblies.Where(item => item.IsClass
            && item.Namespace != null
            && item.IsGenericSubclassOf(typeof(SqlSugarHelp<>))).ToList();
            blls.ForEach(type =>
            {
                services.AddSingleton(type);
            });
            var l = allAssemblies.ToList().Select(m => m);
            services.AddTransient(typeof(Lazy<>), typeof(LazilyResolved<>));
        }

        /// <summary>
        /// 是否继承了指定类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="superType"></param>
        /// <returns></returns>
        public static bool IsGenericSubclassOf(this Type type, Type superType)
        {
            if (type.BaseType != null
                && !type.BaseType.Equals(typeof(object))
                && type.BaseType.IsGenericType)
            {
                if (type.BaseType.GetGenericTypeDefinition().Equals(superType))
                {
                    return true;
                }
                return type.BaseType.IsGenericSubclassOf(superType);
            }

            return false;
        }
    }

    /// <summary>
    /// 懒加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazilyResolved<T> : Lazy<T>
    {
        public LazilyResolved(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<T>)
        {
        }
    }
}
