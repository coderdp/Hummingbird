﻿using Hummingbird.Extensions.Cacheing;
using Hummingbird.Extensions.Cacheing.StackExchange;
using Hummingbird.Extensions.Cacheing.StackExchangeImplement;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
#if NETSTANDARD || NET461
        public static Hummingbird.Core.IHummingbirdHostBuilder AddCacheing(this Hummingbird.Core.IHummingbirdHostBuilder hostBuilder, Action<RedisCacheConfig> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));

            hostBuilder.Services.AddSingleton<ICacheManager>(CacheFactory.Build(action));

            return hostBuilder;

        }
#endif
    }


}

namespace Hummingbird.Extensions.Cacheing
{

    public static class CacheFactory
    {
        public static ICacheManager Build(Action<RedisCacheConfig> action)
        {
            var option = new RedisCacheConfig();
            action(option);

            var cacheManager = RedisCacheManage.Create(option);
            return cacheManager;
        }

        public static ICacheManager Build(RedisCacheConfig option)
        {
            var cacheManager = RedisCacheManage.Create(option);
            return cacheManager;
        }
    }
}