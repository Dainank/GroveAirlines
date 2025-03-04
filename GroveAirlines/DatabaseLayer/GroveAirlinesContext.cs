﻿using System;
using GroveAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace GroveAirlines.DatabaseLayer
{
    public class GroveAirlinesContext : DbContext
    {
        public GroveAirlinesContext(DbContextOptions<GroveAirlinesContext> options) // passing instance type
            : base(options)
        {
        }

        // allowance of override (virtual)
        public virtual DbSet<Airport> Airport { get; set; } 
        public virtual DbSet<Booking> Booking { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Flight> Flight { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return; // set to empty string if null (false)
            var connectionString = Environment.GetEnvironmentVariable("GroveAirlines_Database_Connection_String") ?? string.Empty;   // get connection string without hard-coding it 
            optionsBuilder.UseSqlServer(connectionString);  // connects to MSSQL
        }

        // generates entities (and overrides)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airport>(entity =>  // className
            {
                entity.Property(e => e.AirportId)   // column + column name
                    .HasColumnName("AirportID") // name
                    .ValueGeneratedNever(); // generated null

                entity.Property(e => e.City)
                    .IsRequired()   // assigned value or null
                    .HasMaxLength(50)   // maxLength
                    .IsUnicode(false);  // allowance of uniCode characters

                entity.Property(e => e.Iata)
                    .IsRequired()
                    .HasColumnName("IATA")
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.HasOne(d => d.Customer)  // oneTo
                    .WithMany(p => p.Booking)   // Many
                    .HasForeignKey(d => d.CustomerId)   // target column that is FK
                    .HasConstraintName("FK__Booking__Custome__71D1E811");   // ConstraintName (autogenerated)

                entity.HasOne(d => d.FlightNumberNavigation)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.FlightNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking__FlightN__4F7CD00D");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.FlightNumber);

                entity.Property(e => e.FlightNumber).ValueGeneratedNever(); // never auto-generate values

                entity.HasOne(d => d.DestinationNavigation)
                    .WithMany(p => p.FlightDestinationNavigation)
                    .HasForeignKey(d => d.Destination)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_AirportDestination");

                entity.HasOne(d => d.OriginNavigation)
                    .WithMany(p => p.FlightOriginNavigation)
                    .HasForeignKey(d => d.Origin)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
