using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static async Task InitializeAsync(AssetivoContext context)
    {
        if (await context.Users.AnyAsync())
        {
            // DB already seeded
            return;
        }

        // Add Firebase user as per your request
        var user = new User
        {
            Id = Guid.Parse("0196df89-5b1c-7d12-a2bc-52b1c7371809"),
            FirebaseUid = "sqrNTmh4o7cSeUgowbFLfCdyFdN2",
            Email = "a@a.com",
            Name = "Firebase Registered User",
            Role = UserRole.Owner
        };

        var property1 = new Property
        {
            Id = Guid.NewGuid(),
            Owner = user,
            Name = "Sunset Villa",
            Type = PropertyType.IndependentHouse,
            Address = "123 Main Street, Springfield",
            Size = 2500,
            Latitude = 37.7749,
            Longitude = -122.4194,
            CurrentMarketValue = 550000m,
            Status = OccupancyStatus.SelfOccupied
        };

        var tenant1 = new Tenant
        {
            Id = Guid.NewGuid(),
            Property = property1,
            Name = "John Doe",
            Phone = "+1234567890",
            Email = "john@example.com",
            LeaseStartDate = DateTime.UtcNow.AddMonths(-6),
            LeaseEndDate = DateTime.UtcNow.AddMonths(6),
            MonthlyRent = 1500m
        };

        var rentPayment1 = new RentPayment
        {
            Id = Guid.NewGuid(),
            Tenant = tenant1,
            Amount = 1500m,
            DueDate = DateTime.UtcNow.AddDays(5),
            Paid = false,
            PaymentStatus = PaymentStatus.Pending
        };

        var reminder1 = new Reminder
        {
            Id = Guid.NewGuid(),
            Owner = user,
            Message = "Property tax payment due next month",
            ReminderDate = DateTime.UtcNow.AddDays(25),
            Completed = false
        };

        var document1 = new Document
        {
            Id = Guid.NewGuid(),
            Property = property1,
            FileName = "Ownership_Deed.pdf",
            FileUrl = "https://example.com/files/ownership_deed.pdf",
            FileType = "pdf",
            UploadedOn = DateTime.UtcNow.AddDays(-10)
        };

        // Attach navigation collections
        property1.Tenants = new List<Tenant> { tenant1 };
        property1.Documents = new List<Document> { document1 };
        tenant1.Documents = new List<Document>(); // empty for now

        user.Properties = new List<Property> { property1 };

        await context.Users.AddAsync(user);
        await context.Properties.AddAsync(property1);
        await context.Tenants.AddAsync(tenant1);
        await context.RentPayments.AddAsync(rentPayment1);
        await context.Reminders.AddAsync(reminder1);
        await context.Documents.AddAsync(document1);

        await context.SaveChangesAsync();
    }
}
