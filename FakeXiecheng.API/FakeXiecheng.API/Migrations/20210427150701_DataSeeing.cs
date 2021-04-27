using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class DataSeeing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("16cb17a4-1a48-49b0-9600-e52f7a53ea43"), new DateTime(2021, 4, 27, 15, 7, 0, 331, DateTimeKind.Utc).AddTicks(7027), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "shuoming", null, null, null, null, 0m, "TestTitle", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("16cb17a4-1a48-49b0-9600-e52f7a53ea43"));
        }
    }
}
