using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagementSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DocumentationMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentations_AspNetUsers_UserId",
                table: "Documentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Documentations_Projects_ProjectId",
                table: "Documentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations");

            migrationBuilder.DropIndex(
                name: "IX_Documentations_ProjectId",
                table: "Documentations");

            migrationBuilder.DropIndex(
                name: "IX_Documentations_TaskId",
                table: "Documentations");

            migrationBuilder.DropIndex(
                name: "IX_Documentations_UserId",
                table: "Documentations");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Documentations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Documentations");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentationId",
                table: "Tasks",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Documentations",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.CreateIndex(
                name: "IX_Documentations_TaskId",
                table: "Documentations",
                column: "TaskId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations");

            migrationBuilder.DropIndex(
                name: "IX_Documentations_TaskId",
                table: "Documentations");

            migrationBuilder.DropColumn(
                name: "DocumentationId",
                table: "Tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Documentations",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Documentations",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Documentations",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                name: "FK_Documentations_AspNetUsers_UserId",
                table: "Documentations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentations_Projects_ProjectId",
                table: "Documentations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
