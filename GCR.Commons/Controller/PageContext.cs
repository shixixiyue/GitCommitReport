

namespace GCR.Commons
{
    /// <summary>
    /// 当前页面上下文
    /// </summary>
    public static class PageContext
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHDUIControl(this IApplicationBuilder app)
        {
            _ServiceProvider = app.ApplicationServices;
            _ApplicationBuilder = app;
            SetVersion();
            app.Use(async (context, next) =>
            {
                var share = new SharedScope();
                context.Features.Set(share);
                share.Configure(app);
                await next();
            });
            return app;
        }

        #region 目录

        /// <summary>根目录</summary>
        public static string RootPath => (!(_ServiceProvider?.GetRequiredService(typeof(IWebHostEnvironment)) is IWebHostEnvironment requiredService) ? null : requiredService.ContentRootPath) ?? "";

        /// <summary>静态资源根目录（一般是根目录下的wwwroot子目录）</summary>
        public static string WebRootPath => (!(_ServiceProvider?.GetRequiredService(typeof(IWebHostEnvironment)) is IWebHostEnvironment requiredService) ? null : requiredService.WebRootPath) ?? "";

        /// <summary>
        /// 获取服务端可用的路径（比如将 ~/appsettings.json 映射为 c:\project1\appsettings.json）
        /// </summary>
        /// <param name="url">路径</param>
        /// <returns>服务端路径</returns>
        public static string MapPath(string url) => GetPath(url, RootPath);

        /// <summary>
        /// 获取服务端可用的静态资源路径（比如将 ~/res/menu.xml 映射为 c:\project1\wwwroot\res\menu.xml）
        /// </summary>
        /// <param name="url">路径</param>
        /// <returns>服务端路径</returns>
        public static string MapWebPath(string url) => GetPath(url, WebRootPath);

        private static string GetPath(string obj0, string? obj1)
        {
            string path2 = obj0 ?? string.Empty;
            if (path2.StartsWith("~"))
                path2 = path2.Substring(1);
            if (path2.StartsWith("/"))
                path2 = path2[1..];
            if (Path.DirectorySeparatorChar != '/')
                path2 = path2.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(obj1 ?? "", path2);
        }

        #endregion 目录

        #region Version 版本信息

        /// <summary>
        /// 当前版本号
        /// </summary>
        public static string Version { get; set; } = "";

        /// <summary>
        /// 版权
        /// </summary>
        public static string Copyright { get; set; } = "";

        /// <summary>
        /// 配置版本号
        /// </summary>
        private static void SetVersion()
        {
            //当前源
            Assembly? assem = Assembly.GetEntryAssembly();

            //版本号
            AssemblyName assemName = assem!.GetName();
            Version = assemName.Version!.ToString() ?? "";

            //注册信息
            AssemblyCopyrightAttribute? asmcpr = (AssemblyCopyrightAttribute?)Attribute.GetCustomAttribute(assem, typeof(AssemblyCopyrightAttribute));

            Copyright = asmcpr?.Copyright ?? "";
        }

        #endregion Version 版本信息

        #region IService 注入帮助

        /// <summary>
        /// 当前配置
        /// </summary>
        public static IServiceProvider? _ServiceProvider { get; set; }

        /// <summary>
        /// 当前配置
        /// </summary>
        public static IApplicationBuilder? _ApplicationBuilder { get; set; }

        /// <summary>
        /// 当前请求上下文
        /// </summary>
        public static HttpContext? Current => (_ServiceProvider?.GetRequiredService(typeof(IHttpContextAccessor)) as IHttpContextAccessor)?.HttpContext;

        /// <summary>
        ///
        /// </summary>
        public static string? sessionKey => typeof(DistributedSession).GetTypeInfo().GetField("_sessionKey", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(
        Current?.Session)?.ToString() ?? "";

        // var field = typeof(DistributedSession).GetTypeInfo().GetField("_sessionKey", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // var sessionKey = field.GetValue(
        //PageContext.Current.Session);

        /// <summary>
        /// 当有多个实现类，根据类名得到 具体那个
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static T GetServerByStr<T>(string type = "")
            where T : class
        {
            T server = default;
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    try
                    {
                        server = _ServiceProvider!.GetService<T>();
                    }
                    catch { }
                    if (server == null)
                    {
                        server = _ServiceProvider!.CreateScope().ServiceProvider.GetRequiredService<T>();
                    }
                    return server;
                }
                try
                {
                    var servers = _ServiceProvider!.GetServices<T>().ToList();
                    server = servers.FirstOrDefault(m => m.GetType().ToString().Contains(type));
                }
                catch
                {
                    var servers = Current?.RequestServices.GetServices<T>().ToList();
                    server = servers!.FirstOrDefault(m => m.GetType().ToString().Contains(type));
                }
            }
            catch { }
            return server!;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetServerByApp<T>()
        {
            if (_ApplicationBuilder == null && _ServiceProvider != null)
            {
                return _ServiceProvider!.GetService<T>() ?? _ServiceProvider!.CreateScope().ServiceProvider.GetService<T>();
            };
            var serviceScope = _ApplicationBuilder?.ApplicationServices.CreateScope();
            return serviceScope!.ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// 从上下文获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static dynamic GetServerByApp(Type T)
        {
            if (_ApplicationBuilder == null)
            {
                return _ServiceProvider!.GetService(T) ?? _ServiceProvider!.CreateScope().ServiceProvider.GetService(T);
            };
            var serviceScope = _ApplicationBuilder?.ApplicationServices.CreateScope();
            return serviceScope!.ServiceProvider.GetService(T);
        }

        /// <summary>
        /// 得到服务从上下文共享的Scope中，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static dynamic GetServerScope(Type T)
        {
            var share = Current.Features.Get<SharedScope>();
            return share.sharedScope.ServiceProvider.GetService(T);
        }

        /// <summary>
        /// 得到服务从上下文共享的Scope中，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetServerScope<T>()
        {
            var share = Current.Features.Get<SharedScope>();
            return share.sharedScope.ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// 得到服务集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T>? GetServers<T>()
            where T : class
        {
            try
            {
                try
                {
                    return _ServiceProvider!.GetServices<T>().ToList();
                }
                catch
                {
                    return Current!.RequestServices.GetServices<T>().ToList();
                }
            }
            catch { return default; }
        }

        #endregion IService 注入帮助

        #region EnvironmentName 判断调试模式

        /// <summary>
        /// 当前发布方式 Development 调试模式
        /// </summary>
        public static string EnvironmentName
        {
            get
            {
                try
                {
                    var host = GetServerByStr<IWebHostEnvironment>();
                    return host?.EnvironmentName ?? "";
                }
                catch { return "Development"; }
            }
        }

        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public static bool isDevelopment => EnvironmentName == "Development";

        #endregion EnvironmentName 判断调试模式
    }

    public class SharedScope
    {
        public IServiceScope? sharedScope;

        public void Configure(IApplicationBuilder app)

        {
            //var app = PageContext.GetServerByApp<IApplicationBuilder>();
            sharedScope = app?.ApplicationServices?.CreateScope();
        }
    }
}
