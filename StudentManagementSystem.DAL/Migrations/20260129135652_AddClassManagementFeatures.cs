using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClassManagementFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseClasses_Instructors_InstructorId",
                table: "CourseClasses");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "CourseClasses");

            migrationBuilder.RenameColumn(
                name: "MaxCapacity",
                table: "CourseClasses",
                newName: "SemesterId");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "Semesters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InstructorId",
                table: "CourseClasses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MaxStudents",
                table: "CourseClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "CourseClasses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "CourseClasses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$VIyNc4Iwowtc5bU9WDr3qekUJwvHp6DMt80V/b.g0C8AMyIrGlmGG");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_SemesterId",
                table: "Enrollments",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseClasses_SemesterId",
                table: "CourseClasses",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseClasses_Instructors_InstructorId",
                table: "CourseClasses",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseClasses_Semesters_SemesterId",
                table: "CourseClasses",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "SemesterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Semesters_SemesterId",
                table: "Enrollments",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "SemesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseClasses_Instructors_InstructorId",
                table: "CourseClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseClasses_Semesters_SemesterId",
                table: "CourseClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Semesters_SemesterId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_SemesterId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseClasses_SemesterId",
                table: "CourseClasses");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "MaxStudents",
                table: "CourseClasses");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "CourseClasses");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "CourseClasses");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "CourseClasses",
                newName: "MaxCapacity");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "Semesters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "InstructorId",
                table: "CourseClasses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "CourseClasses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$a2b3NHxJgiXMVhSvM6U8zuMG6doXeprrvilrnWfIXNrmaXfYmceNS");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseClasses_Instructors_InstructorId",
                table: "CourseClasses",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
