using System.ComponentModel.DataAnnotations;

namespace SimpleCmsWebApi.DTO
{
    public class ArticleUpdateDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
