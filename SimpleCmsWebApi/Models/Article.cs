using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleCmsWebApi.Models
{
    public class Article : ITrackable
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
