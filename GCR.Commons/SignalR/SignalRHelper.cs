
namespace GCR.Commons
{
    /// <summary>
    ///
    /// </summary>
    public static class SignalRHelper
    {
        /// <summary>
        /// 向在线客户端发送消息 如果ID为空则向全局发送
        /// </summary>
        /// <returns></returns>
        public static void SendHubMessage(string evname, string msg = "", string id = "")
        {
            var Hub = PageContext.GetServerByApp<IHubContext<SignalRHub>>();
            var jsonmsg = new JObject();
            jsonmsg.Add("evname", evname);
            jsonmsg.Add("msg", msg);
            if (id == "")
            {
                Hub.Clients.All.SendAsync("HubMessage", jsonmsg.ToString());
            }
            else
            {
                Hub.Clients.Client(id).SendAsync("HubMessage", jsonmsg.ToString());
            }
        }

        /// <summary>
        /// 向在线客户端发送消息
        /// </summary>
        /// <returns></returns>
        public static async Task SendHubMessageAsync(string evname, string msg, string id = "")
        {
            var Hub = PageContext.GetServerByApp<IHubContext<SignalRHub>>();
            //TEST实现该接口
            var jsonmsg = new JObject();
            jsonmsg.Add("evname", evname);
            jsonmsg.Add("msg", msg);
            if (string.IsNullOrEmpty(id))
            {
                await Hub.Clients.All.SendAsync("HubMessage", jsonmsg.ToString());
            }
            else
            {
                await Hub.Clients.Client(id).SendAsync("HubMessage", jsonmsg.ToString());
            }
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="evname">事件</param>
        /// <param name="hubid">本地消息总线的ID</param>
        /// <returns></returns>
        public static Func<string, Task> SendHubMessageFun(string evname, string hubid = "")
        {
            var isend = PageContext.GetServerByApp<ISendHubMessageFun>();
            return isend.send(evname, hubid);
        }

        /// <summary>
        /// 得到客户端结果 返回一个json格式
        /// </summary>
        /// <param name="evname"></param>
        /// <param name="msg"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        //public static Task<JToken> GetHubMessage(string evname, string msg, string id = "")
        //{
        //	return GetHubMessage(evname, msg, () => (JToken)new JObject(), id);
        //}

        /// <summary>
        /// 得到客户端结果 返回一个<see cref="T"/>格式
        /// </summary>
        /// <param name="evname"></param>
        /// <param name="msg"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<T> GetHubMessage<T>(string evname, string msg, Func<dynamic, T> fun, string id = "")
        {
            var res = await GetHubMessage(evname, msg, id);
            return (T)fun(res);
        }

        /// <summary>
        /// 得到客户端结果 返回一个<see cref="T"/>格式
        /// </summary>
        /// <param name="evname"></param>
        /// <param name="msg"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<dynamic> GetHubMessage(string evname, string msg, string id = "")
        {
            //尝试拿一个hubid
            if (string.IsNullOrEmpty(id)) { id = GetHubID(); }
            if (string.IsNullOrEmpty(id)) { return default; }
            var Hub = PageContext.GetServerByApp<IHubContext<SignalRHub>>();
            var msr = await Hub.Clients.Client(id).InvokeAsync<dynamic>(evname, msg, new CancellationToken());
            return msr ?? default;
        }

        /// <summary>
        /// 直接在前端执行js代码 并且返回结果
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="id">如果省略将执行本地</param>
        /// <returns></returns>
        public static async Task ExecuteScript(string fun, string id = "")
        {
            if (string.IsNullOrEmpty(id)) { id = GetHubID(); }
            var Hub = PageContext.GetServerByApp<IHubContext<SignalRHub>>();
            if (string.IsNullOrEmpty(id))
            {
                await Hub.Clients.All.SendAsync("ExecuteScript", fun.ToString());
            }
            else
            {
                await Hub.Clients.Client(id).SendAsync("ExecuteScript", fun.ToString());
            }
        }

        /// <summary>
        /// 直接在前端执行js代码 并且返回结果
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="id">如果省略将执行本地</param>
        /// <returns></returns>
        public static async Task ExecuteScriptAsync(string fun, string id = "")
        {
            if (string.IsNullOrEmpty(id)) { id = GetHubID(); }
            var Hub = PageContext.GetServerByApp<IHubContext<SignalRHub>>();
            if (string.IsNullOrEmpty(id))
            {
                await Hub.Clients.All.SendAsync("ExecuteScriptAsync", fun.ToString());
            }
            else
            {
                await Hub.Clients.Client(id).SendAsync("ExecuteScriptAsync", fun.ToString());
            }
        }

        /// <summary>
        /// 尝试得到本次的hubid
        /// </summary>
        /// <returns></returns>
        public static string GetHubID()
        {
            var hubid = string.Empty;
            var head = PageContext.Current!.Request.Headers;
            if (head.ContainsKey("hubid"))
            {
                hubid = head["hubid"];
            }
            else if (PageContext.Current!.Request.Form.ContainsKey("hubid"))
            {
                hubid = PageContext.Current!.Request.Form["hubid"];
            }
            return hubid;
        }
    }

    /// <summary>
    /// 用于测试拦截 模拟返回
    /// </summary>
    public interface ISendHubMessageFun
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="evname"></param>
        /// <param name="hubid"></param>
        /// <returns></returns>
        Func<string, Task> send(string evname, string hubid = "")
        {
            return new Func<string, Task>((msg) =>
            {
                Task.Delay(10).Wait();
                return SignalRHelper.SendHubMessageAsync(evname, msg, hubid);
            });
        }
    }
}
