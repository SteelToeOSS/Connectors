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

using Steeltoe.CloudFoundry.Connector.App;
using Steeltoe.CloudFoundry.Connector.Services;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Redis.Test
{
    public class RedisServiceConnectorFactoryTest
    {
        [Fact]
        public void CreateCache_ReturnsRedisCache()
        {
            RedisCacheConnectorOptions config = new RedisCacheConnectorOptions()
            {
                Host = "localhost",
                Port = 1234,
                Password = "password",
                InstanceId = "instanceId"
            };
            RedisServiceInfo si = new RedisServiceInfo("myId", "foobar", 4321, "sipassword");
            si.ApplicationInfo = new ApplicationInstanceInfo()
            {
                InstanceId = "instanceId"
            };
            var factory = new RedisServiceConnectorFactory(si, config);
            var cache = factory.CreateCache(null);
            Assert.NotNull(cache);
        }
        [Fact]
        public void CreateConnection_ReturnsConnectinMultiplexer()
        {
            RedisCacheConnectorOptions config = new RedisCacheConnectorOptions()
            {
                Host = "localhost",
                Port = 1234,
                Password = "password",
                InstanceId = "instanceId", 
                AbortOnConnectFail = false
            };
            RedisServiceInfo si = new RedisServiceInfo("myId", "127.0.0.1", 4321, "sipassword");
            si.ApplicationInfo = new ApplicationInstanceInfo()
            {
                InstanceId = "instanceId"
            };
            var factory = new RedisServiceConnectorFactory(si, config);
            var multi = factory.CreateConnection(null);
            Assert.NotNull(multi);
        }
    }
}
