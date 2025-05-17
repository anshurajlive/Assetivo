using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FirebaseUid { get; set; } // Link to Firebase Auth user

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } // e.g. Owner, Tenant (future)
        
        public ICollection<Property> Properties { get; set; }
    }

    public class Property
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        [Required]
        public string Name { get; set; } // Friendly name or identifier

        [Required]
        public PropertyType Type { get; set; }

        [Required]
        public string Address { get; set; }

        public double Size { get; set; } // e.g. in sqft or acres

        public string GoogleMapLocation { get; set; } // latitude, longitude as string or json

        public decimal CurrentMarketValue { get; set; }

        public OccupancyStatus Status { get; set; }

        public ICollection<Tenant> Tenants { get; set; }

        public ICollection<Document> Documents { get; set; }
    }

    public class Tenant
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PropertyId { get; set; }
        
        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }

        [Required]
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime LeaseStartDate { get; set; }

        public DateTime LeaseEndDate { get; set; }

        public decimal MonthlyRent { get; set; }

        public ICollection<Document> Documents { get; set; }
    }

    public class Document
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }

        public Guid? TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileUrl { get; set; } // URL in Firebase Storage

        public string FileType { get; set; } // e.g. pdf, jpg, mp4

        public DateTime UploadedOn { get; set; }
    }

    public class RentPayment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        public bool Paid { get; set; }

        public DateTime? PaidOn { get; set; }

        public string PaymentLink { get; set; }

        public string PaymentStatus { get; set; } // Pending, Success, Failed, etc.
    }

    public class Reminder
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        public string Message { get; set; }

        public DateTime ReminderDate { get; set; }

        public bool Completed { get; set; }
    }

    public class AssetivoContext : DbContext
    {
        public AssetivoContext(DbContextOptions<AssetivoContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<RentPayment> RentPayments { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure any relationships, constraints here if needed

            modelBuilder.Entity<User>()
                .HasIndex(u => u.FirebaseUid)
                .IsUnique();

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.OwnerId);

            modelBuilder.Entity<Tenant>()
                .HasOne(t => t.Property)
                .WithMany(p => p.Tenants)
                .HasForeignKey(t => t.PropertyId);

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

            modelBuilder.Entity<RentPayment>()
                .HasOne(rp => rp.Tenant)
                .WithMany()
                .HasForeignKey(rp => rp.TenantId);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerId);
        }
    }