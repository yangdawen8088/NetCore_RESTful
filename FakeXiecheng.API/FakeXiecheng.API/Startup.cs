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
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using FakeXiecheng.API.Moldes;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            // 注入系统身份认证服务依赖
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            // 注入身份认证服务
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // 是否验证 Token 的发布者
                        ValidateIssuer = true,
                        // 设置发布者是谁
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        // 第一部分表示只有后端设置的 "fakexiecheng.com" 发出的 Token 才会被接受

                        // 是否验证 Token 的持有者
                        ValidateAudience = true,
                        // 设置持有者是谁
                        ValidAudience = Configuration["Authentication:Audience"],

                        // 设置 Token 是否需要过期
                        ValidateLifetime = true,

                        // 从配置文件中将 Token 的私钥传入进来 并且进行加密
                        IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                    };
                });
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(
                //    new XmlDataContractSerializerOutputFormatter());
            })
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetail = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "无所谓",
                        Title = "数据验证失败",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "请看详细说明",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetail)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });//设置这个过后，用户可以在请求头部中设置返回的数据格式，如json、xml等
            //services.AddTransient<ITouristRouteRepository, MockTouristRouteRepository>();
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            //services.AddTransient:每一次请求都会创建要给数据仓库，请求结束后将释放这个创建的仓库
            //services.AddSingleton:所有服务请求时共用这一个，
            //services.AddScoped:介于以上两种方法之间,
            services.AddDbContext<AppDbContext>(option =>
            {
                //option.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                //option.UseSqlServer("");
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);//这是Docker中的SQL server数据库
                //option.UseMySql(Configuration["DbContext:MySQLConnectionString"]);//这是物理机中的MySQL数据库
            });
            //扫描profile文件
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient();
            // URLHelper 服务注册
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // 注入 PropertyMappingService 服务依赖
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 表示 你在哪里？
            app.UseRouting();
            // 表示 你是谁？
            app.UseAuthentication();
            // 表示 你可以干什么？你有什么权限？
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/test", async context =>
                //{
                //    throw new Exception("这是一个测试错误！");
                //    //await context.Response.WriteAsync("Hello from test!");
                //});
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello FakeXiecheng.API");
                });
                endpoints.MapControllers();
            });
        }
    }
}
