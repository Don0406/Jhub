using Microsoft.EntityFrameworkCore;

namespace _UPDATED__FrontEnd_Capstone.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Booking> Bookings { get; set; } 
    
    public DbSet<Vendor> Vendors { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Booking>().ToTable("Bookings");
    }
}
