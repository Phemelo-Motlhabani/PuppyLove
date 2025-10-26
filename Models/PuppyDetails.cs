using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models
{
    public class PuppyDetails
    {
        [Key]
        public int PuppyId { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Breed { get; set; }
        public string? Colour { get; set; }
        public string? Size { get; set; }
        public string? BreederName { get; set; }
        public int BreederId { get; set; }

        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
    }
}
