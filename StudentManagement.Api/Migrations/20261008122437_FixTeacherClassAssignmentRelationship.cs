using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixTeacherClassAssignmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_Teachers_TeacherId1",
                table: "ClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ClassAssignments_TeacherId1",
                table: "ClassAssignments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "ClassAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "GradeLevel",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GradeLevel",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
