using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// Enums for PropertyType and OccupancyStatus
public enum PropertyType
{
    IndependentHouse,
    Apartment,
    AgriculturalLand,
    CommercialShop,
    ResidentialPlot
}

public enum OccupancyStatus
{
    SelfOccupied,
    Rented,
    AvailableForRent,
    AvailableForLease
}

// Enum for User Roles (instead of string Role)
public enum UserRole
{
    Owner,
    Tenant,
    Admin // you can add more roles as needed
}

// Enum for PaymentStatus
public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Cancelled
}

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    //[Index(IsUnique = true)]
    public required string FirebaseUid { get; set; } // Link to Firebase Auth user

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.Owner; // Default role

    public ICollection<Property>? Properties { get; set; }
}

public class Property
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }

    [Required]
    public required string Name { get; set; } // Friendly name or identifier

    [Required]
    public PropertyType Type { get; set; }

    [Required]
    public required string Address { get; set; }

    public double Size { get; set; } // e.g. in sqft or acres

    // Store lat/lng separately for easier querying or JSON if preferred
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public decimal CurrentMarketValue { get; set; }

    public OccupancyStatus Status { get; set; }

    public ICollection<Tenant>? Tenants { get; set; }

    public ICollection<Document>? Documents { get; set; }
}

public class Tenant
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PropertyId { get; set; }

    [ForeignKey(nameof(PropertyId))]
    public Property? Property { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    [Phone]
    public required string Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public DateTime LeaseStartDate { get; set; }

    public DateTime LeaseEndDate { get; set; }

    public decimal MonthlyRent { get; set; }

    public ICollection<Document>? Documents { get; set; }
}

public class Document
{
    [Key]
    public Guid Id { get; set; }

    public Guid? PropertyId { get; set; }

    [ForeignKey(nameof(PropertyId))]
    public Property? Property { get; set; }

    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant? Tenant { get; set; }

    [Required]
    public required string FileName { get; set; }

    [Required]
    public required string FileUrl { get; set; } // URL in Firebase Storage or any file storage

    [Required]
    public required string FileType { get; set; } // e.g. pdf, jpg, mp4

    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
}

public class RentPayment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant? Tenant { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public bool Paid { get; set; } = false;

    public DateTime? PaidOn { get; set; }

    public string? PaymentLink { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
}

public class Reminder
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }

    [Required]
    public required string Message { get; set; }

    [Required]
    public DateTime ReminderDate { get; set; }

    public bool Completed { get; set; } = false;
}

public class AssetivoContext : DbContext
{
    public AssetivoContext(DbContextOptions<AssetivoContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<RentPayment> RentPayments { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique index on FirebaseUid
        modelBuilder.Entity<User>()
            .HasIndex(u => u.FirebaseUid)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        // User-Property relationship
        modelBuilder.Entity<Property>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.Properties)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Property-Tenant relationship
        modelBuilder.Entity<Tenant>()
            .HasOne(t => t.Property)
            .WithMany(p => p.Tenants)
            .HasForeignKey(t => t.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Document relationships
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Property)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Tenant)
            .WithMany(t => t.Documents)
            .HasForeignKey(d => d.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // RentPayment-Tenant relationship
        modelBuilder.Entity<RentPayment>()
            .HasOne(rp => rp.Tenant)
            .WithMany()
            .HasForeignKey(rp => rp.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Reminder-Owner relationship
        modelBuilder.Entity<Reminder>()
            .HasOne(r => r.Owner)
            .WithMany()
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes on foreign keys to speed queries (optional but recommended)
        modelBuilder.Entity<Property>().HasIndex(p => p.OwnerId);
        modelBuilder.Entity<Tenant>().HasIndex(t => t.PropertyId);
        modelBuilder.Entity<Document>().HasIndex(d => d.PropertyId);
        modelBuilder.Entity<Document>().HasIndex(d => d.TenantId);
        modelBuilder.Entity<RentPayment>().HasIndex(rp => rp.TenantId);
        modelBuilder.Entity<Reminder>().HasIndex(r => r.OwnerId);
    }
}