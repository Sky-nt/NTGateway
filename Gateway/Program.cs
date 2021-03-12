using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(option =>
                    {
                        option.Limits.MaxRequestBodySize = null;
                    });
                    webBuilder.ConfigureAppConfiguration((context, builder) =>
                    {
                        //3.1�������������д����
                        //��һ��:
                        builder.AddJsonFile("ocelot.global.json", true, true);
                        builder.AddJsonFile($"ocelot.{context.HostingEnvironment.EnvironmentName}.json", true, true);

                        //�ڶ��֣�
                        builder.AddOcelot(context.HostingEnvironment);
                    });
                    webBuilder.ConfigureServices(service =>
                    {
                        //���Ocelot������Ϊֻ��Ҫ��һ����
                        //���Բ���Ҫ������Startup����
                        service.AddOcelot();
                    });
                    webBuilder.Configure(app =>
                    {
                        //ע�������м��
                        app.UseOcelot();
                    });
                    webBuilder.UseUrls("http://*:5000");
                });
    }
}