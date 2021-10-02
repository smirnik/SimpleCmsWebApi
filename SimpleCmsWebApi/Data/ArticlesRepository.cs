using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCmsWebApi.Helpers;
using SimpleCmsWebApi.Models;

namespace SimpleCmsWebApi.Data
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly SimpleCmsDbContext _dbContext;

        public ArticlesRepository(SimpleCmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            _dbContext.Add(article);
        }

        public Article GetArticle(int id)
        {
            return _dbContext.Articles.FirstOrDefault(article => article.Id == id);
        }

        public IEnumerable<Article> GetArticles(string sort, int? offset = null, int? limit = null)
        {
            var articles = _dbContext.Articles.AsQueryable();
            
            if (!string.IsNullOrEmpty(sort))
            {
                articles = articles.Sort(sort);
            }

            if (offset.HasValue)
            {
                articles.Skip(offset.Value);
            }

            if (limit.HasValue)
            {
                articles.Take(limit.Value);
            }

            return articles.ToList();
        }

        public void DeleteArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            _dbContext.Articles.Remove(article);
        }

        public bool SaveChanges()
        {
            return _dbContext.SaveChanges() >= 0;
        }
    }
}
