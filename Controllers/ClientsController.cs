using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankClientAPI.Data;
using BankClientAPI.Models;
using BankClientAPI.DTOs;

namespace BankClientAPI.Controllers
{
    /// <summary>
    /// Controlador que maneja todas las operaciones CRUD de clientes
    /// [ApiController] habilita características como validación automática
    /// [Route] define la ruta base: /api/clients
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientsController> _logger;

        // Inyección de dependencias en el constructor
        public ClientsController(AppDbContext context, ILogger<ClientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/clients
        // Obtiene todos los clientes
        /// <summary>
        /// [HttpGet] indica que responde a peticiones GET
        /// ActionResult<T> permite retornar diferentes códigos HTTP
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetClients()
        {
            _logger.LogInformation("Obteniendo lista de clientes");

          
            var clients = await _context.Clients
                .Where(c => c.IsActive) // Solo clientes activos
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    AccountType = c.AccountType,
                    Balance = c.Balance,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            // Ok() retorna un 200 con el contenido
            return Ok(clients);
        }

        // GET: api/clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetClient(int id)
        {
            _logger.LogInformation($"Obteniendo cliente con ID {id}");

            // FindAsync busca por clave primaria
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                _logger.LogWarning($"Cliente {id} no encontrado");
                // NotFound() retorna un 404
                return NotFound(new { message = "Cliente no encontrado" });
            }

            var clientDto = new ClientResponseDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                AccountType = client.AccountType,
                Balance = client.Balance,
                CreatedAt = client.CreatedAt,
                IsActive = client.IsActive
            };

            return Ok(clientDto);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<ClientResponseDto>> CreateClient(CreateClientDto clientDto)
        {
            _logger.LogInformation("Creando nuevo cliente");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad Request
            }

            if (await _context.Clients.AnyAsync(c => c.Email == clientDto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            var client = new Client
            {
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                Email = clientDto.Email,
                PhoneNumber = clientDto.PhoneNumber,
                Address = clientDto.Address,
                AccountType = clientDto.AccountType,
                Balance = clientDto.Balance,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Clients.Add(client);
            
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cliente creado con ID {client.Id}");

            var responseDto = new ClientResponseDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                AccountType = client.AccountType,
                Balance = client.Balance,
                CreatedAt = client.CreatedAt,
                IsActive = client.IsActive
            };

            // CreatedAtAction retorna 201 Created con la ubicación del recurso
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, responseDto);
        }

        // PUT: api/clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, UpdateClientDto clientDto)
        {
            _logger.LogInformation($"Actualizando cliente {id}");

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            if (clientDto.FirstName != null)
                client.FirstName = clientDto.FirstName;
            
            if (clientDto.LastName != null)
                client.LastName = clientDto.LastName;
            
            if (clientDto.Email != null)
            {
                if (await _context.Clients.AnyAsync(c => c.Email == clientDto.Email && c.Id != id))
                {
                    return BadRequest(new { message = "El email ya está registrado" });
                }
                client.Email = clientDto.Email;
            }
            
            if (clientDto.PhoneNumber != null)
                client.PhoneNumber = clientDto.PhoneNumber;
            
            if (clientDto.Address != null)
                client.Address = clientDto.Address;
            
            if (clientDto.AccountType != null)
                client.AccountType = clientDto.AccountType;
            
            if (clientDto.Balance.HasValue)
                client.Balance = clientDto.Balance.Value;
            
            if (clientDto.IsActive.HasValue)
                client.IsActive = clientDto.IsActive.Value;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Cliente {id} actualizado exitosamente");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clients.AnyAsync(c => c.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            _logger.LogInformation($"Eliminando cliente {id}");

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            // Soft delete: solo marcamos como inactivo
            client.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cliente {id} desactivado");

            return NoContent();
        }

        // GET: api/clients/search?term=juan
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClientResponseDto>>> SearchClients([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { message = "El término de búsqueda es requerido" });
            }

            _logger.LogInformation($"Buscando clientes con término: {term}");

            var clients = await _context.Clients
                .Where(c => c.IsActive && 
                       (c.FirstName.Contains(term) || 
                        c.LastName.Contains(term) || 
                        c.Email.Contains(term)))
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    AccountType = c.AccountType,
                    Balance = c.Balance,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(clients);
        }
    }
}