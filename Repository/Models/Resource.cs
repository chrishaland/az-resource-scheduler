namespace Repository.Models;

public class VirtualMachine : Resource
{
}

public class VirtualMachineScaleSet : Resource
{
    public int? Capacity { get; set; }
}

public class Resource
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ResourceGroup { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }

    public VirtualMachine? VirtualMachine { get; set; }
    public VirtualMachineScaleSet? VirtualMachineScaleSet { get; set; }
    public ICollection<Environment> Environments { get; set; } = new List<Environment>();
    public ICollection<ResourceStopJob> ResourceStopJobs { get; set; } = new List<ResourceStopJob>();
}
