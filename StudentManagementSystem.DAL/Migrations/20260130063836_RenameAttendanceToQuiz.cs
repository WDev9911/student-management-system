using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameAttendanceToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttendanceScore",
                table: "Grades",
                newName: "QuizScore");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$fWslbqtaua.sdAkM2RHkxerKttU1Pzj.aaQ1wUZ3I5GUql73iNzwO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuizScore",
                table: "Grades",
                newName: "AttendanceScore");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$tmJQN3Rws6ufSojY2biFs.HD8yf5M0g0eelI7hCTzgHgCgIrXFZge");
        }
    }
}
