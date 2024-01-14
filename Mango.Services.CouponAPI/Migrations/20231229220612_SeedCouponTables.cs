using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mango.Services.CouponAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCouponTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10PERCENTMANGO", 10.0, 100 },
                    { 2, "20PERCENTMANGO", 20.0, 200 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2);
        }
    }
}
