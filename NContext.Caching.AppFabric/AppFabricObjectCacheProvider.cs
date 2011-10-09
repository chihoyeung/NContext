﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppFabricObjectCacheProvider.cs">
//   This file is part of NContext.
//
//   NContext is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or any later version.
//
//   NContext is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with NContext.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Defines a caching provider for AppFabric.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Caching;

using Microsoft.ApplicationServer.Caching;

namespace NContext.Application.Caching
{
    /// <summary>
    /// Defines a caching provider for AppFabric.
    /// </summary>
    public class AppFabricObjectCacheProvider : ObjectCache
    {
        #region Constants and Fields

        /// <summary>
        /// The region key template.
        /// </summary>
        private const String regionKeyTemplate = "{0}:{1}";

        /// <summary>
        /// The cache factory.
        /// </summary>
        private readonly DataCacheFactory cacheFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFabricObjectCacheProvider"/> class.
        /// </summary>
        public AppFabricObjectCacheProvider()
        {
            cacheFactory = new DataCacheFactory();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets DefaultCacheCapabilities.
        /// </summary>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return DefaultCacheCapabilities.OutOfProcessProvider | DefaultCacheCapabilities.AbsoluteExpirations |
                       DefaultCacheCapabilities.SlidingExpirations;
            }
        }

        /// <summary>
        /// Gets Name.
        /// </summary>
        public override String Name
        {
            get
            {
                return "AppFabricCacheService";
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// The 
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override Object this[String key]
        {
            get
            {
                return Get(key);
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add or get existing.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The add or get existing.
        /// </returns>
        public override Object AddOrGetExisting(
            String key, Object value, CacheItemPolicy policy, String regionName = null)
        {
            return AddOrGetExisting(new CacheItem(key, value, regionName), policy).Value;
        }

        /// <summary>
        /// The add or get existing.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="absoluteExpiration">
        /// The absolute expiration.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The add or get existing.
        /// </returns>
        public override Object AddOrGetExisting(
            String key, Object value, DateTimeOffset absoluteExpiration, String regionName = null)
        {
            return
                AddOrGetExisting(
                    new CacheItem(key, value, regionName), 
                    new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration }).Value;
        }

        /// <summary>
        /// The add or get existing.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <returns>
        /// </returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            var data = Get(value.Key, value.RegionName);
            if (data != null)
            {
                return new CacheItem(value.Key, data, value.RegionName);
            }

            Set(value, policy);
            return value;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The contains.
        /// </returns>
        public override Boolean Contains(String key, String regionName = null)
        {
            return Get(key, regionName) != null;
        }

        /// <summary>
        /// The create cache entry change monitor.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(
            IEnumerable<String> keys, String regionName = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The get.
        /// </returns>
        public override Object Get(String key, String regionName = null)
        {
            try
            {
                return cacheFactory.GetDefaultCache().Get(GetKey(key, regionName));
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RetryLater)
                {
                    // temporal failure, ignore and continue
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        /// The get cache item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// </returns>
        public override CacheItem GetCacheItem(String key, String regionName = null)
        {
            var data = Get(key, regionName);
            if (data != null)
            {
                return new CacheItem(key, data, regionName);
                
            }

            return null;
        }

        /// <summary>
        /// The get count.
        /// </summary>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The get count.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override Int64 GetCount(String regionName = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The get values.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override IDictionary<String, Object> GetValues(IEnumerable<String> keys, String regionName = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The remove.
        /// </returns>
        public override Object Remove(String key, String regionName = null)
        {
            var data = Get(key, regionName);
            if (data != null)
            {
                cacheFactory.GetDefaultCache().Remove(GetKey(key, regionName));
                return data;
            }

            return null;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        public override void Set(String key, Object value, CacheItemPolicy policy, String regionName = null)
        {
            Set(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="absoluteExpiration">
        /// The absolute expiration.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        public override void Set(String key, Object value, DateTimeOffset absoluteExpiration, String regionName = null)
        {
            Set(
                new CacheItem(key, value, regionName), new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            if (item.Value == null)
            {
                return;
            }

            try
            {
                TimeSpan timeout;
                if (policy.SlidingExpiration != TimeSpan.Zero)
                {
                    timeout = policy.SlidingExpiration;
                }
                else
                {
                    timeout = policy.AbsoluteExpiration - DateTime.UtcNow;
                }

                cacheFactory.GetDefaultCache().Put(GetKey(item.Key, item.RegionName), item.Value, timeout);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RetryLater)
                {
                    // temporal failure, ignore and continue
                    return;
                }

                throw;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        protected override IEnumerator<KeyValuePair<String, Object>> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The get key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <returns>
        /// The get key.
        /// </returns>
        private static String GetKey(String key, String regionName)
        {
            if (String.IsNullOrWhiteSpace(regionName))
            {
                return key;
            }

            return String.Format(CultureInfo.InvariantCulture, regionKeyTemplate, key, regionName);
        }

        #endregion
    }
}