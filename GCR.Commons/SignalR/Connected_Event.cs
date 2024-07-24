namespace GCR.Commons
{
    /// <summary>
    /// 连接到SignalR的事件
    /// </summary>
    public class Connected_Event : INotification
    {
        /// <summary>
        /// 当前的链接ID
        /// </summary>
        public readonly string connectionId;

        /// <summary>
        ///
        /// </summary>
        public readonly Type type;

        /// <summary>
        /// 类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 在线
            /// </summary>
            OnLine,

            /// <summary>
            /// 掉线
            /// </summary>
            OutLine
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="_connectionId"></param>
        /// <param name="_type"></param>
        public Connected_Event(string _connectionId, Type _type)
        {
            this.connectionId = _connectionId;
            this.type = _type;
        }
    }
}
