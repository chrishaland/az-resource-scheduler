namespace Repository.Models;

public class Environment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string ScheduledStartup { get; set; } = string.Empty;

    public int ScheduledUptime { get; set; } = 0;

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
