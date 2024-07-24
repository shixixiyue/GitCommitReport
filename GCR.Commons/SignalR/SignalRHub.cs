

namespace GCR.Commons
{
    /// <summary>
    /// 消息总线
    /// </summary>
    public class SignalRHub : Hub, ISendHubMessageFun
    {
        /// <summary>
        /// 事件
        /// </summary>
        private readonly IMediator Event;

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="_Event"></param>
        public SignalRHub(IMediator _Event)
        {
            this.Event = _Event;
        }

        /// <summary>
        /// 链接成功
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            await Clients.Clients(connectionId).SendAsync("conncted", connectionId);
            await Event.Publish(
                new Connected_Event(connectionId, Connected_Event.Type.OnLine));
            await base.OnConnectedAsync();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Event.Publish(
                new Connected_Event(Context.ConnectionId, Connected_Event.Type.OutLine));
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 发送由前台触发 window.top.hub.connection.invoke
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [HubMethodName("send")]
        public async Task Send(string message = "", string toUserId = "")
        {
            if (toUserId == "")
            {
                await Clients.All.SendAsync("HubMessage", message);
            }
            else
            {
                await Clients.Client(toUserId).SendAsync("HubMessage", message);
            }
        }
    }
}
