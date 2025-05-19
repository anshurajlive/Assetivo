using System.ComponentModel.DataAnnotations;

public class UpdateTenantDto : CreateTenantDto
{
    [Required]
    public Guid Id { get; set; }
}