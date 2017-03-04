namespace UCS.Database
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class Mysql : DbContext
    {
        public Mysql(string connectionString) : base("name=" + connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Clan> Clan { get; set; }
        public virtual DbSet<Player> Player { get; set; }
    }
}
