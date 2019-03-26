﻿//
// Copyright 2015 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//


using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Services;
using System;
using StackExchange.Redis;

namespace Steeltoe.CloudFoundry.Connector.Redis
{
    public static class RedisCacheServiceCollectionExtensions
    {
    
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration config, ILoggerFactory logFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            RedisCacheConnectorOptions redisConfig = new RedisCacheConnectorOptions(config);
            RedisServiceInfo info = config.GetSingletonServiceInfo<RedisServiceInfo>();
            RedisServiceConnectorFactory factory = new RedisServiceConnectorFactory(info, redisConfig); ;
            services.AddSingleton(typeof(IDistributedCache), factory.CreateCache);
            return services;
        }

        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration config, string serviceName, ILoggerFactory logFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            RedisCacheConnectorOptions redisConfig = new RedisCacheConnectorOptions(config);
            RedisServiceInfo info = config.GetRequiredServiceInfo<RedisServiceInfo>(serviceName);
            RedisServiceConnectorFactory factory = new RedisServiceConnectorFactory(info, redisConfig);
            services.AddSingleton(typeof(IDistributedCache), factory.CreateCache);
            return services;
        }
        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            RedisCacheConnectorOptions redisConfig = new RedisCacheConnectorOptions(config);
            RedisServiceInfo info = config.GetSingletonServiceInfo<RedisServiceInfo>();
            RedisServiceConnectorFactory factory = new RedisServiceConnectorFactory(info, redisConfig); ;
            services.AddSingleton(typeof(IConnectionMultiplexer), factory.CreateConnection);
            return services;
        }

        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services, IConfiguration config, string serviceName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            RedisCacheConnectorOptions redisConfig = new RedisCacheConnectorOptions(config);
            RedisServiceInfo info = config.GetRequiredServiceInfo<RedisServiceInfo>(serviceName);
            RedisServiceConnectorFactory factory = new RedisServiceConnectorFactory(info, redisConfig);
            services.AddSingleton(typeof(IConnectionMultiplexer), factory.CreateConnection);
            return services;
        }

    }
}
