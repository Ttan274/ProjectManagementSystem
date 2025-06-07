using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AppInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
