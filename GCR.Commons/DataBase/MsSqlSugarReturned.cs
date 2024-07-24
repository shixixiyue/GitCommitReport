
using SqlSugar;
using SqlSugar.Extensions;

namespace GCR.Commons
{
    /// <summary>
    /// 对应 <see cref="SqlSugarQueryedType.Query"/>类型
    /// </summary>
    public class MsSqlSugarQuery : MsReturned
    {
        /// <summary>
        /// 查询的条件<br/>
        /// GetChildList GetMsByKey 时没有该值
        /// </summary>
        public MsQuery msq { get; set; } = new MsQuery();

        /// <summary>
        /// 本次查询的结果 可能是 T 或者 List&lt;T&gt;
        /// </summary>
        public object? QueryData { get; set; }

        /// <summary>
        /// 通过 QueryData 得到查询的结果
        /// </summary>
        /// <remarks>
        /// GetQueryData(()=>new Msxxx())<br/>
        /// 可以通过 QueryData is IEnumerable 来判断是否是集合<br/>
        /// 注意 GetPageCount Exists 方法返回的是 int 和 bool 在转换时需要注意
        /// </remarks>
        /// <typeparam name="T">当前的类型</typeparam>
        /// <param name="item">指定当前的类型</param>
        /// <returns>这里统一使用 IEnumerable&lt;T&gt; 返回</returns>
        public IEnumerable<T?> GetQueryData<T>(Func<T> item)
        where T : class, new()
        {
            if (QueryData is IEnumerable && QueryData.GetType().IsGenericType)
            {
                Type genericType = QueryData.GetType().GetGenericArguments()[0];
                if (genericType == typeof(T))
                {
                    // QueryData 是 List<T> 类型
                    List<T> list = (List<T>)QueryData;
                    // 进行 List<T> 的操作
                    return list;
                }
                else if (typeof(DataTable).IsAssignableFrom(QueryData.GetType()))
                {
                    // QueryData 是 DataTable 类型
                    DataTable dataTable = (DataTable)QueryData;
                    IEnumerable<DataRow> dataRows = dataTable.AsEnumerable();
                    IEnumerable<T?> result = dataRows.Select(row => row.Field<T>("ColumnName"));
                    return result;
                }
                else
                {
                    // res 是其他泛型类型，而不是 List<T>
                    return Enumerable.Repeat(default(T), 0);
                }
            }
            else if (QueryData is T)
            {
                // res 是单独的 T 类型
                T res = (T)QueryData;
                // 进行 T 的操作
                return Enumerable.Repeat(res, 1);
            }
            else
            {
                // res 不是 List<T> 类型，也不是 T 类型
                return Enumerable.Repeat(default(T), 0);
            }
        }

        /// <summary>
        /// 是否子查询
        /// </summary>
        public bool MapperIF { get; set; } = true;

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool WithCache { get; set; } = false;

        /// <summary>
        /// 子项表达式 GetTree 时生效<br/>
        /// 应该是一个 Expression&lt;Func&lt;T, IEnumerable&lt;object&gt;&gt;&gt; 类型
        /// </summary>
        public object childListExpression { get; set; }

        /// <summary>
        /// 父项表达式 GetTree 时生效
        /// 应该是一个 Expression&lt;Func&lt;T, IEnumerable&lt;object&gt;&gt;&gt; 类型
        /// </summary>
        public object parentIdExpression { get; set; }

        /// <summary>
        /// 父级的值 GetChildList 时生效
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// key GetMsByKey 时生效
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// id集合 GetMsByKey 时生效
        /// </summary>
        public object[] ids { get; set; }
    }

    /// <summary>
    /// 对应 <see cref="SqlSugarQueryedType.Insert"/>类型
    /// 如果保存失败 强转MsSqlSugarInsert类型可能失败
    /// </summary>
    public class MsSqlSugarInsert : MsReturned
    {
        /// <summary>
        /// 保存的数据
        /// </summary>
        public object SaveData { get; set; }

