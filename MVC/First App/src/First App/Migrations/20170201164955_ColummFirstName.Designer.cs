using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using First_App.Data;

namespace FirstApp.Migrations.SchoolDb
{
    [DbContext(typeof(SchoolDbContext))]
    [Migration("20170201164955_ColummFirstName")]
    partial class ColummFirstName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("First_App.Models.Course", b =>
                {
                    b.Property<int>("CourseId");

                    b.Property<int>("Credits");

                    b.Property<string>("Title");

                    b.HasKey("CourseId");

                    b.ToTable("Course");
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

            modelBuilder.Entity("First_App.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EnrollmentDate");

                    b.Property<string>("FirstMidName")
                        .HasColumnName("FirstName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("LastName")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("Student");
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
        }
    }
}
