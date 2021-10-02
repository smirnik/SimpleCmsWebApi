using SimpleCmsWebApi.Data;
using SimpleCmsWebApi.Models;
using System.Collections.Generic;

namespace SimpleCmsWebApi.IntegrationTests
{
    public class DbUtilities
    {
        public static IEnumerable<Article> Articles = new List<Article>()
            {
                new Article()
                {
                    Title = "Article 1",
                    Body = "Article body 3",
                },
                new Article()
                {
                    Title = "Article 1",
                    Body = "Article body 2",
                },
                new Article()
                {
                    Title = "Article 2",
                    Body = "Article body 1",
                }
            };

        internal static void InitializeDbForTests(SimpleCmsDbContext db)
        {
            db.AddRange(Articles);
            db.SaveChanges();
        }
    }
}
