using cats.Models;
using Microsoft.EntityFrameworkCore;

namespace cats.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cat> Cats { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }

    
    // создание таблиц и тестовых данных (несколько котов, позиций и админа)
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = "admin", RoleId = 1 },
            new User { Id = 2, Username = "user", Password = "user", RoleId = 2 }
        );

        modelBuilder.Entity<Cat>().HasData(
            new Cat { Id = 1, Breed = "Siamese", Gender = "Male", Color = "Cream", Age = 2 },
            new Cat { Id = 2, Breed = "Persian", Gender = "Female", Color = "White", Age = 3 }
        );

        modelBuilder.Entity<Position>().HasData(
            new Position
                { Id = 1, DateAdded = DateTime.UtcNow, DateModified = DateTime.UtcNow, Price = 100.00m, CatId = 1 },
            new Position
                { Id = 2, DateAdded = DateTime.UtcNow, DateModified = DateTime.UtcNow, Price = 150.00m, CatId = 2 }
        );

        base.OnModelCreating(modelBuilder);
    }

    public void EnsureSeedData()
    {
        if (!Roles.Any())
        {
            Roles.AddRange(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );
            SaveChanges();
        }

        if (!Users.Any())
        {
            Users.AddRange(
                new User { Id = 1, Username = "admin", Password = "admin", RoleId = 1 },
                new User { Id = 2, Username = "user", Password = "user", RoleId = 2 }
            );
            SaveChanges();
        }

        if (!Cats.Any())
        {
            Cats.AddRange(
                new Cat { Id = 1, Breed = "Siamese", Gender = "Male", Color = "Cream", Age = 2 },
                new Cat { Id = 2, Breed = "Persian", Gender = "Female", Color = "White", Age = 3 }
            );
            SaveChanges();
        }

        if (!Positions.Any())
        {
            Positions.AddRange(
                new Position
                    { Id = 1, DateAdded = DateTime.UtcNow, DateModified = DateTime.UtcNow, Price = 100.00m, CatId = 1 },
                new Position
                    { Id = 2, DateAdded = DateTime.UtcNow, DateModified = DateTime.UtcNow, Price = 150.00m, CatId = 2 }
            );
            SaveChanges();
        }
    }
}