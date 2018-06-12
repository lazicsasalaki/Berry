﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Berry.Cache
{
    public class RedisCache : ICache
    {
        private static RedisHelper redisHelper = new RedisHelper();

        /// <summary>
        /// 写入缓存，单体，默认过期时间10分钟
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        public void WriteCache<T>(T value, string cacheKey) where T : class
        {
            WriteCache<T>(value, cacheKey, DateTime.Now.AddMinutes(10));
        }

        /// <summary>
        /// 写入缓存，单体
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        /// <param name="expireTime">到期时间</param>
        public void WriteCache<T>(T value, string cacheKey, DateTime expireTime) where T : class
        {
            TimeSpan span = expireTime - DateTime.Now;

            Type type = typeof(T);
            if (type == typeof(string))
            {
                redisHelper.StringSet<T>(cacheKey,value, span);
            }
            else if (type == typeof(T))
            {
                redisHelper.ListRightPush<T>(cacheKey, value);

                redisHelper.KeyExpire(cacheKey, span);
            }
        }

        /// <summary>
        /// 写入缓存，集合，默认过期时间10分钟
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="cacheKey"></param>
        public void WriteListCache<T>(List<T> value, string cacheKey) where T : class
        {
            WriteListCache<T>(value, cacheKey, DateTime.Now.AddMinutes(10));
        }

        /// <summary>
        /// 写入缓存，集合
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        /// <param name="expireTime">到期时间</param>
        public void WriteListCache<T>(List<T> value, string cacheKey, DateTime expireTime) where T : class
        {
            TimeSpan span = expireTime - DateTime.Now;

            redisHelper.ListRightPush<List<T>>(cacheKey, value);

            redisHelper.KeyExpire(cacheKey, span);
        }

        /// <summary>
        /// 读取缓存，单体
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <returns></returns>
        public T GetCache<T>(string cacheKey) where T : class
        {
            Type type = typeof(T);
            T res = null;
            if (type == typeof(string))
            {
                res = redisHelper.StringGet(cacheKey) as T;
            }
            else if (type == typeof(T))
            {
                res = redisHelper.StringGet<T>(cacheKey) as T;
            }
            return res;
        }

        /// <summary>
        /// 读取缓存，集合
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <param name="total">当前Key下面缓存记录数</param>
        /// <returns></returns>
        public List<T> GetListCache<T>(string cacheKey, out long total) where T : class
        {
            List<T> res = redisHelper.ListRange<T>(cacheKey) as List<T>;
            total = redisHelper.ListLength(cacheKey);

            return res;
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        public void RemoveCache(string cacheKey)
        {
            redisHelper.KeyDelete(cacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public void RemoveCache()
        {
            List<string> keys = redisHelper.GetKeys();
            foreach (string key in keys)
            {
                redisHelper.KeyDelete(key);
            }
        }
    }
}