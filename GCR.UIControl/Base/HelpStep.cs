
namespace GCR.UIControl
{
    /// <summary>
    ///
    /// </summary>
    public static class HelpStep
    {
        public static IServiceCollection AddGCRConfig
            (this IServiceCollection services, IConfiguration config)
        {
            HelpConfig ms = new();
            config.Bind(ms);
            GPTHelper.Projects = ms;

            IChangeToken token = config.GetReloadToken();
            ChangeToken.OnChange(() => config.GetReloadToken(), () =>
            {
                HelpConfig ms = new();
                config.Bind(ms);
                GPTHelper.Projects = ms;
                //输出
                Console.WriteLine(GPTHelper.Projects.ToJson());
            });
            Console.WriteLine(GPTHelper.Projects.ToJson());
            return services;
        }
    }
}
