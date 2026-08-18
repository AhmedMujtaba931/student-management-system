using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Courses_CourseId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_CourseId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "AttendanceRecords",
                newName: "EnrollmentId");

            migrationBuilder.RenameColumn(
                name: "AttendanceId",
                table: "AttendanceRecords",
                newName: "AttendanceRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_StudentId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Enrollments_EnrollmentId",
                table: "AttendanceRecords",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Enrollments_EnrollmentId",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "AttendanceRecords",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "AttendanceRecordId",
                table: "AttendanceRecords",
                newName: "AttendanceId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_EnrollmentId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_StudentId");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AttendanceRecords",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_CourseId",
                table: "AttendanceRecords",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Courses_CourseId",
                table: "AttendanceRecords",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
