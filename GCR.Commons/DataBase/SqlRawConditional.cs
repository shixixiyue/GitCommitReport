using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SqlSugar;

using SqlSugar.Extensions;

namespace GCR.Commons
{
    /// <summary>
    /// 输出原始内容
    /// </summary>
    public class SqlRawConditional : ICustomConditionalFunc
    {
        /// <summary>
        /// 连接符
        /// </summary>
        private readonly string _Conditional;

        /// <summary>
        /// 通过函数 编辑
        /// </summary>
        private readonly Func<string, string, string> _ConditionalFun;

        /// <summary>
        /// 原始接口返回
        /// </summary>
        private readonly Func<ConditionalModel, int, KeyValuePair<string, SugarParameter[]>> _RawConditionalFun;

        public SqlRawConditional(Func<ConditionalModel, int, KeyValuePair<string, SugarParameter[]>> rawConditionalFun)
        {
            _RawConditionalFun = rawConditionalFun;
        }

        /// <summary>
        /// 输出原始的SQL内容，用于查询
        /// </summary>
        /// <remarks>
        /// msq.quermodel.Add(new ConditionalModel()<br/>
        ///	{<br/>
        ///     FieldName = $"{AUTOID}",<br/>
        ///		FieldValue = "IN ('20')",<br/>
        ///		CustomConditionalFunc = new SqlRawConditional((name, value) => $"{name} {value}") //赋值<br/>
        ///	});<br/>
        /// </remarks>
        /// <param name="conditionalFun"></param>
        public SqlRawConditional(Func<string, string, string> conditionalFun)
        {
            _ConditionalFun = conditionalFun;
        }

        public SqlRawConditional(string Conditional)
        {
            _Conditional = Conditional;
        }

        public KeyValuePair<string, SugarParameter[]> GetConditionalSql(ConditionalModel conditionalModel, int index)
        {
            var parameterName = $"@{conditionalModel.FieldName}{index}";
            SugarParameter[] pars = new SugarParameter[]
                {
                     new SugarParameter(parameterName, conditionalModel.FieldValue )
                };
            if (!string.IsNullOrWhiteSpace(_Conditional))
            {
                var sql = $" {conditionalModel.FieldName} {_Conditional} {conditionalModel.FieldValue}";
                //自已处理字符串安全，也可以使用我自带的
                return new KeyValuePair<string, SugarParameter[]>
                    (sql, pars);
            }
            else if (_ConditionalFun != null)
            {
                var sql = $" {_ConditionalFun(conditionalModel.FieldName, conditionalModel.FieldValue)} ";
                return new KeyValuePair<string, SugarParameter[]>
                    (sql, pars);
            }
            else if (_RawConditionalFun != null)
            {
                return _RawConditionalFun(conditionalModel, index);
            }
            return new KeyValuePair<string, SugarParameter[]>("", pars);
        }
    }
}
