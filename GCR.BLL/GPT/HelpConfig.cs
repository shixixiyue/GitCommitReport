namespace GCR.BLL
{
    /// <summary>
    ///
    /// </summary>
    public class HelpConfig
    {
        /// <summary>
        /// 代理
        /// </summary>
        public string url { get; set; } = "https://api.chatanywhere.com.cn/v1/chat/completions";

        /// <summary>
        /// 密钥
        /// </summary>
        public string key { get; set; } = string.Empty;

        /// <summary>
        /// 模型
        /// </summary>
        public string model { get; set; } = "gpt-3.5-turbo-16k-0613";//gpt-3.5-turbo|gpt-3.5-turbo-16k-0613
    }
}
