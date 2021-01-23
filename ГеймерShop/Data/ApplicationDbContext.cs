using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ГеймерShop.Models;

namespace ГеймерShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<KeyStatus> KeyStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<PlaingField> PlaingFields { get; set; }
        public DbSet<SystemRequirements> SystemRequirements { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<KeyStatus>().HasData(
                new KeyStatus[]
                {
                    new KeyStatus { Id = 1, Name = "активный"},
                    new KeyStatus { Id = 2, Name = "неактивный"}
                });
            modelBuilder.Entity<Genre>().HasData(
               new Genre[]
               {
                    new Genre { Id = 1, Name = "Экшн"},
                    new Genre { Id = 2, Name = "Ролевая"},
                    new Genre { Id = 3, Name = "Приключения"},
                    new Genre { Id = 4, Name = "Стратегия"},
                    new Genre { Id = 5, Name = "Симулятор"},
                    new Genre { Id = 6, Name = "Онлайн"},
                    new Genre { Id = 1, Name = "Инди"},
                    new Genre { Id = 2, Name = "Спорт"},
                    new Genre { Id = 3, Name = "Шутер"},
                    new Genre { Id = 4, Name = "Платформер"}
               });
            modelBuilder.Entity<PlaingField>().HasData(
               new PlaingField[]
               {
                    new PlaingField { Id = 1, Name = "Steam"},
                    new PlaingField { Id = 2, Name = "Origin"},
                    new PlaingField { Id = 3, Name = "Battle.net"}
               });

        }
    }
}
