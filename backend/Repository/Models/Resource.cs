using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
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
        public string Name { get; set; }

        [Required]
        public string ResourceGroup { get; set; }

        [Required]
        public string Description { get; set; }

        public VirtualMachine VirtualMachine { get; set; }
        public VirtualMachineScaleSet VirtualMachineScaleSet { get; set; }
        public ICollection<Environment> Environments { get; set; }
    }
}
