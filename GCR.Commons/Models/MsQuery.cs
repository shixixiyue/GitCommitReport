using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GCR.Commons
{
    /// <summary>
    /// 条件信息
    /// </summary>
    public class MsQuery : MsBase
    {
        /// <summary>
        /// 条件信息
        /// </summary>
        public MsQuery()
        {
        }

        /// <summary>
        /// 拼接的条件
        /// </summary>
        /// <param name="str"></param>
        public MsQuery(string str)
        {
            _strquery = str;
        }

        private string _strquery;//条件字符串
        private string _menuID;//菜单ID
        private string _strorder;//排序语句
        private int _pageindex;//页号
        private int _pagesize;//页大小
        private string _ordertype;//排序类型非0则降序(默认升序)
        private string _tablename;//表名
        private string _strselect;//查找字段

        /// <summary>
        /// 连接字段
        /// </summary>
        public ISqlSugarClient? sqlClient { get; set; } = null;

        /// <summary>
        /// 总数据数量
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 异步总数
        /// </summary>
        public RefAsync<int> total { get; set; } = 0;

        /// <summary>
        /// 总页数  需要设置 RecordCount 和 pagesize
        /// </summary>
        public int pagecount
        {
            get
            {
                if (Convert.ToInt32(pagesize) != 0)
                {
                    return (RecordCount + Convert.ToInt32(pagesize) - 1) / Convert.ToInt32(pagesize);
                }
                return RecordCount;
            }
        }

        /// <summary>
        /// SqlSugar 查询
        /// </summary>
        public List<IConditionalModel> quermodel = new List<IConditionalModel>();

        /// <summary>
        /// 新增条件 默认like
        /// </summary>
        /// <param name="FieldName">字段</param>
        /// <param name="FieldValue">值</param>
        /// <param name="type">默认Like</param>
        public void AddQuermodel(string FieldName, string FieldValue, ConditionalType type = ConditionalType.Like)
        {
            quermodel.Add(new ConditionalModel()
            {
                FieldName = FieldName,
                ConditionalType = type,
                FieldValue = FieldValue
            });
        }

        /// <summary>
        /// 新增条件 传入集合 默认in
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="listFieldValue">数据集合</param>
        /// <param name="type">类型</param>
        public void AddQuermodel(string FieldName, IEnumerable<string> listFieldValue, ConditionalType type = ConditionalType.In)
        {
            var FieldValue = string.Join(",", listFieldValue);
            AddQuermodel(FieldName, FieldValue, type);
        }

        /// <summary>
        /// 新增条件 Like
        /// </summary>
        /// <typeparam name="T">当然类型</typeparam>
        /// <param name="ms">当前数据</param>
        /// <param name="expression">表达试树</param>
        /// <returns></returns>
        public MsQuery AddQuermodel<T>(T ms, params Expression<Func<T, object>>[] expression) where T : MsBase
        {
            ConditionalType type = ConditionalType.Like;
            foreach (var item in expression)
            {
                AddQuermodel(ms, item, type);
            }
            return this;
        }

        /// <summary>
        /// 新增条件 默认Like
        /// </summary>
        /// <typeparam name="T">当然类型</typeparam>
        /// <param name="ms">当前数据</param>
        /// <param name="expression">表达试树</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public MsQuery AddQuermodel<T>(T ms, Expression<Func<T, object>> expression, ConditionalType type) where T : MsBase
        {
            var filed = GetMemberInfo(expression).Name;
            var value = GetValue(ms, expression)?.ToString()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(value))
            {
                AddQuermodel(filed, value, type);
            }
            return this;
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
        public object GetValue<T>(T ms, Expression expression)
        {
            object value = new object();
            //try
            //{
            //    var memberExpression = GetMemberInfo(expression);
            //    if (memberExpression.MemberType == MemberTypes.Field)
            //    {
            //        value = ((FieldInfo)memberExpression).GetValue(ms);
            //    }
            //    else if (memberExpression.MemberType == MemberTypes.Property)
            //    {
            //        value = ((PropertyInfo)memberExpression).GetValue(ms);
            //    }
            //}
            //catch { }

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
        /// 查找字段
        /// </summary>
        public string strselect
        {
            get { return _strselect; }
            set { _strselect = value; }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string tablename
        {
            get { return _tablename; }
            set { _tablename = value; }
        }

        /// <summary>
        /// 排序类型非0则降序(默认升序)
        /// </summary>
        public string ordertype
        {
            get { return _ordertype; }
            set { _ordertype = value; }
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public int pagesize
        {
            get { return _pagesize == null ? 20 : _pagesize; }
            set { _pagesize = value; }
        }

        /// <summary>
        /// 页号
        /// </summary>
        public int pageindex
        {
            get { return _pageindex == null ? 1 : _pageindex; }
            set { _pageindex = value; }
        }

        /// <summary>
        /// 排序语句 已经包含order by
        /// </summary>
        public string strorder
        {
            get { return _strorder; }
            set { _strorder = value; }
        }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public string menuID
        {
            get { return _menuID; }
            set { _menuID = value; }
        }

        /// <summary>
        /// 条件字符串
        /// </summary>
        public string strquery
        {
            get { return _strquery; }
            set { _strquery = value; }
        }

        /// <summary>
        /// Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MsQuery parse(JObject data)
        {
            MsQuery ms = (MsQuery)data.ToObject(typeof(MsQuery));
            return ms;
        }
    }
}
