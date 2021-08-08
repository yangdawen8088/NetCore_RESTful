using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class ShoppingCartMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_ApplicationUserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_ApplicationUserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_touristRoutePictures_TouristRoutes_TouristRouteId",
                table: "touristRoutePictures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_touristRoutePictures",
                table: "touristRoutePictures");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserTokens_ApplicationUserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserClaims_ApplicationUserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "touristRoutePictures",
                newName: "TouristRoutePictures");

            migrationBuilder.RenameIndex(
                name: "IX_touristRoutePictures_TouristRouteId",
                table: "TouristRoutePictures",
                newName: "IX_TouristRoutePictures_TouristRouteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TouristRoutePictures",
                table: "TouristRoutePictures",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingCartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPresent = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineItems_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineItems_TouristRoutes_TouristRouteId",
                        column: x => x.TouristRouteId,
                        principalTable: "TouristRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "4c576579-50cf-437c-8070-9c1f4225c0a9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "045e12be-f6af-431d-b63f-cefa42f57b92", "AQAAAAEAACcQAAAAEOGq5fnkBGofHJ+N4o+O8Af0a+trlyf8Z4yK3mX3fIGXM0C/H8w0OA8q8cnSl+Am+w==", "ef0c623c-c596-4e84-95e3-29da20450ec8" });

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_ShoppingCartId",
                table: "LineItems",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_TouristRouteId",
                table: "LineItems",
                column: "TouristRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristRoutePictures_TouristRoutes_TouristRouteId",
                table: "TouristRoutePictures",
                column: "TouristRouteId",
                principalTable: "TouristRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristRoutePictures_TouristRoutes_TouristRouteId",
                table: "TouristRoutePictures");

            migrationBuilder.DropTable(
                name: "LineItems");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TouristRoutePictures",
                table: "TouristRoutePictures");

            migrationBuilder.RenameTable(
                name: "TouristRoutePictures",
                newName: "touristRoutePictures");

            migrationBuilder.RenameIndex(
                name: "IX_TouristRoutePictures_TouristRouteId",
                table: "touristRoutePictures",
                newName: "IX_touristRoutePictures_TouristRouteId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUserClaims",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_touristRoutePictures",
                table: "touristRoutePictures",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "af82f9d0-446c-4a26-862e-e4027c63cde3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c2cbfeb8-a130-47f1-96cd-cf7544d0caff", "AQAAAAEAACcQAAAAEJTuQaphPcIs+Gw38DKijvOmlUffA0uE/FUo+uawNa4vKnWqHPMB1AdUNYHEMIC7MQ==", "369cd935-9bd4-4f5a-b6dc-2fd40bf3c907" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserTokens_ApplicationUserId",
                table: "AspNetUserTokens",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_ApplicationUserId",
                table: "AspNetUserClaims",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_ApplicationUserId",
                table: "AspNetUserClaims",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_ApplicationUserId",
                table: "AspNetUserTokens",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_touristRoutePictures_TouristRoutes_TouristRouteId",
                table: "touristRoutePictures",
                column: "TouristRouteId",
                principalTable: "TouristRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
