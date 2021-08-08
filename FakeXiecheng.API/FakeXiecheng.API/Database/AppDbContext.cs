using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXiecheng.API.Moldes;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>//DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //    Id = Guid.NewGuid(),
            //    Title="TestTitle",
            //    Description="shuoming",
            //    OriginalPrice=0,
            //    CreateTime=DateTime.UtcNow
            //});
            //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);:获取当前文件夹绝对路径，引入命名空间：using System.Reflection;
            //更新数据命令：add-migration Dataseeing2
            //更新数据命令：update-database
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);
            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);
            // 初始化用户与角色的种子数据
            // 1. 更新用户与角色的外键关系
            modelBuilder.Entity<ApplicationUser>(u =>
            u.HasMany(x => x.UserRoles)
            .WithOne().HasForeignKey(ur => ur.UserId).IsRequired());
            // 2. 添加管理员角色
            var admiinRoleId = "308660dc-ae51-480f-824d-7dca6714c3e2";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = admiinRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            // 3. 添加用户
            var adminUserId = "90184155-dee0-40c9-bb1e-b5ed07afc04e";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@fakexicheng.com",
                NormalizedUserName = "admin@fakexicheng.com".ToUpper(),
                Email = "admin@fakexicheng.com",
                NormalizedEmail = "admin@fakexicheng.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123465789",
                PhoneNumberConfirmed = false
            };
            // 保存用户信息的时候，用户的登陆密码不能使用明文保存，必须给密码做一个 Hash
            // 创建一个密码的 Hash 工具
            var ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
            // 使用 modelBuilder 来创建用户的数据，向数据库传入刚刚写入的管理员用户信息
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            // 4. 给用户加入管理员角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = admiinRoleId,
                    UserId = adminUserId
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}
