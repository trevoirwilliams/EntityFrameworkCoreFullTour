using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EntityFrameworkCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "TeamId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "TeamId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "TeamId",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Teams",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Teams",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Teams",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Coaches",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Coaches",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Coaches",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HomeTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "CoachId", "CreatedBy", "CreatedDate", "LeagueId", "ModifiedBy", "ModifiedDate", "Name", "TeamId" },
                values: new object[,]
                {
                    { 1, 0, null, new DateTime(2023, 9, 6, 13, 40, 34, 749, DateTimeKind.Unspecified).AddTicks(644), 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tivoli Gardens F.C.", null },
                    { 2, 0, null, new DateTime(2023, 9, 6, 13, 40, 34, 749, DateTimeKind.Unspecified).AddTicks(653), 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Waterhouse F.C.", null },
                    { 3, 0, null, new DateTime(2023, 9, 6, 13, 40, 34, 749, DateTimeKind.Unspecified).AddTicks(655), 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Humble Lions F.C.", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyColumnType: "INTEGER",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyColumnType: "INTEGER",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyColumnType: "INTEGER",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Coaches");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "TeamId");

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "TeamId", "CreatedDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 8, 31, 20, 42, 11, 206, DateTimeKind.Unspecified).AddTicks(9543), "Tivoli Gardens F.C." },
                    { 2, new DateTime(2023, 8, 31, 20, 42, 11, 206, DateTimeKind.Unspecified).AddTicks(9556), "Waterhouse F.C." },
                    { 3, new DateTime(2023, 8, 31, 20, 42, 11, 206, DateTimeKind.Unspecified).AddTicks(9557), "Humble Lions F.C." }
                });
        }
    }
}
