using Microsoft.EntityFrameworkCore;
using MiEstudio.Server.Data.Models;

namespace MiEstudio.Server.Data.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<UserModel> Users { get; set; }
        public virtual DbSet<ClientModel> Clients { get; set; }
        public virtual DbSet<ClientExpenseModel> ClientsExpense { get; set; }
        public virtual DbSet<MovementModel> Movements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(options =>
            {
                options.HasKey(e => e.Id);
                options.HasIndex(e => e.User).IsUnique();
                options.ToTable(nameof(UserModel));
            });

            modelBuilder.Entity<ClientModel>(options =>
            {
                options.HasKey(e => e.Id);
                options.HasIndex(e => e.Name).IsUnique();
                options.ToTable(nameof(ClientModel));
            });

            modelBuilder.Entity<MovementModel>(options =>
            {
                options.HasKey(e => e.Id);
                options.ToTable(nameof(MovementModel));
            });

            modelBuilder.Entity<ClientExpenseModel>(options =>
            {
                options.HasKey(e => e.Id);
                options.ToTable(nameof(ClientExpenseModel));
            });
        }
    }
}