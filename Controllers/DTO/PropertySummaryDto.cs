public class PropertySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public PropertyType Type { get; set; }
    public string Address { get; set; } = default!;
    public OccupancyStatus Status { get; set; }
}