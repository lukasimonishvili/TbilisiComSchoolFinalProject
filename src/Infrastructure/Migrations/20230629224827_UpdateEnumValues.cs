using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class UpdateEnumValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BirthDate", "Password", "Role" },
                values: new object[] { new DateTime(2023, 6, 30, 2, 48, 27, 61, DateTimeKind.Local).AddTicks(5208), "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG", "Accountant" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BirthDate", "Password", "Role" },
                values: new object[] { new DateTime(2023, 6, 30, 2, 45, 33, 315, DateTimeKind.Local).AddTicks(8431), "$2a$10$/Kc/mmZ3/oM9zo8Psg2Qc.r1Cz7xVB5WRoieBgeQb8FAdh3WHRSBm", null });
        }
    }
}
