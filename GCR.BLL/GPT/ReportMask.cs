namespace GCR.BLL.GPT
{
    public class ReportMask : MaskBase
    {
        private const string system = "你是一个专业产品经理和开发人员，通过Git提交记录，你可以总结生成一份周报。请以markdown格式返回报告。不用写提交的记录ID ，对项目做总结即可;在提交记录前增加前缀；只写项目名称，忽略路径，明细改为编号，把提交日期写到每项的最后。可以参考以下格式: 周报\r\n\r\n日期：2024年7月15日 - 2024年7月19日\r\n提交者：shiyue\r\nHD_System\r\n\r\n    命名优化: 对 FormAjaxEx 进行了命名修改，提升了代码的可读性和一致性。\r\n    异步处理: 实现了多个异步方法，如 GetSelectedRowsAsync, OnClickAsync, 和 ListenerAsync，增强了系统的异步处理能力，提高了用户交互体验。\r\n    数据操作: 新增了 DeleteByKey 方法，用于通过键值删除数据，并多次优化了 FGetData 方法，提升了数据获取的效率和稳定性。\r\n    功能增强: 增加了 FView 用于 FPageReady 的回调，并设置了超时机制，优化了页面加载流程。\r\n    问题修复: 尝试修复了 SignalR 报错问题，提升了系统的稳定性。\r\n\r\nPCBA_MES\r\n\r\n    问题修复: 紧急修复了 SignalR 报错问题，解决系统关键性故障。\r\n\r\nCQS\r\n\r\n    新增功能: 实现了加载并选择人员的功能，增强系统用户管理能力。\r\n    新增功能: 开发了业务员管理页面，提升系统业务管理效率。\r\n";

        public ReportMask(string sys)
        {
            addsystem(system + sys);
        }
    }
}
