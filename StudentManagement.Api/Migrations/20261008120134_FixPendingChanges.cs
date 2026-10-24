using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId1",
                table: "ClassAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssignments_TeacherId1",
                table: "ClassAssignments",
                column: "TeacherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssignments_Teachers_TeacherId1",
                table: "ClassAssignments",
                column: "TeacherId1",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_Teachers_TeacherId1",
                table: "ClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ClassAssignments_TeacherId1",
                table: "ClassAssignments");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "ClassAssignments");
        }
    }
}
