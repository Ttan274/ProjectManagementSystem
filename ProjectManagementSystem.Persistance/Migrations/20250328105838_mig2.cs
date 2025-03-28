using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagementSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Teams_TeamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Teams",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "Tasks",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "Tasks",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Tasks",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskEffort",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Tasks",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskNumber",
                table: "Tasks",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Tasks",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishDate",
                table: "Sprints",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Sprints",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Sprints",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "Projects",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectStatus",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Projects",
                type: "longtext",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.CreateTable(
                name: "Documentations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Header = table.Column<string>(type: "longtext", nullable: true),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    View = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TaskId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDatee = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tag = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documentations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documentations_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Header = table.Column<string>(type: "longtext", nullable: true),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    DocumentationId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDatee = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tag = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Documentations_DocumentationId",
                        column: x => x.DocumentationId,
                        principalTable: "Documentations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AppUserId",
                table: "Tasks",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DocumentationId",
                table: "Comments",
                column: "DocumentationId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentations_ProjectId",
                table: "Documentations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentations_TaskId",
                table: "Documentations",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentations_UserId",
                table: "Documentations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Teams_TeamId",
                table: "AspNetUsers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_AppUserId",
                table: "Tasks",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Teams_TeamId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_AppUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Documentations");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AppUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskEffort",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskNumber",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Projects");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Projects",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Teams_TeamId",
                table: "AspNetUsers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
