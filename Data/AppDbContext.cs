using Microsoft.EntityFrameworkCore;
using BankClientAPI.Models;

namespace BankClientAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                // Aseguramos que el email sea único en la base de datos
                entity.HasIndex(e => e.Email).IsUnique();

                // Configuramos la precisión del decimal (18 dígitos, 2 decimales)
                entity.Property(e => e.Balance)
                    .HasPrecision(18, 2);

                // Seed data: datos iniciales para pruebas
                entity.HasData(
                    new Client
                    {
                        Id = 1,
                        FirstName = "Juan",
                        LastName = "Pérez",
                        Email = "juan.perez@email.com",
                        PhoneNumber = "0999123456",
                        Address = "Av. Amazonas y Naciones Unidas, Quito",
                        AccountType = "Ahorros",
                        Balance = 5000.00m,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Client
                    {
                        Id = 2,
                        FirstName = "María",
                        LastName = "González",
                        Email = "maria.gonzalez@email.com",
                        PhoneNumber = "0998765432",
                        Address = "Calle 10 de Agosto, Quito",
                        AccountType = "Corriente",
                        Balance = 12500.50m,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                );
            });
        }
    }
}