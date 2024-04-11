using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WingetNexus.Data.Migrations.PGSql.WingetNexus
{
    /// <inheritdoc />
    public partial class PackageDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Installers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseDate",
                table: "Installers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpgradeBehavior",
                table: "Installers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PackageDependencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumVersion = table.Column<string>(type: "TEXT", nullable: true),
                    InstallerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageDependencies_Installers_InstallerId",
                        column: x => x.InstallerId,
                        principalTable: "Installers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackageDependencies_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_InstallerId",
                table: "PackageDependencies",
                column: "InstallerId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_PackageId",
                table: "PackageDependencies",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageDependencies");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Installers");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Installers");

            migrationBuilder.DropColumn(
                name: "UpgradeBehavior",
                table: "Installers");
        }
    }
}
