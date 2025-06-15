using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagementSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTeamConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTeamConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TeamIntroduction = table.Column<string>(type: "longtext", nullable: true),
                    TaskCompletionWeight = table.Column<double>(type: "double", nullable: false),
                    OnTimeDeliveryWeight = table.Column<double>(type: "double", nullable: false),
                    TargetProximityWeight = table.Column<double>(type: "double", nullable: false),
                    CodingScoreWeight = table.Column<double>(type: "double", nullable: false),
                    CommitWeight = table.Column<double>(type: "double", nullable: false),
                    NetChangeWeight = table.Column<double>(type: "double", nullable: false),
                    RefactorWeight = table.Column<double>(type: "double", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDatee = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tag = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeamConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTeamConfigs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamConfigs_ProjectId",
                table: "ProjectTeamConfigs",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTeamConfigs");
        }
    }
}
