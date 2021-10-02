using System;

namespace SimpleCmsWebApi.DTO
{
    public class ArticleDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
