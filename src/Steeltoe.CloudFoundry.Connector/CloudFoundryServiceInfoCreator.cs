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
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.CloudFoundry.Connector.App;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace Steeltoe.CloudFoundry.Connector
{
    public class CloudFoundryServiceInfoCreator
    {
        private IList<IServiceInfo> _serviceInfos = new List<IServiceInfo>();
        public IList<IServiceInfo> ServiceInfos { get { return _serviceInfos;  } }

        private IList<IServiceInfoFactory> _factories = new List<IServiceInfoFactory>();
        internal IList<IServiceInfoFactory> Factories {  get { return _factories;  } }

        private static IConfiguration _config;
        private static CloudFoundryServiceInfoCreator _me = null;

        private static object _lock = new object();

        public static CloudFoundryServiceInfoCreator Instance(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (config == _config)
            {
                return _me;
            }

            lock(_lock)
            {
                if (config == _config)
                {
                    return _me;
                }

                _me = new CloudFoundryServiceInfoCreator(config);
            }
            return _me;

        }

        internal CloudFoundryServiceInfoCreator(IConfiguration config)
        {

            _config = config;
            BuildServiceInfoFactories();
            BuildServiceInfos();

        }

        public List<SI> GetServiceInfos<SI>() where SI : class
        {

            List<SI> results = new List<SI>();
            foreach (IServiceInfo info in _serviceInfos)
            {
                SI si = info as SI;
                if (si != null)
                {
                    results.Add(si);
                }

            }
            return results;
        }

        public SI GetServiceInfo<SI>(string name) where SI : class
        {
            List<SI> typed = GetServiceInfos<SI>();
            foreach(var si in typed)
            {
                var info = si as IServiceInfo;
                if (info.Id.Equals(name))
                    return (SI)info;
            }
            return null;
        }

        public List<IServiceInfo> GetServiceInfos(Type type)
        {
            return _serviceInfos.Where( (_info) => _info.GetType() == type ).ToList();
        }

        public IServiceInfo GetServiceInfo(string name)
        {
            return _serviceInfos.Where((_info) => _info.Id.Equals(name) ).FirstOrDefault();
        }

        internal void BuildServiceInfoFactories()
        {
            _factories.Clear();

            var assembly = GetType().GetTypeInfo().Assembly;
            var types = assembly.DefinedTypes;
            foreach(var type in types)
            {
                if (type.IsDefined(typeof(ServiceInfoFactoryAttribute)))
                {
                    IServiceInfoFactory instance = CreateServiceInfoFactory(type.DeclaredConstructors);
                    if (instance != null)
                    {
                        _factories.Add(instance);
                    }
                }
            }
        }

        private IServiceInfoFactory CreateServiceInfoFactory(IEnumerable<ConstructorInfo> declaredConstructors)
        {
            IServiceInfoFactory result = null;
            foreach(ConstructorInfo ci in declaredConstructors)
            {
                if (ci.GetParameters().Length == 0 && ci.IsPublic && !ci.IsStatic)
                {
                    result = ci.Invoke(null) as IServiceInfoFactory;
                    break;
                }
            }

            return result;
        }

        private void BuildServiceInfos()
        {
            _serviceInfos.Clear();

            CloudFoundryApplicationOptions appOpts = new CloudFoundryApplicationOptions();
            _config.Bind(appOpts);
            ApplicationInstanceInfo appInfo = new ApplicationInstanceInfo(appOpts);  

            CloudFoundryServicesOptions serviceOpts = new CloudFoundryServicesOptions();
            _config.Bind(serviceOpts);

            foreach (Service s in serviceOpts.Services)
            {
                IServiceInfoFactory factory = FindFactory(s);
                if (factory != null)
                {
                    var info = factory.Create(s) as ServiceInfo;
                    if (info != null)
                    {
                        info.ApplicationInfo = appInfo;
                        _serviceInfos.Add(info);
                    }
                }
            }
        }

        private IServiceInfoFactory FindFactory(Service s)
        {
            foreach (IServiceInfoFactory f in _factories)
            {
                if (f.Accept(s))
                {
                    return f;
                }
            }
            return null;
        }
    }
}
