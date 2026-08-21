using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class IndustryUpgrade_TeacherAndGradeService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Courses_CourseId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Students_StudentId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "AttendanceRecords",
                newName: "EnrollmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_StudentId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_EnrollmentId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Students",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMarks",
                table: "Grades",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarksObtained",
                table: "Grades",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Grades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TeacherUserId",
                table: "Courses",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceSessionId",
                table: "AttendanceRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceDate",
                table: "AttendanceRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Students_RegistrationNumber",
                table: "Students",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherUserId",
                table: "Courses",
                column: "TeacherUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "AttendanceSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Enrollments_EnrollmentId",
                table: "AttendanceRecords",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_TeacherUserId",
                table: "Courses",
                column: "TeacherUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Courses_CourseId",
                table: "Grades",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Students_StudentId",
                table: "Grades",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Enrollments_EnrollmentId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_TeacherUserId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Courses_CourseId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Students_StudentId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_RegistrationNumber",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherUserId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "TeacherUserId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AttendanceDate",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "AttendanceRecords",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_EnrollmentId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_StudentId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMarks",
                table: "Grades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MarksObtained",
                table: "Grades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceSessionId",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AttendanceRecords",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "AttendanceSessionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Courses_CourseId",
                table: "Grades",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Students_StudentId",
                table: "Grades",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
