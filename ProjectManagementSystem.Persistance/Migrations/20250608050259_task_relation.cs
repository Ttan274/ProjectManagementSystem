using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagementSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class task_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentations_Tasks_TaskId",
                table: "Documentations",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_TaskId",
                table: "Tasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
