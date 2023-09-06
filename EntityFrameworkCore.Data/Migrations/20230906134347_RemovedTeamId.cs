using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTeamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Teams");

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2023, 9, 6, 13, 43, 47, 24, DateTimeKind.Unspecified).AddTicks(2703));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2023, 9, 6, 13, 43, 47, 24, DateTimeKind.Unspecified).AddTicks(2711));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2023, 9, 6, 13, 43, 47, 24, DateTimeKind.Unspecified).AddTicks(2712));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Teams",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "TeamId" },
                values: new object[] { new DateTime(2023, 9, 6, 13, 40, 34, 749, DateTimeKind.Unspecified).AddTicks(644), null });

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "TeamId" },
                values: new object[] { new DateTime(2023, 9, 6, 13, 40, 34, 749, DateTimeKind.Unspecified).AddTicks(653), null });

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "TeamId" },
                values: new object[] { new DateTime(2023, 9, 6, 13, 40, 34, 749, DateTimeKind.Unspecified).AddTicks(655), null });
        }
    }
}
