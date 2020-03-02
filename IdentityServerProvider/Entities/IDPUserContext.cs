// Migration NameSpace  Galal.IDP.Entities.IDPUserContext
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Galal.IDP.Entities
{
    public class IDPUserContext : DbContext
    {
        public IDPUserContext(DbContextOptions<IDPUserContext> options)
           : base(options)
        {
           
        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<User>().HasKey(s => s.SubjectId);
        //}

        public DbSet<User> Users { get; set; }
    }
}
