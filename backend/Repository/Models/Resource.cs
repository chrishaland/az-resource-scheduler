﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public class Resource
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string AzureId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public ResourceKind Kind { get; set; }

        public ICollection<Tenant> Tenants { get; set; }
        public ICollection<Environment> Environments { get; set; }
    }

    public enum ResourceKind
    {
        NotSet = 0,
        VirtualMachine = 1,
        NodePool = 2
    }
}
