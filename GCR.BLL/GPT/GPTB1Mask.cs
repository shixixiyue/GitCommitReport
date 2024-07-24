namespace GCR.BLL.GPT
{
    public class GPTB1Mask : MaskBase
    {
        private const string system = "我给你当前的日期，你计算该日期的当前年份，是当年的第几周，本周的开始日期和结束日期，返回给我一段json文本，注意是文本不要输出markdown格式，格式如下 {GCR_B1_10:'{当前年}第N周',GCR_B1_20:'开始日期',GCR_B1_30:'结束日期'}，日期格式为 yyyy-MM-dd，请检查文本格式，不要输出其他内容；";

        public GPTB1Mask()
        {
            addsystem(system);
        }
    }
}
