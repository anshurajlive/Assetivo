public class PropertyDetailDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = default!;
    public PropertyType Type { get; set; }
    public string Address { get; set; } = default!;
    public double Size { get; set; }
    public string? GoogleMapLocation { get; set; }
    public decimal CurrentMarketValue { get; set; }
    public OccupancyStatus Status { get; set; }

    public List<TenantDto> Tenants { get; set; } = [];
    public List<DocumentDto> Documents { get; set; } = [];
}

// public class TenantDto
// {
//     public Guid Id { get; set; }
//     public string FullName { get; set; } = default!;
//     public string? Phone { get; set; }
//     public string? Email { get; set; }
// }
