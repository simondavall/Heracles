﻿// <auto-generated />
using System;
using Heracles.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Heracles.Infrastructure.Data.Migrations
{
    [DbContext(typeof(GpxDbContext))]
    [Migration("20211122153531_AddTrackSequencing")]
    partial class AddTrackSequencing
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Heracles.Application.TrackAggregate.Track", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ActivityType")
                        .HasColumnType("int");

                    b.Property<int>("Calories")
                        .HasColumnType("int");

                    b.Property<double>("Distance")
                        .HasColumnType("float");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<double>("Elevation")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<TimeSpan>("Pace")
                        .HasColumnType("time");

                    b.Property<double>("Speed")
                        .HasColumnType("float");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Heracles.Application.TrackAggregate.TrackPoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Elevation")
                        .HasColumnType("float");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<int>("Seq")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TrackSegmentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TrackSegmentId");

                    b.ToTable("TrackPoints");
                });

            modelBuilder.Entity("Heracles.Application.TrackAggregate.TrackSegment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Calories")
                        .HasColumnType("int");

                    b.Property<double>("Distance")
                        .HasColumnType("float");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<double>("Elevation")
                        .HasColumnType("float");

                    b.Property<int>("Seq")
                        .HasColumnType("int");

                    b.Property<Guid>("TrackId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.ToTable("TrackSegments");
                });

            modelBuilder.Entity("Heracles.Application.TrackAggregate.TrackPoint", b =>
                {
                    b.HasOne("Heracles.Application.TrackAggregate.TrackSegment", null)
                        .WithMany("TrackPoints")
                        .HasForeignKey("TrackSegmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Heracles.Application.TrackAggregate.TrackSegment", b =>
                {
                    b.HasOne("Heracles.Application.TrackAggregate.Track", null)
                        .WithMany("TrackSegments")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Heracles.Application.TrackAggregate.Track", b =>
                {
                    b.Navigation("TrackSegments");
                });

            modelBuilder.Entity("Heracles.Application.TrackAggregate.TrackSegment", b =>
                {
                    b.Navigation("TrackPoints");
                });
#pragma warning restore 612, 618
        }
    }
}
