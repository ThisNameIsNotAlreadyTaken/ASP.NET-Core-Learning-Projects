using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FirstApp.Migrations.SchoolDb
{
    public partial class Inheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
        name: "FK_CourseAssignment_Instructor_InstructorId",
        table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Instructor_InstructorId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Student_StudentId",
                table: "Enrollment");

            migrationBuilder.DropIndex(name: "IX_Enrollment_StudentId", table: "Enrollment");

            migrationBuilder.RenameTable(name: "Instructor", newName: "Person");
            migrationBuilder.AddColumn<DateTime>(name: "EnrollmentDate", table: "Person", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Discriminator", table: "Person", nullable: false, maxLength: 128, defaultValue: "Instructor");
            migrationBuilder.AlterColumn<DateTime>(name: "HireDate", table: "Person", nullable: true);
            migrationBuilder.AddColumn<int>(name: "OldId", table: "Person", nullable: true);

            // Copy existing Student data into new Person table.
            migrationBuilder.Sql("INSERT INTO dbo.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, Id AS OldId FROM dbo.Student");
            // Fix up existing relationships to match new PK's.
            migrationBuilder.Sql("UPDATE dbo.Enrollment SET StudentId = (SELECT Id FROM dbo.Person WHERE OldId = Enrollment.StudentId AND Discriminator = 'Student')");

            // Remove temporary key
            migrationBuilder.DropColumn(name: "OldId", table: "Person");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Person_StudentId",
                table: "Enrollment",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                 name: "IX_Enrollment_StudentId",
                 table: "Enrollment",
                 column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignment_Person_InstructorId",
                table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Person_InstructorId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Person_StudentId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_OfficeAssignment_Person_InstructorId",
                table: "OfficeAssignment");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    HireDate = table.Column<DateTime>(nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EnrollmentDate = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignment_Instructor_InstructorId",
                table: "CourseAssignment",
                column: "InstructorId",
                principalTable: "Instructor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Instructor_InstructorId",
                table: "Department",
                column: "InstructorId",
                principalTable: "Instructor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Student_StudentId",
                table: "Enrollment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeAssignment_Instructor_InstructorId",
                table: "OfficeAssignment",
                column: "InstructorId",
                principalTable: "Instructor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
