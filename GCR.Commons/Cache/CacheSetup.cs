using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCR.Commons
{
    /// <summary>
    /// 注册缓存
    /// </summary>
    public static class CacheSetup
    {
        /// <summary>
        /// 缓存帮助类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddCacheSetup(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICache>(new MemoryCacheHelper());
        }
    }
}
