﻿// <auto-generated />
using System;
using HabitTrackerAPI.Data;
using HabitTrackerAPI.Habits.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HabitTrackerAPI.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230331082937_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HabitTrackerAPI.Data.DataModel.DayInformationDataModel", b =>
                {
                    b.Property<int>("HabitId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<bool>("Checked")
                        .HasColumnType("bit");

                    b.HasKey("HabitId", "Date");

                    b.ToTable("DaysInformation");
                });

            modelBuilder.Entity("HabitTrackerAPI.Data.DataModel.HabitDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SettingsId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Habits");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = ".NET Learning",
                            SettingsId = 0,
                            UserId = "00000000-0000-0000-0000-000000000000"
                        });
                });

            modelBuilder.Entity("HabitTrackerAPI.Data.DataModel.HabitSettingsDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("HabitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HabitId")
                        .IsUnique();

                    b.ToTable("HabitSettings");
                });

            modelBuilder.Entity("HabitTrackerAPI.Data.DataModel.DayInformationDataModel", b =>
                {
                    b.HasOne("HabitTrackerAPI.Data.DataModel.HabitDataModel", "Habit")
                        .WithMany("Days")
                        .HasForeignKey("HabitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Habit");
                });

            modelBuilder.Entity("HabitTrackerAPI.Data.DataModel.HabitSettingsDataModel", b =>
                {
                    b.HasOne("HabitTrackerAPI.Data.DataModel.HabitDataModel", "Habit")
                        .WithOne("Settings")
                        .HasForeignKey("HabitTrackerAPI.Data.DataModel.HabitSettingsDataModel", "HabitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Habit");
                });

            modelBuilder.Entity("HabitTrackerAPI.Data.DataModel.HabitDataModel", b =>
                {
                    b.Navigation("Days");

                    b.Navigation("Settings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
