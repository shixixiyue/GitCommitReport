

namespace GCR.Commons
{
    /// <summary>
    /// 重写参数
    /// <para>如果参数名 以ms 或 listms 开头 则尝试转换为 同名的基于 MsBase的类</para>
    /// </summary>
    public class ParameterOverwrite : ActionFilterAttribute
    {
        /// <summary>
        /// 按钮之前回发
        /// 通过  filterContext.Result 可以阻止提交
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MsBaseOverwrite(filterContext);
            UplodeExcelOverwrite(filterContext);
        }

        /// <summary>
        /// 读取  uplodeExcelGuid 转成 UplodeExcelHelper
        /// </summary>
        /// <param name="filterContext"></param>
        private static void UplodeExcelOverwrite(ActionExecutingContext filterContext)
        {
            //接收的项
            var backpars = filterContext.ActionDescriptor.Parameters;
            //
            var ismsbase = new Func<IFormCollection, ParameterDescriptor, bool>((Form, item) =>
            {
                var type = ((ControllerParameterDescriptor)item).ParameterInfo.ParameterType;

                var msbase = Form.ContainsKey("uplodeExcelGuid") && type == typeof(UplodeExcelHelper);

                return msbase;
            });

            try
            {
                //当前传入的表单项
                var Form = filterContext.HttpContext.Request?.Form;

                //转成UplodeExcelHelper模型
                backpars.Where((item) => ismsbase(Form!, item)).ToList().ForEach(item =>
                {
                    var type = ((ControllerParameterDescriptor)item).ParameterInfo.ParameterType;//参数类型
                    try
                    {
                        var up = new UplodeExcelHelper(Form!["uplodeExcelGuid"]);//转化后的值

                        //赋值
                        filterContext.ActionArguments[item.Name] = up;
                    }
                    catch { }
                });
            }
            catch { }
        }

        /// <summary>
        /// 基于MsBase 并且 以ms或listms开头的参数的类型转换
        /// </summary>
        /// <param name="filterContext">当前提交</param>
        private static void MsBaseOverwrite(ActionExecutingContext filterContext)
        {
            var backpars = filterContext.ActionDescriptor.Parameters;
            //如果接收值在传入的表单中 并且继承自MsBase 并且以ms开头 也可能是泛型 以listms开头的
            var ismsbase = new Func<IFormCollection, ParameterDescriptor, bool>((Form, item) =>
            {
                var type = ((ControllerParameterDescriptor)item).ParameterInfo.ParameterType;

                var msbase = Form.ContainsKey(item.Name) && type.IsSubclassOf(typeof(MsBase)) && item.Name.IndexOf("ms") >= 0;

                var listms = false;
                // && type.IsAssignableFrom(typeof(List<>))
                if (Form.ContainsKey(item.Name) && item.Name.IndexOf("listms") >= 0 && type.IsGenericType)
                {
                    Type[] genericTypes = type.GetGenericArguments();
                    listms = genericTypes.Any(t => t.IsSubclassOf(typeof(MsBase)));
                }

                return msbase || listms;
            });

            try
            {
                //当前传入的表单项
                var Form = filterContext.HttpContext.Request?.Form;

                //转成基于MsBase的模型
                backpars.Where((item) => ismsbase(Form!, item)).ToList().ForEach(item =>
                {
                    var type = ((ControllerParameterDescriptor)item).ParameterInfo.ParameterType;//参数类型
                    if (type.IsGenericType)
                    {
                        try
                        {
                            var listvalue = JArray.Parse(Form![item.Name]).ToObject(type);//转化后的值

                            //赋值
                            filterContext.ActionArguments[item.Name] = listvalue;
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            var value = JObject.Parse(Form![item.Name]).ToObject(type);//转化后的值

                            //赋值
                            filterContext.ActionArguments[item.Name] = value;
                        }
                        catch { }
                    }
                });
            }
            catch { }
        }
    }
}
