using FakeXiecheng.API.Database;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FakeXiecheng.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction=>{
                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(
                //    new XmlDataContractSerializerOutputFormatter());
            }).AddXmlDataContractSerializerFormatters();
            //services.AddTransient<ITouristRouteRepository, MockTouristRouteRepository>();
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            //services.AddTransient:ÿһ�����󶼻ᴴ��Ҫ�����ݲֿ⣬����������ͷ���������Ĳֿ�
            //services.AddSingleton:���з�������ʱ������һ����
            //services.AddScoped:�����������ַ���֮��,
            services.AddDbContext<AppDbContext>(option=> {
                //option.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                //option.UseSqlServer("");
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);//����Docker�е�SQL server���ݿ�
                //option.UseMySql(Configuration["DbContext:MySQLConnectionString"]);//����������е�MySQL���ݿ�
            });
            //ɨ��profile�ļ�
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
                //    throw new Exception("����һ�����Դ���");
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
