using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using First_App.Data;

namespace FirstApp.Migrations
{
    [DbContext(typeof(SchoolDbContext))]
    partial class SchoolDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("First_App.Models.Course", b =>
                {
                    b.Property<int>("CourseId");

                    b.Property<int>("Credits");

                    b.Property<int>("DepartmentId");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("CourseId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("First_App.Models.CourseAssignment", b =>
                {
                    b.Property<int>("CourseId");

                    b.Property<int>("InstructorId");

                    b.HasKey("CourseId", "InstructorId");

                    b.HasIndex("CourseId");

                    b.HasIndex("InstructorId");

                    b.ToTable("CourseAssignment");
                });

            modelBuilder.Entity("First_App.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Budget")
                        .HasColumnType("money");

                    b.Property<int?>("InstructorId");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("StartDate");

                    b.HasKey("DepartmentId");

                    b.HasIndex("InstructorId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("First_App.Models.Enrollment", b =>
                {
                    b.Property<int>("EnrollmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CourseId");

                    b.Property<int?>("Grade");

                    b.Property<int>("StudentId");

                    b.HasKey("EnrollmentId");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Enrollment");
                });

            modelBuilder.Entity("First_App.Models.OfficeAssignment", b =>
                {
                    b.Property<int>("InstructorId");

                    b.Property<string>("Location")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("InstructorId");

                    b.HasIndex("InstructorId")
                        .IsUnique();

                    b.ToTable("OfficeAssignment");
                });

            modelBuilder.Entity("First_App.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("FirstMidName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("Person");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Person");
                });

            modelBuilder.Entity("First_App.Models.Instructor", b =>
                {
                    b.HasBaseType("First_App.Models.Person");

                    b.Property<DateTime>("HireDate");

                    b.ToTable("Instructor");

                    b.HasDiscriminator().HasValue("Instructor");
                });

            modelBuilder.Entity("First_App.Models.Student", b =>
                {
                    b.HasBaseType("First_App.Models.Person");

                    b.Property<DateTime>("EnrollmentDate");

                    b.ToTable("Student");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("First_App.Models.Course", b =>
                {
                    b.HasOne("First_App.Models.Department", "Department")
                        .WithMany("Courses")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("First_App.Models.CourseAssignment", b =>
                {
                    b.HasOne("First_App.Models.Course", "Course")
                        .WithMany("Assignments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("First_App.Models.Instructor", "Instructor")
                        .WithMany("Courses")
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("First_App.Models.Department", b =>
                {
                    b.HasOne("First_App.Models.Instructor", "Administrator")
                        .WithMany()
                        .HasForeignKey("InstructorId");
                });

            modelBuilder.Entity("First_App.Models.Enrollment", b =>
                {
                    b.HasOne("First_App.Models.Course", "Course")
                        .WithMany("Enrollments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("First_App.Models.Student", "Student")
                        .WithMany("Enrollments")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("First_App.Models.OfficeAssignment", b =>
                {
                    b.HasOne("First_App.Models.Instructor", "Instructor")
                        .WithOne("OfficeAssignment")
                        .HasForeignKey("First_App.Models.OfficeAssignment", "InstructorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
