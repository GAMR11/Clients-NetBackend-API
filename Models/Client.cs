using System.ComponentModel.DataAnnotations;

namespace BankClientAPI.Models
{
    public class Client
    {
        // [Key] indica que esta propiedad es la clave primaria
        // Entity Framework la hará auto-incremental
        [Key]
        public int Id { get; set; }

        // [Required] hace que el campo sea obligatorio (NOT NULL en DB)
        // [StringLength] limita la longitud máxima del string
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // [EmailAddress] valida que sea un formato de email válido
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; } // El ? indica que puede ser null

        // Tipo de cuenta (Ahorros, Corriente, etc.)
        [Required]
        [StringLength(50)]
        public string AccountType { get; set; } = string.Empty;

        // Balance actual de la cuenta
        [Required]
        public decimal Balance { get; set; }

        // Fecha de creación del registro
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Indica si el cliente está activo
        public bool IsActive { get; set; } = true;
    }
}