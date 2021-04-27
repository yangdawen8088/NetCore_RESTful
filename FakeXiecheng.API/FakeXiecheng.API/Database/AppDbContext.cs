using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXiecheng.API.Moldes;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> touristRoutePictures { get; set; }
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
            base.OnModelCreating(modelBuilder); 
        }
    }
}
