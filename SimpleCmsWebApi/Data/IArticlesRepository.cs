using System.Collections.Generic;
using SimpleCmsWebApi.Models;

namespace SimpleCmsWebApi.Data
{
    public interface IArticlesRepository
    {
        void CreateArticle(Article article);
        void DeleteArticle(Article article);
        Article GetArticle(int id);
        IEnumerable<Article> GetArticles(string sort, int? offset = null, int? limit = null);
        bool SaveChanges();
    }
}