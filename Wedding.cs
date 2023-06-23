using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace LogReg.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Wedder name must be at least 2 characters.")]
        public string WedderOne { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Wedder name must be at least 2 characters.")]
        public string WedderTwo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        
        public DateTime Date { get; set; }

        [Required]
        public string Address { get; set; }

        public int UserId {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

          public List<Guests> Guests { get; set; } = new List<Guests>();
        //    public List<Guests>? Guests { get; set; } 
    }
}
