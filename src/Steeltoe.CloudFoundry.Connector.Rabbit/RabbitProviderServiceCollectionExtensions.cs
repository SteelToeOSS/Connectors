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

namespace Steeltoe.CloudFoundry.Connector.Rabbit
{
    public static class RabbitProviderServiceCollectionExtensions
    {

        public static IServiceCollection AddRabbitConnection(this IServiceCollection services, IConfiguration config, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ILoggerFactory logFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            RabbitServiceInfo info = config.GetSingletonServiceInfo<RabbitServiceInfo>();

            DoAdd(services, info, config, contextLifetime);
            return services;
        }

        public static IServiceCollection AddRabbitConnection(this IServiceCollection services, IConfiguration config, string serviceName, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ILoggerFactory logFactory = null)
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
            RabbitServiceInfo info = config.GetRequiredServiceInfo<RabbitServiceInfo>(serviceName);

            DoAdd(services, info, config, contextLifetime);
            return services;
        }

        
        private static string[] rabbitAssemblies = new string[] {"RabbitMQ.Client"};
        private static string[] rabbitTypeNames = new string[] {"RabbitMQ.Client.ConnectionFactory"};
        private static void DoAdd(IServiceCollection services, RabbitServiceInfo info, IConfiguration config, ServiceLifetime contextLifetime)
        {
            Type rabbitFactory = ConnectorHelpers.FindType(rabbitAssemblies, rabbitTypeNames);
            if (rabbitFactory == null) {
                throw new ConnectorException("Unable to find ConnectionFactory, are you missing RabbitMQ assembly");
            }
            RabbitProviderConnectorOptions rabbitConfig = new RabbitProviderConnectorOptions(config);
            RabbitProviderConnectorFactory factory = new RabbitProviderConnectorFactory(info, rabbitConfig, rabbitFactory);
            services.Add(new ServiceDescriptor(rabbitFactory, factory.Create, contextLifetime));
        }


    }
}
