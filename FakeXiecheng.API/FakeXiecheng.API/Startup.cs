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
            // ע��ϵͳ�����֤��������
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            // ע�������֤����
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // �Ƿ���֤ Token �ķ�����
                        ValidateIssuer = true,
                        // ���÷�������˭
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        // ��һ���ֱ�ʾֻ�к�����õ� "fakexiecheng.com" ������ Token �Żᱻ����

                        // �Ƿ���֤ Token �ĳ�����
                        ValidateAudience = true,
                        // ���ó�������˭
                        ValidAudience = Configuration["Authentication:Audience"],

                        // ���� Token �Ƿ���Ҫ����
                        ValidateLifetime = true,

                        // �������ļ��н� Token ��˽Կ������� ���ҽ��м���
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
                        Type = "����ν",
                        Title = "������֤ʧ��",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "�뿴��ϸ˵��",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetail)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });//������������û�����������ͷ�������÷��ص����ݸ�ʽ����json��xml��
            //services.AddTransient<ITouristRouteRepository, MockTouristRouteRepository>();
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            //services.AddTransient:ÿһ�����󶼻ᴴ��Ҫ�����ݲֿ⣬����������ͷ���������Ĳֿ�
            //services.AddSingleton:���з�������ʱ������һ����
            //services.AddScoped:�����������ַ���֮��,
            services.AddDbContext<AppDbContext>(option =>
            {
                //option.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                //option.UseSqlServer("");
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);//����Docker�е�SQL server���ݿ�
                //option.UseMySql(Configuration["DbContext:MySQLConnectionString"]);//����������е�MySQL���ݿ�
            });
            //ɨ��profile�ļ�
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient();
            // URLHelper ����ע��
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // ע�� PropertyMappingService ��������
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ��ʾ �������
            app.UseRouting();
            // ��ʾ ����˭��
            app.UseAuthentication();
            // ��ʾ ����Ը�ʲô������ʲôȨ�ޣ�
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/test", async context =>
                //{
                //    throw new Exception("����һ�����Դ���");
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
