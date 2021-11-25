namespace Repository.Models;

public class ResourceStopJob
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string JobId { get; set; } = string.Empty;

    [Required]
    public DateTime StopAt { get; set; }

    [Required]
    public Guid ResourceId { get; set; }
    public Resource? Resource { get; set; }
}
