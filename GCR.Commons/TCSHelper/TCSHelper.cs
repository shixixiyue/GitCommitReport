namespace GCR.Commons
{
    /// <summary>
    /// 自定义异步任务
    /// </summary>
    public class TCSHelper
    {
        /// <summary>
        /// 任务帮助类
        /// </summary>
        /// <param name="_guid">任务ID</param>
        public TCSHelper(string _guid)
        {
            guid = _guid;
            TCS = new TaskCompletionSource<MsReturned>();
            mre = new ManualResetEvent(false);
        }

        /// <summary>
        /// 任务帮助类
        /// </summary>
        /// <param name="_guid"></param>
        public TCSHelper(out string _guid)
        {
            guid = Guid.NewGuid().ToString();
            _guid = guid;
            TCS = new TaskCompletionSource<MsReturned>();
            mre = new ManualResetEvent(false);
        }

        /// <summary>
        /// 缓存
        /// </summary>
        protected static MemoryCacheHelper cache = new MemoryCacheHelper();

        /// <summary>
        /// 延迟Task类
        /// </summary>
        private TaskCompletionSource<MsReturned> TCS { get; set; }

        /// <summary>
        /// Task
        /// </summary>
        public Task<MsReturned> Task
        { get { return TCS.Task; } }

        /// <summary>
        /// 超时
        /// </summary>
        private ManualResetEvent mre { get; set; }

        /// <summary>
        /// 当前的任务ID
        /// </summary>
        private string guid { set; get; }

        /// <summary>
        /// 超时
        /// </summary>
        /// <param name="timeoutMillis">毫秒 为0就是不超时</param>
        /// <param name="func">超时回调</param>
        private void StartWithTimeout(int timeoutMillis, Action<MsReturned> func)
        {
            mre.Reset();
            System.Threading.Tasks.Task.Run(() =>
            {
                if (timeoutMillis != 0)
                {
                    bool wasStopped = mre.WaitOne(timeoutMillis);
                    if (!wasStopped)
                    {
                        MsReturned msr = new MsReturned()
                        {
                            strMsId = guid,
                            strMS = "任务超时"
                        };
                        TrySetResult(msr);
                        func?.Invoke(msr);
                    }
                }
            });
        }

        /// <summary>
        /// 停止超时线程
        /// </summary>
        private void StopTimeout()
        {
            mre.Set();
        }

        /// <summary>
        /// 尝试回发改变状态，并且释放资源
        /// </summary>
        /// <param name="msr"></param>
        public void TrySetResult(MsReturned msr)
        {
            if (TCS.TrySetResult(msr))
            {
                cache?.Remove(guid);
                StopTimeout();
                Dispose();
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            mre?.Dispose();
            TCS?.Task?.Dispose();
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="timeoutMillis">超时时间 为0就是不超时</param>
        /// <param name="func">超时后方法</param>
        public void Start(int timeoutMillis = 118000, Action<MsReturned> func = null)
        {
            StartWithTimeout(timeoutMillis, func);
            cache.Set(guid, this);
        }

        /// <summary>
        /// 通过GUID得到缓存的任务
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>缓存的任务如果没找到，则创建新任务</returns>
        public static TCSHelper Get(string guid)
        {
            if (cache.Exists(guid))
            {
                return (TCSHelper)cache.Get(guid);
            }
            else
            {
                return new TCSHelper(guid);
            }
        }
    }
}
