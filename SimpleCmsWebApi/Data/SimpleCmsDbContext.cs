using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using SimpleCmsWebApi.Models;

namespace SimpleCmsWebApi.Data
{
    public class SimpleCmsDbContext : DbContext
    {
        public SimpleCmsDbContext(DbContextOptions contextOptions) : base(contextOptions)
        {
            SavingChanges += SuperCmsDbContext_SavingChanges;
        }

        private void SuperCmsDbContext_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            var modifiedAndCreatedEntities = ChangeTracker.Entries()
                .Where(entry => entry.Entity is ITrackable && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                .Select(entry => entry.Entity as ITrackable);
            
            foreach(var entity in modifiedAndCreatedEntities)
            {
                entity.Timestamp = DateTime.UtcNow;
            }
        }

        public DbSet<Article> Articles { get; set; }
    }
}
