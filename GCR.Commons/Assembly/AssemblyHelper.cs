namespace GCR.Commons
{
    /// <summary>
    /// 运行库 帮助类
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// 返回 指定特性的 方法
        /// </summary>
        /// <typeparam name="T">特性</typeparam>
        /// <returns>
        /// <para>context 当前类</para>
        /// <para>methodInfo 当前方法</para>
        /// <para>attrdata 当前特性</para>
        /// <para>constructorArguments 特性的参数</para>
        /// <para>namedArguments 特性的属性</para>
        /// </returns>
        public static IEnumerable<(
            Type context, MethodInfo methodInfo,
            CustomAttributeData? attrdata,
            IList<CustomAttributeTypedArgument>? constructorArguments,
            IList<CustomAttributeNamedArgument>? namedArguments)>
            GetAttributeMethodInfo<T>()
        {
            var listassembly = GetAllAssembly();
            var listtypes = listassembly.AsParallel()
                .Where(m => m!.FullName!.StartsWith("HD") || m!.FullName!.Contains("MES")).Select(m => m.GetTypes()).SelectMany(m => m);
            foreach (var context in listtypes)
            {
                var listmethodInfo = context.GetMethods().AsParallel()
                    .Where(t => t.GetCustomAttributes(typeof(T), false).Length > 0);
                foreach (var methodInfo in listmethodInfo)
                {
                    var attrdata = methodInfo.CustomAttributes.Where(t => t.AttributeType == typeof(T))
                        .FirstOrDefault();

                    yield return (context, methodInfo,
                        attrdata,
                        attrdata?.ConstructorArguments,
                        attrdata?.NamedArguments);
                }
            }
        }

        /// <summary>
        /// 得到所有程序集
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAllAssembly()
        {
            var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var asslembyNames in refAssembyNames)
            {
                Assembly.Load(asslembyNames);
            }
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
