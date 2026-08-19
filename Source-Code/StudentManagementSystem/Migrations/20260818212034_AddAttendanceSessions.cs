using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Enrollments_EnrollmentId",
                table: "AttendanceRecords");

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

            migrationBuilder.AddColumn<int>(
                name: "AttendanceSessionId",
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

            migrationBuilder.CreateTable(
                name: "AttendanceSessions",
                columns: table => new
                {
                    AttendanceSessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    LectureNumber = table.Column<int>(type: "int", nullable: false),
                    LectureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSessions", x => x.AttendanceSessionId);
                    table.ForeignKey(
                        name: "FK_AttendanceSessions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId_StudentId",
                table: "AttendanceRecords",
                columns: new[] { "AttendanceSessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_CourseId_LectureNumber",
                table: "AttendanceSessions",
                columns: new[] { "CourseId", "LectureNumber" },
                unique: true);

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
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "AttendanceSessionId",
                table: "AttendanceRecords");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceDate",
                table: "AttendanceRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Enrollments_EnrollmentId",
                table: "AttendanceRecords",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
