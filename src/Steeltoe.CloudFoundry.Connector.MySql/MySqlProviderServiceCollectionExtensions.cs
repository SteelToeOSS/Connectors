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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Services;
using System;


namespace Steeltoe.CloudFoundry.Connector.MySql
{
    public static class MySqlProviderServiceCollectionExtensions
    {

        public static IServiceCollection AddMySqlConnection(this IServiceCollection services, IConfiguration config, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ILoggerFactory logFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            MySqlServiceInfo info = config.GetSingletonServiceInfo<MySqlServiceInfo>();

            DoAdd(services, info, config, contextLifetime);
            return services;
        }

        public static IServiceCollection AddMySqlConnection(this IServiceCollection services, IConfiguration config, string serviceName, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ILoggerFactory logFactory = null)
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
            MySqlServiceInfo info = config.GetRequiredServiceInfo<MySqlServiceInfo>(serviceName);

            DoAdd(services, info, config, contextLifetime);
            return services;
        }

        private static string[] mySqlAssemblies = new string[] {"MySql.Data", "MySqlConnector"};
        private static string[] mySqlTypeNames = new string[] {"MySql.Data.MySqlClient.MySqlConnection","MySql.Data.MySqlClient.MySqlConnection"};
        private static void DoAdd(IServiceCollection services, MySqlServiceInfo info, IConfiguration config, ServiceLifetime contextLifetime)
        {
            Type mySqlConnection = ConnectorHelpers.FindType(mySqlAssemblies, mySqlTypeNames);
            if (mySqlConnection == null) {
                throw new ConnectorException("Unable to find MySqlConnection, are you missing MySql ADO.NET assembly");
            }

            MySqlProviderConnectorOptions mySqlConfig = new MySqlProviderConnectorOptions(config);
            MySqlProviderConnectorFactory factory = new MySqlProviderConnectorFactory(info, mySqlConfig, mySqlConnection);
            services.Add(new ServiceDescriptor(mySqlConnection, factory.Create, contextLifetime));
        }

    }
}
