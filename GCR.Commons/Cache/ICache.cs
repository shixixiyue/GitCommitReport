
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GCR.Commons
{
    /// <summary>
    /// 缓存操作接口类
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false);

        /// <summary>
        /// 添加缓存 默认1天
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        void Set(string key, object value, bool isSliding = false);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        void Set(string key, object value, TimeSpan ts);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public void Set(string key, object value, int seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            Set(key, value, ts, false);
        }

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        void Remove(string key);

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <returns></returns>
        void RemoveAll(IEnumerable<string> keys);

        #endregion 删除缓存

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        IDictionary<string, object> GetAll(IEnumerable<string> keys);

        #endregion 获取缓存

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        void RemoveCacheAll();

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        void RemoveCacheRegex(string pattern);

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IList<string> SearchCacheRegex(string pattern);

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        List<string> GetCacheKeys();
    }

    #region 类型定义

    /// <summary>
    /// 值信息 RedisCache
    /// </summary>
    public struct ValueInfoEntry
    {
        /// <summary>
        ///
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public TimeSpan? ExpireTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ExpireType? ExpireType { get; set; }
    }

    /// <summary>
    /// 过期类型
    /// </summary>
    public enum ExpireType
    {
        /// <summary>
        /// 绝对过期
        /// 注：即自创建一段时间后就过期
        /// </summary>
        Absolute,

        /// <summary>
        /// 相对过期
        /// 注：即该键未被访问后一段时间后过期，若此键一直被访问则过期时间自动延长
        /// </summary>
        Relative,
    }

    #endregion 类型定义
}
