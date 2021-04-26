using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<ITouristRouteRepository, MockTouristRouteRepository>();
            //services.AddTransient:每一次请求都会创建要给数据仓库，请求结束后将释放这个创建的仓库
            //services.AddSingleton:所有服务请求时共用这一个，
            //services.AddScoped:介于以上两种方法之间,
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/test", async context =>
                //{
                //    throw new Exception("这是一个测试错误！");
                //    //await context.Response.WriteAsync("Hello from test!");
                //});
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello world!");
                //});
                endpoints.MapControllers();
            });
        }
    }
}
