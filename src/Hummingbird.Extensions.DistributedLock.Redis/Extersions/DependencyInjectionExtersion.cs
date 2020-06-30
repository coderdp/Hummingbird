﻿
using Hummingbird.Core;


using Hummingbird.Extensions.DistributedLock;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {


        public static IHummingbirdHostBuilder AddDistributedLock(this IHummingbirdHostBuilder hostBuilder, Action<Config> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));
            var RedisDistributedLock = DistributedLockFactory.Build(action);
            hostBuilder.Services.AddSingleton<IDistributedLock>(RedisDistributedLock);
            return hostBuilder;

        }

    }
}

namespace Hummingbird.Extensions.DistributedLock
{
    public static class DistributedLockFactory
    {
        public static IDistributedLock Build(Action<Config> configSetup)
        {
            var config = new Config();
            configSetup(config);

            return new RedisDistributedLock(Cacheing.CacheFactory.Build(option =>
            {
                option.WithDb(config.DBNum);
                option.WithKeyPrefix(config.KeyPrefix);
                option.WithWriteServerList(config.WriteServerList);
                option.WithReadServerList(config.WriteServerList);
                option.WithPassword(config.Password);
                option.WithSsl(config.Ssl);

            }));
        }
    }
}