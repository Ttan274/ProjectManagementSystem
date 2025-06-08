using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagementSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class teamMetricsAndAppInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Tasks` CHANGE COLUMN `TaskEffort` `State` int NULL;");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Tasks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EffortScore",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    AppCode = table.Column<int>(type: "int", nullable: false),
                    GitHubPatToken = table.Column<string>(type: "longtext", nullable: true),
                    GitHubOwner = table.Column<string>(type: "longtext", nullable: true),
                    GitHubRepo = table.Column<string>(type: "longtext", nullable: true),
                    DecommissionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ProjectId = table.Column<Guid>(type: "char(36)", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDatee = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tag = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppInfos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_ProjectId",
                table: "AppInfos",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInfos");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EffortScore",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Tasks",
                newName: "TaskEffort");
        }
    }
}
