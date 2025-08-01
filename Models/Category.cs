﻿using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Category
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