        /// <summary>
        /// 通过 SaveData 得到保存的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T?> GetSaveData<T>(Func<T> item)
        where T : class, new()
        {
            if (SaveData is IEnumerable && SaveData.GetType().IsGenericType)
            {
                Type genericType = SaveData.GetType().GetGenericArguments()[0];
                if (genericType == typeof(T))
                {
                    // QueryData 是 List<T> 类型
                    List<T> list = (List<T>)SaveData;
                    // 进行 List<T> 的操作
                    return list;
                }
                else
                {
                    // res 是其他泛型类型，而不是 List<T>
                    return Enumerable.Repeat(default(T), 0);
                }
            }
            else if (SaveData is T)
            {
                // res 是单独的 T 类型
                T res = (T)SaveData;
                // 进行 T 的操作
                return Enumerable.Repeat(res, 1);
            }
            else
            {
                // res 不是 List<T> 类型，也不是 T 类型
                return Enumerable.Repeat(default(T), 0);
            }
        }

        /// <summary>
        /// 受影响的key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// id集合 GetMsByKey 时生效
        /// </summary>
        private object[] _ids;

        /// <summary>
        /// id集合 GetMsByKey 时生效
        /// </summary>
        public object[] ids
        {
            get { if (_ids == null && key != null) { return new[] { key }; } return _ids; }
            set { _ids = value; }
        }
    }

    /// <summary>
    /// 对应 <see cref="SqlSugarQueryedType.Update"/>类型
    /// </summary>
    public class MsSqlSugarUpdate : MsReturned
    {
        /// <summary>
        /// 保存的数据
        /// </summary>
        public object SaveData { get; set; }

        /// <summary>
        /// 通过 SaveData 得到保存的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T?> GetSaveData<T>(Func<T> item)
        where T : class, new()
        {
            if (SaveData is IEnumerable && SaveData.GetType().IsGenericType)
            {
                Type genericType = SaveData.GetType().GetGenericArguments()[0];
                if (genericType == typeof(T))
                {
                    // QueryData 是 List<T> 类型
                    List<T> list = (List<T>)SaveData;
                    // 进行 List<T> 的操作
                    return list;
                }
                else
                {
                    // res 是其他泛型类型，而不是 List<T>
                    return Enumerable.Repeat(default(T), 0);
                }
            }
            else if (SaveData is T)
            {
                // res 是单独的 T 类型
                T res = (T)SaveData;
                // 进行 T 的操作
                return Enumerable.Repeat(res, 1);
            }
            else
            {
                // res 不是 List<T> 类型，也不是 T 类型
                return Enumerable.Repeat(default(T), 0);
            }
        }

        /// <summary>
        /// 受影响的key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// id集合 GetMsByKey 时生效
        /// </summary>
        private object[] _ids;

        /// <summary>
        /// id集合 GetMsByKey 时生效
        /// </summary>
        public object[] ids
        {
            get { if (_ids == null && key != null) { return new[] { key }; } return _ids; }
            set { _ids = value; }
        }

        /// <summary>
        /// 影响的列
        /// </summary>
        public object columns { get; set; }
    }

    /// <summary>
    /// 对应 <see cref="SqlSugarQueryedType.Delete"/>类型
    /// </summary>
    public class MsSqlSugarDelete : MsReturned
    {
        /// <summary>
        /// 受影响的数据
        /// </summary>
        public object DeleteData { get; set; }

        /// <summary>
        /// 通过 DeleteData 得到保存的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T?> GetDeleteData<T>(Func<T> item)
        where T : class, new()
        {
            if (DeleteData is IEnumerable && DeleteData.GetType().IsGenericType)
            {
                Type genericType = DeleteData.GetType().GetGenericArguments()[0];
                if (genericType == typeof(T))
                {
                    // QueryData 是 List<T> 类型
                    List<T> list = (List<T>)DeleteData;
                    // 进行 List<T> 的操作
                    return list;
                }
                else
                {
                    // res 是其他泛型类型，而不是 List<T>
                    return Enumerable.Repeat(default(T), 0);
                }
            }
            else if (DeleteData is T)
            {
                // res 是单独的 T 类型
                T res = (T)DeleteData;
                // 进行 T 的操作
                return Enumerable.Repeat(res, 1);
            }
            else
            {
                // res 不是 List<T> 类型，也不是 T 类型
                return Enumerable.Repeat(default(T), 0);
            }
        }

        /// <summary>
        /// 传入的id 可能有逗号
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 受影响的ID
        /// </summary>
        public object[] ids { get; set; }
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum SqlSugarQueryedType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Insert,

        /// <summary>
        /// 修改
        /// </summary>
        Update,

        /// <summary>
        /// 删除
        /// </summary>
        Delete,

        /// <summary>
        /// 查询 msr的类型 <see cref="MsSqlSugarQuery"/>
        /// </summary>
        Query,
    }
}