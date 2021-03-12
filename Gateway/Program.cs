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
                        //3.1这里可以有两种写法：
                        //第一种:
                        builder.AddJsonFile("ocelot.global.json", true, true);
                        builder.AddJsonFile($"ocelot.{context.HostingEnvironment.EnvironmentName}.json", true, true);

                        //第二种：
                        builder.AddOcelot(context.HostingEnvironment);
                    });
                    webBuilder.ConfigureServices(service =>
                    {
                        //添加Ocelot服务，因为只需要这一个，
                        //所以不需要再整个Startup类了
                        service.AddOcelot();
                    });
                    webBuilder.Configure(app =>
                    {
                        //注册请求中间件
                        app.UseOcelot();
                    });
                    webBuilder.UseUrls("http://*:5000");
                });
    }
}