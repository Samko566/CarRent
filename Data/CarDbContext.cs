using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarRent.Models;
using System.Runtime.ConstrainedExecution;
using System.Xml;
using Microsoft.Extensions.Options;
using CarRent.Areas.Identity.Data;

namespace CarRent.Data
{
    public class CarDbContext : DbContext
    {
        public CarDbContext (DbContextOptions<CarDbContext> options)
            : base(options)
        {
        }
        public DbSet<Car> Car { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<CartViewModel> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурація моделі
            modelBuilder.Entity<CartItem>().Ignore(c => c.Car);
            modelBuilder.Entity<CartViewModel>().HasNoKey();

            //явно вказуємо тип даних для властивості TotalPrice
            modelBuilder.Entity<CartViewModel>()
                .Property(c => c.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalPrice)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<CartItem>()
           .HasOne(c => c.Car)
           .WithMany()
           .HasForeignKey(c => c.CarId);

            modelBuilder.Entity<CartItem>()
        .HasOne(ci => ci.Car)
        .WithMany()
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.DailyRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Discount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Penalty)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.UserId, ci.CarId })
                .IsUnique();

            modelBuilder.Entity<Car>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Car)
                .HasForeignKey(ci => ci.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
    .HasMany(u => u.CartItems)
    .WithOne(ci => ci.User)
    .HasForeignKey(ci => ci.UserId)
    .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
    }
}