using System.ComponentModel.DataAnnotations;

public class CreateTenantDto
{
    [Required]
    public Guid PropertyId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    public DateTime LeaseStartDate { get; set; }
    public DateTime LeaseEndDate { get; set; }
    public decimal MonthlyRent { get; set; }
}