using SqlSugar;
using SqlSugar.Extensions;

namespace GCR.Commons
{
    /// <summary>
    /// 数据库 基类 拼接连接字符串
    /// </summary>
    public static class DataBase
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string SQLTYPE { get; set; } = "";

        /// <summary>
        /// 得到数据路径
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString(IConfiguration Configuration)
        {
            string SQLTYPE = Configuration["DataConfig:SQLTYPE"];
            string server = Configuration["DataConfig:SQLServer:SERVER"];
            string database = Configuration["DataConfig:SQLServer:DATABASE"];
            string uid = Configuration["DataConfig:SQLServer:USER"];
            string pwd = Configuration["DataConfig:SQLServer:PASSWORD"];
            string config = "server=" + server + ";database=" + database + ";uid=" + uid + ";pwd=" + pwd + "";
            if (SQLTYPE == "2")
            {
                server = Configuration["DataConfig:MySQL:SERVER"];
                database = Configuration["DataConfig:MySQL:DATABASE"];
                uid = Configuration["DataConfig:MySQL:USER"];
                pwd = Configuration["DataConfig:MySQL:PASSWORD"];
                config = "server=" + server + ";database=" + database + ";user id=" + uid + ";password=" + pwd + ";CharSet=utf8mb4;AllowLoadLocalInfile=true;";
            }
            DataBase.SQLTYPE = SQLTYPE;
            //Console.WriteLine(config);
            return config;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            //dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            dt.Columns.AddRange(props.Select((p) => new DataColumn(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
    }
}
