
using SqlSugar;
using SqlSugar.Extensions;

namespace GCR.Commons
{
    public static class SQLSugarEx
    {
        #region BindLeft

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TLast"></typeparam>
        /// <param name="sugarQueryable"></param>
        /// <param name="Tname"></param>
        /// <param name="TLastname"></param>
        /// <param name="andfun"></param>
        /// <returns></returns>
        public static ISugarQueryable<T, TLast> BindLeft<T, TLast>(this ISugarQueryable<T> sugarQueryable
            , string Tname, string TLastname, Expression<Func<T, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            Expression<Func<T, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, TLast, bool>>(body, new ParameterExpression[]
                {
                      paramleft,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        /// <summary>
        /// 专门绑定 LeftJoin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TLast"></typeparam>
        /// <param name="sugarQueryable"></param>
        /// <param name="Tname"></param>
        /// <param name="T2name"></param>
        /// <param name="TLastname"></param>
        /// <param name="andfun"></param>
        /// <returns></returns>
        public static ISugarQueryable<T, T2, TLast> BindLeft<T, T2, TLast>(
            this ISugarQueryable<T, T2> sugarQueryable
            , string Tname, string T2name, string TLastname, Expression<Func<T, T2, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            ParameterExpression T2Parameter = Expression.Parameter(typeof(T2), T2name);
            Expression<Func<T, T2, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, T2, TLast, bool>>
                (body, new ParameterExpression[]
                {
                      paramleft,
                      T2Parameter,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        public static ISugarQueryable<T, T2, T3, TLast> BindLeft<T, T2, T3, TLast>(
            this ISugarQueryable<T, T2, T3> sugarQueryable
            , string Tname, string T2name, string T3name, string TLastname
            , Expression<Func<T, T2, T3, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            ParameterExpression T2Parameter = Expression.Parameter(typeof(T2), T2name);
            ParameterExpression T3Parameter = Expression.Parameter(typeof(T3), T3name);
            Expression<Func<T, T2, T3, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, T2, T3, TLast, bool>>
                (body, new ParameterExpression[]
                {
                      paramleft,
                      T2Parameter,
                      T3Parameter,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        public static ISugarQueryable<T, T2, T3, T4, TLast> BindLeft<T, T2, T3, T4, TLast>(
            this ISugarQueryable<T, T2, T3, T4> sugarQueryable
            , string Tname, string T2name, string T3name, string T4name, string TLastname
            , Expression<Func<T, T2, T3, T4, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            ParameterExpression T2Parameter = Expression.Parameter(typeof(T2), T2name);
            ParameterExpression T3Parameter = Expression.Parameter(typeof(T3), T3name);
            ParameterExpression T4Parameter = Expression.Parameter(typeof(T4), T4name);
            Expression<Func<T, T2, T3, T4, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, T2, T3, T4, TLast, bool>>
                (body, new ParameterExpression[]
                {
                      paramleft,
                      T2Parameter,
                      T3Parameter,
                      T4Parameter,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        public static ISugarQueryable<T, T2, T3, T4, T5, TLast> BindLeft<T, T2, T3, T4, T5, TLast>(
            this ISugarQueryable<T, T2, T3, T4, T5> sugarQueryable
            , string Tname, string T2name, string T3name, string T4name, string T5name, string TLastname
            , Expression<Func<T, T2, T3, T4, T5, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            ParameterExpression T2Parameter = Expression.Parameter(typeof(T2), T2name);
            ParameterExpression T3Parameter = Expression.Parameter(typeof(T3), T3name);
            ParameterExpression T4Parameter = Expression.Parameter(typeof(T4), T4name);
            ParameterExpression T5Parameter = Expression.Parameter(typeof(T5), T5name);
            Expression<Func<T, T2, T3, T4, T5, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, T2, T3, T4, T5, TLast, bool>>
                (body, new ParameterExpression[]
                {
                      paramleft,
                      T2Parameter,
                      T3Parameter,
                      T4Parameter,
                      T5Parameter,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        public static ISugarQueryable<T, T2, T3, T4, T5, T6, TLast> BindLeft<T, T2, T3, T4, T5, T6, TLast>(
            this ISugarQueryable<T, T2, T3, T4, T5, T6> sugarQueryable
            , string Tname, string T2name, string T3name, string T4name, string T5name
            , string T6name, string TLastname
            , Expression<Func<T, T2, T3, T4, T5, T6, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            ParameterExpression T2Parameter = Expression.Parameter(typeof(T2), T2name);
            ParameterExpression T3Parameter = Expression.Parameter(typeof(T3), T3name);
            ParameterExpression T4Parameter = Expression.Parameter(typeof(T4), T4name);
            ParameterExpression T5Parameter = Expression.Parameter(typeof(T5), T5name);
            ParameterExpression T6Parameter = Expression.Parameter(typeof(T6), T6name);
            Expression<Func<T, T2, T3, T4, T5, T6, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, T2, T3, T4, T5, T6, TLast, bool>>
                (body, new ParameterExpression[]
                {
                      paramleft,
                      T2Parameter,
                      T3Parameter,
                      T4Parameter,
                      T5Parameter,
                      T6Parameter,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, TLast> BindLeft<T, T2, T3, T4, T5, T6, T7, TLast>(
            this ISugarQueryable<T, T2, T3, T4, T5, T6, T7> sugarQueryable
            , string Tname, string T2name, string T3name, string T4name, string T5name
            , string T6name, string T7name, string TLastname
            , Expression<Func<T, T2, T3, T4, T5, T6, T7, TLast, bool>> andfun = null)
        {
            ParameterExpression paramleft, rightparam;
            Expression body;
            (paramleft, rightparam, body) = GetNewExpressionBody<T, TLast>(Tname, TLastname);

            if (andfun != null)
            {
                body = Expression.AndAlso(body, andfun.Body);
            }

            ParameterExpression T2Parameter = Expression.Parameter(typeof(T2), T2name);
            ParameterExpression T3Parameter = Expression.Parameter(typeof(T3), T3name);
            ParameterExpression T4Parameter = Expression.Parameter(typeof(T4), T4name);
            ParameterExpression T5Parameter = Expression.Parameter(typeof(T5), T5name);
            ParameterExpression T6Parameter = Expression.Parameter(typeof(T6), T6name);
            ParameterExpression T7Parameter = Expression.Parameter(typeof(T7), T7name);
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TLast, bool>> joinExpression =
                Expression.Lambda<Func<T, T2, T3, T4, T5, T6, T7, TLast, bool>>
                (body, new ParameterExpression[]
                {
                      paramleft,
                      T2Parameter,
                      T3Parameter,
                      T4Parameter,
                      T5Parameter,
                      T6Parameter,
                      T7Parameter,
                      rightparam
                });

            return sugarQueryable.LeftJoin(joinExpression);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        public static ISugarQueryable<T, T2> BindLeftAll<T, T2>(
            this ISugarQueryable<T> sugarQueryable
            , string Tname, string T2name
            , Expression<Func<T, T2, bool>> T2fun = null)
        {
            return sugarQueryable.BindLeft(Tname, T2name, T2fun);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="sugarQueryable"></param>
        /// <param name="Tname"></param>
        /// <param name="T2name"></param>
        /// <param name="T3name"></param>
        /// <param name="T2fun"></param>
        /// <param name="T3fun"></param>
        /// <returns></returns>
        public static ISugarQueryable<T, T2, T3> BindLeftAll<T, T2, T3>(
            this ISugarQueryable<T> sugarQueryable
            , string Tname, string T2name, string T3name
            , Expression<Func<T, T2, bool>> T2fun = null
            , Expression<Func<T, T2, T3, bool>> T3fun = null)
        {
            return sugarQueryable.BindLeft(Tname, T2name, T2fun)
                 .BindLeft(Tname, T2name, T3name, T3fun);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        /// <typeparam name="T">主表/typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <typeparam name="T3">T3</typeparam>
        /// <typeparam name="T4">T4</typeparam>
        /// <param name="sugarQueryable">当前的Queryable</param>
        /// <param name="Tname">主表别名</param>
        /// <param name="T2name">T2别名</param>
        /// <param name="T3name">T3别名</param>
        /// <param name="T4name">T4别名</param>
        /// <param name="T2fun">T2扩展方法</param>
        /// <param name="T3fun">T3扩展方法</param>
        /// <param name="T4fun">T4扩展方法</param>
        /// <returns></returns>
        public static ISugarQueryable<T, T2, T3, T4> BindLeftAll<T, T2, T3, T4>(
           this ISugarQueryable<T> sugarQueryable
           , string Tname, string T2name, string T3name, string T4name
           , Expression<Func<T, T2, bool>> T2fun = null
           , Expression<Func<T, T2, T3, bool>> T3fun = null
           , Expression<Func<T, T2, T3, T4, bool>> T4fun = null)
        {
            return sugarQueryable
                 .BindLeft(Tname, T2name, T2fun)
                 .BindLeft(Tname, T2name, T3name, T3fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T4fun);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        public static ISugarQueryable<T, T2, T3, T4, T5> BindLeftAll<T, T2, T3, T4, T5>(
           this ISugarQueryable<T> sugarQueryable
           , string Tname, string T2name, string T3name, string T4name, string T5name
           , Expression<Func<T, T2, bool>> T2fun = null
           , Expression<Func<T, T2, T3, bool>> T3fun = null
           , Expression<Func<T, T2, T3, T4, bool>> T4fun = null
           , Expression<Func<T, T2, T3, T4, T5, bool>> T5fun = null)
        {
            return sugarQueryable
                 .BindLeft(Tname, T2name, T2fun)
                 .BindLeft(Tname, T2name, T3name, T3fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T4fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T5fun);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        public static ISugarQueryable<T, T2, T3, T4, T5, T6> BindLeftAll<T, T2, T3, T4, T5, T6>(
           this ISugarQueryable<T> sugarQueryable
           , string Tname, string T2name, string T3name, string T4name, string T5name, string T6name
           , Expression<Func<T, T2, bool>> T2fun = null
           , Expression<Func<T, T2, T3, bool>> T3fun = null
           , Expression<Func<T, T2, T3, T4, bool>> T4fun = null
           , Expression<Func<T, T2, T3, T4, T5, bool>> T5fun = null
           , Expression<Func<T, T2, T3, T4, T5, T6, bool>> T6fun = null)
        {
            return sugarQueryable
                 .BindLeft(Tname, T2name, T2fun)
                 .BindLeft(Tname, T2name, T3name, T3fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T4fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T5fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T6name, T6fun);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7> BindLeftAll<T, T2, T3, T4, T5, T6, T7>(
           this ISugarQueryable<T> sugarQueryable
           , string Tname, string T2name, string T3name, string T4name, string T5name, string T6name
            , string T7name
           , Expression<Func<T, T2, bool>> T2fun = null
           , Expression<Func<T, T2, T3, bool>> T3fun = null
           , Expression<Func<T, T2, T3, T4, bool>> T4fun = null
           , Expression<Func<T, T2, T3, T4, T5, bool>> T5fun = null
           , Expression<Func<T, T2, T3, T4, T5, T6, bool>> T6fun = null
           , Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> T7fun = null)
        {
            return sugarQueryable
                 .BindLeft(Tname, T2name, T2fun)
                 .BindLeft(Tname, T2name, T3name, T3fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T4fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T5fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T6name, T6fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T6name
                 , T7name, T7fun);
        }

        /// <summary>
        /// 接到 Queryable 之后 以主表 T Left Join 其他表
        /// </summary>
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8>
            BindLeftAll<T, T2, T3, T4, T5, T6, T7, T8>(
           this ISugarQueryable<T> sugarQueryable
           , string Tname, string T2name, string T3name, string T4name, string T5name, string T6name
            , string T7name, string T8name
           , Expression<Func<T, T2, bool>> T2fun = null
           , Expression<Func<T, T2, T3, bool>> T3fun = null
           , Expression<Func<T, T2, T3, T4, bool>> T4fun = null
           , Expression<Func<T, T2, T3, T4, T5, bool>> T5fun = null
           , Expression<Func<T, T2, T3, T4, T5, T6, bool>> T6fun = null
           , Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> T7fun = null
           , Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> T8fun = null)
        {
            return sugarQueryable
                 .BindLeft(Tname, T2name, T2fun)
                 .BindLeft(Tname, T2name, T3name, T3fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T4fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T5fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T6name, T6fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T6name
                 , T7name, T7fun)
                 .BindLeft(Tname, T2name, T3name, T4name, T5name, T6name
                 , T7name, T8name, T8fun);
        }

        /// <summary>
        /// 根据参数名称 和 类型 得到 组装的表达式树
        /// </summary>
        /// <typeparam name="T">主表类型</typeparam>
        /// <typeparam name="TLast">附表类型</typeparam>
        /// <param name="Tname">主表参数</param>
        /// <param name="TLastname">附表参数</param>
        /// <returns>
        /// paramleft第一个参数  rightparam最有一个参数 body身子
        /// </returns>
        public static (ParameterExpression paramleft, ParameterExpression rightparam, Expression body) GetNewExpressionBody<T, TLast>(string Tname, string TLastname)
        {
            PropertyInfo relpro = GetRelation<T, TLast>();
            //跟这个字段有关系的字段
            var msrelationfield = relpro.GetCustomAttribute<Relation>().field;

            //等号左边 Tname.leftfield
            var leftfield = relpro?.Name;
            var lefttype = relpro?.PropertyType;
            var righttype = typeof(TLast).GetProperty(msrelationfield).PropertyType;

            MethodInfo mistring = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });

            ParameterExpression paramleft = Expression.Parameter(typeof(T), Tname);
            Expression keyleft = paramleft;
            keyleft = Expression.Property(keyleft, leftfield);

            //等号右边 TLastname.rightfield
            var rightfield = msrelationfield;
            ParameterExpression rightparam = Expression.Parameter(typeof(TLast), TLastname);
            Expression keyright = rightparam;
            keyright = Expression.Property(keyright, rightfield);

            #region TEST

            //身子 Tname.xxx = TLastname.xxx
            //Expression body = Expression.Equal(keyleft, keyright);

            //MethodInfo mi = lefttype.GetMethod("Equals", new Type[] { righttype });
            //mi = mi.MakeGenericMethod(new Type[] { righttype });
            //第一个参数是obj
            //var Equalstype = mi.GetParameters().First().ParameterType;
            //var converted = Expression.Constant(keyright, Equalstype);
            //Expression body = Expression.Call(keyleft, mi, converted);//
            //Expression body = Expression.Call(keyleft, mi, keyright);//
            //Expression body = Expression.Call(typeof(object), "Equals", new sys.Type[] { typeof(object), typeof(object) }, keyleft, keyright);//
            //Expression body = Expression.Equal(keyleft, keyright);

            //var converted = Expression.Convert(keyleft, righttype);
            //Expression body = Expression.Call(converted, mi, keyright);//

            #endregion TEST

            //转为ToString()
            var lefttostring = lefttype.GetMethod("ToString", Type.EmptyTypes);
            keyleft = Expression.Call(keyleft, lefttostring);//
            var righttostring = righttype.GetMethod("ToString", Type.EmptyTypes);
            keyright = Expression.Call(keyright, righttostring);//
            Expression body = Expression.Call(keyleft, mistring, keyright);//

            return (paramleft, rightparam, body);
        }

        /// <summary>
        /// 得到包含关系的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TLast"></typeparam>
        /// <returns></returns>
        private static PropertyInfo GetRelation<T, TLast>()
        {
            //继承了Relation 的字段
            var listmethodInfo = typeof(T).GetProperties().AsParallel()
    .Where(t => t.GetCustomAttributes(typeof(Relation), false).Length > 0);

            //关系为T类型的字段
            var relpro = listmethodInfo.FirstOrDefault(item =>
            {
                var reattr = item.GetCustomAttribute<Relation>();
                return reattr.retype == typeof(TLast);
            });
            if (relpro == null)
            {
                throw new Exception($"没有找到 类型 {typeof(T).Name} 和 {typeof(TLast).Name} 的关系\r\n" +
                    $"请在{typeof(T).Name}的关系字段增加[Relation]特性");
            }
            return relpro;
        }

        /// <summary>
        /// 非主表关系 得到表达式树
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="asg">顺序的表别名 三个 逗号分隔</param>
        /// <param name="relation">关系的表名 两个</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Expression<Func<T1, T2, T3, bool>> GetRelation<T1, T2, T3>(string asg, string relation)
        {
            if (asg.Split(",").Length != 3 || relation.Split(",").Length != 2)
            {
                throw new Exception($"参数长度错误");
            }
            Expression body;
            IEnumerable<ParameterExpression> pars;
            (body, pars) = GetRelation(asg, relation, new Type[] { typeof(T1), typeof(T2), typeof(T3) });

            Expression<Func<T1, T2, T3, bool>> joinExpression =
                Expression.Lambda<Func<T1, T2, T3, bool>>
                (body, pars);

            return joinExpression;
        }

        /// <summary>
        /// 非主表关系 得到表达式树
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="asg">顺序的表别名 四个 逗号分隔</param>
        /// <param name="relation">关系的表名 两个</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Expression<Func<T1, T2, T3, T4, bool>> GetRelation<T1, T2, T3, T4>(string asg, string relation)
        {
            if (asg.Split(",").Length != 4 || relation.Split(",").Length != 2)
            {
                throw new Exception($"参数长度错误");
            }
            Expression body;
            IEnumerable<ParameterExpression> pars;
            (body, pars) = GetRelation(asg, relation, new Type[] { typeof(T1), typeof(T2),
                typeof(T3), typeof(T4) });

            Expression<Func<T1, T2, T3, T4, bool>> joinExpression =
                Expression.Lambda<Func<T1, T2, T3, T4, bool>>
                (body, pars);

            return joinExpression;
        }

        /// <summary>
        /// 非主表关系 得到表达式树
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="asg">顺序的表别名 五个 逗号分隔</param>
        /// <param name="relation">关系的表名 两个</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Expression<Func<T1, T2, T3, T4, T5, bool>> GetRelation<T1, T2, T3, T4, T5>(string asg, string relation)
        {
            if (asg.Split(",").Length != 5 || relation.Split(",").Length != 2)
            {
                throw new Exception($"参数长度错误");
            }
            Expression body;
            IEnumerable<ParameterExpression> pars;
            (body, pars) = GetRelation(asg, relation, new Type[] { typeof(T1), typeof(T2),
                typeof(T3), typeof(T4) , typeof(T5)});

            Expression<Func<T1, T2, T3, T4, T5, bool>> joinExpression =
                Expression.Lambda<Func<T1, T2, T3, T4, T5, bool>>
                (body, pars);

            return joinExpression;
        }

        /// <summary>
        /// 非主表关系 得到表达式树
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="asg">顺序的表别名 六个 逗号分隔</param>
        /// <param name="relation">关系的表名 两个</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Expression<Func<T1, T2, T3, T4, T5, T6, bool>> GetRelation<T1, T2, T3, T4, T5, T6>(string asg, string relation)
        {
            if (asg.Split(",").Length != 6 || relation.Split(",").Length != 2)
            {
                throw new Exception($"参数长度错误");
            }
            Expression body;
            IEnumerable<ParameterExpression> pars;
            (body, pars) = GetRelation(asg, relation, new Type[] { typeof(T1), typeof(T2),
                typeof(T3), typeof(T4) , typeof(T5) , typeof(T6)});

            Expression<Func<T1, T2, T3, T4, T5, T6, bool>> joinExpression =
                Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, bool>>
                (body, pars);

            return joinExpression;
        }

        /// <summary>
        /// 非主表关系 得到表达式树
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="asg">顺序的表别名 七个 逗号分隔</param>
        /// <param name="relation">关系的表名 两个</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> GetRelation<T1, T2, T3, T4, T5, T6, T7>(string asg, string relation)
        {
            if (asg.Split(",").Length != 7 || relation.Split(",").Length != 2)
            {
                throw new Exception($"参数长度错误");
            }
            Expression body;
            IEnumerable<ParameterExpression> pars;
            (body, pars) = GetRelation(asg, relation, new Type[] { typeof(T1), typeof(T2),
                typeof(T3), typeof(T4) , typeof(T5) , typeof(T6), typeof(T7)});

            Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> joinExpression =
                Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, bool>>
                (body, pars);

            return joinExpression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="asg"></param>
        /// <param name="relation"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        private static (Expression, IEnumerable<ParameterExpression>) GetRelation(string asg, string relation, Type[] types)
        {
            ParameterExpression paramleft, rightparam;
            string Tname = relation.Split(",").First();
            string TLastname = relation.Split(",").Last();

            var Tindex = types[asg.Split(",").ToList().FindIndex(i => i == Tname)];
            var TLast = types[asg.Split(",").ToList().FindIndex(i => i == TLastname)];

            var bodyMethod = typeof(SQLSugarEx).GetMethod("GetNewExpressionBody",
                new Type[] { typeof(string), typeof(string) }).MakeGenericMethod(new Type[] { Tindex, TLast });
            var getbody = bodyMethod.Invoke(null, new object[] { Tname, TLastname });
            paramleft = (ParameterExpression)typeof((ParameterExpression, ParameterExpression, Expression)).GetField("Item1").GetValue(getbody);
            rightparam = (ParameterExpression)typeof((ParameterExpression, ParameterExpression, Expression)).GetField("Item2").GetValue(getbody);
            Expression body = (Expression)typeof((ParameterExpression, ParameterExpression, Expression)).GetField("Item3").GetValue(getbody);

            IEnumerable<ParameterExpression> pars = asg.Split(",").Select((item, index) => Expression.Parameter(types[index], item));
            return (body, pars);
        }

        #endregion BindLeft

        #region ToContext SetContextAsync的简单实现

        /// <summary>
        /// 查询关联关系 返回list
        /// <para>是SetContextAsync的封装</para>
        /// <para>最后触发 queryEvent(Query) </para>
        /// </summary>
        /// <typeparam name="T">附表类</typeparam>
        /// <typeparam name="ParameterT">主表类</typeparam>
        /// <param name="t"></param>
        /// <param name="thisFiled">附表关系</param>
        /// <param name="mappingFiled">主表关系</param>
        /// <param name="parameter">主表数据</param>
        /// <param name="msq">最终绑定到Queryed_Event事件的exobj1.msq中</param>
        /// <returns><see cref="List{T}"/></returns>
        public static async Task<List<T>> ToContextListAsync<T, ParameterT>(this ISugarQueryable<T> t, Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter, MsQuery? msq = null) where ParameterT : class, new() where T : class, new()
        {
            var queryEvent = new QueryedEventBase<T>();
            var data = await t.SetContextExAsync(thisFiled, mappingFiled, parameter);
            var msr = new MsReturned();
            msr.data = data.thisval;
            msr.listdata = data.allval.Cast<object>().ToList();
            if (msq != null)
            {
                msr.exobj1.msq = msq;
            }
            await queryEvent.Queryed_Event(msr, SqlSugarQueryedType.Query, "ToContextListAsync");
            return data.thisval;
        }

        /// <summary>
        /// 查询关联关系 返回实体
        /// <para>是SetContextAsync的封装</para>
        /// <para>最后触发 queryEvent(Query) </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ParameterT"></typeparam>
        /// <param name="t"></param>
        /// <param name="thisFiled">附表关系</param>
        /// <param name="mappingFiled">主表关系</param>
        /// <param name="parameter">主表数据</param>
        /// <param name="msq">最终绑定到Queryed_Event事件的exobj1.msq中</param>
        /// <returns>实体</returns>
        public static async Task<T> ToContextAsync<T, ParameterT>(this ISugarQueryable<T> t, Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter, MsQuery? msq = null)
            where ParameterT : class, new()
            where T : class, new()
        {
            var data = await t.ToContextListAsync(thisFiled, mappingFiled, parameter, msq);
            return data.FirstOrDefault() ?? new T();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ParameterT"></typeparam>
        /// <param name="t"></param>
        /// <param name="mappingFiled">主表关系</param>
        /// <param name="parameter">主表数据</param>
        /// <param name="msq">最终绑定到Queryed_Event事件的exobj1.msq中</param>
        /// <returns><see cref="List{T}"/></returns>
        public static Task<List<T>> ToContextListAsync<T, ParameterT>(this ISugarQueryable<T> t, Expression<Func<object>> mappingFiled, ParameterT parameter, MsQuery? msq = null) where ParameterT : class, new() where T : class, new()
        {
            Expression<Func<T, object>> thisFiled = GetRelationForContext<T, ParameterT>(mappingFiled);

            return t.ToContextListAsync(thisFiled, mappingFiled, parameter);
        }

        /// <summary>
        /// 得到关联的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ParameterT"></typeparam>
        /// <param name="t"></param>
        /// <param name="mappingFiled">主表关系</param>
        /// <param name="parameter">主表数据</param>
        /// <param name="msq">最终绑定到Queryed_Event事件的exobj1.msq中</param>
        /// <returns></returns>
        public static async Task<T> ToContextAsync<T, ParameterT>(this ISugarQueryable<T> t, Expression<Func<object>> mappingFiled, ParameterT parameter, MsQuery? msq = null)
            where ParameterT : class, new()
            where T : class, new()
        {
            var data = await t.ToContextListAsync(mappingFiled, parameter, msq);
            return data.FirstOrDefault() ?? new T();
        }

        /// <summary>
        /// 为关系查询方法 提供拼接后的 关系字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ParameterT"></typeparam>
        /// <param name="mappingFiled"></param>
        /// <returns></returns>
        private static Expression<Func<T, object>> GetRelationForContext<T, ParameterT>(Expression<Func<object>> mappingFiled) where ParameterT : class, new()
        {
            //这是传进来的主表的字段
            var filed = GetMemberInfo(mappingFiled).Name;
            //根据这个字段找到关联类型和字段
            var listmethodInfo = typeof(ParameterT).GetProperties().AsParallel()
                .Where(t => t.GetCustomAttributes(typeof(Relation), false).Length > 0);
            var relpro = listmethodInfo.FirstOrDefault(item =>
            {
                return item.Name == filed;
            });
            var msrelationfield = relpro.GetCustomAttribute<Relation>().field;
            // 创建参数表达式
            ParameterExpression param = Expression.Parameter(typeof(T), "ms");
            // 创建属性访问表达式
            Expression property = Expression.PropertyOrField(param, msrelationfield);
            var lefttype = property?.Type;
            var lefttostring = lefttype.GetMethod("ToString", Type.EmptyTypes);
            //property = Expression.Call(typeof(object), "", new Type[] { typeof(object) }, property);//// 创建 lambda 表达式
            //property = Expression.Constant(property, typeof(object));
            property = Expression.Call(property, lefttostring);//

            Expression<Func<T, object>> thisFiled = Expression.Lambda<Func<T, object>>(
                property,
                new ParameterExpression[] { param });
            return thisFiled;
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

        #endregion ToContext SetContextAsync的简单实现

        #region AfterQueryable 查询之后 关联用 扩展

        /// <summary>
        /// 连接中的key
        /// </summary>
        public const string AfterQueryable_temp_key = "Befor_Queryable_To_Context";

        /// <summary>
        /// 参见 <see cref="SqlSugarHelp{T}.Queryed_Event"/>
        /// </summary>
        /// <remarks>使用 ThenMapper 参数的HashCode 作为key<br/>
        /// 使用 <see cref="SqlSugar.MapperContext{T}"/> 存储
        /// <br>创建 <see cref="SqlSugarScope.GetSimpleClient{T}"/><see cref="SimpleClient{T}.AsSugarClient"/>存储集合</br>
        /// <br/>让该方法可以配置多次，在执行<see cref="SqlSugarHelp{T}.Queryed_Event"/>后删除缓存
        /// <br/>最后会调用<see cref="SqlSugarScope.ThenMapperAsync"/>方法
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ThenMapper">
        /// 回调包括三个参数
        /// <br/> item 当前项
        /// <br/> list 当前集合 <see cref="List{T}"/>
        /// <br/> request 当前事件 <see cref="SqlSugarQueryed_Event{T}"/>
        /// </param>
        public static void AfterQueryable<T>(this ISugarQueryable<T> t,
            Func<T, List<T>, SqlSugarQueryed_Event<T>, Task> ThenMapper
            )
            where T : class, new()
        {
            string key = ThenMapper.GetHashCode().ToString();
            string tempkey = $"{AfterQueryable_temp_key}{typeof(T).FullName}";
            var sugarclient = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient();
            if (!sugarclient.TempItems.ContainsKey(tempkey))
            {
                var res = new MapperContext<T>();
                res.TempChildLists = new Dictionary<string, object>();
                sugarclient.TempItems[tempkey] = res;
            }
            var queryableContext = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient()
                        .TempItems[tempkey] as MapperContext<T>;
            queryableContext!.TempChildLists[key] = ThenMapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="t"></param>
        /// <param name="list"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task ThenMapperExAsync<T>(this SqlSugarScope t, IEnumerable<T> list, Func<T, Task> action)
            where T : class, new()
        {
            ILoggerHelper log = PageContext.GetServerByApp<ILoggerHelper>();
            MapperContext<T> result = new MapperContext<T>();
            result.context = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient();
            if (result.context.TempItems == null)
            {
                result.context.TempItems = new Dictionary<string, object>();
            }
            var key = $"Queryable_To_Context_{typeof(T).FullName}";
            result.context.TempItems.TryAdd(key, result);
#if DEBUG

            log.Debug(typeof(SQLSugarEx), $"包含了 并且类型是 {result.context.TempItems[key].GetType()} 当前类型 {typeof(T).FullName} 当前key{key}");
#endif
            result.list = list.ToList();
            foreach (var item in list)
            {
                await action.Invoke(item);
            }
            //如果前面有 Queryable_To_Context_ 开头的 说明没有完成 查询 则不删除
            int currentIndex = result.context.TempItems.Select((item, index) => new { item.Key, Index = index }).FirstOrDefault(x => x.Key == key)?.Index ?? -1;
            var hasPreviousItem = result.context.TempItems.TakeWhile((item, index) => index < currentIndex).Any(item => item.Key.Contains("Queryable_To_Context_"));

            //如果前面没有 Queryable_To_Context_ 开头的 说明已经完成 查询 则删除
            if (!hasPreviousItem)
            {
                result.context.TempItems.Where(item => item.Key.Contains("Queryable_To_Context_")).Select(item => item.Key).ToList().ForEach(item =>
                {
                    result.context.TempItems.Remove(item);
                });
                result.context.TempItems.Remove(key);
            }
            //result.context.TempItems.Remove(key2);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ParameterT"></typeparam>
        /// <param name="t"></param>
        /// <param name="thisFiled"></param>
        /// <param name="mappingFiled"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static async Task<(List<T> thisval, List<T> allval)> SetContextExAsync<T, ParameterT>(this ISugarQueryable<T> t, Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter)
            where T : class, new()
        {
            var Queryablekey = $"Queryable_To_Context_{typeof(ParameterT).FullName}";
            List<T> result = new List<T>();
            var entity = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient().EntityMaintenance.GetEntityInfo<ParameterT>();
            var queryableContext = SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient().TempItems[Queryablekey] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var pkName = "";
            if ((mappingFiled as LambdaExpression).Body is UnaryExpression)
            {
                pkName = (((mappingFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                pkName = ((mappingFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var key = thisFiled.ToString() + mappingFiled.ToString() + typeof(ParameterT).FullName + typeof(T).FullName;
            var ids = list.Select(it => it.GetType().GetProperty(pkName).GetValue(it)).Where(m => m != null).ToArray();
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null && queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                if (queryableContext.TempChildLists == null)
                    queryableContext.TempChildLists = new Dictionary<string, object>();
                await SqlsugarSetup.Db.GetSimpleClient<T>().AsSugarClient().Utilities.PageEachAsync(ids, 200, async pageIds =>
                {
                    try
                    {
                        result.AddRange(await t.Clone().In(thisFiled, pageIds).ToListAsync());
                    }
                    catch (Exception ex) { }
                });
                queryableContext.TempChildLists[key] = result;
            }
            var name = "";
            //var names = GetMemberInfo(thisFiled).Name;
            if ((thisFiled as LambdaExpression).Body is UnaryExpression)
            {
                name = (((thisFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else if ((thisFiled as LambdaExpression).Body is MethodCallExpression)
            {
                name = (((thisFiled as LambdaExpression).Body as MethodCallExpression).Object as MemberExpression).Member.Name;
            }
            else
            {
                name = ((thisFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var pkValue = parameter.GetType().GetProperty(pkName).GetValue(parameter);
            var thisresult = result.Where(it => it.GetType().GetProperty(name).GetValue(it).ObjToString() == pkValue.ObjToString()).ToList();
            return (thisresult, result);
        }

        #endregion AfterQueryable 查询之后 关联用 扩展

        #region isInsert

        /// <summary>
        /// 是否是新增
        /// </summary>
        /// <param name="ms"></param>
        /// <returns>如果为空 返回true</returns>
        public static bool isInsert<T>(this T ms) where T : MsBase
        {
            var ThisColumns = SqlsugarSetup.Db?.EntityMaintenance.GetEntityInfo<T>().Columns;
            var GetKeyField = ThisColumns?.FirstOrDefault(it => it.IsPrimarykey);
            string KeyValue = GetKeyField?.PropertyInfo?.GetValue(ms)?.ToString() ?? "";
            int.TryParse(KeyValue, out int value);
            if (string.IsNullOrEmpty(KeyValue) || KeyValue.ToString() == "0" || value == null || value <= 0)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// !!注意这个方法写错了
        /// </summary>
        /// <param name="ms"></param>
        /// <returns>如果为空 返回true，不为空返回false</returns>
        public static bool hasKeyValue<T>(this T ms) where T : MsBase
        {
            var ThisColumns = SqlsugarSetup.Db?.EntityMaintenance.GetEntityInfo<T>().Columns;
            var GetKeyField = ThisColumns?.FirstOrDefault(it => it.IsPrimarykey);
            string KeyValue = GetKeyField?.PropertyInfo?.GetValue(ms)?.ToString() ?? "";
            int.TryParse(KeyValue, out int value);
            if (string.IsNullOrEmpty(KeyValue) || KeyValue.ToString() == "0" || value == null || value <= 0)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// key是否包含值
        /// </summary>
        /// <param name="ms"></param>
        /// <returns>如果为空 返回true</returns>
        public static bool noKeyValue<T>(this T ms) where T : MsBase
        {
            var ThisColumns = SqlsugarSetup.Db?.EntityMaintenance.GetEntityInfo<T>().Columns;
            var GetKeyField = ThisColumns?.FirstOrDefault(it => it.IsPrimarykey);
            string KeyValue = GetKeyField?.PropertyInfo?.GetValue(ms)?.ToString() ?? "";
            int.TryParse(KeyValue, out int value);
            if (string.IsNullOrEmpty(KeyValue) || KeyValue.ToString() == "0" || value == null || value <= 0)
            {
                return true;
            }
            else { return false; }
        }

        #endregion isInsert
    }

    /// <summary>
    /// 数据关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class Relation : Attribute
    {
        /// <summary>
        /// 类型
        /// </summary>
        public readonly Type retype;

        /// <summary>
        /// 字段
        /// </summary>
        public readonly string field;

        /// <summary>
        /// 只绑定left join 的关系，
        /// <para>不支持多个字段对应一个表</para>
        /// </summary>
        /// <param name="T"></param>
        /// <param name="field"></param>
        public Relation(Type T, string field)
        {
            retype = T;
            this.field = field;
        }
    }

    #region 字段监听

    /// <summary>
    /// 配置监听 与 <see cref="IUpdateColumn_Event"/> 配合使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class SqlWatchUpAttr : Attribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// 数据库名 <see cref="DataFilterModel.EntityColumnInfo"/>.DbTableName <br/>
    /// 字段 <see cref="DataFilterModel.EntityColumnInfo"/>.DbColumnName <br/>
    /// 变后的值 Value
    /// </summary>
    public class UpdateColumn_Event : INotification
    {
        /// <summary>
        /// 原始信息
        /// </summary>
        public readonly DataFilterModel entityInfo;

        /// <summary>
        /// 字段修改后的值
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// 表名
        /// </summary>
        public readonly string tableName;

        /// <summary>
        /// 列名
        /// </summary>
        public readonly string columnName;

        /// <summary>
        /// 字段备注
        /// </summary>
        public readonly string columnDes;

        /// <summary>
        /// 当前类的key
        /// </summary>
        public readonly string keyField;

        /// <summary>
        /// 当前类的key值
        /// </summary>
        public readonly string keyValue;

        public UpdateColumn_Event(string Value, DataFilterModel entityInfo)
        {
            this.entityInfo = entityInfo;
            this.Value = Value;
            try
            {
                tableName = entityInfo.EntityColumnInfo.DbTableName;
                columnName = entityInfo.EntityColumnInfo.DbColumnName;
                columnDes = entityInfo.EntityColumnInfo.ColumnDescription;
                var key = SqlsugarSetup.Db?.EntityMaintenance.GetEntityInfo(entityInfo.EntityValue.GetType()).Columns.FirstOrDefault(it => it.IsPrimarykey);
                keyField = key?.DbColumnName ?? "";
                keyValue = key?.PropertyInfo?.GetValue(entityInfo.EntityValue)?.ToString() ?? "";
            }
            catch (Exception ex) { }
        }
    }

    /// <summary>
    /// 使用<see cref="SqlWatchUpAttr"/>标识 当改变时触发
    /// 数据库名 <see cref="DataFilterModel.EntityColumnInfo"/>.DbTableName <br/>
    /// 字段 <see cref="DataFilterModel.EntityColumnInfo"/>.DbColumnName <br/>
    /// 变后的值 Value
    /// </summary>
    public interface IUpdateColumn_Event : INotificationHandler<UpdateColumn_Event>
    { }

    #endregion 字段监听
}
