using Microsoft.EntityFrameworkCore;
using StudentServiceRequestSystem.Models;

namespace StudentServiceRequestSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Users table
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(u => u.UniversityId)
                .HasMaxLength(50);

            entity.Property(u => u.Department)
                .HasMaxLength(150);

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });

        // Configure ServiceRequests table
        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.ToTable("ServiceRequests");

            entity.HasKey(sr => sr.Id);

            entity.Property(sr => sr.Description)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(sr => sr.StaffRemarks)
                .HasMaxLength(500);

            entity.Property(sr => sr.RequestType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(sr => sr.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(sr => sr.RequestDate)
                .HasDefaultValueSql("NOW()");

            // Configure 1-to-many relationship
            entity.HasOne(sr => sr.User)
                .WithMany(u => u.ServiceRequests)
                .HasForeignKey(sr => sr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
