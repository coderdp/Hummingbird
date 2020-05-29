// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using RabbitMQ.Client;
using System;
using System.Data;

namespace Hummingbird.Extensions.HealthChecks
{
    public class RabbitMqOption
    {

        /// <summary>
        /// �ս������
        /// </summary>
        /// <param name="HostName">��ַ</param>
        /// <param name="Port">�˿�</param>
        public void WithEndPoint(string HostName = "locahost", int Port = 5672)
        {
            this.HostName = HostName;
            this.Port = Port;
        }


        /// <summary>
        /// ������֤��Ϣ
        /// </summary>
        /// <param name="UserName">�˺�</param>
        /// <param name="Password">����</param>
        public void WithAuth(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }


        /// <summary>
        /// ���ý�������Ϣ
        /// </summary>
        /// <param name="VirtualHost">�����</param>
        /// <param name="ExchangeType">����������</param>
        /// <param name="Exchange">����������</param>
        public void WithExchange(string VirtualHost = "/")
        {
            this.VirtualHost = VirtualHost;
        }

       
        #region Endpoint
        /// <summary>
        /// ��������ַ(Ĭ��:localhost)
        /// </summary>
        internal string HostName { get; set; } = "locahost";
        /// <summary>
        /// �˿ڣ�Ĭ�ϣ�5672��
        /// </summary>
        internal int Port { get; set; } = 5672;
        #endregion

        #region Auth

        /// <summary>
        /// �˺�(Ĭ��:guest)
        /// </summary>
        internal string UserName { get; set; } = "guest";

        /// <summary>
        /// ����(Ĭ��:guest)
        /// </summary>
        internal string Password { get; set; } = "guest";
        #endregion

        #region Exchange
        /// <summary>
        /// ��������(Ĭ�ϣ�/)
        /// </summary>
        internal string VirtualHost { get; set; } = "/";

        #endregion



    }
    public static class HealthCheckBuilderRabbitmqExtensions
    {
        public static HealthCheckBuilder AddRabbitMQCheck(this HealthCheckBuilder builder, string name, Action<RabbitMqOption> setup)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return AddRabbitMQCheck(builder, name, setup, builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddRabbitMQCheck(this HealthCheckBuilder builder, string name, Action<RabbitMqOption> setup, TimeSpan cacheDuration)
        {
            var option = new RabbitMqOption();
            setup(option);
            var factory = new ConnectionFactory();
            factory.HostName = option.HostName;
            factory.Port = option.Port;
            factory.Password = option.Password;
            factory.UserName = option.UserName;
            factory.VirtualHost = option.VirtualHost;
            factory.AutomaticRecoveryEnabled = true;
            factory.TopologyRecoveryEnabled = true;
            factory.UseBackgroundThreadsForIO = true;
            var hosts = option.HostName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            builder.AddCheck($"RabbitMqCheck({name})", () =>
            {
                try
                {
                    using (var connection = factory.CreateConnection(hosts))
                    {
                        if(connection.IsOpen)
                        {
                            return HealthCheckResult.Healthy($"Healthy");
                        }
                        else
                        {
                            return HealthCheckResult.Unhealthy($"Unhealthy");

                        }
                    }                    
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"{ex.GetType().FullName}");
                }
            }, cacheDuration);

            return builder;
        }
    }
}
