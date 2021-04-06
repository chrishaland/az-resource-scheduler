using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public class Environment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string ScheduledStartup { get; set; }

        public int ScheduledUptime{ get; set; }

        public ICollection<Resource> Resources { get; set; }
    }
}
