
using SqlSugar;

namespace GCR.Commons
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public static class SqlsugarSetup
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration config)
        {
            ICacheService myCache = new SqlSugarCache();
            //string connstr = DataBase.GetConnectionString(config);
            string connstr = $"DataSource={Path.Combine(Environment.CurrentDirectory, "DataBase", "database.db")}";
            var setup = new SqlSugarScope(new ConnectionConfig()
            {
                //DbType = DataBase.SQLTYPE == "2" ? SqlSugar.DbType.MySql : SqlSugar.DbType.SqlServer,
                DbType = SqlSugar.DbType.Sqlite,
                ConnectionString = connstr,
                IsAutoCloseConnection = true,//自动释放
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    //自动清除缓存
                    IsAutoRemoveDataCache = true
                },
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //注册二级缓存
                    DataInfoCacheService = myCache,
                    EntityService = (property, column) =>
                    {
                        var attributes = property.GetCustomAttributes(true);//get all attributes

                        //if (attributes.Any(it => it is KeyAttribute))// by attribute set primarykey
                        //{
                        //    column.IsPrimarykey = true; //有哪些特性可以看 1.2 特性明细
                        //}
                    },
                    EntityNameService = (type, entity) =>
                    {
                        var attributes = type.GetCustomAttributes(true);
                        //if (attributes.Any(it => it is TableAttribute))
                        //{
                        //    entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute).Name;
                        //}
                    }
                }
            },
            db =>
            {
                //单例参数配置，所有上下文生效
                db.Aop.OnLogExecuting = (s, p) =>
                {
                    if (PageContext.EnvironmentName == "Development")
                    {
                        //Console.WriteLine("*******************************");
                        if (!s.Contains("system_16"))
                        {
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ff"));
                            Console.WriteLine(s);
                            foreach (var item in p)
                            {
                                Console.WriteLine($"{item.ParameterName}:{item.Value}");
                            }
                            Console.WriteLine("------------------------------------------");
                        }
                    }
                };
                db.Aop.OnError = (exp) =>//SQL报错
                {
                    var log = PageContext.GetServerByApp<ILoggerHelper>();
                    if (log != null)
                    {
                        log.Error(typeof(SqlsugarSetup), "SQL执行错误" + exp.Sql);
                        log.Error(typeof(SqlsugarSetup), exp);
                    }
                };
                //在修改之前 按属性遍历
                db.Aop.DataExecuting = (Value, entityInfo) =>
                {
                    var attr = entityInfo.EntityColumnInfo.PropertyInfo.GetCustomAttribute<SqlWatchUpAttr>();
                    if (attr != null && entityInfo.OperationType == DataFilterType.UpdateByObject)
                    {
                        //实体名 当 字段名用 SqlWatchUpAttr的name
                        if (!string.IsNullOrEmpty(attr.name))
                        {
                            entityInfo.EntityColumnInfo.ColumnDescription = attr.name;
                        }
                        //Console.WriteLine($"表名：{entityInfo.EntityColumnInfo.DbTableName}的字段：{entityInfo.EntityColumnInfo.DbColumnName}改为值{Value}");

                        //Event?.Publish(new UpdateColumn_Event(Value?.ToString() ?? "", entityInfo));
                        //var log = PageContext.GetServerByApp<ILoggerHelper>();
                        //if (log != null)
                        //{
                        //	log.Debug(typeof(SqlsugarSetup), "DataExecuting" + );
                        //}
                        myCache.Add($"Update_{db.ContextID}", new UpdateColumn_Event(Value?.ToString() ?? "", entityInfo));
                    }
                };

                //db.Aop.OnLogExecuted = (sql, p) =>
                //{
                //	if (sql.IndexOf("UPDATE") == 0)
                //	{
                //	}
                //};
                //在修改之前 按属性遍历
                //db.Aop.DataExecuting = (oldValue, entityInfo) =>
                //{
                //    if (entityInfo.PropertyName == "UpdateTime" && entityInfo.OperationType == DataFilterType.UpdateByObject)
                //    {
                //        Console.WriteLine(oldValue);
                //    }
                //};
                //db.Aop.OnExecutingChangeSql = (sql, pars) => //可以修改SQL和参数的值
                //{
                //    //加入字符配置
                //    //sql = $"{sql.Replace("N'", "'")}";

                //    return new KeyValuePair<string, SugarParameter[]>(sql, pars);
                //};
            });

            services.AddSingleton<ISqlSugarClient>(setup);//这边是SqlSugarScope用AddSingleton
        }

        /// <summary>
        ///
        /// </summary>
        public static SqlSugarScope? Db => (SqlSugarScope)PageContext.GetServerByStr<ISqlSugarClient>();

        /// <summary>
        ///
        /// </summary>
        public static IMediator? Event => PageContext.GetServerByStr<IMediator>();
    }
}
