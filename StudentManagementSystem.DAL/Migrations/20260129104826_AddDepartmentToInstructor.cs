using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentToInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Instructors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 1,
                column: "Department",
                value: null);

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 2,
                column: "Department",
                value: null);

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 3,
                column: "Department",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$a2b3NHxJgiXMVhSvM6U8zuMG6doXeprrvilrnWfIXNrmaXfYmceNS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Instructors");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$HVDWavkPrpCn7NPxL1tqEeTCK2xBxzaSvMagtFx/o5JgKrSkIWvp2");
        }
    }
}
