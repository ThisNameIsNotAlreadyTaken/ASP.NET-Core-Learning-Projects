using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FirstApp.Migrations.SchoolDb
{
    public partial class ComplexDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "CourseAssignment",
                columns: table => new
                {
                    CourseId = table.Column<int>(nullable: false),
                    InstructorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAssignment", x => new { x.CourseId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_CourseAssignment_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAssignment_Instructor_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Budget = table.Column<decimal>(type: "money", nullable: false),
                    InstructorId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentId);
                    table.ForeignKey(
                        name: "FK_Department_Instructor_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfficeAssignment",
                columns: table => new
                {
                    InstructorId = table.Column<int>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeAssignment", x => x.InstructorId);
                    table.ForeignKey(
                        name: "FK_OfficeAssignment_Instructor_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("INSERT INTO dbo.Department (Name, Budget, StartDate) VALUES ('Temp', 0.00, GETDATE())");
            // Default value for FK points to department created above, with
            // defaultValue changed to 1 in following AddColumn statement.

            migrationBuilder.AddColumn<int>(
                name: "DepartmentID",
                table: "Course",
                nullable: false,
                defaultValue: 1);

            /*migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Course",
                nullable: false,
                defaultValue: 0);*/

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Student",
                maxLength: 50,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Student",
                maxLength: 50,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Course",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_DepartmentId",
                table: "Course",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignment_CourseId",
                table: "CourseAssignment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignment_InstructorId",
                table: "CourseAssignment",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_InstructorId",
                table: "Department",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignment_InstructorId",
                table: "OfficeAssignment",
                column: "InstructorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Department_DepartmentId",
                table: "Course",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Department_DepartmentId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_DepartmentId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Course");

            migrationBuilder.DropTable(
                name: "CourseAssignment");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "OfficeAssignment");

            migrationBuilder.DropTable(
                name: "Instructor");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Student",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Student",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Course",
                nullable: true);
        }
    }
}
