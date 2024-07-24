
using SqlSugar;
using System.Diagnostics;

namespace GCR.Commons
{
    /// <summary>
    /// SqlSugar 基类 所有BLL集成该类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SqlSugarHelp<T> : QueryedEventBase<T> where T : class, new()
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="msq"></param>
        /// <param name="MapperIF"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<T> Queryable(MsQuery msq, bool MapperIF = true)
        {
            var db = SqlsugarSetup.Db!
                    .Queryable<T>();
            if (msq.sqlClient != null)
            {
                db = msq.sqlClient!.Queryable<T>();
            }
            return Queryable(msq, db, MapperIF);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="msq"></param>
        /// <param name="MapperIF"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<T> Queryable(MsQuery msq, ISugarQueryable<T>? db, bool MapperIF = true)
        {
            db = db ?? SqlsugarSetup.Db!
                    .Queryable<T>();
            db = db.Where(msq.quermodel);
            if (!string.IsNullOrEmpty(msq.strquery))
            {
                db = db.Where(" 1=1 " + msq.strquery);
            }
            if (!string.IsNullOrEmpty(msq.strorder))
            {
                db = db.OrderBy(msq.strorder);
            }

            var IsMethodAfterQueryable = () =>
            {
                StackTrace stackTrace = new StackTrace();
                StackFrame[] stackFrames = stackTrace.GetFrames();

                //这里要遍历向上找，因为异步,找到 AfterQueryable 或 Queryed_Event
                return stackFrames?.Any(frame =>
                (frame.GetMethod()?.Name.Contains("AfterQueryable") ?? false)
                || (frame.GetMethod()?.Name.Contains("Queryed_Event") ?? false)) ?? false;
            };
            if (IsMethodAfterQueryable()) { return db.Clone(); }
            return db;//注意orderby要和别名一致
        }

        /// <summary>
        /// 得到列表
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否读缓存</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetList(MsQuery msq, bool MapperIF = true, bool WithCache = false)
        {
            var data = await Queryable(msq, MapperIF).WithCacheIF(WithCache)
            .ToListAsync();
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 得到实体
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否读缓存</param>
        /// <returns></returns>
        public virtual async Task<T> GetMs(MsQuery msq, bool MapperIF = true, bool WithCache = false)
        {
            var data = await Queryable(msq, MapperIF).WithCacheIF(WithCache)
                    .FirstAsync();
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;
            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 通过某字段得到集合 通常用于非主键ID或其他字典字段
        /// </summary>
        /// <param name="name">字段</param>
        /// <param name="value">值</param>
        /// <param name="conditionalType">默认是in</param>
        /// <param name="msq">条件</param>
        /// <param name="MapperIF"></param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual Task<T> GetMsByField(string name, string value, ConditionalType conditionalType = ConditionalType.In, MsQuery msq = null, bool MapperIF = true, bool WithCache = false)
        {
            if (msq == null) msq = new MsQuery();
            msq.AddQuermodel(name, value, conditionalType);
            return GetMs(msq);
        }

        /// <summary>
        /// 通过某字段得到集合 通常用于非主键ID或其他字典字段
        /// </summary>
        /// <param name="name">字段</param>
        /// <param name="value">值</param>
        /// <param name="conditionalType">默认是in</param>
        /// <param name="msq">条件</param>
        /// <param name="MapperIF"></param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual Task<List<T>> GetListByField(string name, string value, ConditionalType conditionalType = ConditionalType.In, MsQuery msq = null, bool MapperIF = true, bool WithCache = false)
        {
            if (msq == null) msq = new MsQuery();
            msq.AddQuermodel(name, value, conditionalType);
            return GetList(msq);
        }

        /// <summary>
        /// 通过某字段得到集合 通常用于非主键ID或其他字典字段
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="expression">多个条件，条件之间是and</param>
        /// <returns></returns>
        public virtual Task<List<T>> GetListByField(T ms, params Expression<Func<T, object>>[] expression)
        {
            var msq = new MsQuery();
            ConditionalType type = ConditionalType.In;
            foreach (var item in expression)
            {
                var filed = GetMemberInfo(item).Name;
                var value = GetValue(ms, item)?.ToString()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(value))
                {
                    msq.AddQuermodel(filed, value, type);
                }
            }
            if (expression.Length == 0 || expression == null)
            {
                foreach (var property in ms.GetType().GetProperties())
                {
                    try
                    {
                        var value = property.GetValue(ms)?.ToString() ?? "";
                        if (value != null && !string.IsNullOrEmpty(value))
                        {
                            msq.AddQuermodel(property.Name, value, type);
                        }
                    }
                    catch { }
                }
            }
            return GetList(msq);
        }

        /// <summary>
        /// 通过某字段得到集合 通常用于非主键ID或其他字典字段
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual Task<T> GetMsByFieldName(T ms, params string[] fields)
        {
            var msq = new MsQuery();
            ConditionalType type = ConditionalType.In;
            if (fields.Length != 0 && fields != null)
            {
                foreach (var property in ms.GetType().GetProperties().Where(m => fields.Contains(m.Name)))
                {
                    try
                    {
                        var value = property.GetValue(ms)?.ToString() ?? "";
                        if (value != null && !string.IsNullOrEmpty(value))
                        {
                            msq.AddQuermodel(property.Name, value, type);
                        }
                    }
                    catch { }
                }
            }
            return GetMs(msq);
        }

        /// <summary>
        /// 通过某字段得到集合 通常用于非主键ID或其他字典字段
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual Task<List<T>> GetListByFieldName(T ms, params string[] fields)
        {
            var msq = new MsQuery();
            ConditionalType type = ConditionalType.In;
            if (fields.Length != 0 && fields != null)
            {
                foreach (var property in ms.GetType().GetProperties().Where(m => fields.Contains(m.Name)))
                {
                    try
                    {
                        var value = property.GetValue(ms)?.ToString() ?? "";
                        if (value != null && !string.IsNullOrEmpty(value))
                        {
                            msq.AddQuermodel(property.Name, value, type);
                        }
                    }
                    catch { }
                }
            }
            return GetList(msq);
        }

        /// <summary>
        /// 得到属性 字段
        /// <para>如传入 m=>m.str200 则返回 str200</para>
        /// </summary>
        /// <param name="expression">表达试树</param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(Expression expression)
        {
            LambdaExpression lambdaExpression = (LambdaExpression)expression;
            MemberExpression memberExpression = !(lambdaExpression.Body is UnaryExpression)
                ? (MemberExpression)lambdaExpression.Body
                : (MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand;
            return memberExpression.Member;
        }

        /// <summary>
        /// 得到值
        /// <para>在本实例中尝试接执行 获取值 也可以通过字段名GetValue获取值</para>
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ms">数据</param>
        /// <param name="expression">表达试树</param>
        /// <returns></returns>
        private static object GetValue<T>(T ms, Expression expression)
        {
            object value = new object();
            try
            {
                LambdaExpression lambdaExpression = (LambdaExpression)expression;
                var fun = Expression.Lambda<Func<T, object>>(lambdaExpression.Body, lambdaExpression.Parameters).Compile();
                value = fun.Invoke(ms);
            }
            catch { }
            return value;
        }

        /// <summary>
        /// 得到列表 非异步
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否读缓存</param>
        /// <returns></returns>
        public virtual List<T> GetListNoAwait(MsQuery msq, bool MapperIF = true, bool WithCache = false)
        {
            var data = Queryable(msq, MapperIF).WithCacheIF(WithCache)
                    .ToList();
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;
            Queryed_Event(msr, SqlSugarQueryedType.Query).GetAwaiter().GetResult();
            return data;
        }

        /// <summary>
        /// 得到树结构
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="childListExpression">子项</param>
        /// <param name="parentIdExpression">父项</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否读缓存</param>
        /// <param name="rootValue">顶级的ID 默认0</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetTree(MsQuery msq, Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, bool MapperIF = false, bool WithCache = false, int rootValue = 0)
        {
            var data = await Queryable(msq, MapperIF).WithCacheIF(WithCache)
                  .ToTreeAsync(childListExpression, parentIdExpression, rootValue);
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache,
                childListExpression = childListExpression,
                parentIdExpression = parentIdExpression
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;
            msr.exobj1.childListExpression = childListExpression;
            msr.exobj1.parentIdExpression = parentIdExpression;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 得到子项
        /// </summary>
        /// <param name="ParentId">父级的值</param>
        /// <param name="childListExpression">子项</param>
        /// <param name="parentIdExpression">父项</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetChildList(string ParentId, Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, bool MapperIF = false, bool WithCache = false)
        {
            var data = (await Queryable(new MsQuery(), MapperIF).WithCacheIF(WithCache)
                .ToChildListAsync(parentIdExpression, ParentId)).ToList();
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                ParentId = ParentId,
                MapperIF = MapperIF,
                WithCache = WithCache,
                childListExpression = childListExpression,
                parentIdExpression = parentIdExpression
            };
            msr.exobj1.ParentId = ParentId;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;
            msr.exobj1.childListExpression = childListExpression;
            msr.exobj1.parentIdExpression = parentIdExpression;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 查询Table
        /// </summary>
        /// <param name="msq">查询条件</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<DataTable> GetTable(MsQuery msq, bool MapperIF = true, bool WithCache = false)
        {
            var data = await Queryable(msq, MapperIF).WithCacheIF(WithCache)
                    .ToDataTableAsync();
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 非异步查询Table
        /// </summary>
        /// <param name="msq">查询条件</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual DataTable GetTableNoWait(MsQuery msq, bool MapperIF = true, bool WithCache = false)
        {
            var data = Queryable(msq, MapperIF).WithCacheIF(WithCache)
                    .ToDataTable();
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            Queryed_Event(msr, SqlSugarQueryedType.Query).GetAwaiter().GetResult();
            return data;
        }

        /// <summary>
        /// 得到第一个
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<T> GetMsByKey(string key, bool MapperIF = true, bool WithCache = false)
        {
            var msq = new MsQuery();
            msq.AddQuermodel(Keyfield, key, ConditionalType.Equal);
            var data = await Queryable(msq, MapperIF)
                .WithCacheIF(WithCache)
                .FirstAsync();//注意要和别名一致

            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                key = key,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.key = key;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 得到第一个
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual T GetMsByKeyNoAwait(string key, bool MapperIF = true, bool WithCache = false)
        {
            var msq = new MsQuery();
            msq.AddQuermodel(Keyfield, key, ConditionalType.Equal);
            var data = Queryable(msq, MapperIF)
                .WithCacheIF(WithCache)
                .First();//注意要和别名一致
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                key = key,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.key = key;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            Queryed_Event(msr, SqlSugarQueryedType.Query).GetAwaiter().GetResult();
            return data;
        }

        /// <summary>
        /// 通过ID得到集合
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetMsByKey(object[] ids, bool MapperIF = true, bool WithCache = false)
        {
            var msq = new MsQuery();
            msq.AddQuermodel(Keyfield, string.Join(",", ids), ConditionalType.In);
            var data = await GetList(msq, MapperIF, WithCache);
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                ids = ids,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.ids = ids;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="MapperIF">是否子查询</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetPageList(MsQuery msq, bool MapperIF = true, bool WithCache = false)
        {
            var data = await Queryable(msq, MapperIF)
                .WithCacheIF(WithCache)
                .ToPageListAsync(Convert.ToInt32(msq.pageindex), Convert.ToInt32(msq.pagesize), msq.total);
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                MapperIF = MapperIF,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.MapperIF = MapperIF;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<int> GetPageCount(MsQuery msq, bool WithCache = false)
        {
            var data = await (SqlsugarSetup.Db?
                  .Queryable<T>()
                  .Where(msq.quermodel).WithCacheIF(WithCache).CountAsync() ?? Task.Run(() => -1));
            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="msq">条件</param>
        /// <param name="WithCache">是否缓存</param>
        /// <returns></returns>
        public virtual async Task<bool> Exists(MsQuery msq, bool WithCache = false)
        {
            var data = await (SqlsugarSetup.Db?
              .Queryable<T>()
              .Where(msq.quermodel)
              .WithCacheIF(WithCache).AnyAsync() ?? Task.Run(() => false));

            MsSqlSugarQuery msr = new()
            {
                data = data,
                QueryData = data,
                msq = msq,
                WithCache = WithCache
            };
            msr.exobj1.msq = msq;
            msr.exobj1.WithCache = WithCache;

            await Queryed_Event(msr, SqlSugarQueryedType.Query);
            return data;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="ms">实体</param>
        /// <returns></returns>
        public virtual Task<MsReturned> Save(T ms)
        {
            return Save(ms, true);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="ms">实体</param>
        /// <param name="ignoreAllNullColumns">更新时是否过滤空值</param>
        /// <returns></returns>
        public virtual async Task<MsReturned> Save(T ms, bool ignoreAllNullColumns)
        {
            MsReturned msr = new();
            MsSqlSugarInsert inevemsr = new();
            MsSqlSugarUpdate upevemsr = new();
            try
            {
                var save = SqlsugarSetup.Db!.Storageable(ms).WhereColumns(new string[] { Keyfield }).ToStorage(); //将数据进行分组
                var insert = await save.AsInsertable.ExecuteReturnIdentityAsync(); //执行插入
                                                                                   //msr = new MsSqlSugarInsert();
                inevemsr.strMsId = insert.ToString();
                msr.strMsId = insert.ToString();
                if (insert <= 0)
                {
                    var update = await save.AsUpdateable.IgnoreColumns(ignoreAllNullColumns).ExecuteCommandAsync(); //执行更新　
                    if (update > 0)
                    {
                        //msr = new MsSqlSugarUpdate();
                        upevemsr.strMsId = GetKeyField?.PropertyInfo?.GetValue(ms)?.ToString() ?? "";
                        upevemsr.isInsert = false;
                        msr.strMsId = GetKeyField?.PropertyInfo?.GetValue(ms)?.ToString() ?? "";
                        msr.isInsert = false;
                    }
                }
                msr.strMS = "保存成功";
                msr.MsRbool = true;
                inevemsr.strMS = "保存成功";
                inevemsr.MsRbool = true;
                upevemsr.strMS = "保存成功";
                upevemsr.MsRbool = true;
            }
            catch (Exception ex)
            {
                msr.strMsId = "-1";
                msr.strMS = "保存失败";
                msr.MsRbool = false;
                inevemsr.strMsId = "-1";
                inevemsr.strMS = "保存失败";
                inevemsr.MsRbool = false;
                upevemsr.strMsId = "-1";
                upevemsr.strMS = "保存失败";
                upevemsr.MsRbool = false;
                ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
                log?.Error(this, ex.StackTrace ?? "", ex);
            }
            if (msr.isInsert)
            {
                MsSqlSugarInsert _inevemsr = (MsSqlSugarInsert)SqlsugarSetup.Db.Utilities.TranslateCopy(inevemsr);
                _inevemsr.exobj1.listms = SqlsugarSetup.Db.Utilities.TranslateCopy(new List<T>() { ms });
                _inevemsr.exobj1.ms = ms;
                _inevemsr.listdata = new List<object> { _inevemsr.strMsId };
                _inevemsr.SaveData = ms;
                _inevemsr.key = _inevemsr.strMsId;
                _inevemsr.ids = _inevemsr.listdata.ToArray();
                await Queryed_Event(_inevemsr, SqlSugarQueryedType.Insert);
            }
            else
            {
                MsSqlSugarUpdate _upevemsr = (MsSqlSugarUpdate)SqlsugarSetup.Db.Utilities.TranslateCopy(upevemsr);

                _upevemsr.exobj1.listms = SqlsugarSetup.Db.Utilities.TranslateCopy(new List<T>() { ms });
                _upevemsr.exobj1.ms = ms;
                _upevemsr.listdata = new List<object> { _upevemsr.strMsId };

                _upevemsr.SaveData = ms;
                _upevemsr.key = _upevemsr.strMsId;
                _upevemsr.ids = _upevemsr.listdata.ToArray();
                await Queryed_Event(_upevemsr, SqlSugarQueryedType.Update);
            }

            return msr;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="listms">团体新增</param>
        /// <returns></returns>
        public virtual async Task<MsReturned> Insert(List<T> listms)
        {
            MsReturned msr = new MsSqlSugarInsert() { isInsert = true };
            try
            {
                var ids = await SqlsugarSetup.Db!.Insertable(listms).ExecuteReturnPkListAsync<int>();
                msr.strMsId = ids.Count.ToString();
                msr.listdata = ids.ConvertAll(m => (object)m);
                listms = listms.Select((item, index) =>
                {
                    var thistype = GetKeyField?.PropertyInfo.PropertyType;
                    var dynmicValue = TypeDescriptor.GetConverter(thistype)?.ConvertFromString(ids[index].ToString());//创建对象
                    GetKeyField?.PropertyInfo?.SetValue(item, dynmicValue, null);
                    return item;
                }).ToList();
                msr.strMS = "保存成功";
            }
            catch (Exception ex)
            {
                msr.strMsId = "-1";
                msr.strMS = "保存失败";
                ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
                log?.Error(this, ex.StackTrace ?? "", ex);
            }
            var evemsr = SqlsugarSetup.Db.Utilities.TranslateCopy((MsSqlSugarInsert)msr);
            evemsr.exobj1.listms = SqlsugarSetup.Db.Utilities.TranslateCopy(listms); ;
            evemsr.listdata = listms.ConvertAll(item =>
            {
                var value = GetKeyField?.PropertyInfo?.GetValue(item)?.ToString() ?? "";
                return value;
            }).Cast<object>().ToList();

            evemsr.ids = evemsr.listdata.ToArray();
            evemsr.SaveData = listms;
            await Queryed_Event(evemsr, SqlSugarQueryedType.Insert);
            return msr;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="ms">实体</param>
        /// <returns></returns>
        public virtual async Task<MsReturned> Insert(T ms)
        {
            var KeyValue = GetKeyField?.PropertyInfo?.GetValue(ms)?.ToString() ?? "";
            MsReturned msr = new MsSqlSugarInsert();
            try
            {
                if (string.IsNullOrEmpty(KeyValue) || KeyValue.ToString() == "0")
                {
                    var insert = await SqlsugarSetup.Db!.Insertable(ms).ExecuteReturnIdentityAsync(); //执行插入
                    msr.strMsId = insert.ToString();
                }
                else
                {
                    var insert = await SqlsugarSetup.Db!.Insertable(ms).ExecuteCommandAsync(); //执行插入
                    msr.strMsId = KeyValue;
                }
                msr.strMS = "保存成功";
            }
            catch (Exception ex)
            {
                msr.strMsId = "-1";
                msr.strMS = "保存失败";
                ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
                log?.Error(this, ex.StackTrace ?? "", ex);
            }
            var evemsr = SqlsugarSetup.Db.Utilities.TranslateCopy((MsSqlSugarInsert)msr);
            evemsr.exobj1.listms = SqlsugarSetup.Db.Utilities.TranslateCopy(new List<T>() { ms });
            evemsr.exobj1.ms = ms;
            evemsr.listdata = new List<object> { evemsr.strMsId };
            evemsr.key = evemsr.strMsId;
            evemsr.SaveData = ms;
            await Queryed_Event(evemsr, SqlSugarQueryedType.Insert);
            return msr;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="ms">实体</param>
        /// <param name="columns">字段</param>
        /// <returns></returns>
        public virtual async Task<MsReturned> Update(List<T> ms, dynamic? columns = null)
        {
            MsReturned msr = new MsSqlSugarUpdate() { isInsert = false };
            try
            {
                dynamic up = SqlsugarSetup.Db!.Updateable(ms);
                if (columns != null)
                {
                    var _c = columns;
                    if (columns.GetType().Name == "String")
                    {
                        _c = columns.ToString().Split(",");
                    }
                    up = up.UpdateColumns(_c);
                }
                //Expression<Func<MsSystem_20, bool>> expression = it => ms.ConvertAll(m => m.System_20_AUTOID).Contains(it.System_20_AUTOID);
                var res = await up
                    //.Where(expression)
                    .WhereColumns(Keyfield)
                    .ExecuteCommandAsync();
                msr.strMsId = res.ToString();
                msr.strMS = "保存成功";
                msr.isInsert = false;
            }
            catch (Exception ex)
            {
                msr.strMsId = "-1";
                msr.strMS = "保存失败";
                ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
                log?.Error(this, ex.StackTrace ?? "", ex);
            }
            var evemsr = SqlsugarSetup.Db.Utilities.TranslateCopy((MsSqlSugarUpdate)msr);
            evemsr.exobj1.ms = ms;
            evemsr.exobj1.columns = columns;
            evemsr.listdata = ms.ConvertAll(item =>
            {
                var value = GetKeyField?.PropertyInfo?.GetValue(item)?.ToString() ?? "";
                return value;
            }).Cast<object>().ToList();
            evemsr.SaveData = ms;
            evemsr.ids = evemsr.listdata.ToArray();
            evemsr.columns = columns;

            await Queryed_Event(evemsr, SqlSugarQueryedType.Update);
            return msr;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="msq"></param>
        /// <returns></returns>
        public virtual async Task<MsReturned> DeleteByQuery(MsQuery msq)
        {
            MsReturned msr = new();

            var listdb = SqlsugarSetup.Db!.Queryable<T>().Select($"{Keyfield}");
            if (!string.IsNullOrEmpty(msq.strquery))
            {
                listdb = listdb.Where(" 1=1 " + msq.strquery);
            }
            if (msq.quermodel.Count > 0)
            {
                listdb = listdb.Where(msq.quermodel);
            }
            var list = await listdb.ToListAsync();

            var ids = string.Join(",", list.ConvertAll(item =>
            {
                var value = GetKeyField?.PropertyInfo?.GetValue(item)?.ToString() ?? "";
                return value;
            }));

            msr = await DeleteByKey(ids);
            //var evemsr = SqlsugarSetup.Db.Utilities.TranslateCopy(msr);
            //evemsr.exobj1.msq = msq;
            //evemsr.listdata = list.ConvertAll(item =>
            //{
            //	var value = GetKeyField?.PropertyInfo?.GetValue(item)?.ToString() ?? "";
            //	return value;
            //}).Cast<object>().ToList();
            ////await Queryed_Event(evemsr, SqlSugarQueryedType.Delete);
            return msr;
        }

        /// <summary>
        /// 通过主键删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<MsReturned> DeleteByKey(string key)
        {
            MsReturned msr = new MsSqlSugarDelete();
            MsQuery msq = new();
            if (string.IsNullOrEmpty(key))
            {
                msr.strMsId = "0";
                msr.strMS = "删除数据为空";
                return msr;
            }
            msq.quermodel.Add(new ConditionalModel()
            {
                FieldName = Keyfield,
                ConditionalType = ConditionalType.In,
                FieldValue = key
            });

            //var listdb = SqlsugarSetup.Db!.Queryable<T>();
            //if (!string.IsNullOrEmpty(msq.strquery))
            //{
            //	listdb = listdb.Where(" 1=1 " + msq.strquery);
            //}
            //if (msq.quermodel.Count > 0)
            //{
            //	listdb = listdb.Where(msq.quermodel);
            //}
            var list = await GetList(msq);

            try
            {
                var db = SqlsugarSetup.Db!.Deleteable<T>();
                if (!string.IsNullOrEmpty(msq.strquery))
                {
                    db = db.Where(" 1=1 " + msq.strquery);
                }
                if (msq.quermodel.Count > 0)
                {
                    db = db.Where(msq.quermodel);
                }
                var index = await db.ExecuteCommandAsync();
                if (index > 0)
                {
                    msr.strMsId = "1";
                    msr.strMS = "删除成功";
                }
                else
                {
                    msr.strMsId = "0";
                    msr.strMS = "删除失败";
                }
            }
            catch (Exception ex)
            {
                msr.strMsId = "0";
                msr.strMS = "服务器繁忙";
                ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
                log?.Error(this, ex.StackTrace ?? "", ex);
            }
            var evemsr = SqlsugarSetup.Db.Utilities.TranslateCopy((MsSqlSugarDelete)msr);
            evemsr.exobj1.key = key;
            evemsr.listdata = key.Split(",").Cast<object>().ToList();
            evemsr.data = list;

            evemsr.DeleteData = list;
            evemsr.ids = evemsr.listdata.ToArray();
            evemsr.key = key;

            await Queryed_Event(evemsr, SqlSugarQueryedType.Delete);
            return msr;
        }

        /// <summary>
        /// 执行后的同步事件
        /// </summary>
        /// <param name="msr"></param>
        /// <param name="type"></param>
        /// <param name="funname"></param>
        /// <returns></returns>
        public override sealed async Task Queryed_Event(MsReturned msr, SqlSugarQueryedType type, [CallerMemberName] string funname = "")
        {
            if (type == SqlSugarQueryedType.Update)
            {
                var log = PageContext.GetServerByApp<ILoggerHelper>();
                var sysEvent = PageContext.GetServerByStr<IMediator>();
                var cachekey = $"Update_{SqlsugarSetup.Db.ContextID}";
                var upevent = SqlsugarSetup.Db.DataCache.Get<UpdateColumn_Event>(cachekey);
                if (upevent != null && (Convert.ToInt32(msr.strMsId) > 0 || msr.MsRbool))
                {
#if DEBUG

                    log?.Debug(this, $"由Queryed_Event触发 UpdateColumn_Event事件 {msr.ToString()}");
#endif
                    sysEvent?.Publish(upevent);

                    SqlsugarSetup.Db.DataCache.Servie.Remove<string>(cachekey);
                }
            }
            await base.Queryed_Event(msr, type, funname);
        }

        /// <summary>
        /// 主键的字段
        /// </summary>
        public static string Keyfield => GetKeyField?.DbColumnName ?? "";

        /// <summary>
        /// 当前实体类的配置
        /// </summary>
        /// <returns></returns>
        public static List<EntityColumnInfo>? ThisColumns => SqlsugarSetup.Db?.EntityMaintenance.GetEntityInfo<T>().Columns;

        /// <summary>
        /// 当前实体key字段配置
        /// </summary>
        /// <returns></returns>
        public static EntityColumnInfo? GetKeyField => ThisColumns?.FirstOrDefault(it => it.IsPrimarykey);
    }
}

namespace GCR.Commons
{
    /// <summary>
    /// 执行后的同步事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryedEventBase<T> where T : class, new()
    {
        private IMediator Event => PageContext.GetServerByApp<IMediator>();

        /// <summary>
        /// 执行后的同步事件
        /// </summary>
        /// <param name="msr"></param>
        /// <param name="type"></param>
        /// <param name="funname">触发的事件名称</param>
        public virtual async Task Queryed_Event(MsReturned msr, SqlSugarQueryedType type, string funname = "")
        {
            try
            {
                //如果是query的 执行一次 AfterQueryable
                if (type == SqlSugarQueryedType.Query)
                {
                    await DoAfterQueryable(msr, type, funname);
                }
            }
            catch { }
            try
            {
                await Event.Send(new SqlSugarQueryed_Event<T>(msr, type, funname));
            }
            catch { }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="msr"></param>
        /// <param name="type"></param>
        /// <param name="funname"></param>
        /// <returns></returns>
        private async Task DoAfterQueryable(MsReturned msr, SqlSugarQueryedType type, string funname)
        {
            ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
            try
            {
                var list = new List<T>();
                if (msr.data is List<T>)
                {
                    list = (List<T>)msr.data;
                }
                else if (msr.data is T)
                {
                    list.Add((T)msr.data);
                }
                string key = $"{typeof(T).FullName}_AfterQueryable";
                var request = new SqlSugarQueryed_Event<T>(msr, type, funname);
                string tempkey = $"{SQLSugarEx.AfterQueryable_temp_key}{typeof(T).FullName}";

                //如果是子查询，不能用 上一级的data 应该用上一级的listdata
                var Itemkey = $"Queryable_To_Context_{typeof(T).FullName}";
                int currentIndex = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient().TempItems.Select((item, index) => new { item.Key, Index = index }).FirstOrDefault(x => x.Key == Itemkey)?.Index ?? -1;
                var hasPreviousItem = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient().TempItems.Any(item => item.Key.Contains("Queryable_To_Context_"));
                if (hasPreviousItem)
                {
                    list = msr.listdata.Cast<T>().ToList();
                }

                var queryableContext = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient()
                .TempItems[tempkey] as MapperContext<T>;
                if (queryableContext != null && queryableContext.TempChildLists != null)
                {
                    foreach (var funcitem in queryableContext.TempChildLists)
                    {
                        var AfterQueryable = funcitem.Value as Func<T, List<T>, SqlSugarQueryed_Event<T>, Task>;
                        if (AfterQueryable != null)
                        {
                            string name = typeof(T).Name;
                            var temp = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient()
                               .TempItems;

#if DEBUG
                            log.Debug(typeof(T), $"当前类型 {name}  {tempkey} \n 当前Temp {JsonConvert.SerializeObject(temp.Keys)}");
#endif
                            try
                            {
                                await SqlsugarSetup.Db!.ThenMapperExAsync(list, async item =>
                                {
                                    await AfterQueryable(item, list, request);
                                });
                            }
                            catch (Exception ex)
                            {
                                log.Debug(typeof(T), $"当前类型 {name}   {tempkey} \n 报错了 {ex.Message}");
                                log?.Error(this, ex.StackTrace ?? "", ex);
                            }
                        }
                    }
                }
                SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient().TempItems.Remove(tempkey);
            }
            catch { }
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlSugarQueryed_Event<T> : IRequest<MsReturned> where T : class
    {
        /// <summary>
        /// 调用的事件 包括 data 和 exobj1 可用
        /// </summary>
        public readonly MsReturned msr;

        /// <summary>
        /// 触发的方法名称
        /// </summary>
        public readonly string? name;

        /// <summary>
        /// 类型
        /// </summary>
        public readonly SqlSugarQueryedType? type;

        /// <summary>
        /// 触发事件
        /// <para>
        ///  _Handler:IRequestHandler&lt;SqlSugarQueryed_Event&lt;T&gt;, MsReturned>
        /// </para>
        /// </summary>
        /// <param name="msr"></param>
        /// <param name="name"></param>
        public SqlSugarQueryed_Event(MsReturned msr, SqlSugarQueryedType? type, string? name)
        {
            this.msr = msr;
            this.name = name;
            this.type = type;
        }

        /// <summary>
        /// 得到返回值中MsQuery的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public object? GetMQAsg(string exname, string mqname = "msq")
        {
            var asg = (IDictionary<string, object>)msr.exobj1;//参数
            if (asg.ContainsKey(mqname))
            {
                //withWMSA51 增加主表的查询
                try
                {
                    var msq = (MsQuery)asg[mqname];
                    var objex = (IDictionary<string, object>)msq.objex1;
                    if (objex.ContainsKey(exname))
                    {
                        return objex[exname];
                    }
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// 得到返回值中Msr的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public object? GetMsrAsg(string exname)
        {
            var asg = (IDictionary<string, object>)msr.exobj1;//参数
            if (asg.ContainsKey(exname))
            {
                return asg[exname];
            }
            return null;
        }

        /// <summary>
        /// 得到返回值中MsQuery的参数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exname"></param>
        /// <param name="mqname"></param>
        /// <returns></returns>
        public object? GetMqAsg(string exname, string mqname = "msq")
        {
            return GetMQAsg(exname, mqname);
        }

        /// <summary>
        /// 得到返回值中MsQuery的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="request"></param>
        /// <param name="exname"></param>
        /// <param name="converter">默认的返回</param>
        /// <param name="mqname"></param>
        /// <returns></returns>
        public TOut? GetMqAsg<TOut>(string exname, Func<TOut> converter, string mqname = "msq")
        {
            return (TOut?)(GetMQAsg(exname, mqname) ?? converter());
        }

        /// <summary>
        /// 得到返回值中Msr的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="request"></param>
        /// <param name="exname"></param>
        /// <param name="converter">默认的返回</param>
        /// <param name="mqname"></param>
        /// <returns></returns>
        public TOut? GetMsrAsg<TOut>(string exname, Func<TOut> converter)
            where TOut : class, new()
        {
            var asg = GetMsrAsg(exname);
            var defaultout = converter();
            if (asg == null) { return defaultout; }
            try
            {
                //转换失败了
                var @out = (TOut)asg;
                if (@out != null) return @out;
            }
            catch { }
            try
            {
                var @out2 = JToken.FromObject(asg).ToObject<TOut>();
                if (@out2 != null) return @out2;
            }
            catch { }
            return defaultout;
        }
    }

    /// <summary>
    ///  SqlSugarQueryed_Event的简化写法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISqlQueryed<T> : IRequestHandler<SqlSugarQueryed_Event<T>, MsReturned> where T : class
    {
    }
}