using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WingetNexus.Data.Migrations.PGSql.WingetNexus
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locales",
                columns: table => new
                {
                    PackageLocale = table.Column<string>(type: "TEXT", nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", nullable: true),
                    PublisherUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PublisherSupportUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PrivacyUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Author = table.Column<string>(type: "TEXT", nullable: true),
                    PackageName = table.Column<string>(type: "TEXT", nullable: true),
                    PackageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    License = table.Column<string>(type: "TEXT", nullable: true),
                    LicenseUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Copyright = table.Column<string>(type: "TEXT", nullable: true),
                    CopyrightUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ReleaseNotesUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PurchaseUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.PackageLocale);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: false),
                    VersionCode = table.Column<string>(type: "TEXT", nullable: false),
                    Channel = table.Column<string>(type: "TEXT", nullable: true),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    PackageLocale = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageVersions_Locales_PackageLocale",
                        column: x => x.PackageLocale,
                        principalTable: "Locales",
                        principalColumn: "PackageLocale",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageVersions_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Installers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Architecture = table.Column<string>(type: "TEXT", nullable: false),
                    InstallerType = table.Column<string>(type: "TEXT", nullable: false),
                    InstallerPath = table.Column<string>(type: "TEXT", nullable: false),
                    IsLocalPackage = table.Column<bool>(type: "INTEGER", nullable: false),
                    InstallerSha256 = table.Column<string>(type: "TEXT", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: false),
                    NestedInstallerType = table.Column<string>(type: "TEXT", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PackageVersionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Installers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Installers_PackageVersions_PackageVersionId",
                        column: x => x.PackageVersionId,
                        principalTable: "PackageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstallerSwitches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstallerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallerSwitches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstallerSwitches_Installers_InstallerId",
                        column: x => x.InstallerId,
                        principalTable: "Installers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NestedInstallerFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstallerId = table.Column<int>(type: "INTEGER", nullable: false),
                    RelativeFilePath = table.Column<string>(type: "TEXT", nullable: false),
                    PortableCommandAlias = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NestedInstallerFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NestedInstallerFiles_Installers_InstallerId",
                        column: x => x.InstallerId,
                        principalTable: "Installers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Installers_PackageVersionId",
                table: "Installers",
                column: "PackageVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallerSwitches_InstallerId",
                table: "InstallerSwitches",
                column: "InstallerId");

            migrationBuilder.CreateIndex(
                name: "IX_NestedInstallerFiles_InstallerId",
                table: "NestedInstallerFiles",
                column: "InstallerId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Identifier",
                table: "Packages",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Name",
                table: "Packages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Publisher",
                table: "Packages",
                column: "Publisher");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersions_PackageId_VersionCode",
                table: "PackageVersions",
                columns: new[] { "PackageId", "VersionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersions_PackageLocale",
                table: "PackageVersions",
                column: "PackageLocale");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstallerSwitches");

            migrationBuilder.DropTable(
                name: "NestedInstallerFiles");

            migrationBuilder.DropTable(
                name: "Installers");

            migrationBuilder.DropTable(
                name: "PackageVersions");

            migrationBuilder.DropTable(
                name: "Locales");

            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
