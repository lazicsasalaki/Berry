﻿using System;
using Berry.BLL.BaseManage;
using Berry.Cache;
using Berry.Entity.BaseManage;
using System.Collections.Generic;
using System.Linq;

namespace Berry.App.Cache
{
    /// <summary>
    /// /用户组信息缓存
    /// </summary>
    public class UserGroupCache
    {
        private UserGroupBLL busines = new UserGroupBLL();

        /// <summary>
        /// 用户组列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetList()
        {
            var cacheList = CacheFactory.GetCacheInstance().GetListCache<RoleEntity>(busines.CacheKey, out long toatl);
            if (cacheList == null || cacheList.Count == 0)
            {
                cacheList = busines.GetUserGroupList().ToList();
                CacheFactory.GetCacheInstance().WriteListCache<RoleEntity>(cacheList, busines.CacheKey);
            }
            return cacheList;
        }

        /// <summary>
        /// 用户组列表
        /// </summary>
        /// <param name="organizeId">机构Id</param>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetRoleList(string organizeId)
        {
            var data = this.GetList();
            if (!string.IsNullOrEmpty(organizeId))
            {
                data = data.Where(t => t.OrganizeId == organizeId);
            }
            return data;
        }
    }
}