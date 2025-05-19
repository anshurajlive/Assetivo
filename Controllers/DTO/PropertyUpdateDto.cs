public class PropertyUpdateDto
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public PropertyType Type { get; set; }
    public double Size { get; set; }
    public string? GoogleMapLocation { get; set; }
    public decimal CurrentMarketValue { get; set; }
    public OccupancyStatus Status { get; set; }
}
